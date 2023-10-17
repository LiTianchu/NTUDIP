using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerARChat : MonoBehaviour
{
    private Animator animator;

    private bool isOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get the Animator component from the parent GameObject
        animator = transform.parent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleAnimation()
    {
        if (animator != null)
        {
            if (isOpen)
            {
                animator.SetTrigger("Close");
            }
            else
            {
                animator.SetTrigger("Open");
            }

            isOpen = !isOpen;
        }
    }
}
