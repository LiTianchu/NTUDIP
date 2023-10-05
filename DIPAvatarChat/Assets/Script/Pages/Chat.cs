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
    UserData recipientUserData;
    bool isPopulated = false;
    ListenerRegistration listener;

    AvatarData myAvatarData = null;
    AvatarData theirAvatarData = null;


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
            SendMessage();
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
                            Debug.Log(AuthManager.Instance.currUser.email + " " + messageId);
                            if (msgSender == AuthManager.Instance.currUser.email)
                            {
                                // Message is sent by the current user, spawn text bubble at right side
                                Debug.Log("Received message from current user");
                                InstantiateChatBubble(ChatBubbleParent, MyChatBubblePrefab, msgText, messageId);
                            }
                            else
                            {
                                // Message is sent by another user, spawn text bubble at left side
                                Debug.Log("Received message from another user");
                                InstantiateChatBubble(ChatBubbleParent, TheirChatBubblePrefab, msgText, messageId);
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

    public async void SendMessage()
    {
        string myEmail = AuthManager.Instance.currUser.email;
        string theirEmail = null;

        DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
        ConversationData currConvData = conversationDoc.ConvertTo<ConversationData>();

        Debug.Log(currConvData.messages.Count + " in SendMessage.");

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
                MessageInputField.text = "";
            }
        }
        else
        {
            Debug.Log("message is null...");
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
            // !displayedMessageIds.Contains(messageId)

            if (msgSender.Equals(AuthManager.Instance.emailData))
            {
                // Message is sent by me
                // Spawn text bubble at right side of the chat
                InstantiateChatBubble(ChatBubbleParent, MyChatBubblePrefab, msgText, messageId);
            }
            else
            {
                // Message is sent by the other party
                InstantiateChatBubble(ChatBubbleParent, TheirChatBubblePrefab, msgText, messageId);
            }
        }

        SetRecipientName();
        isPopulated = true;
        if (isPopulated)
        {
            Debug.Log("Message Populated!");
        }
    }

    public void InstantiateChatBubble(GameObject _ChatBubbleParent, GameObject _ChatBubblePrefab, string msgText, string messageId)
    {
        GameObject box = Instantiate(_ChatBubblePrefab, _ChatBubbleParent.transform) as GameObject;
        box.name = messageId;

        box.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = msgText;
        //box.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = msgText;
    }

    public async void SetRecipientName()
    {
        recipientUserData = await GetRecipientData();
        RecipientName.text = recipientUserData.username;
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
            Vector3 myAvatarSpawnPosition = new Vector3(-30f, 10f, -30f);
            Vector3 theirAvatarSpawnPosition = new Vector3(40f, 10f, -30f);

            // Spawn both avatar bodies
            LoadAvatarBody("Blender/CatBaseTest2_v0_30", AvatarDisplayArea, myAvatarSpawnPosition, Quaternion.Euler(0f, -75f, 0f), "MyAvatarBody");
            LoadAvatarBody("Blender/CatBaseTest2_v0_30", AvatarDisplayArea, theirAvatarSpawnPosition, Quaternion.Euler(0f, 75f, 0f), "TheirAvatarBody");

            // Load hat accessory
            Vector3 hatPosition = new Vector3(0f, 3.6f, 0f);
            Vector3 hatScale = new Vector3(0.2f, 0.2f, 0.2f);
            LoadAccessory(myAvatarData.hat, AvatarDisplayArea.transform.GetChild(0).gameObject, hatPosition, hatScale);
            LoadAccessory(theirAvatarData.hat, AvatarDisplayArea.transform.GetChild(1).gameObject, hatPosition, hatScale);

            // Load arm accessory
            Vector3 armPosition = new Vector3(-1.087f, 1.953f, 0f);
            Vector3 armPosition2 = new Vector3(1.087f, 1.953f, 0f);
            Vector3 armScale = new Vector3(0.08f, 0.08f, 0.08f);
            LoadAccessory(myAvatarData.arm, AvatarDisplayArea.transform.GetChild(0).gameObject, armPosition, armScale);
            LoadAccessory(theirAvatarData.arm, AvatarDisplayArea.transform.GetChild(1).gameObject, armPosition2, armScale);

            // Todo: Load all the other accessory
        }
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
                        myAvatarData = myAvatarDoc.ConvertTo<AvatarData>();
                    }
                    else
                    {
                        DocumentSnapshot theirAvatarDoc = await AvatarBackendManager.Instance.GetAvatarByEmailTask(member);
                        theirAvatarData = theirAvatarDoc.ConvertTo<AvatarData>();
                    }
                }
            }

            Debug.Log("My Avatar: " + myAvatarData.avatarId);
            Debug.Log("Their Avatar: " + theirAvatarData.avatarId);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Avatar Display Error: " + e.Message);
            return false;
        }
    }

    private void LoadAvatarBody(string avatarBaseFbxFileName, GameObject AvatarDisplayArea, Vector3 itemPosition, Quaternion itemRotation, string avatarName)
    {
        if (avatarBaseFbxFileName != null && avatarBaseFbxFileName != "")
        {
            GameObject loadedFBX = Resources.Load<GameObject>(avatarBaseFbxFileName); // Eg. Blender/catbasetest.fbx

            if (loadedFBX != null)
            {
                GameObject fbx = Instantiate(loadedFBX, itemPosition, itemRotation);
                fbx.transform.SetParent(AvatarDisplayArea.transform, false);
                fbx.name = avatarName;

                float scale = 30f;
                fbx.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                Debug.LogError("FBX asset not found: " + avatarBaseFbxFileName);
            }
        }
    }

    private void LoadAccessory(string fbxFileName, GameObject AvatarBody, Vector3 itemPosition, Vector3 itemScale)
    {
        if (fbxFileName != null && fbxFileName != "")
        {
            // Load the FBX asset from the Resources folder
            GameObject loadedFBX = Resources.Load<GameObject>(fbxFileName); // Eg. Blender/porkpiehat.fbx

            if (loadedFBX != null)
            {
                // Instantiate the loaded FBX as a GameObject in the scene
                GameObject fbx = Instantiate(loadedFBX, itemPosition, Quaternion.identity);
                fbx.transform.SetParent(AvatarBody.transform, false);
                fbx.transform.localScale = itemScale;
            }
            else
            {
                Debug.LogError("FBX asset not found: " + fbxFileName);
            }
        }
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