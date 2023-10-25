using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public TMP_InputField MessageInputField;
    public TMP_Text RecipientName;
    public GameObject MyChatBubblePrefab;
    public GameObject TheirChatBubblePrefab;
    public GameObject ChatBubbleParent;
    public GameObject AvatarDisplayArea;
    public GameObject AvatarPopupDisplayArea;
    //public RuntimeAnimatorController animatorController;

    //public static string currConvId { get; set; }
    //ConversationData currConvData;
    private UserData recipientUserData;
    private bool isPopulated = false;
    private ListenerRegistration listener;
    private GameObject myAvatarBody;
    private GameObject theirAvatarBody;

    private readonly float AVATAR_SCALE_CHAT = 30f;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Scene 6 Loaded...");
        InitializeChatData();
        //DisplayAvatars();
        ListenForNewMessages(); // Start listening for new messages
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Pressed enter key!");
            ChatManager.Instance.SendMessage(MessageInputField);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pressed spacebar key!");
        }
    }

    void OnDestroy()
    {
        // Destroy the listener when the scene is changed
        listener.Stop();
    }

    private async void InitializeChatData()
    {
        //display username
        recipientUserData = await GetRecipientData();
        RecipientName.text = recipientUserData.username;

        ChatManager.Instance.CurrentRecipientName = recipientUserData.username;

        //display avatar
        if (await AvatarBackendManager.Instance.GetAvatarsForChat())
        {
            GameObject myAvatar = ChatManager.Instance.LoadAvatar(AuthManager.Instance.currUser.email);
            GameObject theirAvatar = ChatManager.Instance.LoadAvatar(recipientUserData.email);


            //initial settings
            SetAvatar("MyAvatarBody", myAvatar, AvatarDisplayArea, ChatManager.Instance.MY_AVATAR_POS, ChatManager.Instance.MY_AVATAR_ROTATION);
            SetAvatar("TheirAvatarBody", theirAvatar, AvatarDisplayArea, ChatManager.Instance.THEIR_AVATAR_POS, ChatManager.Instance.THEIR_AVATAR_ROTATION);

            //Display popup avatar when click on friend's avatar
            GameObject popupAvatar = ChatManager.Instance.LoadAvatar(recipientUserData.email);
            SetAvatar("PopupAvatarBody", popupAvatar, AvatarPopupDisplayArea, ChatManager.Instance.POPUP_AVATAR_POS, Quaternion.identity);

            myAvatarBody = GameObject.Find("/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/MyAvatarBody");
            theirAvatarBody = GameObject.Find("/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/theirAvatarBody");
        }
    }

    private async void ListenForNewMessages()
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
                        PopulateMessage(AuthManager.Instance.currConvId);
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
                                Debug.Log("Received message from current user");
                                ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, MyChatBubblePrefab, msgText, messageId);
                                ChatManager.Instance.PlayAnimation(myAvatarBody, msgText);
                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                Debug.Log("Received message from another user");
                                ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, TheirChatBubblePrefab, msgText, messageId);
                                ChatManager.Instance.PlayAnimation(theirAvatarBody, msgText);
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
    }



    private async void PopulateMessage(string conversationID)
    {
        ClearDisplay();
        // Populate the data onto the UI
        QuerySnapshot messages = await MessageBackendManager.Instance.GetAllMessagesTask(conversationID);
        foreach (DocumentSnapshot message in messages.Documents)
        {
            MessageData msg = message.ConvertTo<MessageData>();
            string msgText = msg.message;
            string msgSender = msg.sender;
            string msgReceiver = msg.receiver;
            Timestamp msgTime = msg.createdAt;
            string messageId = message.Id;

            // Check if the message has not been displayed already
            // !displayedMessageIds.Contains(messageId)

            //cache the msg
            ChatManager.Instance.CacheMessage(conversationID, msg);

            if (msgSender.Equals(AuthManager.Instance.currUser.email))
            {
                // Message is sent by me
                // Spawn text bubble at right side of the chat
                ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, MyChatBubblePrefab, msgText, messageId);
            }
            else
            {
                // Message is sent by the other party
                ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, TheirChatBubblePrefab, msgText, messageId);
            }
        }


        isPopulated = true;
        if (isPopulated)
        {
            Debug.Log("Message Populated!");
        }
    }

    public void SendMessage()
    {
        if (!UIManager.Instance.isCooldown || UIManager.Instance.isCooldown == null)
        {
            StartCoroutine(UIManager.Instance.StartCooldown(0.25f));

            // Perform your button click actions here
            ChatManager.Instance.SendMessage(MessageInputField);
            Debug.Log("Button clicked!");
        }
        else
        {
            Debug.Log("Button on cooldown!");
        }
    }



    //public async void SetRecipientName()
    //{
    //    recipientUserData = await GetRecipientData();
    //    RecipientName.text = recipientUserData.username;

    //    ChatManager.Instance.CurrentRecipientName = recipientUserData.username;
    //}

    public async Task<UserData> GetRecipientData()
    {
        DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
        ConversationData currConvData = conversationDoc.ConvertTo<ConversationData>();

        string recipientEmail = null;

        foreach (string member in currConvData.members)
        {
            if (!member.Equals(AuthManager.Instance.currUser.email))
            {
                recipientEmail = member;
            }
        }

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(recipientEmail);
        UserData userData = userDoc.ConvertTo<UserData>();

        return userData;
    }

    //public async void DisplayAvatars()
    //{
    //    if (await AvatarBackendManager.Instance.GetAvatarsForChat())
    //    {
    //        GameObject myAvatar = ChatManager.Instance.LoadAvatar(AuthManager.Instance.currUser.email);
    //        GameObject theirAvatar = ChatManager.Instance.LoadAvatar(recipientUserData.email);


    //        //initial settings
    //        SetAvatar("MyAvatarBody", myAvatar, AvatarDisplayArea, ChatManager.Instance.MY_AVATAR_POS, ChatManager.Instance.MY_AVATAR_ROTATION);
    //        SetAvatar("TheirAvatarBody", theirAvatar, AvatarDisplayArea, ChatManager.Instance.THEIR_AVATAR_POS,ChatManager.Instance.THEIR_AVATAR_ROTATION);

    //        //Display popup avatar when click on friend's avatar
    //        GameObject popupAvatar = ChatManager.Instance.LoadAvatar(recipientUserData.email);
    //        SetAvatar("PopupAvatarBody", popupAvatar, AvatarPopupDisplayArea,ChatManager.Instance.POPUP_AVATAR_POS,Quaternion.identity);
    //    }
    //}

    private void SetAvatar(string name, GameObject avatarObj, GameObject avatarParent, Vector3 pos, Quaternion rot)
    {
        avatarObj.transform.SetParent(avatarParent.transform, false);
        avatarObj.name = name;
        avatarObj.transform.localPosition = pos;
        avatarObj.transform.localRotation = rot;

        float scale = AVATAR_SCALE_CHAT;
        avatarObj.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void ReturnToChatList()
    {
        //ChatManager.Instance.CurrentMessages.Clear();
        ChatManager.Instance.CurrentRecipientName = "";
        //ChatManager.Instance.MyAvatarData = null;
        //ChatManager.Instance.TheirAvatarData = null;

        AppManager.Instance.LoadScene("4-ChatList");
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
}