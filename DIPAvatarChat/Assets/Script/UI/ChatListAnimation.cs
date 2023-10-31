using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChatListAnimation : MonoBehaviour
{
    public RectTransform chatList;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize DOTween
        DOTween.Init();

        // Set the initial position off-screen to the left
        chatList.anchoredPosition = new Vector2(-chatList.rect.width, 0);

        // Add a delay of 0.5 seconds, then animate the chatList to slide
        chatList.DOAnchorPos(Vector2.zero, 0.5f).SetDelay(1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}