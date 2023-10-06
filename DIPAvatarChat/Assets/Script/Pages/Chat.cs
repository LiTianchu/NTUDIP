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

    //public static string currConvId { get; set; }
    //ConversationData currConvData;
    private UserData recipientUserData;
    private bool isPopulated = false;
    private ListenerRegistration listener;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Scene 6 Loaded...");
        DisplayAvatars();
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
                            ChatManager.Instance.CurrentMessages.Add(msg);

                            Debug.Log(AuthManager.Instance.currUser.email + " " + messageId);
                            if (msgSender == AuthManager.Instance.currUser.email)
                            {
                                // Message is sent by the current user, spawn text bubble at right side
                                Debug.Log("Received message from current user");
                                ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, MyChatBubblePrefab, msgText, messageId);
                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                Debug.Log("Received message from another user");
                                ChatManager.Instance.InstantiateChatBubble(ChatBubbleParent, TheirChatBubblePrefab, msgText, messageId);
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
            ChatManager.Instance.CurrentMessages.Add(msg);

            if (msgSender.Equals(AuthManager.Instance.emailData))
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

        SetRecipientName();
        isPopulated = true;
        if (isPopulated)
        {
            Debug.Log("Message Populated!");
        }
    }

    public void SendMessage()
    {
        ChatManager.Instance.SendMessage(MessageInputField);
    }

    public async void SetRecipientName()
    {
        recipientUserData = await GetRecipientData();
        RecipientName.text = recipientUserData.username;

        ChatManager.Instance.CurrentRecipientName = recipientUserData.username;
    }

    public async Task<UserData> GetRecipientData()
    {
        DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
        ConversationData currConvData = conversationDoc.ConvertTo<ConversationData>();

        string recipientEmail = null;

        foreach (string member in currConvData.members)
        {
            if (member != AuthManager.Instance.currUser.email)
            {
                recipientEmail = member;
            }
        }

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(recipientEmail);
        UserData userData = userDoc.ConvertTo<UserData>();

        return userData;
    }

    public async void DisplayAvatars()
    {
        if (await GetAvatars())
        {

            GameObject myAvatar = ChatManager.Instance.LoadMyAvatar();
            GameObject theirAvatar = ChatManager.Instance.LoadTheirAvatar();

            //initial settings
            SetAvatar("MyAvatarBody", myAvatar);
            SetAvatar("TheirAvatarBody", theirAvatar);

        }

    }

    private void SetAvatar(string name, GameObject avatarObj) {
        avatarObj.transform.SetParent(AvatarDisplayArea.transform, false);
        avatarObj.name = name;

        float scale = 30f;
        avatarObj.transform.localScale = new Vector3(scale, scale, scale);
    }

    private async Task<bool> GetAvatars()
    {
        try
        {
            DocumentSnapshot currConvDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
            ConversationData currConvData = currConvDoc.ConvertTo<ConversationData>();

            if (currConvData != null)
            {
                foreach (string member in currConvData.members)
                {
                    if (member == AuthManager.Instance.currUser.email)
                    {
                        DocumentSnapshot myAvatarDoc = await AvatarBackendManager.Instance.GetAvatarByEmailTask(member);
                        ChatManager.Instance.MyAvatarData = myAvatarDoc.ConvertTo<AvatarData>();
                    }
                    else
                    {
                        DocumentSnapshot theirAvatarDoc = await AvatarBackendManager.Instance.GetAvatarByEmailTask(member);
                        ChatManager.Instance.TheirAvatarData = theirAvatarDoc.ConvertTo<AvatarData>();
                    }
                }
            }

            Debug.Log("My Avatar: " + ChatManager.Instance.MyAvatarData.avatarId);
            Debug.Log("Their Avatar: " + ChatManager.Instance.TheirAvatarData.avatarId);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Avatar Display Error: " + e.Message);
            return false;
        }
    }


    public void ReturnToChatList()
    {
        ChatManager.Instance.CurrentMessages.Clear();
        ChatManager.Instance.CurrentRecipientName = "";
        ChatManager.Instance.MyAvatarData = null;
        ChatManager.Instance.TheirAvatarData = null;

        AppManager.Instance.LoadScene("4-ChatList");
    }

    public void LoadARChat() {
        //GameObject[] sceneObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        //foreach (GameObject obj in sceneObjects)
        //{
        //    obj.SetActive(false); // Deactivate the GameObject
        //}
        AppManager.Instance.LoadScene("7-ARChat");
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