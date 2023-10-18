using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackgroundImageToggle : MonoBehaviour
{
    public Image backgroundImage;
    public Sprite originalSprite;
    public Sprite alternateSprite;
    public Image headerImage; // Reference to the header UI element.
    public Image sourceImage; // Reference to the source image UI element.

    private bool isAlternate = false;

    private void Start()
    {
        // Load the saved state when the scene starts.
        LoadBackgroundState();

        // Initialize the button's state based on the loaded state.
        UpdateButtonState();
    }

    public void ToggleBackgroundImage()
    {
        isAlternate = !isAlternate;
        UpdateButtonState();
        SaveBackgroundState(); // Save the state when toggling.
    }

    private void UpdateButtonState()
{
    if (isAlternate)
    {
        backgroundImage.sprite = alternateSprite;
        headerImage.color = new Color(0.75f, 0.75f, 0.75f); // Grey color (R: 0.75, G: 0.75, B: 0.75).
        sourceImage.color = Color.black; // Change the source image color to black.
    }
    else
    {
        backgroundImage.sprite = originalSprite;
        headerImage.color = new Color(0.86f, 0.63f, 0.63f); // Pink color (R: 0.86, G: 0.63, B: 0.63).
        sourceImage.color = Color.white; // Change the source image color to white.
    }
}



    private void SaveBackgroundState()
    {
        // Save the state to PlayerPrefs.
        PlayerPrefs.SetInt("IsAlternateBackground", isAlternate ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadBackgroundState()
    {
        // Load the state from PlayerPrefs.
        isAlternate = PlayerPrefs.GetInt("IsAlternateBackground", 0) == 1;
    }

    // This method is called when the scene changes.
    private void OnLevelWasLoaded(int level)
    {
        LoadBackgroundState();
        UpdateButtonState();
    }
}
