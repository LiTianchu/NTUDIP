using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ChatList : MonoBehaviour
{

    public TMP_InputField emailSearchBar;
    public TMP_Text nameDisplay;
    public TMP_Text emailDisplay;
    public TMP_Text statusDisplay;
    public GameObject searchFriendTab;
    public GameObject friendRequestsTab;
    public GameObject friendRequestBoxPrefab;

    List<string> friendRequestsList;

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
        ToggleSearchFriendTab();
        UserBackendManager.Instance.SearchUserByEmail(emailSearchBar.text);

    }

    public void SendFriendRequest()
    {
        //UserBackendManager.Instance.SendFriendRequest(emailSearchBar.text, AuthManager.Instance.emailData);

        //hardcoded test
        UserBackendManager.Instance.SendFriendRequest(friendRequestsList, emailSearchBar.text, "bbbb@gmail.com");
    }

    public void DisplayFriendRequests()
    {
        ToggleFriendRequestsTab();
        UserBackendManager.Instance.SearchFriendRequests("bbbb@gmail.com");
    }

    public void DisplaySearchUserData(UserData userData)
    {
        Debug.Log("User Data Retrieved");

        Debug.Log(userData.username);
        Debug.Log(userData.email);
        Debug.Log(userData.status);

        nameDisplay.text = userData.username;
        emailDisplay.text = userData.email;
        statusDisplay.text = userData.status;

        friendRequestsList = userData.friendRequests;
    }

    public void DisplayFriendRequestsData(UserData userData)
    {
        Debug.Log("User Data Retrieved");

        friendRequestsList = userData.friendRequests;
        int i = 0;

        foreach (string friendRequests in friendRequestsList)
        {
            if (friendRequests != null && friendRequests != "")
            {
                Debug.Log("Display friend: " + friendRequests);

                GameObject box = Instantiate(friendRequestBoxPrefab, new Vector3(0, -150 - (i - 1) * 80, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent(GameObject.Find("FriendRequestsTab").transform, false);

                Debug.Log("Instantiated Friend Request: " + i);
            }
            i++;
        }
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
        nameDisplay.text = "";
        emailDisplay.text = "";
        statusDisplay.text = "";

        if (friendRequestsList != null)
        {
            friendRequestsList.Clear();
        }

        GameObject[] tempPrefabs;

        tempPrefabs = GameObject.FindGameObjectsWithTag("TempPrefab");

        foreach (GameObject tempPrefab in tempPrefabs)
        {
            Destroy(tempPrefab);
        }
    }
}
