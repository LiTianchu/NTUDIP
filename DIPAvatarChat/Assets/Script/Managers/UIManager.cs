using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public void PanelFadeIn(RectTransform panelUIRect, CanvasGroup panelCanvasGroup, float fadeTime)
    {
        panelCanvasGroup.alpha = 0f; //starting opacity
        panelUIRect.transform.localPosition = new Vector3(0,-1000,0); //starting position
        panelUIRect.DOAnchorPos(Vector2.zero, fadeTime, false).SetEase(Ease.OutFlash); //falsh in animation
        panelCanvasGroup.DOFade(1, fadeTime); //fade out animtion
    }

    public void PanelFadeOut(RectTransform panelUIRect, CanvasGroup panelCanvasGroup, float fadeTime)
    {
        panelCanvasGroup.alpha = 1f; //starting opcaity
        panelUIRect.transform.localPosition = Vector3.zero; //starting position
        panelUIRect.DOAnchorPos(new Vector2(0f, -1000f), fadeTime, false).SetEase(Ease.InOutQuint); //flash out animation
        panelCanvasGroup.DOFade(0, fadeTime); //fade out animation
    }
}