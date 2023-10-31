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

    public void PanelFadeIn(CanvasGroup panelCanvasGroup, float fadeTime, UIMoveDir dir, Vector2 uiPos)
    {
        RectTransform panelUIRect = panelCanvasGroup.gameObject.GetComponent<RectTransform>();
        panelCanvasGroup.alpha = 0f; //starting opacity
        if (dir.Equals(UIMoveDir.Stay))
        {
            panelUIRect.transform.localPosition = uiPos;
        }
        else
        {
            panelUIRect.transform.localPosition = GetFadeDirPos(dir); //starting position
        }
        panelUIRect.DOAnchorPos(uiPos, fadeTime, false).SetEase(Ease.OutFlash); //falsh in animation
        panelCanvasGroup.DOFade(1, fadeTime); //fade out animtion
    }

    public void PanelFadeOut(CanvasGroup panelCanvasGroup, float fadeTime, UIMoveDir dir, Vector2 uiPos)
    {
        RectTransform panelUIRect = panelCanvasGroup.gameObject.GetComponent<RectTransform>();
        panelCanvasGroup.alpha = 0f; //starting opacity
        panelUIRect.transform.localPosition = uiPos;
        Vector2 targetPos;
        if (dir.Equals(UIMoveDir.Stay))
        {
            targetPos = uiPos;
        }
        else
        {
            targetPos = GetFadeDirPos(dir); //starting position
        }
        panelUIRect.DOAnchorPos(targetPos, fadeTime, false).SetEase(Ease.OutFlash); //falsh in animation
        panelCanvasGroup.DOFade(1, fadeTime); //fade out animtion
    }

    private Vector2 GetFadeDirPos(UIMoveDir dir)
    {
        Vector2 dirStartPos = Vector2.zero;
        switch (dir)
        {
            case UIMoveDir.FromLeft:
                dirStartPos = new Vector2(-500f, 0f);
                break;
            case UIMoveDir.FromRight:
                dirStartPos = new Vector2(500f, 0f);
                break;
            case UIMoveDir.FromTop:
                dirStartPos = new Vector2(0f, 500f);
                break;
            case UIMoveDir.FromBottom:
                dirStartPos = new Vector2(0f, -500f);
                break;
        }
        return dirStartPos;
    }

    public enum UIMoveDir
    {
        FromLeft,
        FromRight,
        FromTop,
        FromBottom,
        Stay
    }

}