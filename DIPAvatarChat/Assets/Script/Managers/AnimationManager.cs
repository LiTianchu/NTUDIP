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
        { ">:(", "Angry"},
        { ":angry:", "Angry"},
        { "hi!", "Wave"},
        { ":laughing:", "Laugh"},
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
}