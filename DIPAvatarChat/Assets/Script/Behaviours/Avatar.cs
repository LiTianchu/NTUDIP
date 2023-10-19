using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Avatar : MonoBehaviour
{

    private ListenerRegistration _listener;
    private bool _isMsgReceived;

    public AvatarData AvatarData { get; set; }
    public GameObject ChatBubblePrefab { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        ListenForNewMessages();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    if (!_isMsgReceived)
                    {
                        PopulateCachedMessage();
                    }
                    else
                    {
                        // Check if the message has not been displayed already
                        if (GameObject.Find(messageId) == null)
                        {
                            //cache message
                            ChatManager.Instance.ConvIDToMessageDataDict[conversation.conversationID].Add(msg);
                            Debug.Log(AuthManager.Instance.currUser.email + " " + messageId);
                            if (msgSender == AuthManager.Instance.currUser.email)
                            {
                                // Message is sent by the current user, spawn text bubble at right side
                                Debug.Log("Received message from current user");
                                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(this.gameObject, ChatBubblePrefab, msgText, messageId);
                                bubble.transform.localScale = Vector3.one;

                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                Debug.Log("Received message from another user");
                                GameObject bubble = ChatManager.Instance.InstantiateChatBubble(this.gameObject, ChatBubblePrefab, msgText, messageId);
                                bubble.transform.localScale = Vector3.one;
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

    private void PopulateCachedMessage() { }

}
