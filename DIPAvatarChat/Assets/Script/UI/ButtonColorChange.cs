using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChange : MonoBehaviour
{
    [SerializeField] private Button buttonToChange; // Reference to the button
    [SerializeField] private Color newColor = Color.red; // Change this to the desired color

    private void Start()
    {
        InitializeButtonColorChange();
    }

    private void InitializeButtonColorChange()
    {
        // Attach a click event listener to the button
        if (buttonToChange == null)
        {
            Debug.LogError("Button to change is null in ButtonColorChange.cs on GameObject: " + gameObject.name);
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
            Debug.LogError("Button to change is null in ButtonColorChange.cs on GameObject: " + gameObject.name);
            return;
        }
        buttonToChange.image.color = newColor;
    }
}
