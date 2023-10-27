using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BackgroundImageToggle : MonoBehaviour
{
    public Image backgroundImage;
    public Sprite originalSprite;
    public Sprite alternateSprite;
    public Image headerImage;
    public Image chatUiHeaderImage;
    public Image sourceImage;

    public TextMeshProUGUI alternateText1;
    public TextMeshProUGUI alternateText2;
    public TextMeshProUGUI alternateText3;
    public Image alternateButton;
    public Image arChatSlideBtn;

    public GameObject chatlistBoxPrefab;
    public Image chatlistBoxBackground;
    public TextMeshProUGUI chatlistBoxText;

    public GameObject loadingUiPrefab;
    public Image loadingUiBackground;

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
        if (backgroundImage == null)
        {
            Debug.LogWarning("backgroundImage component is missing.");
        }

        if (sourceImage == null)
        {
            Debug.LogWarning("sourceImage component is missing.");
        }

        if (headerImage == null)
        {
            Debug.LogWarning("headerImage component is missing.");
        }

        if (chatUiHeaderImage == null) {
            Debug.LogWarning("chatUiHeaderImage component is missing.");
        }

        if (isAlternate)
        {
            Debug.Log("isAlternate: " + isAlternate);

            if (backgroundImage != null)
            {
                backgroundImage.sprite = alternateSprite;
            }
            if (headerImage != null)
            {
                headerImage.color = new Color(0.188f, 0.161f, 0.212f);
            }
            if (chatUiHeaderImage != null) 
            {
                chatUiHeaderImage.color = new Color(0.278f, 0.239f, 0.31f);
            }
            if (sourceImage != null) 
            {
                sourceImage.color = new Color(0.84f, 0.84f, 0.84f);
            }
            if (alternateText1 != null)
            {
                alternateText1.color = Color.white;
            }
            if (alternateText2 != null)
            {
                alternateText2.color = Color.white;
            }
            if (alternateText3 != null)
            {
                alternateText3.color = Color.white;
            }
            if (alternateButton != null)
            {
                alternateButton.color = Color.white;
            }
            if (arChatSlideBtn != null)
            {
                arChatSlideBtn.color = new Color(0.188f, 0.161f, 0.212f);
            }

            if (chatlistBoxPrefab != null)
            {
                if (chatlistBoxBackground != null)
                {
                    chatlistBoxBackground.color = new Color(0.188f, 0.161f, 0.212f); 
                }
                if (chatlistBoxText != null)
                {
                    chatlistBoxText.color = Color.white;
                }
            }

            if (loadingUiPrefab != null)
            {
                if (loadingUiBackground != null)
                {
                    loadingUiBackground.color = new Color(0.33f, 0.33f, 0.33f);
                }
            }
        }
        else
        {
            backgroundImage.sprite = originalSprite;
            if (headerImage != null)
            {
                headerImage.color = new Color(1f, 0.855f, 0.839f);
            }
            if (chatUiHeaderImage != null) {
                chatUiHeaderImage.color = new Color(1f, 0.855f, 0.839f);
            }
            if (sourceImage != null)
            {
                sourceImage.color = Color.white;
            }
            if (alternateText1 != null)
            {
                alternateText1.color = new Color(0.333f, 0.333f, 0.333f);
            }
            if (alternateText2 != null)
            {
                alternateText2.color = new Color(0.333f, 0.333f, 0.333f);
            }
            if (alternateText3 != null)
            {
                alternateText3.color = new Color(0.333f, 0.333f, 0.333f);
            }
            if (alternateButton != null)
            {
                alternateButton.color = new Color(0.333f, 0.333f, 0.333f);
            }
            if (arChatSlideBtn != null)
            {
                arChatSlideBtn.color = new Color(1f, 0.855f, 0.839f);
            }

            if (chatlistBoxPrefab != null)
            {
                if (chatlistBoxBackground != null)
                {
                    chatlistBoxBackground.color = new Color(0.937f, 0.914f, 0.906f); 
                }
                if (chatlistBoxText != null)
                {
                    chatlistBoxText.color = Color.white;
                }
            }

            if (loadingUiPrefab != null)
            {
                if (loadingUiBackground != null)
                {
                    loadingUiBackground.color = Color.white;
                }
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