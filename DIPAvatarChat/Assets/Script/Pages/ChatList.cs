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
    public GameObject SearchFriendTab;
    public GameObject SearchFriendInfoTab;
    public GameObject FriendRequestsTab;
    public GameObject FriendRequestBoxPrefab;
    public Button SendFriendRequestBtn;
    public GameObject ChatListParent;
    public GameObject ChatListObject;

    List<string> friendRequestsList;
    List<string> friendsList;
    string usernameData;
    string emailData;
    string statusData;
    string friendRequestData;

    // Start is called before the first frame update
    void Start()
    {
        PopulateChatList();
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
                    DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(latestMessage.sender);
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
                    DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(conversation.members[0]);
                    sender = UserBackendManager.Instance.ProcessUserDocument(userDoc);
                }


            }
        }

        Debug.Log("Latest Message: " + latestMessage?.message);
        Debug.Log("Latest Sender: " + sender?.username);

        Debug.Log("Latest Message Timestamp: " + latestMessage?.createdAt);

        string displayMessage = latestMessage.message;
        string displaySenderUsername = sender.username;
        Timestamp displayTime = latestMessage.createdAt;

        //TODO: Fill in the data for ChatListObject

        Instantiate(ChatListObject, ChatListParent.transform);
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
        EnableTab(SearchFriendInfoTab);

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(emailSearchBar.text);
        DisplaySearchUserData(UserBackendManager.Instance.ProcessUserDocument(userDoc));

        SendFriendRequestBtn.interactable = true;
    }

    async public void SendFriendRequest()
    {
        //UserBackendManager.Instance.SendFriendRequestAsync(AuthManager.Instance.emailData, emailSearchBar.text);
        string myEmail = AuthManager.Instance.emailData;
        string theirEmail = emailSearchBar.text;

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(myEmail);
        UserData myUserData = UserBackendManager.Instance.ProcessUserDocument(myUserDoc);

        DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(theirEmail);
        UserData theirUserData = UserBackendManager.Instance.ProcessUserDocument(theirUserDoc);

        List<string> myFriendRequestsList = new List<string>(myUserData.friendRequests);
        List<string> myFriendsList = new List<string>(myUserData.friends);

        List<string> theirFriendRequestsList = new List<string>(theirUserData.friendRequests);
        List<string> theirFriendsList = new List<string>(theirUserData.friends);

        bool isDuplicateFriendRequest = false;

        //checks if friend request already sent by the user
        foreach (string friendRequest in theirUserData.friendRequests)
        {
            Debug.Log(friendRequest);
            if (myEmail == friendRequest)
            {
                isDuplicateFriendRequest = true;
                Debug.Log("You already sent this user a friend request...");
            }
        }

        Debug.Log("isDuplicateFriendRequest: " + isDuplicateFriendRequest);

        //checks if user is already a friend
        bool isAlreadyMyFriend = false;

        foreach (string friend in myUserData.friends)
        {
            Debug.Log(friend);
            if (theirEmail == friend)
            {
                isAlreadyMyFriend = true;
                Debug.Log("This user is already your friend! :P");
            }
        }

        Debug.Log("isAlreadyMyFriend: " + isAlreadyMyFriend);

        if (theirEmail != myEmail && theirEmail != null && !isDuplicateFriendRequest && !isAlreadyMyFriend)
        {
            UserBackendManager.Instance.SendFriendRequestToThem(myEmail, theirEmail, theirFriendRequestsList);
        }
        else
        {
            Debug.Log("Friend Request cannot be sent...");
        }

        SendFriendRequestBtn.interactable = false;
    }

    async public void DisplayFriendRequests()
    {
        EnableTab(FriendRequestsTab);
        Debug.Log(AuthManager.Instance.emailData);

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.emailData);
        UserData myUserData = UserBackendManager.Instance.ProcessUserDocument(myUserDoc);

        foreach (string friendRequest in myUserData.friendRequests)
        {
            if (friendRequest != null && friendRequest != "")
            {
                Debug.Log("Display friend request: " + friendRequest);

                DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(friendRequest);
                UserData theirUserData = UserBackendManager.Instance.ProcessUserDocument(theirUserDoc);

                //Clone prefab for displaying friend request
                GameObject box = Instantiate(FriendRequestBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent(GameObject.Find("FriendRequestContent").transform, false);
                box.name = theirUserData.email;

                //Show the email of the friend request sender
                box.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = theirUserData.username;
                box.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TMP_Text>().text = theirUserData.status;

                friendRequestData = friendRequest;
            }
        }
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

    async public void AcceptFriendRequest()
    {
        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.emailData);
        UserData userData = UserBackendManager.Instance.ProcessUserDocument(myUserDoc);

        UserBackendManager.Instance.AcceptFriendRequest(userData.email, FriendRequestBox.id, userData.friends, userData.friendRequests);
        DisplayFriendRequests();
    }

    async public void RejectFriendRequest()
    {
        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.emailData);
        UserData userData = UserBackendManager.Instance.ProcessUserDocument(myUserDoc);

        UserBackendManager.Instance.RejectFriendRequest(userData.email, FriendRequestBox.id, userData.friendRequests);
        DisplayFriendRequests();
    }

    public void ToggleTab(GameObject Tab)
    {
        UIManager.Instance.ToggleGeneralTab(Tab);
        ClearDisplay();
    }

    public void EnableTab(GameObject Tab)
    {
        UIManager.Instance.EnableGeneralTab(Tab);
        ClearDisplay();
    }

    public void DisableTab(GameObject Tab)
    {
        UIManager.Instance.DisableGeneralTab(Tab);
        ClearDisplay();
    }

    async public void GetCurrentUserData()
    {
        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.emailData);
        UserData userData = UserBackendManager.Instance.ProcessUserDocument(myUserDoc);
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
