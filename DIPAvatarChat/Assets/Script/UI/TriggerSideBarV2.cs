using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSideBarV2 : MonoBehaviour
{
    public RectTransform sidebar; // Reference to the RectTransform of the sidebar panel
    private bool isSidebarVisible = false; // Flag to track if the sidebar is visible
    float slideSpeed = 8f; // Speed at which the sidebar slides in/out
    float sidebarHiddenPos = 250f;
    float sidebarShownPos = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Slide the sidebar based on its visibility status
        AnimationManager.Instance.UI_SlideInFromRightSide(sidebar, isSidebarVisible, slideSpeed, sidebarHiddenPos, sidebarShownPos);
    }

    public void ToggleSidebar()
    {
        // Toggle the visibility flag
        isSidebarVisible = !isSidebarVisible;
    }
}
