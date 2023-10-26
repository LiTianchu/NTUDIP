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
using Firebase.Auth;

public class AnimationManager : Singleton<AnimationManager>
{
    // Map custom commands to play animation
    public readonly Dictionary<string, string> emojiToAnimMap = new Dictionary<string, string>
    {
        { ">:(", "Angry"},
        { ":angry:", "Angry"},
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
                    return;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error playing animation: " + e);
        }
    }
}