using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSideBarV2 : MonoBehaviour
{
    public RectTransform sidebar; // Reference to the RectTransform of the sidebar panel
    public float slideSpeed = 2f; // Speed at which the sidebar slides in/out
    public float sidebarHiddenPos = 1000f;
    public float sidebarShownPos = 0f;

    private bool isSidebarVisible = false; // Flag to track if the sidebar is visible

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Slide the sidebar based on its visibility status
        float targetX = isSidebarVisible ? sidebarShownPos : sidebarHiddenPos;
        float newX = Mathf.Lerp(sidebar.anchoredPosition.x, targetX, Time.deltaTime * slideSpeed);
        sidebar.anchoredPosition = new Vector2(newX, sidebar.anchoredPosition.y);
    }

    public void ToggleSidebar()
    {
        // Toggle the visibility flag
        isSidebarVisible = !isSidebarVisible;
    }
}
