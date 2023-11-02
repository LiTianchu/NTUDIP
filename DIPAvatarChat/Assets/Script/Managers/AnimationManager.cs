using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AnimationManager : Singleton<AnimationManager>
{
    public RuntimeAnimatorController animatorController;
    // Map custom commands to play animation
    public readonly Dictionary<string, string> emojiToAnimMap = new Dictionary<string, string>
    {  
        { "hi!", "Wave"},
        { "<size=24><sprite=10></size>", "Surprised"},
        { "<size=24><sprite=21></size>", "Angry"},
        { "<size=24><sprite=24></size>", "Laugh"},
    };

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayAnimation(GameObject avatar, string msgText)
    {
        Animator _animator = avatar.GetComponent<Animator>();

        try
        {
            foreach (var kvp in emojiToAnimMap)
            {
                if (msgText.Contains(kvp.Key))
                {
                    Debug.Log("Animation: " + kvp.Value);
                    _animator.SetBool(kvp.Value, true);
                    _animator.SetBool("Default", true);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error playing animation: " + e);
        }
    }

    public void UI_SlideInFromRightSide(RectTransform UI, bool isVisible, float slideSpeed, float hiddenPos, float shownPos)
    {
        float targetX = isVisible ? shownPos : hiddenPos;
        float newX = Mathf.Lerp(UI.anchoredPosition.x, targetX, Time.deltaTime * slideSpeed);
        UI.anchoredPosition = new Vector2(newX, UI.anchoredPosition.y);
    }
}