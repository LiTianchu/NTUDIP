using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using TMPro;
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
    public GameObject SendRequestBoxPrefab;
    public Button SendFriendRequestBtn;
    public GameObject ChatListParent;
    public ChatListBox ChatListObject;
    public GameObject AvatarDisplayArea2d;
    public GameObject AvatarSkinDisplayArea2d;
    public GameObject AvatarHeadDisplayArea2d;
    public GameObject AvatarHatDisplayArea2d;
    public GameObject AvatarTextureDisplayArea2d;
    public GameObject LoadingUI;
    public GameObject LoadingSwipe;
    public TMP_Text FriendRequestSentText;
    public TMP_Text AlreadyYourFriendText;

    // Store existing chat list items by conversation ID
    private Dictionary<string, ChatListBox> chatListItems = new Dictionary<string, ChatListBox>();


    // Start is called before the first frame update
    void Start()
    {
        PopulateChatList();
    }

    async public void PopulateChatList()
    {
        if (LoadingUI != null)
        {
            ChatListParent.SetActive(false);
            LoadingUI.SetActive(true);
        }

        // Clear existing chat list items
        ClearChatList();

        //Getting the data by task
        ConversationData conversation = null;
        MessageData latestMessage = null;
        UserData friendData = null;
        string friendEmail = null;

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.currUser.email);
        AuthManager.Instance.currUser = myUserDoc.ConvertTo<UserData>();
        List<string> conversations = AuthManager.Instance.currUser.conversations;
        List<ConversationData> conversationsSorted = new List<ConversationData>();

        for (int i = conversations.Count - 1; i >= 0; i--)
        {

            if (conversations[i] != null && conversations[i] != "")
            {
                //get conversation document
                DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(conversations[i]);
                conversation = conversationDoc.ConvertTo<ConversationData>();

                if (conversation == null)
                {
                    Debug.LogWarning(conversations[i] + " is not found in conversation document");
                    continue;
                }

                conversationsSorted.Add(conversation);
            }
        }

        conversationsSorted.Sort((x, y) => DateTime.Compare(x.latestMessageCreatedAt, y.latestMessageCreatedAt));
        conversationsSorted.Reverse();

        foreach (ConversationData conv in conversationsSorted)
        {
            foreach (string member in conv.members)
            {
                if (member != AuthManager.Instance.currUser.email)
                {
                    friendEmail = member;
                    Debug.Log("Friend email: " + friendEmail);
                }
            }

            //get message document and retrieve the message details and the user
            if (conv.messages != null && conv.messages.Count > 1) // messages[0] is null when instantiated, start count from 1
            {
                DocumentSnapshot messageDoc = await MessageBackendManager.Instance.GetMessageByIDTask(conv.messages[conv.messages.Count - 1]);
                latestMessage = messageDoc.ConvertTo<MessageData>();

                DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(friendEmail);
                friendData = userDoc.ConvertTo<UserData>();
                if (friendData == null)
                {
                    Debug.Log(latestMessage.sender + " has no corresponding document");
                }
            }
            else
            {
                //handle empty conversation
                latestMessage = new MessageData();
                latestMessage.message = "No messages yet";
                DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(friendEmail);
                friendData = userDoc.ConvertTo<UserData>();
            }

            string convId = conv.conversationID;
            string displayMessage = latestMessage.message;
            string displaySenderUsername = friendData.username;
            Timestamp displayTime = latestMessage.createdAt;

            // Instantiate the ChatListObject (ChatDisplayBox) prefab if it doesn't exist
            ChatListBox chatListItem;
            if (!chatListItems.TryGetValue(convId, out chatListItem))
            {
                chatListItem = Instantiate(ChatListObject, new Vector3(0, 0, 0), Quaternion.identity);
                chatListItem.transform.SetParent(ChatListParent.transform, false);
                chatListItem.name = convId;
                chatListItem.CurrentAvatarUserEmail = friendEmail;
                chatListItems.Add(convId, chatListItem);
            }

            // Access the Text components within the prefab
            TMP_Text timeText = chatListItem.transform.Find("ChatDisplayBoxBtn/Time").GetComponent<TMP_Text>();
            TMP_Text usernameText = chatListItem.transform.Find("ChatDisplayBoxBtn/UserInfo/Username").GetComponent<TMP_Text>();
            TMP_Text messageText = chatListItem.transform.Find("ChatDisplayBoxBtn/UserInfo/LatestMessage").GetComponent<TMP_Text>();

            // Set the text values based on your latestMessage and sender data
            messageText.text = latestMessage?.message;
            usernameText.text = friendData?.username;
            timeText.text = ChatTimestamp(displayTime);

            // Set the text values based on your latestMessage and sender data
            string latestMessageText = latestMessage?.message;
            //int maxLength = 20; // Set the maximum length you want for the message
            //if (!string.IsNullOrEmpty(latestMessageText) && latestMessageText.Length > maxLength)
            //{
            //    // If the message exceeds the maximum length, truncate it and add "..."
            //    latestMessageText = latestMessageText.Substring(0, maxLength) + "...";
            //}
            //latestMessageText = ChatManager.Instance.ReverseEmojiUpdate(latestMessageText);
            messageText.text = latestMessageText;

            //cache the chatlist data
            ChatManager.Instance.EmailToUsersDict[friendData.email] = friendData;
            ChatManager.Instance.EmailToConversationDict[friendData.email] = conv;
        }

        //display 2d avatar
        DocumentSnapshot snapshot = await AvatarBackendManager.Instance.GetAvatarByEmailTask(AuthManager.Instance.currUser.email);
        AvatarManager.Instance.DisplayFriendAvatar2d(snapshot, AvatarHeadDisplayArea2d, AvatarSkinDisplayArea2d, AvatarHatDisplayArea2d, AvatarTextureDisplayArea2d);

        if (LoadingUI != null)
        {
            ChatListParent.SetActive(true);
            LoadingUI.SetActive(false);
            StartCoroutine(DisableLoadingAnim(0.5f));
        }
    }

    void ClearChatList()
    {
        // Destroy chat list items that are not in the dictionary
        foreach (var chatListItem in chatListItems.Values)
        {
            if (chatListItem != null)
            {
                Destroy(chatListItem);
            }
        }
        // Clear the dictionary
        chatListItems.Clear();
    }

    private string ChatTimestamp(Timestamp timestamp)
    {
        DateTime chatTime = timestamp.ToDateTime().AddHours(8); // Convert to Singapore Timezone
        DateTime currentTime = DateTime.Now;

        Debug.Log(currentTime.ToString());
        Debug.Log(chatTime.ToString());

        if (chatTime.Day == currentTime.Day)
        {
            return (chatTime.ToString("h:mm tt"));
        }

        if (chatTime.Month == currentTime.Month && chatTime.Day + 7 > currentTime.Day)
        {
            return (chatTime.ToString("ddd"));
        }

        if (chatTime.Year == currentTime.Year)
        {
            return (chatTime.ToString("MMM dd"));
        }

        if (chatTime.Year <= 1970)
        {
            return "";
        }

        return chatTime.ToString("d");
    }

    private List<string> FilterConversationList(QuerySnapshot convDocQuery)
    {
        List<string> conv = new List<string>();
        foreach (DocumentSnapshot convDoc in convDocQuery)
        {
            ConversationData convDocData = convDoc.ConvertTo<ConversationData>();

            bool IsInsideConversation = false;
            foreach (string member in convDocData.members)
            {
                if (member == AuthManager.Instance.currUser.email)
                {
                    IsInsideConversation = true;
                }
            }

            if (IsInsideConversation)
            {
                conv.Add(convDocData.conversationID);
            }
        }

        return conv;
    }

    private IEnumerator DisableLoadingAnim(float delay)
    {
        LoadingSwipe.SetActive(true);
        yield return new WaitForSecondsRealtime(delay);
        LoadingSwipe.SetActive(false);
    }

    async public void SearchUserByEmailAsync()
    {
        if (emailSearchBar.text == null || emailSearchBar.text.Length == 0)
        {
            return;
        }
        EnableTab(SearchFriendInfoTab);

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(emailSearchBar.text);
        DisplaySearchUserData(userDoc.ConvertTo<UserData>());

        //SendFriendRequestBtn.interactable = true;
    }

    async public void SendFriendRequest()
    {
        string myEmail = AuthManager.Instance.currUser.email;
        string theirEmail = emailSearchBar.text;

        List<string>[] friendAndFriendRequestLists = await GetFriendAndFriendRequestListsTask(myEmail, theirEmail);

        bool isDuplicateFriendRequest = false;

        //checks if friend request already sent by the user
        foreach (string friendRequest in friendAndFriendRequestLists[1])
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

        foreach (string friend in friendAndFriendRequestLists[2])
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
            UserBackendManager.Instance.SendFriendRequestToThem(myEmail, theirEmail, friendAndFriendRequestLists[1]);

            // Display "Friend Request Sent!" message
            FriendRequestSentText.text = "Friend Request Sent!";
            FriendRequestSentText.gameObject.SetActive(true);

            // Start a coroutine to hide the message after a delay
            StartCoroutine(HideFriendRequestSentText(2.0f)); // Adjust the delay as needed
        }
        else
        {
            Debug.Log("Friend Request cannot be sent...");

            // Display "Already Your Friend Sent!" message
            AlreadyYourFriendText.text = "Already Your Friend!";
            AlreadyYourFriendText.gameObject.SetActive(true);

            // Start a coroutine to hide the message after a delay
            StartCoroutine(HideAlreadyYourFriendText(2.0f)); // Adjust the delay as needed
        }

        //SendFriendRequestBtn.interactable = false;
    }

    private IEnumerator HideFriendRequestSentText(float delay)
    {
        yield return new WaitForSeconds(delay);
        FriendRequestSentText.gameObject.SetActive(false);
        FriendRequestSentText.text = "";
    }

    private IEnumerator HideAlreadyYourFriendText(float delay)
    {
        yield return new WaitForSeconds(delay);
        AlreadyYourFriendText.gameObject.SetActive(false);
        AlreadyYourFriendText.text = "";
    }


    async public void DisplayFriendRequests()
    {
        //EnableTab(FriendRequestsTab);
        DestroyTempPrefabs("TempPrefab");
        Debug.Log(AuthManager.Instance.currUser.email);

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.currUser.email);
        UserData myUserData = myUserDoc.ConvertTo<UserData>();

        foreach (string friendRequest in myUserData.friendRequests)
        {
            if (friendRequest != null && friendRequest != "")
            {
                Debug.Log("Display friend request: " + friendRequest);

                DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(friendRequest);
                UserData theirUserData = theirUserDoc.ConvertTo<UserData>();

                //Clone prefab for displaying friend request
                GameObject box = Instantiate(FriendRequestBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent(GameObject.Find("FriendRequestContent").transform, false);
                box.name = theirUserData.email;

                //Show the email of the friend request sender
                box.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = theirUserData.username;
                box.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TMP_Text>().text = theirUserData.status;
            }
        }
    }

    public void DisplaySearchUserData(UserData userData)
    {
        DestroyTempPrefabs("SearchFriendPrefab");

        if (userData == null)
        {
            Debug.Log("User Data is not found");
            return;
        }
        Debug.Log("User Data Retrieved");
        Debug.Log(userData.username);
        Debug.Log(userData.email);
        Debug.Log(userData.status);
        Debug.Log(userData.friendRequests);
        Debug.Log(userData.friends);

        //Clone prefab for displaying friend request
        GameObject box = Instantiate(SendRequestBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        box.transform.SetParent(GameObject.Find("SearchFriendInfoTab").transform, false);
        box.name = userData.email;

        //Show the email of the friend request sender
        box.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = userData.username;
        box.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TMP_Text>().text = userData.status;
    }

    async public void AcceptFriendRequest(string theirEmail)
    {
        string myEmail = AuthManager.Instance.currUser.email;

        List<string>[] friendAndFriendRequestLists = await GetFriendAndFriendRequestListsTask(myEmail, theirEmail);

        UserBackendManager.Instance.AcceptFriendRequest(myEmail, theirEmail, friendAndFriendRequestLists[0], friendAndFriendRequestLists[1], friendAndFriendRequestLists[2], friendAndFriendRequestLists[3]);

        DisplayFriendRequests();
    }

    async public void RejectFriendRequest(string theirEmail)
    {
        string myEmail = AuthManager.Instance.currUser.email;

        List<string>[] friendAndFriendRequestLists = await GetFriendAndFriendRequestListsTask(myEmail, theirEmail);

        UserBackendManager.Instance.RejectFriendRequest(myEmail, theirEmail, friendAndFriendRequestLists[0]);
        DisplayFriendRequests();
    }

    async public Task<List<string>[]> GetFriendAndFriendRequestListsTask(string myEmail, string theirEmail)
    {
        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(myEmail);
        UserData myUserData = myUserDoc.ConvertTo<UserData>();

        DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(theirEmail);
        UserData theirUserData = theirUserDoc.ConvertTo<UserData>();

        List<string> myFriendRequestsList = new List<string>(myUserData.friendRequests);
        List<string> theirFriendRequestsList = new List<string>(theirUserData.friendRequests);

        List<string> myFriendsList = new List<string>(myUserData.friends);
        List<string> theirFriendsList = new List<string>(theirUserData.friends);

        List<string>[] friendAndFriendRequestLists = { myFriendRequestsList, theirFriendRequestsList, myFriendsList, theirFriendsList };

        return friendAndFriendRequestLists;
    }

    public void NewChat()
    {
        AppManager.Instance.LoadScene("5-NewChat");
    }

    public void EditProfile()
    {
        AppManager.Instance.LoadScene("3-EditProfile");
    }

    public void CloseSearchFriendTab()
    {
        UIManager.Instance.DisableGeneralTab(SearchFriendTab);
        UIManager.Instance.DisableGeneralTab(SearchFriendInfoTab);
    }

    public void EnableTab(GameObject Tab)
    {
        UIManager.Instance.EnableGeneralTab(Tab);
    }

    public void DisableTab(GameObject Tab)
    {
        UIManager.Instance.DisableGeneralTab(Tab);
    }

    async public void GetCurrentUserData()
    {
        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.currUser.email);
        UserData userData = myUserDoc.ConvertTo<UserData>();
    }

    //public void ClearDisplay()
    //{
    //    // Only clear the display if a refresh is needed
    //    //SearchNameDisplay.text = "";
    //    //SearchEmailDisplay.text = "";
    //    //SearchStatusDisplay.text = "";
    //}

    public void DestroyTempPrefabs(string tag)
    {
        GameObject[] tempPrefabs;

        tempPrefabs = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject tempPrefab in tempPrefabs)
        {
            if (tempPrefab != null)
            {
                Destroy(tempPrefab);
            }
        }
    }

    public void SignOut()
    {
        AuthManager.Instance.SignOut();

        ChatManager.Instance.ConvIDToMessageDataDict = new Dictionary<string, HashSet<MessageData>>();
        ChatManager.Instance.EmailToUsersDict = new Dictionary<string, UserData>();
        ChatManager.Instance.EmailToConversationDict = new Dictionary<string, ConversationData>();

        AppManager.Instance.LoadScene("2-RegisterAndLogin");
    }

    public void LoadARChat()
    {
        AppManager.Instance.LoadScene("7-ARChat");
    }
}
