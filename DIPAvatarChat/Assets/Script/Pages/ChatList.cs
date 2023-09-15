using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ChatList : MonoBehaviour
{

    public TMP_InputField emailSearchBar;
    public TMP_Text SearchNameDisplay;
    public TMP_Text SearchEmailDisplay;
    public TMP_Text SearchStatusDisplay;
    public GameObject searchFriendTab;
    public GameObject friendRequestsTab;
    public GameObject friendRequestBoxPrefab;

    List<string> friendRequestsList;
    List<string> friendsList;
    string usernameData;
    string emailData;
    string statusData;
    string friendRequestData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        //attach event listeners on enable
        UserBackendManager.Instance.SearchUserDataReceived += DisplaySearchUserData;
        UserBackendManager.Instance.SearchUserFriendRequestsReceived += DisplayFriendRequestsData;
    }

    private void OnDisable()
    {
        //attach event listeners on disable
        if (!this.gameObject.scene.isLoaded) return;
        UserBackendManager.Instance.SearchUserDataReceived -= DisplaySearchUserData;
        UserBackendManager.Instance.SearchUserFriendRequestsReceived -= DisplayFriendRequestsData;
    }

    public void NewChat()
    {
        AppManager.Instance.LoadScene("5-NewChat");
    }

    public void EnterChat(string chatID)
    {
        //set as temp data storage to pass to next scene
        PlayerPrefs.SetString("chatID", chatID);
        AppManager.Instance.LoadScene("6-ChatFrontEnd");
    }

    public void SearchUserByEmail()
    {
        ClearDisplay();
        UserBackendManager.Instance.SearchUserByEmail(emailSearchBar.text);

    }

    public void SendFriendRequest()
    {
        //UserBackendManager.Instance.SendFriendRequest(friendRequestsList, emailSearchBar.text, RegisterAndLogin.emailData);

        //hardcoded test
        UserBackendManager.Instance.SendFriendRequest(friendRequestsList, emailSearchBar.text, "bbbb@gmail.com");
    }

    public void DisplayFriendRequests()
    {
        ToggleFriendRequestsTab();
        Debug.Log(RegisterAndLogin.emailData);
        UserBackendManager.Instance.SearchFriendRequests("bbbb@gmail.com");
        //UserBackendManager.Instance.SearchFriendRequests(RegisterAndLogin.emailData);  
    }

    public void DisplaySearchUserData(UserData userData)
    {
        Debug.Log("User Data Retrieved");

        usernameData = userData.username;
        emailData = userData.email;
        statusData = userData.status;
        friendRequestsList = userData.friendRequests;

        SearchNameDisplay.text = usernameData;
        SearchEmailDisplay.text = emailData;
        SearchStatusDisplay.text = statusData;
    }

    public void DisplayFriendRequestsData(UserData userData)
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

                //Clone prefab for displaying friend request
                GameObject box = Instantiate(friendRequestBoxPrefab, new Vector3(0, -150 - (i - 1) * 80, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent(GameObject.Find("FriendRequestsTab").transform, false);
                box.name = friendRequest;

                Debug.Log("Instantiated Friend Request: " + i);

                //Show the email of the friend request sender
                box.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = friendRequest;

                friendRequestData = friendRequest;
            }
            i++;
        }
    }

    public void AcceptFriendRequest()
    {
        UserBackendManager.Instance.AcceptFriendRequest(emailData, FriendRequestBox.id, friendsList, friendRequestsList);
        ClearDisplay();
        UserBackendManager.Instance.SearchFriendRequests("bbbb@gmail.com");
    }

    public void RejectFriendRequest()
    {

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
