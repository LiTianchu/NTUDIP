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
        UserBackendManager.Instance.UserDataReceived += DisplayUserData;
    }

    private void OnDisable()
    {
        //attach event listeners on disable
        UserBackendManager.Instance.UserDataReceived -= DisplayUserData;
    }

    public void NewChat()
    {
        AppManager.Instance.LoadScene("5-NewChat");
    }

    public void EnterChat(string chatID) {
        //set as temp data storage to pass to next scene
        PlayerPrefs.SetString("chatID", chatID);
        AppManager.Instance.LoadScene("6-ChatFrontEnd");
    }

    public void GetUsernameByEmail()
    {
        UserBackendManager.Instance.GetUsernameByEmail(emailSearchBar.text);

    }

    public void DisplayUserData(UserData userData)
    {
        Debug.Log("User Data Retrieved");

        Debug.Log(userData.username);
        Debug.Log(userData.email);
        Debug.Log(userData.status);

        nameDisplay.text = userData.username;
        emailDisplay.text = userData.email;
        statusDisplay.text = userData.status;
    }
}
