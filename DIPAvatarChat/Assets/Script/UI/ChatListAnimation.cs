using UnityEngine;
using DG.Tweening;

public class ChatListAnimation : MonoBehaviour
{
    public RectTransform chatList;
    public float animationDelay = 1.0f;
    public float animationDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        InitializeChatListAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        // You can add any relevant update logic here if needed
    }

    void InitializeChatListAnimation()
    {
        // Initialize DOTween
        DOTween.Init();

        // Set the initial position off-screen to the left
        chatList.anchoredPosition = new Vector2(-chatList.rect.width, 0);

        // Add a delay, then animate the chatList to slide
        chatList.DOAnchorPos(Vector2.zero, animationDuration).SetDelay(animationDelay);
    }
}
