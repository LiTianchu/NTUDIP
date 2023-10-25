using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    public Button buttonToChange; // Reference to the button
    public Color newColor = Color.red; // Change this to the desired color

    private void Start()
    {
        // Attach a click event listener to the button
        if (buttonToChange == null)
        {
            Debug.Log("Button to change is null in ButtonColorChange.cs");
        }
        else
        {
            buttonToChange.onClick.AddListener(ChangeButtonColor);
        }
    }

    private void ChangeButtonColor()
    {
        // Change the color of the button when it's clicked
        if (buttonToChange == null)
        {
            Debug.Log("Button to change is null in ButtonColorChange.cs");
            return;
        }
        buttonToChange.image.color = newColor;
    }
}
