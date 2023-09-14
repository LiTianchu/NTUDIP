using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{

    //Functions to change the login screen UI
    public void ToggleLoginResgister(GameObject loginUI,GameObject registerUI) //Back button
    {
        loginUI.SetActive(!loginUI.activeSelf);
        registerUI.SetActive(!registerUI.activeSelf);
    }

    public void ToggleNotificationTab(GameObject notificationTabUI)
    {
        notificationTabUI.SetActive(!notificationTabUI.activeSelf);
    }

    //public void RegisterScreen(GameObject loginUI, GameObject registerUI) // Regester button
    //{
    //    loginUI.SetActive(false);
    //    registerUI.SetActive(true);
    //}
}