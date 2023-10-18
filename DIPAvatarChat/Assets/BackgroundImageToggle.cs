using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageToggle : MonoBehaviour
{
    public Image backgroundImage;
    public Sprite originalSprite;
    public Sprite alternateSprite;

    private bool isAlternate = false;

    private void Start()
    {
        // Initialize the button's state based on the initial background sprite.
        UpdateButtonState();
    }

    public void ToggleBackgroundImage()
    {
        isAlternate = !isAlternate;
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (isAlternate)
        {
            backgroundImage.sprite = alternateSprite;
        }
        else
        {
            backgroundImage.sprite = originalSprite;
        }
    }
}
