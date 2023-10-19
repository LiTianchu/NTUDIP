using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ARChatScrollBar : MonoBehaviour
{
    [SerializeField]
    private float minWidth = 135f;
    [SerializeField]
    private float maxWidth = 270f;

    private RectTransform _rectTransform;
    private HorizontalLayoutGroup _horizontalLayoutGroup;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>(); 
        ARChat.Instance.OnARFinishedLoading += UpdateBarSize;
    }

    private void OnDestroy()
    {
        ARChat.Instance.OnARFinishedLoading -= UpdateBarSize;
    }

    private void UpdateBarSize()
    {
        int avatarCount = ARChat.Instance.AvatarList.Count;
        float width = avatarCount * _horizontalLayoutGroup.spacing + _horizontalLayoutGroup.padding.left*2;
        _rectTransform.sizeDelta = new Vector2(width, _rectTransform.sizeDelta.y);
    }

    


}
