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
        { ":shocked:", "Surprised"},
        { ":angry:", "Angry"},
        { ":laugh:", "Laugh"},
        { ":sus:", "Thinking"},
        { ":wave:", "Wave"},
    };

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool PlayEmoteAnimation(GameObject avatar, string msgText)
    {
        try
        {
            Animator _animator = avatar.GetComponent<Animator>();
            foreach (var kvp in emojiToAnimMap)
            {
                if (msgText.Contains(kvp.Key))
                {
                    Debug.Log("Animation: " + kvp.Value);
                    _animator.SetBool(kvp.Value, true);
                    _animator.SetBool("Default", true);
                    return true;
                }
            }
            return false;
        }
        catch (Exception e)
        {
            Debug.Log("Error playing animation: " + e);
            return false;
        }
    }

    public void UI_SlideInFromRightSide(RectTransform UI, bool isVisible, float slideSpeed, float hiddenPos, float shownPos)
    {
        float targetX = isVisible ? shownPos : hiddenPos;
        float newX = Mathf.Lerp(UI.anchoredPosition.x, targetX, Time.deltaTime * slideSpeed);
        UI.anchoredPosition = new Vector2(newX, UI.anchoredPosition.y);
    }

    public void AvatarPopUp(GameObject avatarBody, bool isEmojiSent, float speed, float defaultXPos, float movedXPos, float defaultYPos, float movedYPos)
    {
        float targetX = isEmojiSent ? movedXPos : defaultXPos;
        float targetY = isEmojiSent ? movedYPos : defaultYPos;

        // Calculate the new position of the object
        Vector3 targetPos = new Vector3(targetX, targetY, avatarBody.transform.localPosition.z);

        // Move the object upwards smoothly using Lerp
        avatarBody.transform.localPosition = Vector3.Lerp(avatarBody.transform.localPosition, targetPos, speed * Time.deltaTime);
    }
}