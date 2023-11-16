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
    float minHeight = 20f;
    float minWidth = 40f;
    float maxWidth = 160f;
    float paddingHeight = 2f;
    float paddingWidth = 2f;
    float textHeight;
    float textWidth;

    private void Start()
    {
        AutoResizeHeight();
        AutoResizeWidth();

        chatBubbleBox.sizeDelta = new Vector2(textWidth, textHeight + paddingHeight);
        chatBubbleImage.sizeDelta = new Vector2(textWidth, textHeight + paddingHeight);
    }

    private void AutoResizeHeight()
    {
        textHeight = minHeight;
        if (message.preferredHeight > minHeight)
        {
            textHeight = message.preferredHeight;
        }
    }

    private void AutoResizeWidth()
    {
        textWidth = minWidth;
        if (message.preferredWidth > minWidth)
        {
            textWidth = message.preferredWidth;
        }

        if (message.preferredWidth > maxWidth)
        {
            textWidth = maxWidth;
        }
    }
}
