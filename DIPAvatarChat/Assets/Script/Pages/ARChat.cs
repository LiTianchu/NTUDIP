using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using static UnityEngine.GraphicsBuffer;

public class ARChat : PageSingleton<ARChat>, IPageTransition
{
    [Header("UI Elements")]
    public TMP_InputField MessageInputField;
    public GameObject MyChatBubblePrefab;
    public GameObject TheirChatBubblePrefab;
    public GameObject ARChatBubblePrefab;
    public GameObject ScreenChatContainer;
    public GameObject AvatarContainer;
    public GameObject usernameBubblePrefab;//avatar label
    public GameObject AvatarSelectionBar;
    public GameObject EmoteSelectionArea;
    public AvatarIconContainer AvatarIconContainer;
    
    public LayerMask UILayer;

    [Header("AR")]
    public XROrigin XrOrigin;
    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;
    public float PlacedObjectScale = 0.5f;
    public Vector3 NamePos;

    [Header("UI Transition")]
    public CanvasGroup topBar;
    //public CanvasGroup bottomTextFieldBar;
    public CanvasGroup ChatSection;

    private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();
    private List<Avatar> _avatarList;
    private bool _isLoading;
    private ListenerRegistration listener;
    private Camera _mainCam;
    private bool isChatShown;

    private readonly Vector3 TEXT_BUBBLE_POS = new Vector3(0, 4.5f, 0);
    private readonly Vector3 LIGHT_SOURCE_LOCAL_POS = new Vector3(0, 5, 0);

    public Avatar SelectedAvatar { get; set; }
    public Avatar TalkingAvatar {  get; set; }  
    public List<Avatar> AvatarList { get { return _avatarList; } }

    public event Action OnARFinishedLoading;
    public event Action<Avatar> OnAvatarStartMessaging;
    // Start is called before the first frame update
    void Start()
    {
        _avatarList = new List<Avatar>();
        _isLoading = true;
        _mainCam = Camera.main;
        FadeInUI();

        //load all avatar of friends and me
        foreach (string email in ChatManager.Instance.EmailToUsersDict.Keys)
        {

            RetrieveAvatarData(email);

        }
        //ListenForNewMessages();
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
        //avatarObj.transform.parent = AvatarContainer.transform;
        avatarObj.transform.SetParent(AvatarContainer.transform, false);
        avatarObj.name = data.email + "_" + "Avatar";
        avatarObj.transform.localScale = PlacedObjectScale * Vector3.one;
        avatarObj.transform.localRotation = Quaternion.identity;

        // Spawn the text bubble prefab and set its position to follow the avatar
        GameObject textBubble = Instantiate(usernameBubblePrefab, avatarObj.transform);
        textBubble.transform.localPosition = TEXT_BUBBLE_POS; // Adjust position as needed
        textBubble.transform.rotation = Quaternion.identity;
        textBubble.GetComponentInChildren<TMP_Text>().text = ChatManager.Instance.EmailToUsersDict[data.email].username;
        //Set accessories correctly

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

        
        return avatar;
    }

    private void PopulateAvatarSelectionBar(Avatar avatarRetrieved, string email)
    {
        AvatarIconContainer avContainer = Instantiate(AvatarIconContainer, AvatarSelectionBar.transform);
        avContainer.name = email;
        avContainer.AttachedAvatar = avatarRetrieved;
        avContainer.GetComponentInChildren<TMP_Text>().text = ChatManager.Instance.EmailToUsersDict[email].username;
    }

    void OnDestroy()
    {
        // Destroy the listener when the scene is changed
        listener?.Stop();
    }

    public void SendMessage()
    {
        if (TalkingAvatar != null)
        {
            ChatManager.Instance.SendMessage(MessageInputField, TalkingAvatar.ConversationData.conversationID);
        }
        else
        {
            Debug.Log("No avatar selected, aborting sending message");
        }
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

        StartCoroutine(ExitRoutine());
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
                    if (touchedAvatar != TalkingAvatar) { 
                        ClearChatDisplay();
                        TalkingAvatar = touchedAvatar;
                        OnAvatarStartMessaging?.Invoke(TalkingAvatar);
                        AnimationManager.Instance.InitializeAnimation(null, TalkingAvatar.gameObject);
                    }
                    //TalkingAvatar = touchedAvatar;
                    Debug.Log(TalkingAvatar.name + " touched");

                    //OnAvatarStartMessaging?.Invoke(TalkingAvatar);
                    return;
                }
            }

            //then check for plane
            bool collision = RaycastManager.Raycast(Input.mousePosition, _raycastHits, TrackableType.PlaneWithinPolygon);
            if (collision)
            {
                SelectedAvatar.gameObject.SetActive(true);
                SelectedAvatar.transform.position = _raycastHits[0].pose.position;
                
                //SelectedAvatar.transform.rotation = _raycastHits[0].pose.rotation;
                Vector3 direction =  _mainCam.transform.position - SelectedAvatar.transform.position;

                // Project the direction onto the XZ plane
                Vector3 xzDirection = new Vector3(direction.x, 0, direction.z).normalized;

                // Calculate the rotation to look at the target on the XZ plane
                Quaternion targetRotation = Quaternion.LookRotation(xzDirection, Vector3.up);
                Debug.Log("Camera Rotation: " + _mainCam.transform.rotation.ToString());
                Debug.Log("Avatar Rotation: " + SelectedAvatar.transform.rotation.ToString());
                Debug.Log("Rotation: " + targetRotation.ToString());
                // Apply the rotation, only affecting the Y-axis
                SelectedAvatar.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
                

                //SelectedAvatar.transform.rotation = Quaternion.Euler(0, SelectedAvatar.transform.rotation.y + angle, 0);
                SelectedAvatar.transform.localScale = this.PlacedObjectScale * Vector3.one;

                //spawning the chat
                if (TalkingAvatar != SelectedAvatar) //if the avatar is placing is not the same as the one talking right now, switch the talking avatar
                {
                    ClearChatDisplay();
                    TalkingAvatar = SelectedAvatar;
                    OnAvatarStartMessaging?.Invoke(TalkingAvatar);
                    AnimationManager.Instance.InitializeAnimation(null, TalkingAvatar.gameObject);
                }
                
                if (!isChatShown)
                {
                    isChatShown = true;
                    ChatSection.gameObject.SetActive(true);
                    UIManager.Instance.PanelFadeIn(ChatSection, 0.5f, UIManager.UIMoveDir.FromBottom, ChatSection.GetComponent<RectTransform>().anchoredPosition);
                }
            }
        }
    }


    public void FadeInUI()
    {
        UIManager.Instance.PanelFadeIn(topBar, 0.5f, UIManager.UIMoveDir.FromTop, topBar.GetComponent<RectTransform>().anchoredPosition);
        UIManager.Instance.PanelFadeIn(ChatSection, 0.5f, UIManager.UIMoveDir.FromBottom, ChatSection.GetComponent<RectTransform>().anchoredPosition);
    }

    public IEnumerator ExitRoutine()
    {
        float waitTime = 0f;
        if (ChatSection.gameObject.activeSelf)
        {
            UIManager.Instance.PanelFadeOut(ChatSection, 0.5f, UIManager.UIMoveDir.FromBottom, ChatSection.GetComponent<RectTransform>().anchoredPosition); //fade out all UI
            waitTime = 0.5f;
        }
        yield return new WaitForSeconds(waitTime);
        AnimationManager.Instance.EndAnimation();
        AppManager.Instance.LoadScene("4-ChatList");
    }

    public void OpenEmoteSelectionTab()
    {
        EmoteSelectionArea.SetActive(!EmoteSelectionArea.activeSelf);
    }

    public void TypeEmoteInMessageInputField(string code)
    {
        MessageInputField.text = MessageInputField.text + code + " ";
    }

    public void MessageInputFieldEmojiUpdate()
    {
        MessageInputField.text = ChatManager.Instance.EmojiUpdate(MessageInputField.text);
    }

 
}
