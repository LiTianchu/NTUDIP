using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StartingPage : MonoBehaviour
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
        FadeIn();
        
    }

    private void FadeIn()
    {
        RectTransform rectTransform = fadeInUI.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = fadeInUI.GetComponent<CanvasGroup>();

        UIManager.Instance.PanelFadeIn(rectTransform, canvasGroup, 0.5f);
    }
    void LoadSession()
    {
        StartCoroutine(AuthManager.Instance.LoadSession(0.25f, LoadingUI));
    }
}
