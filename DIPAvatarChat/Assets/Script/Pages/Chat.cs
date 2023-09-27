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
    FirebaseFirestore db;

    public TMP_InputField MessageInputField;
    public TMP_Text RecipientName;
    public GameObject MyChatBubblePrefab;
    public GameObject TheirChatBubblePrefab;
    public GameObject ChatBubbleParent;

    //public static string currConvId { get; set; }
    ConversationData currConvData;
    UserData recipientUserData;
    private List<string> displayedMessageIds = new List<string>();


    // Start is called before the first frame update
    void Start()
    {
        // Initialize displayedMessageIds
        PopulateMessage(AuthManager.Instance.currConvId);
        SetRecipientName();
        ListenForNewMessages(); // Start listening for new messages
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Pressed enter key!");
            SendMessage();
        }

    }
    private async void ListenForNewMessages()
    {
        db = FirebaseFirestore.DefaultInstance;
        DocumentReference docRef = db.Collection("conversation").Document(AuthManager.Instance.currConvId);
        docRef.Listen(async snapshot =>
        {
            Debug.Log("New message document received!");
            Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));
            Dictionary<string, object> city = snapshot.ToDictionary();
            foreach (KeyValuePair<string, object> pair in city)
            {
                Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
            }

            // Check if the snapshot exists and contains valid data
            if (snapshot.Exists)
            {
                Debug.Log("snapshot exists");
                // Extract the new message data
                ConversationData conversation = snapshot.ConvertTo<ConversationData>();

                DocumentSnapshot messageDoc = await MessageBackendManager.Instance.GetMessageByIDTask(conversation.messages[conversation.messages.Count - 1]);
                MessageData msg= messageDoc.ConvertTo<MessageData>();

                string msgSender = msg.sender;
                string msgText = msg.message;
                string messageId = messageDoc.Id;



                // Check if the message has not been displayed already
                if (!displayedMessageIds.Contains(messageId))
                {
                    Debug.Log(AuthManager.Instance.currUser.email);
                    if (msgSender == AuthManager.Instance.currUser.email)
                    {
                        // Message is sent by the current user, spawn text bubble at right side
                        Debug.Log("Received message from current user");
                        InstantiateChatBubble(MyChatBubblePrefab, msgText, messageId);
                    }
                    else
                    {
                        // Message is sent by another user, spawn text bubble at left side
                        Debug.Log("Received message from another user");
                        InstantiateChatBubble(TheirChatBubblePrefab, msgText, messageId);
                    }

                    // Add the message ID to the list of displayed messages
                    displayedMessageIds.Add(messageId);
                }
            }
            else
            {
                // Handle the case where the snapshot does not exist or contains invalid data
                Debug.LogError("Snapshot does not exist or contains invalid data.");
            }
        });
    }

    public async void SendMessage()
    {
        string myEmail = AuthManager.Instance.currUser.email;
        string theirEmail = null;

        foreach (string member in currConvData.members)
        {
            if (member != myEmail)
            {
                theirEmail = member;
            }
        }
        if (MessageInputField.text != null && MessageInputField.text != "")
        {
            bool IsMessageSent = await MessageBackendManager.Instance.SendMessageTask(currConvData, MessageInputField.text, myEmail, theirEmail);
            if (IsMessageSent)
            {
                // Generate a timestamp-based message ID
                //string messageId = DateTime.Now.ToString("yyyyMMddHHmmssffff"); // Format: YYYYMMDDHHmmssffff

                // Display the sent message on the right side (sender's side)
                //InstantiateChatBubble(MyChatBubblePrefab, MessageInputField.text, messageId);

                // Add the new message ID to the list
                //displayedMessageIds.Add(messageId);

                // Clear the input field
                MessageInputField.text = "";
            }
        }
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
            if (!displayedMessageIds.Contains(messageId))
            {
                if (msgSender.Equals(AuthManager.Instance.emailData))
                { // Message is sent by me
                  // Spawn text bubble at right side of the chat
                    InstantiateChatBubble(MyChatBubblePrefab, msgText, messageId);
                }
                else
                { // Message is sent by other user
                  // Spawn text bubble at left side of the chat
                    InstantiateChatBubble(TheirChatBubblePrefab, msgText, messageId);

                    // Add the message ID to the list of displayed messages
                    displayedMessageIds.Add(messageId);
                }
            }
        }
    }

    /*private void UpdateChatWithNewMessage(DocumentSnapshot snapshot)
    {
        // Extract the new message data
        MessageData msg = snapshot.ConvertTo<MessageData>();
        string msgText = msg.message;
        string msgSender = msg.sender;
        string messageId = snapshot.Id;

        // Check if the message has not been displayed already
        if (!displayedMessageIds.Contains(messageId))
        {
            /*if (msgSender.Equals(AuthManager.Instance.emailData))
            { // Message is sent by me
                // Spawn text bubble at right side of the chat
                InstantiateChatBubble(MyChatBubblePrefab, msgText, messageId);
            }
            else
            // Message is sent by other user
                // Spawn text bubble at left side of the chat
                InstantiateChatBubble(TheirChatBubblePrefab, msgText, messageId);
            
    
            // Add the message ID to the list of displayed messages
            displayedMessageIds.Add(messageId);
        }
    }*/



    public void InstantiateChatBubble(GameObject ChatBubblePrefab, string msgText, string messageId)
    {
        GameObject box = Instantiate(ChatBubblePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        box.transform.SetParent(ChatBubbleParent.transform, false);
        box.name = messageId;

        box.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = msgText;
    }

    public async void SetRecipientName()
    {
        recipientUserData = await GetRecipientData();
        RecipientName.text = recipientUserData.username;
    }
    public async Task<UserData> GetRecipientData()
    {
        DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
        currConvData = ConversationBackendManager.Instance.ProcessConversationDocument(conversationDoc);

        string recipientEmail = null;

        foreach (string member in currConvData.members)
        {
            if (member != AuthManager.Instance.currUser.email)
            {
                recipientEmail = member;
            }
        }

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(recipientEmail);
        UserData userData = UserBackendManager.Instance.ProcessUserDocument(userDoc);

        return userData;
    }

    public void ReturnToChatList()
    {
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