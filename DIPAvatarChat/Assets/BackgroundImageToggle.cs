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
        // Load the saved state when the scene starts.
        LoadBackgroundState();

        // Initialize the button's state based on the loaded state.
        UpdateButtonState();
    }

    public void ToggleBackgroundImage()
    {
        isAlternate = !isAlternate;
        SaveBackgroundState(); // Save the state before transitioning to the next scene.
        UpdateButtonState();
    }



    private void UpdateButtonState()
    {
        if (isAlternate)
        {
            backgroundImage.sprite = alternateSprite;
            headerImage.color = new Color(0.75f, 0.75f, 0.75f);
            sourceImage.color = Color.black;
        }
        else
        {
            backgroundImage.sprite = originalSprite;
            headerImage.color = new Color(0.86f, 0.63f, 0.63f);
            sourceImage.color = Color.white;
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
        LoadBackgroundState();
        UpdateButtonState();
    }
}
