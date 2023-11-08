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
    public GameObject textBubblePrefab;//avatar label
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
    public CanvasGroup bottomTextFieldBar;

    private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();
    private List<Avatar> _avatarList;
    private bool _isLoading;
    private bool isPopulated = false;
    private ListenerRegistration listener;
    private Camera _mainCam;

    private readonly Vector3 TEXT_BUBBLE_POS = new Vector3(0, 4.5f, 0);
    private readonly Vector3 LIGHT_SOURCE_LOCAL_POS = new Vector3(0, 5, 0);

    public Avatar SelectedAvatar { get; set; }
    public List<Avatar> AvatarList { get { return _avatarList; } }

    public event Action OnARFinishedLoading;
    public event Action<Avatar> OnAvatarSelected;
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

        isPopulated = true;

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
        GameObject textBubble = Instantiate(textBubblePrefab, avatarObj.transform);
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
                
                //Vector3 directionOfCamera = (SelectedAvatar.transform.position - _mainCam.transform.position).normalized;
                //Vector3 forwardDir = SelectedAvatar.transform.forward.normalized;
                //float dotProduct = Vector3.Dot(forwardDir, directionOfCamera);
                //float angle = Mathf.Acos(dotProduct) * Mathf.Rad2Deg;
                //SelectedAvatar.transform.rotation = Quaternion.Euler(0, SelectedAvatar.transform.rotation.y + angle, 0);


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
            }
        }
    }


    public void FadeInUI()
    {
        UIManager.Instance.PanelFadeIn(topBar, 0.5f, UIManager.UIMoveDir.FromTop, topBar.GetComponent<RectTransform>().anchoredPosition);
        UIManager.Instance.PanelFadeIn(bottomTextFieldBar, 0.5f, UIManager.UIMoveDir.FromBottom, bottomTextFieldBar.GetComponent<RectTransform>().anchoredPosition);
    }

    public IEnumerator ExitRoutine()
    {
        UIManager.Instance.PanelFadeOut(bottomTextFieldBar, 0.5f, UIManager.UIMoveDir.FromBottom, bottomTextFieldBar.GetComponent<RectTransform>().anchoredPosition); //fade out all UI
        yield return new WaitForSeconds(0.5f);
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

    /*private async void ListenForNewMessages()
    {
        DocumentReference docRef = await ConversationBackendManager.Instance.GetConversationReferenceTask(AuthManager.Instance.currConvId);
        listener = docRef.Listen(async snapshot =>
        {

            // Check if the snapshot exists and contains valid data
            if (snapshot.Exists)
            {
                Debug.Log("snapshot exists");
                // Extract the new message data
                ConversationData conversation = snapshot.ConvertTo<ConversationData>();

                if (conversation.messages.Last() != null)
                {
                    DocumentSnapshot messageDoc = await MessageBackendManager.Instance.GetMessageByIDTask(conversation.messages.Last());
                    MessageData msg = messageDoc.ConvertTo<MessageData>();

                    string msgSender = msg.sender;
                    string msgText = msg.message;
                    string messageId = messageDoc.Id;

                    // if messages are not loaded in yet
                    if (!isPopulated)
                    {
                        foreach (string email in ChatManager.Instance.EmailToUsersDict.Keys)
                        {

                            RetrieveAvatarData(email);

                        }
                    }
                    else
                    {

                        // Check if the message has not been displayed already
                        if (GameObject.Find(messageId) == null)
                        {
                            //cache message
                            ChatManager.Instance.CacheMessage(conversation.conversationID, msg);

                            Debug.Log(AuthManager.Instance.currUser.email + " " + messageId);
                            if (msgSender == AuthManager.Instance.currUser.email)
                            {
                                // Message is sent by the current user, spawn text bubble at right side
                                string myMsgText = msgText;
                                Debug.Log("Received message from current user " + myMsgText);
                                ChatManager.Instance.InstantiateChatBubble(ScreenChatContainer, MyChatBubblePrefab, myMsgText, messageId);
                                //StartCoroutine(PlayMyEmoteAnimation(popupTime, myMsgText));
                                AnimationManager.Instance.PlayEmoteAnimation(AnimationManager.Instance.myAvatarBodyChat, AnimationManager.Instance.myAnimatorChat, msgText, true);
                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                string theirMsgText = msgText;
                                Debug.Log("Received message from another user " + theirMsgText);
                                ChatManager.Instance.InstantiateChatBubble(ScreenChatContainer, TheirChatBubblePrefab, theirMsgText, messageId);
                                //StartCoroutine(PlayTheirEmoteAnimation(popupTime, theirMsgText));
                                AnimationManager.Instance.PlayEmoteAnimation(AnimationManager.Instance.theirAvatarBodyChat, AnimationManager.Instance.theirAnimatorChat, msgText, false);
                            }
                        }
                    }
                }
            }
            else
            {
                // Handle the case where the snapshot does not exist or contains invalid data
                Debug.LogError("Snapshot does not exist or contains invalid data.");
            }
        });
    }*/
}
