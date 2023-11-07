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
    public bool isEmoteSent = false;
    public GameObject myAvatarBodyChat;
    public GameObject theirAvatarBodyChat;
    string myAnimNameChat = null;
    string theirAnimNameChat = null;
    public Animator myAnimatorChat = null;
    public Animator theirAnimatorChat = null;

    // Map custom commands to play animation
    public readonly Dictionary<string, string> emojiToAnimMap = new Dictionary<string, string>
    {
        { ":shocked:", "Surprised"},
        { ":angry:", "Angry"},
        { ":laugh:", "Laugh"},
        { ":sus:", "Thinking"},
        { ":wave:", "Wave"},
        { ":xdface:", "Excited"},
    };

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (myAnimatorChat != null && myAnimNameChat != null)
        {
            AvatarPopUp(myAvatarBodyChat, myAnimatorChat.GetCurrentAnimatorStateInfo(0).IsName(myAnimNameChat), 8f, 100f, 65f, 40f, 160f);
        }

        if (theirAnimatorChat != null && theirAnimNameChat != null)
        {
            AvatarPopUp(theirAvatarBodyChat, theirAnimatorChat.GetCurrentAnimatorStateInfo(0).IsName(theirAnimNameChat), 8f, -100f, -65f, 40f, 160f);
        }
    }

    public void PlayEmoteAnimation(GameObject avatar, Animator _animator, string msgText, bool isMyAvatar)
    {
        try
        {
            foreach (var kvp in emojiToAnimMap)
            {
                if (msgText.Contains(kvp.Key))
                {
                    Debug.Log("Animation: " + kvp.Value);
                    _animator.SetBool(kvp.Value, true);
                    
                    if (isMyAvatar)
                    {
                        myAnimNameChat = kvp.Value;
                    }
                    else
                    {
                        theirAnimNameChat = kvp.Value;
                    }

                    _animator.SetBool("Default", true);
                    return;
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

    public void AvatarPopUp(GameObject avatarBody, bool isEmojiSent, float speed, float defaultXPos, float movedXPos, float defaultYPos, float movedYPos)
    {
        if (avatarBody != null)
        {
            float targetX = isEmojiSent ? movedXPos : defaultXPos;
            float targetY = isEmojiSent ? movedYPos : defaultYPos;

            // Calculate the new position of the object
            Vector3 targetPos = new Vector3(targetX, targetY, avatarBody.transform.localPosition.z);

            // Move the object upwards smoothly using Lerp
            avatarBody.transform.localPosition = Vector3.Lerp(avatarBody.transform.localPosition, targetPos, speed * Time.deltaTime);
        }
    }
}