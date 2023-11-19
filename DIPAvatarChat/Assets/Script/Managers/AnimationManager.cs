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
        { ":cry:", "Crying"},
    };

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

    public void InitializeAnimation(GameObject myAvatarObj, GameObject theirAvatarObj)
    {
        //myAvatarBodyChat = GameObject.Find(ChatManager.Instance.MY_AVATAR_BODY_PATH);
        //theirAvatarBodyChat = GameObject.Find(ChatManager.Instance.THEIR_AVATAR_BODY_PATH);
        myAvatarBodyChat = myAvatarObj;
        theirAvatarBodyChat = theirAvatarObj;

        if (myAvatarObj != null)
        {
            myAnimatorChat = myAvatarBodyChat.GetComponent<Animator>();
        }
        if (theirAvatarObj != null)
        {
            theirAnimatorChat = theirAvatarBodyChat.GetComponent<Animator>();
        }
    }

    public void EndAnimation()
    {
        myAvatarBodyChat = null;
        Instance.theirAvatarBodyChat = null;

        Instance.myAnimatorChat = null;
        Instance.theirAnimatorChat = null;
    }

    public void PlayEmoteAnimation(string msgText, bool isPoppingUp ,bool isMyAvatar)
    {
        try
        {
            foreach (var kvp in emojiToAnimMap)
            {
                if (msgText.Contains(kvp.Key))
                {
                    Debug.Log("Animation: " + kvp.Value);
                    

                    if (isMyAvatar && myAnimatorChat!=null)
                    {
                        myAnimatorChat.SetBool(kvp.Value, true);
                        if (isPoppingUp) //set flag for popping up
                        {
                            myAnimNameChat = kvp.Value;
                        }
                        myAnimatorChat.SetBool("Default", true);
                    }
                    else if(!isMyAvatar && theirAnimatorChat!=null)
                    {
                        theirAnimatorChat.SetBool(kvp.Value, true);
                        if (isPoppingUp) //set flag for popping up
                        {
                            theirAnimNameChat = kvp.Value;
                        }
                        theirAnimatorChat.SetBool("Default", true);
                    }

                    //if (myAnimatorChat != null)
                    //{
                    //    myAnimatorChat.SetBool("Default", true);
                    //}
                    //else
                    //{
                    //    Debug.LogWarning("myAnimatorChat is null");
                    //}
                    
                    //if (theirAnimatorChat != null)
                    //{
                    //    theirAnimatorChat.SetBool("Default", true);
                    //}
                    //else
                    //{
                    //    Debug.LogWarning("theirAnimatorChat is null");
                    //}

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

    public void UI_SlideInFromBelow(RectTransform UI, bool isVisible, float slideSpeed, float hiddenPos, float shownPos)
    {
        float targetY = isVisible ? shownPos : hiddenPos;
        float newY = Mathf.Lerp(UI.anchoredPosition.y, targetY, Time.deltaTime * slideSpeed);
        UI.anchoredPosition = new Vector2(UI.anchoredPosition.x, newY);
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