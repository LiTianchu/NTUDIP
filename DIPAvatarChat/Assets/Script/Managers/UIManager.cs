using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public bool isCooldown { get; set; }
    //Functions to change the login screen UI
    public void ToggleLoginRegister(GameObject loginUI, GameObject registerUI) //Back button
    {
        loginUI.SetActive(!loginUI.activeSelf);
        registerUI.SetActive(!registerUI.activeSelf);
    }

    public void ToggleThreeTabs(GameObject DisplayTab, GameObject HideTab1, GameObject HideTab2)
    {
        DisplayTab.SetActive(true);
        HideTab1.SetActive(false);
        HideTab2.SetActive(false);
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

    public IEnumerator StartCooldown(float f)
    {
        isCooldown = true;
        yield return new WaitForSecondsRealtime(f);
        isCooldown = false;
    }

    //public void RegisterScreen(GameObject loginUI, GameObject registerUI) // Regester button
    //{
    //    loginUI.SetActive(false);
    //    registerUI.SetActive(true);
    //}
}