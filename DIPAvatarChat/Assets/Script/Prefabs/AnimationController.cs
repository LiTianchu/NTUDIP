using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Example: Play the animation when the space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Trigger the animation by setting a trigger parameter
            animator.SetTrigger("TriggerName"); // Replace "TriggerName" with the actual trigger parameter name
        }
    }
}