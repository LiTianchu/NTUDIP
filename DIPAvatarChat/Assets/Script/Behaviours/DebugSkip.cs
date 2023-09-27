using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSkip : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Skip(string landingScene)
    {
        AuthManager.Instance.StartLogin("dipgrp6@gmail.com", "Dip12345_", landingScene);
        //RegisterAndLogin.emailData = "dipgrp6@gmail.com";
        //if (UserBackendManager.Instance.UpdateUsernameAndStatus("DIP Tester", "Hi, I'm a tester!"))
        //{
        //    AppManager.Instance.LoadScene("4-ChatList");
        //}
    }

    public void Skip2(string landingScene)
    {
        AuthManager.Instance.StartLogin("aloysiusgohkw@gmail.com", "aaaaaa", landingScene);
    }
}
