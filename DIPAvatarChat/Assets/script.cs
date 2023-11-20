using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;

    void Start()
    {
        // Assuming you have set up references to your panels in the Unity Editor
        // If not, you can drag and drop the panels into these fields in the Unity Editor.
        // Alternatively, you can use GameObject.Find or other methods to find the panels.

        // Example:
        // panel1 = GameObject.Find("Panel1");
        // panel2 = GameObject.Find("Panel2");

        // Set the initial state
        panel1.SetActive(true);
        panel2.SetActive(false);
    }

    public void OnBackButtonPress()
    {
        // Handle the back button press event here
        // For simplicity, let's just toggle the visibility of the panels

        // Hide the current panel
        panel1.SetActive(false);

        // Show the other panel
        panel2.SetActive(true);
    }
}
