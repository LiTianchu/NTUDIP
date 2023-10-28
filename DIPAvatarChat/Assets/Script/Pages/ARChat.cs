using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARChat : PageSingleton<ARChat>
{
    [Header("UI Elements")]
    public TMP_InputField MessageInputField;
    //public TMP_Text RecipientName;
    public GameObject MyChatBubblePrefab;
    public GameObject TheirChatBubblePrefab;
    public GameObject ARChatBubblePrefab;
    public GameObject ScreenChatContainer;
    public GameObject AvatarContainer;
    //public GameObject UsernameContainer;
    public GameObject AvatarSelectionBar;
    public AvatarIconContainer AvatarIconContainer;
    public LayerMask UILayer;

    [Header("AR")]
    public XROrigin XrOrigin;
    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;
    public float PlacedObjectScale = 0.5f;
    public Vector3 TextBubblePos;
    public Vector3 NamePos;

    private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();

    private bool _isPopulated = false;
    //private ListenerRegistration _listener;
    private List<Avatar> _avatarList;

    private bool _isLoading;

    private readonly Vector3 LIGHT_SOURCE_LOCAL_POS = new Vector3(0, 5, 0);
    private string[] AR_AVATAR_BODY_PATHS;

    public Avatar SelectedAvatar { get; set; }
    public List<Avatar> AvatarList { get { return _avatarList; } }

    public event Action OnARFinishedLoading;
    public event Action<Avatar> OnAvatarSelected;
    // Start is called before the first frame update
    void Start()
    {
        _avatarList = new List<Avatar>();
        _isLoading = true;
        //load all avatar of friends and me
        foreach (string email in ChatManager.Instance.EmailToUsersDict.Keys)
        {

            RetrieveAvatarData(email);

        }
    }

    private void Update()
    {
        if (_isLoading)
        {
            //TODO: show loading UI
            Debug.Log("loading...");
        }

    }
    private async void RetrieveAvatarData(string email)
    {
        bool avatarHasLoaded = AvatarManager.Instance.EmailToAvatarDict.TryGetValue(email, out AvatarData data); //check if the avatar is already cached
        if (!avatarHasLoaded) //if not cached, load from database
        {
            DocumentSnapshot avatarDataDoc = await AvatarBackendManager.Instance.GetAvatarByEmailTask(email);
            data = avatarDataDoc.ConvertTo<AvatarData>();


            AvatarManager.Instance.EmailToAvatarDict[email] = data;
            _avatarList.Add(LoadAvatarObject(data));

        }
        else
        {
            _avatarList.Add(LoadAvatarObject(data)); //if it is cached, load from the avatar list
        }


        if (_avatarList.Count == ChatManager.Instance.EmailToUsersDict.Keys.Count) //if loaded all users
        {
            _isLoading = false;
            OnARFinishedLoading?.Invoke(); //invoke the event
        }

    }

    private Avatar LoadAvatarObject(AvatarData data)
    {
        GameObject avatarObj = AvatarManager.Instance.LoadAvatar(data);
        avatarObj.transform.parent = AvatarContainer.transform;
        avatarObj.name = data.email + "_" + "Avatar";
        avatarObj.transform.localScale = PlacedObjectScale * Vector3.one;

        //Set accessories correctly
        AvatarManager.Instance.SetAccessories(ChatManager.Instance.AVATAR_BODY_PATHS);

        //spawn light source
        GameObject lightsource = new GameObject();
        lightsource.transform.parent = avatarObj.transform;
        lightsource.name = "Lightsource";
        Light light = lightsource.AddComponent<Light>();
        light.type = LightType.Point;
        light.intensity = 0.5f;
        lightsource.transform.localPosition = LIGHT_SOURCE_LOCAL_POS;

        Avatar avatar = avatarObj.AddComponent<Avatar>();
        avatar.AvatarData = data;
        avatarObj.SetActive(false);
        avatarObj.layer = 6; //avatar layer

        //need a way to get the username
        PopulateAvatarSelectionBar(avatar, data.email);
        //avatar.ListenForNewMessages();
        return avatar;
    }

    private void PopulateAvatarSelectionBar(Avatar avatarRetrieved, string email)
    {
        AvatarIconContainer avContainer = Instantiate(AvatarIconContainer, AvatarSelectionBar.transform);
        avContainer.AttachedAvatar = avatarRetrieved;
        avContainer.GetComponentInChildren<TMP_Text>().text = ChatManager.Instance.EmailToUsersDict[email].username;
    }

    void OnDestroy()
    {
    }

    public void SendMessage()
    {
        ChatManager.Instance.SendMessage(MessageInputField, SelectedAvatar.ConversationData.conversationID);
    }

    public void ClearChatDisplay()
    {

        foreach (Transform temp in ScreenChatContainer.transform)
        {
            Destroy(temp.gameObject);
        }
    }

    public void BackToNormalChat()
    {

        AppManager.Instance.LoadScene("4-ChatList");
    }

    public bool IsUIPressed()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

        }
        return !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    }

    //This method is to check whether the user is touching on the UI
    private bool TouchedOnUi()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        // return results.Count > 0;
        foreach (var item in results)
        {
            if ((UILayer & (1 << item.gameObject.layer)) != 0) //UI layer contains the item's layer
            {
                return true;
            }
        }
        return false;
    }

    public void HandleTouch(InputAction.CallbackContext context)
    {
        if (SelectedAvatar != null && context.performed)
        {
            if (TouchedOnUi()) { Debug.Log("Touched on UI"); return; }

            //first check if the player is touching on the avatar
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Avatar touchedAvatar = hit.transform.GetComponent<Avatar>();
                if (touchedAvatar != null)
                {
                    if (touchedAvatar != SelectedAvatar) { ClearChatDisplay(); }
                    SelectedAvatar = touchedAvatar;
                    Debug.Log(SelectedAvatar.name + " touched");

                    OnAvatarSelected?.Invoke(SelectedAvatar);
                    return;
                }
            }

            //then check for plane
            bool collision = RaycastManager.Raycast(Input.mousePosition, _raycastHits, TrackableType.PlaneWithinPolygon);
            if (collision)
            {
                SelectedAvatar.gameObject.SetActive(true);
                SelectedAvatar.transform.position = _raycastHits[0].pose.position;
                SelectedAvatar.transform.rotation = _raycastHits[0].pose.rotation;
                SelectedAvatar.transform.localScale = this.PlacedObjectScale * Vector3.one;
            }
        }
    }





}
