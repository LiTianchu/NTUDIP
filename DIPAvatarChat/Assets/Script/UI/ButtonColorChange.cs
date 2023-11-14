using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonColorChange : MonoBehaviour
{

    [SerializeField] private Color newColor = Color.red; // Change this to the desired color
    private Button _buttonToChange; // Reference to the button

    private void Start()
    {
        _buttonToChange = GetComponent<Button>();
        InitializeButtonColorChange();
    }

    private void InitializeButtonColorChange()
    {
        // Attach a click event listener to the button
        if (_buttonToChange == null)
        {
            Debug.LogError("Button to change is null in ButtonColorChange.cs on GameObject: " + gameObject.name);
        }
        else
        {
            _buttonToChange.onClick.AddListener(ChangeButtonColor);
        }
    }

    private void ChangeButtonColor()
    {
        // Change the color of the button when it's clicked
        if (_buttonToChange == null)
        {
            Debug.LogError("Button to change is null in ButtonColorChange.cs on GameObject: " + gameObject.name);
            return;
        }
        _buttonToChange.image.color = newColor;
    }
}
