using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARChat : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField MessageInputField;
    public TMP_Text RecipientName;
    public GameObject MyChatBubblePrefab;
    public GameObject TheirChatBubblePrefab;
    public GameObject ARChatBubbleContainer;
    public GameObject AvatarContainer;
    public GameObject UsernameContainer;
    
    [Header("AR")]
    public XROrigin XrOrigin;
    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;
    public float PlacedObjectScale = 0.5f;
    public Vector3 TextBubblePos;
    public Vector3 NamePos;

    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

    private bool isPopulated = false;
    private ListenerRegistration listener;
    private GameObject myAvatar;
    private GameObject theirAvatar;
    private GameObject selectedAvatar;
    private GameObject myARChatBubbleContainer;
    private GameObject theirARChatBubbleContainer;
    // Start is called before the first frame update
    void Start()
    {
        myAvatar = ChatManager.Instance.LoadMyAvatar();
        theirAvatar = ChatManager.Instance.LoadTheirAvatar();

        myARChatBubbleContainer = Instantiate(ARChatBubbleContainer, myAvatar.transform) as GameObject;
        myARChatBubbleContainer.transform.parent = myAvatar.transform;
        myARChatBubbleContainer.transform.localPosition = TextBubblePos;
        myARChatBubbleContainer.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);

        theirARChatBubbleContainer = Instantiate(ARChatBubbleContainer, theirAvatar.transform) as GameObject;
        theirARChatBubbleContainer.transform.parent = theirAvatar.transform;
        theirARChatBubbleContainer.transform.localPosition = TextBubblePos;

        myAvatar.SetActive(false);
        theirAvatar.SetActive(false);
        selectedAvatar = myAvatar;

        ListenForNewMessages();
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
                                GameObject parent = myARChatBubbleContainer.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
                                ChatManager.Instance.InstantiateChatBubble(parent, MyChatBubblePrefab, msgText, messageId);
                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                Debug.Log("Received message from another user");
                                GameObject parent = theirARChatBubbleContainer.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
                                ChatManager.Instance.InstantiateChatBubble(parent, TheirChatBubblePrefab, msgText, messageId);
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
                GameObject parent = myARChatBubbleContainer.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
                ChatManager.Instance.InstantiateChatBubble(parent, MyChatBubblePrefab, msgText, messageId);
            }
            else
            {
                // Message is sent by the other party
                GameObject parent = theirARChatBubbleContainer.GetComponentInChildren<VerticalLayoutGroup>().gameObject;
                ChatManager.Instance.InstantiateChatBubble(parent, TheirChatBubblePrefab, msgText, messageId);
            }
        }

        SetRecipientName();
        isPopulated = true;
        if (isPopulated)
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

    public void BackToNormalChat() {

        AppManager.Instance.LoadScene("6-ChatUI");
    }

    public void PlaceAvatar(InputAction.CallbackContext context) {
        if (context.performed) {
            bool collision = RaycastManager.Raycast(Input.mousePosition, raycastHits, TrackableType.PlaneWithinPolygon);

            if (collision)
            {
                selectedAvatar.SetActive(true);
                selectedAvatar.transform.position = raycastHits[0].pose.position;
                selectedAvatar.transform.rotation = raycastHits[0].pose.rotation;
                selectedAvatar.transform.localScale = this.PlacedObjectScale * Vector3.one;
            }
        }
        


    }
    
}
