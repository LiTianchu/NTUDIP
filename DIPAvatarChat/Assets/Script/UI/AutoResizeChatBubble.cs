using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoResizeChatBubble : MonoBehaviour
{
    public RectTransform chatBubbleBox;    // Reference to your chat bubble RectTransform
    public RectTransform chatBubbleImage;
    public TextMeshProUGUI message; // Reference to your TMP Text component

    private void Start()
    {
        float textHeight = message.preferredHeight;
        chatBubbleBox.sizeDelta = new Vector2(chatBubbleBox.sizeDelta.x, textHeight);
        chatBubbleImage.sizeDelta = new Vector2(chatBubbleImage.sizeDelta.x, textHeight);
    }
}
