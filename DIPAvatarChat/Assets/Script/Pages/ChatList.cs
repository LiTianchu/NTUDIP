using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChatList : MonoBehaviour
{

    public TMP_InputField emailSearchBar;
    public TMP_Text SearchNameDisplay;
    public TMP_Text SearchEmailDisplay;
    public TMP_Text SearchStatusDisplay;
    public GameObject searchFriendTab;
    public GameObject searchFriendInfoTab;
    public GameObject friendRequestsTab;
    public GameObject friendRequestBoxPrefab;
    public Button SendFriendRequestBtn;

    List<string> friendRequestsList;
    List<string> friendsList;
    string usernameData;
    string emailData;
    string statusData;
    string friendRequestData;

    // Start is called before the first frame update
    void Start()
    {

        //retrieve conversation list to populate chat list
        //List<string> conversations = UserBackendManager.Instance.currentUser.conversations;
        //foreach (string conversationID in conversations)
        //{
        //    if (conversationID != null && conversationID != "")
        //    {
        //        ConversationBackendManager.Instance.GetConversationByID(conversationID);
        //    }
        //}
        PopulateChatList();
        //attach event listeners for user data
        UserBackendManager.Instance.SearchUserFriendRequestsReceived += DisplayFriendRequestsData;
        UserBackendManager.Instance.OtherUserDataReceived += FriendRequestsData;

        //attach event listeners for conversation data
        //ConversationBackendManager.Instance.ConversationDataRetrieved += GenerateChat;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        //attach event listeners on enable
        //Idk why this function is called twice when only one script is enabled in the scene
        //After looking at forum it seems like a stupid Unity bug
        //So I moved the event listener assignment to Start() instead - Tianchu

    }

    private void OnDisable()
    {
        //attach event listeners on disable
        if (!this.gameObject.scene.isLoaded) return;
        UserBackendManager.Instance.SearchUserFriendRequestsReceived -= DisplayFriendRequestsData;
        //ConversationBackendManager.Instance.ConversationDataRetrieved -= GenerateChat;
        UserBackendManager.Instance.OtherUserDataReceived -= FriendRequestsData;
    }

    async public void PopulateChatList()
    {
        //Getting the data by task
        ConversationData conversation = null;
        MessageData latestMessage = null;
        UserData sender = null;

        List<string> conversations = UserBackendManager.Instance.currentUser.conversations;

        foreach (string conversationID in conversations)
        {

            if (conversationID != null && conversationID != "")
            {
                //get conversation document
                DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(conversationID);
                conversation = ConversationBackendManager.Instance.ProcessConversationDocument(conversationDoc);

                //get message document and retrieve the message details and the user
                if (conversation.messages != null && conversation.messages.Count > 0)
                {
                    DocumentSnapshot messageDoc = await MessageBackendManager.Instance.GetMessageByIDTask(conversation.messages[conversation.messages.Count - 1]);
                    latestMessage = MessageBackendManager.Instance.ProcessMessageDocument(messageDoc);
                    DocumentSnapshot userDoc = await UserBackendManager.Instance.GetOtherUserTask(latestMessage.sender);
                    sender = UserBackendManager.Instance.ProcessUserDocument(userDoc);
                    if (sender == null)
                    {
                        Debug.Log(latestMessage.sender + " has no corresponding document");
                    }
                }
                else
                {
                    //handle empty conversation
                    latestMessage = new MessageData();
                    latestMessage.message = "No messages yet";
                    DocumentSnapshot userDoc = await UserBackendManager.Instance.GetOtherUserTask(conversation.members[0]);
                    sender = UserBackendManager.Instance.ProcessUserDocument(userDoc);
                }


            }
        }

        Debug.Log("Latest Message: " + latestMessage?.message);
        Debug.Log("Latest Sender: " + sender?.username);

        Debug.Log("Latest Message Timestamp: " + latestMessage?.createdAt);



        //string chatOpponentEmail = "";
        //foreach (string member in conversation.members)
        //{
        //    if (!member.Equals(UserBackendManager.Instance.currentUser.email))
        //    {
        //        chatOpponentEmail = member;
        //        break;
        //    }
        //}

        ////string chatDesc = conversation.description;
        //string latestMessageID = conversation.messages[conversation.messages.Count - 1];

        //FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        //db.Collection("message").Document(latestMessageID).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //{
        //    DocumentSnapshot messageSnapshot = task.Result;
        //    Dictionary<string, object> temp = messageSnapshot.ToDictionary();
        //    MessageData messageData = MessageBackendManager.Instance.DictionaryToMessageData(temp);

        //});
        //db.Collection("user").Document(chatOpponentEmail).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        //{
        //    DocumentSnapshot userSnapshot = task.Result;
        //    Dictionary<string, object> temp = userSnapshot.ToDictionary();
        //    string opponentAvatar = temp["currentAvatar"].ToString();
        //});
        ////Instantiate chat thumbnail
        //Debug.Log(chatOpponentEmail);
        //Debug.Log(conversation.description);
        ////TODO: Generate chat UI, use the data to spawn the chat thumbnail

    }



    public void NewChat()
    {
        AppManager.Instance.LoadScene("5-NewChat");
    }

    public void EnterChat(string chatID)
    {
        //set as temp data storage to pass to next scene
        PlayerPrefs.SetString("chatID", chatID);
        AppManager.Instance.LoadScene("6-ChatUI");
    }

    async public void SearchUserByEmailAsync()
    {
        EnableSearchFriendInfoTab();

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetOtherUserTask(emailSearchBar.text);
        DisplaySearchUserData(UserBackendManager.Instance.ProcessUserDocument(userDoc));

        SendFriendRequestBtn.interactable = true;
    }

    public void SendFriendRequest()
    {
        //hardcoded test
        //UserBackendManager.Instance.SendFriendRequest(friendRequestsList, emailSearchBar.text, "bbbb@gmail.com");

        UserBackendManager.Instance.SendFriendRequest(friendsList, friendRequestsList, emailSearchBar.text, RegisterAndLogin.emailData);

        SendFriendRequestBtn.interactable = false;
    }

    async public void DisplayFriendRequests()
    {
        ToggleFriendRequestsTab();
        Debug.Log(RegisterAndLogin.emailData);

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetCurrentUserTask();
        DisplayFriendRequestsData(UserBackendManager.Instance.ProcessUserDocument(myUserDoc));
    }

    public void DisplaySearchUserData(UserData userData)
    {
        Debug.Log("User Data Retrieved");
        Debug.Log(userData.username);
        Debug.Log(userData.email);
        Debug.Log(userData.status);
        Debug.Log(userData.friendRequests);
        Debug.Log(userData.friends);

        usernameData = userData.username;
        emailData = userData.email;
        statusData = userData.status;
        friendRequestsList = userData.friendRequests;
        friendsList = userData.friends;

        SearchNameDisplay.text = usernameData;
        SearchEmailDisplay.text = emailData;
        SearchStatusDisplay.text = statusData;
    }

    async public void DisplayFriendRequestsData(UserData userData)
    {
        Debug.Log("User Data Retrieved");

        usernameData = userData.username;
        emailData = userData.email;
        statusData = userData.status;
        friendsList = userData.friends;
        friendRequestsList = userData.friendRequests;
        int i = 0;

        foreach (string friendRequest in friendRequestsList)
        {
            if (friendRequest != null && friendRequest != "")
            {
                Debug.Log("Display friend: " + friendRequest);

                DocumentSnapshot userDoc = await UserBackendManager.Instance.GetOtherUserTask(friendRequest);
                FriendRequestsData(UserBackendManager.Instance.ProcessUserDocument(userDoc));
                friendRequestData = friendRequest;
            }
            i++;
        }
    }

    public void FriendRequestsData(UserData userData)
    {
        //Clone prefab for displaying friend request
        GameObject box = Instantiate(friendRequestBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        box.transform.SetParent(GameObject.Find("FriendRequestContent").transform, false);
        box.name = userData.email;

        //Show the email of the friend request sender
        box.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = userData.username;
        box.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TMP_Text>().text = userData.status;
    }


    public void AcceptFriendRequest()
    {
        UserBackendManager.Instance.AcceptFriendRequest(emailData, FriendRequestBox.id, friendsList, friendRequestsList);
        ClearDisplay();
        UserBackendManager.Instance.SearchFriendRequests(RegisterAndLogin.emailData);
    }

    public void RejectFriendRequest()
    {
        UserBackendManager.Instance.RejectFriendRequest(emailData, FriendRequestBox.id, friendRequestsList);
        ClearDisplay();
        UserBackendManager.Instance.SearchFriendRequests(RegisterAndLogin.emailData);
    }

    public void ToggleFriendRequestsTab()
    {
        UIManager.Instance.ToggleGeneralTab(friendRequestsTab);
        ClearDisplay();
    }

    public void ToggleSearchFriendTab()
    {
        UIManager.Instance.ToggleGeneralTab(searchFriendTab);
        ClearDisplay();
        DisableSearchFriendInfoTab();
    }

    public void EnableSearchFriendInfoTab()
    {
        UIManager.Instance.EnableGeneralTab(searchFriendInfoTab);
        ClearDisplay();
    }

    public void DisableSearchFriendInfoTab()
    {
        UIManager.Instance.DisableGeneralTab(searchFriendInfoTab);
        ClearDisplay();
    }

    public void ClearDisplay()
    {
        SearchNameDisplay.text = "";
        SearchEmailDisplay.text = "";
        SearchStatusDisplay.text = "";

        if (friendRequestsList != null)
        {
            friendRequestsList.Clear();
        }

        if (friendsList != null)
        {
            friendsList.Clear();
        }

        GameObject[] tempPrefabs;

        tempPrefabs = GameObject.FindGameObjectsWithTag("TempPrefab");

        foreach (GameObject tempPrefab in tempPrefabs)
        {
            Destroy(tempPrefab);
        }
    }
}
