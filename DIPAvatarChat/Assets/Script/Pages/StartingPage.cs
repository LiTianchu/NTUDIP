using System.Collections;
using UnityEngine;

public class StartingPage : MonoBehaviour, IPageTransition
{
    public GameObject LoadingUI;
    public bool autoLogin = true;
    public GameObject fadeInUI;
    // Start is called before the first frame update
    void Start()
    {
        // Auto log in if session is stored
        if (autoLogin)
        {
            LoadSession();
        }
        FadeInUI();
        
    }

    public void FadeInUI()
    {
        CanvasGroup canvasGroup = fadeInUI.GetComponent<CanvasGroup>();

        UIManager.Instance.PanelFadeIn(canvasGroup, 2f,UIManager.UIMoveDir.FromBottom, Vector2.zero);
    }
    void LoadSession()
    {
        StartCoroutine(ExitRoutine());
        StartCoroutine(AuthManager.Instance.LoadSession(0.25f, LoadingUI));
    }

    public IEnumerator ExitRoutine()
    {
        UIManager.Instance.PanelFadeOut(fadeInUI.GetComponent<CanvasGroup>(), 0.5f, UIManager.UIMoveDir.FromBottom, fadeInUI.GetComponent<RectTransform>().anchoredPosition); //fade out all UI
        yield return new WaitForSeconds(0.5f);
    }
}
