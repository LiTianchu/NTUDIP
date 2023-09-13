using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChatList : MonoBehaviour
{

    public TMP_InputField getUsernameByEmail;
    public static TMP_Text nameDisplay;
    public static TMP_Text emailDisplay;
    public static TMP_Text statusDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        UserBackendManager.Instance.GetUsernameByEmail(getUsernameByEmail.text);
    }
}
