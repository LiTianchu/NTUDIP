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

    //General Function to open another UI in the same scene
    public void ToggleGeneralTab(GameObject tabUI)
    {
        tabUI.SetActive(!tabUI.activeSelf);
    }

    public void EnableGeneralTab(GameObject tabUI)
    {
        tabUI.SetActive(true);
    }

    public void DisableGeneralTab(GameObject tabUI)
    {
        tabUI.SetActive(false);
    }

    //public void RegisterScreen(GameObject loginUI, GameObject registerUI) // Regester button
    //{
    //    loginUI.SetActive(false);
    //    registerUI.SetActive(true);
    //}
}