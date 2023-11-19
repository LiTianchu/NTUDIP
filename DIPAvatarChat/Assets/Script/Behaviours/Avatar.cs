using Firebase.Firestore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Avatar : MonoBehaviour
{

    private ListenerRegistration _listener;
    private bool _isMsgReceived;
    private ConversationData _conversationData;
    public AvatarData AvatarData { get; set; }
    public ConversationData ConversationData { get { return _conversationData;} }
    // Start is called before the first frame update
    void Start()
    {
        //ListenForNewMessages();
    }
    private void OnEnable()
    {
        ARChat.Instance.OnAvatarStartMessaging += HandleAvatarSelected;
    }

    private void OnDisable()
    {
        if (!this.gameObject.scene.isLoaded) return;
        _listener?.Stop();
        ARChat.Instance.OnAvatarStartMessaging -= HandleAvatarSelected;
    }

    private void HandleAvatarSelected(Avatar avatarSelected)
    {
        if (avatarSelected == this) //if the selected avatar is this avatar, register message listener
        {
            _isMsgReceived = false;
            ListenForNewMessages();
        }
        else //if not this avatar, stop listening for messages
        {
            _listener?.Stop();
        }
    }


    public async void ListenForNewMessages()
    {
        _conversationData = ChatManager.Instance.EmailToConversationDict[AvatarData.email];
        DocumentReference docRef = await ConversationBackendManager.Instance.GetConversationReferenceTask(_conversationData.conversationID);
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
                    if (!_isMsgReceived)
                    {
                        PopulateMessage();
                    }
                    else
                    {
                        // Check if the message has not been displayed already
                        if (GameObject.Find(messageId) == null)
                        {
                            //cache message
                            if (!ChatManager.Instance.ConvIDToMessageDataDict.ContainsKey(conversation.conversationID))
                            {
                                ChatManager.Instance.ConvIDToMessageDataDict[conversation.conversationID] = new HashSet<MessageData>();
                            }
                            ChatManager.Instance.ConvIDToMessageDataDict[conversation.conversationID].Add(msg);

                            Debug.Log(AuthManager.Instance.currUser.email + " " + messageId);
                            if (msgSender == AuthManager.Instance.currUser.email)
                            {
                                // Message is sent by the current user, spawn text bubble at right side
                                Debug.Log("Received message from current user");
                                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ARChat.Instance.ScreenChatContainer, ARChat.Instance.MyChatBubblePrefab, msgText, messageId);
                                bubble.transform.localScale = Vector3.one;
                                AnimationManager.Instance.PlayEmoteAnimation(msgText, false, true);

                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                Debug.Log("Received message from another user");
                                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ARChat.Instance.ScreenChatContainer, ARChat.Instance.TheirChatBubblePrefab, msgText, messageId);
                                bubble.transform.localScale = Vector3.one;
                                AnimationManager.Instance.PlayEmoteAnimation(msgText, false, false);
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

    private async void PopulateMessage()
    {
        string email = AvatarData.email;
        Debug.Log("AvatarData email: " + AvatarData.email + "| Conversation ID: " + _conversationData.conversationID);

        if (!ChatManager.Instance.ConvIDToMessageDataDict.ContainsKey(_conversationData.conversationID))
        {
            QuerySnapshot messages = await MessageBackendManager.Instance.GetAllMessagesTask(_conversationData.conversationID);
            foreach (DocumentSnapshot message in messages.Documents)
            {
                MessageData msg = message.ConvertTo<MessageData>();

                //cache the msg
                ChatManager.Instance.CacheMessage(_conversationData.conversationID, msg);


            }
        }
        if (!ChatManager.Instance.ConvIDToMessageDataDict.ContainsKey(_conversationData.conversationID))
        {  //check if there is no message
            Debug.Log("Has no message with user " + email);
            return; //return early
        }

        HashSet<MessageData> messageList = ChatManager.Instance.ConvIDToMessageDataDict[_conversationData.conversationID];

        // Populate the data onto the UI
        foreach (MessageData msg in messageList)
        {

            string msgText = msg.message;
            string msgSender = msg.sender;
            string messageId = msg.messageID;

            // Check if the message has not been displayed already
            if (msgSender.Equals(AuthManager.Instance.currUser.email))
            {
                // Message is sent by me
                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ARChat.Instance.ScreenChatContainer, ARChat.Instance.MyChatBubblePrefab, msgText, messageId);
                bubble.transform.localScale = Vector3.one;
            }
            else
            {
                // Message is sent by the other party
                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(ARChat.Instance.ScreenChatContainer, ARChat.Instance.TheirChatBubblePrefab, msgText, messageId);
                bubble.transform.localScale = Vector3.one;
            }
        }

        _isMsgReceived = true;
        Debug.Log("Message Populated!");

    }



}
