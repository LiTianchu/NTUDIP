using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BackgroundImageToggle : MonoBehaviour
{
    public Image backgroundImage;
    public Sprite originalSprite;
    public Sprite alternateSprite;
    public Image headerImage;
    public Image sourceImage;

    private bool isAlternate = false;

    private void Start()
    {
        Debug.Log("Start method called.");
        // Load the saved state when the scene starts.
        LoadBackgroundState();

        // Initialize the button's state based on the loaded state.
        UpdateButtonState();
    }

    public void ToggleBackgroundImage()
    {
        Debug.Log("ToggleBackgroundImage method called.");
        isAlternate = !isAlternate;
        SaveBackgroundState(); // Save the state before transitioning to the next scene.
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        if (backgroundImage == null) {
            Debug.LogWarning("backgroundImage component is missing.");
            return;
        }

        if (sourceImage == null) {
            Debug.LogWarning("sourceImage component is missing.");
        }

        if (headerImage == null) {
            Debug.LogWarning("headerImage component is missing.");
        }

        if (isAlternate)
        {
            backgroundImage.sprite = alternateSprite;
            if (headerImage != null) {
                headerImage.color = new Color(0.75f, 0.75f, 0.75f);
            }
            if (sourceImage != null) {
                sourceImage.color = Color.black;
            }
        }
        else
        {
            backgroundImage.sprite = originalSprite;
            if (headerImage != null) {
                headerImage.color = new Color(0.86f, 0.63f, 0.63f);
            }
            if (sourceImage != null) {
                sourceImage.color = Color.white;
            }
        }
    }

    private void SaveBackgroundState()
    {
        PlayerPrefs.SetInt("IsAlternateBackground", isAlternate ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadBackgroundState()
    {
        isAlternate = PlayerPrefs.GetInt("IsAlternateBackground", 0) == 1;
        Debug.Log("Loaded background state: " + isAlternate); // Add this line for debugging.
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("SceneLoaded method called.");
        LoadBackgroundState();
        UpdateButtonState();
    }
}
