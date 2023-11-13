using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSideBarV2 : MonoBehaviour
{
    public RectTransform sidebar; // Reference to the RectTransform of the sidebar panel
    private bool isSidebarVisible = false; // Flag to track if the sidebar is visible
    [SerializeField] private float slideSpeed = 8f; // Speed at which the sidebar slides in/out
    [SerializeField] private float sidebarHiddenPos = 250f;
    [SerializeField] private float sidebarShownPos = 10f;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the sidebar position based on its initial visibility
        UpdateSidebarPosition();
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

        // Update the sidebar position immediately when toggling
        UpdateSidebarPosition();
    }

    #region Helper Methods

    private void UpdateSidebarPosition()
    {
        // Set the sidebar position based on its visibility status
        sidebar.anchoredPosition = isSidebarVisible ? new Vector2(sidebarShownPos, sidebar.anchoredPosition.y) : new Vector2(sidebarHiddenPos, sidebar.anchoredPosition.y);
    }

    #endregion
}
