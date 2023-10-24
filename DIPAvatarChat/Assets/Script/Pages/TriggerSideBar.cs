using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSideBar : MonoBehaviour
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

    public void ToggleAnimationSideBar()
    {
        if (animator != null)
        {
            if (isOpen)
            {
                animator.SetTrigger("CloseSideBar");
            }
            else
            {
                animator.SetTrigger("OpenSideBar");
            }

            isOpen = !isOpen;
        }
    }
}
