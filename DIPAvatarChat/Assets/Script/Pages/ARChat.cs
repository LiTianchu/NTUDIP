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
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARChat : PageSingleton<ARChat>
{
    [Header("UI Elements")]
    public TMP_InputField MessageInputField;
    public TMP_Text RecipientName;
    public GameObject MyChatBubblePrefab;
    public GameObject TheirChatBubblePrefab;
    public GameObject ChatBubbleParent;
    public GameObject AvatarContainer;
    public GameObject UsernameContainer;
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
    private ListenerRegistration _listener;
    private List<Avatar> _avatarList;


    private readonly Vector3 LIGHT_SOURCE_LOCAL_POS = new Vector3(0, 5, 0);

    public Avatar SelectedAvatar { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        _avatarList = new List<Avatar>();
        SelectedAvatar = RetrieveAvatar(AuthManager.Instance.currUser.email); //retrieve my avatar
        _avatarList.Add(SelectedAvatar);

        AvatarIconContainer myContainer = Instantiate(AvatarIconContainer, AvatarSelectionBar.transform);
        myContainer.AttachedAvatar = SelectedAvatar;
        myContainer.GetComponentInChildren<TMP_Text>().text = "Me";

        //load all avatar
        foreach (UserData friend in ChatManager.Instance.Friends)
        {
            Avatar avatarRetrieved = RetrieveAvatar(friend.email);
            _avatarList.Add(avatarRetrieved);


            //populate the avatar selection list
            AvatarIconContainer avContainer = Instantiate(AvatarIconContainer, AvatarSelectionBar.transform);
            avContainer.AttachedAvatar = avatarRetrieved;
            avContainer.GetComponentInChildren<TMP_Text>().text = friend.username;
        }

        //iterate through avatar list, activate user's avatar and disable others
        foreach (Avatar av in _avatarList)
        {
            if (av.AvatarData.email.Equals(AuthManager.Instance.currUser.email))
            {
                SelectedAvatar = av;
            }
            else
            {
                av.gameObject.SetActive(false);
            }
        }

        ListenForNewMessages();
    }

    private Avatar RetrieveAvatar(string email)
    {
        bool avatarHasLoaded = ChatManager.Instance.EmailToAvatarDict.TryGetValue(email, out AvatarData data); //check if the avatar is already cached
        if (!avatarHasLoaded) //if not cached, load from database
        {
            AvatarBackendManager.Instance.GetAvatarByEmailTask(email).ContinueWith((task) =>
            {
                data = task.Result.ConvertTo<AvatarData>();
                ChatManager.Instance.EmailToAvatarDict[email] = data;
            });
        }
        GameObject avatarObj = ChatManager.Instance.LoadAvatar(data);
        avatarObj.transform.parent = AvatarContainer.transform;
        avatarObj.name = email+"_"+"Avatar";
        avatarObj.transform.localScale = PlacedObjectScale * Vector3.one;

        //spawn light source
        GameObject lightsource = new GameObject();
        lightsource.transform.parent = avatarObj.transform;
        lightsource.name = "Lightsource";
        Light light = lightsource.AddComponent<Light>();
        light.type = LightType.Point;
        light.intensity = 1;
        lightsource.transform.localPosition = LIGHT_SOURCE_LOCAL_POS;

        Avatar avatar = avatarObj.AddComponent<Avatar>();
        avatar.AvatarData = data;
        return avatar;
    }

    void OnDestroy()
    {
        // Destroy the listener when the scene is changed
        _listener.Stop();
    }

    private async void ListenForNewMessages()
    {
        DocumentReference docRef = await ConversationBackendManager.Instance.GetConversationReferenceTask(AuthManager.Instance.currConvId);
        _listener = docRef.Listen(async snapshot =>
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
                    if (!_isPopulated)
                    {
                        PopulateCachedMessage();
                    }
                    else
                    {
                        // Check if the message has not been displayed already
                        if (GameObject.Find(messageId) == null)
                        {
                            //cache message
                            ChatManager.Instance.CurrentMessages.Add(msg);
                            Debug.Log(AuthManager.Instance.currUser.email + " " + messageId);
                            if (msgSender == AuthManager.Instance.currUser.email)
                            {
                                // Message is sent by the current user, spawn text bubble at right side
                                Debug.Log("Received message from current user");
                                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, MyChatBubblePrefab, msgText, messageId);
                                bubble.transform.localScale = Vector3.one;

                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                Debug.Log("Received message from another user");
                                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, TheirChatBubblePrefab, msgText, messageId);
                                bubble.transform.localScale = Vector3.one;
                            }
                        }
                    }
                }
                else
                {
                    SetRecipientName();
                }
            }
            else
            {
                // Handle the case where the snapshot does not exist or contains invalid data
                Debug.LogError("Snapshot does not exist or contains invalid data.");
            }
        });
    }

    private void PopulateCachedMessage()
    {
        ClearDisplay();
        // Populate the data onto the UI

        foreach (MessageData msg in ChatManager.Instance.CurrentMessages)
        {

            string msgText = msg.message;
            string msgSender = msg.sender;
            string msgReceiver = msg.receiver;
            Timestamp msgTime = msg.createdAt;
            string messageId = msg.messageID;

            // Check if the message has not been displayed already
            // !displayedMessageIds.Contains(messageId)

            if (msgSender.Equals(AuthManager.Instance.emailData))
            {
                // Message is sent by me
                // Spawn text bubble at right side of the chat
                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, MyChatBubblePrefab, msgText, messageId);
                bubble.transform.localScale = Vector3.one;
            }
            else
            {
                // Message is sent by the other party
                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, TheirChatBubblePrefab, msgText, messageId);
                bubble.transform.localScale = Vector3.one;
            }
        }

        SetRecipientName();
        _isPopulated = true;
        if (_isPopulated)
        {
            Debug.Log("Message Populated!");
        }
    }


    public void SetRecipientName()
    {
        RecipientName.text = ChatManager.Instance.CurrentRecipientName;
    }

    public void SendMessage()
    {
        ChatManager.Instance.SendMessage(MessageInputField);
    }

    public void ClearDisplay()
    {
        GameObject[] tempPrefabs;

        tempPrefabs = GameObject.FindGameObjectsWithTag("TempPrefab");

        foreach (GameObject tempPrefab in tempPrefabs)
        {
            Destroy(tempPrefab);
        }
    }

    public void BackToNormalChat()
    {

        AppManager.Instance.LoadScene("4-ChatList");
    }

    public bool IsUIPressed()
    {
        if (Input.touchCount > 0) { 
            Touch touch = Input.GetTouch(0);
            
        }
        return !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
    }

    private bool ClickedOnUi()
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

    public void PlaceAvatar(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(ClickedOnUi()) { Debug.Log("Clicked on UI"); return; }
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
