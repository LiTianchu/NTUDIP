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
    public Image sidebarImage;
    public Image sourceImage;

    public TextMeshProUGUI alternateText1;
    public TextMeshProUGUI alternateText2;
    public TextMeshProUGUI alternateText3;
    public Image alternateButton;
    public Image arChatSlideBtn;
    public Image arHeaderImage;

    public GameObject chatlistBoxPrefab;
    public GameObject friendRequestBoxPrefab;
    public GameObject contactsBoxPrefab;
    public GameObject myTextBubblePrefab;
    public GameObject theirTextBubblePrefab;

    public GameObject loadingUiPrefab1;
    public Image loadingUiBackground1;
    public GameObject loadingUiPrefab2;
    public Image loadingUiBackground2;

    public Image messageBarImage;

    private bool isAlternate = false;

    Color newPinkColor = new Color(0.9339623f, 0.8576292f, 0.849377f);
    Color oldPinkColor = new Color(1f, 0.855f, 0.839f);
    Color blackColor1 = new Color(0.188f, 0.161f, 0.212f);
    Color blackColor2 = new Color(0.278f, 0.239f, 0.31f);
    Color blackColor3 = new Color(0.33f, 0.33f, 0.33f);
    Color offWhiteColor = new Color(0.937f, 0.914f, 0.906f);
    Color textGreyColor = new Color(0.4666667f, 0.4666667f, 0.4666667f);
    Color textBlackColor = new Color(0.1960784f, 0.1960784f, 0.1960784f);
    Color purpleColor = new Color(0.6509804f, 0.5450981f, 0.8588236f);
    Color sidebarPurpleColor = new Color(0.8035422f, 0.7181085f, 0.8584906f, 0.509804f);
    Color sidebarBlackColor = new Color(0.188f, 0.161f, 0.212f, 0.509804f);

    private void Start()
    {
        Debug.Log("Start method called.");
        // Load the saved state when the scene starts.
        LoadBackgroundState();

        // Initialize the button's state based on the loaded state.
        UpdateButtonState();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleBackgroundImage()
    {
        Debug.Log("ToggleBackgroundImage method called.");
        isAlternate = !isAlternate;
        SaveBackgroundState(); // Save the state before transitioning to the next scene.
        SceneManager.LoadScene("4-Chatlist");
    }

    private void UpdateButtonState()
    {
        GameObject[] sidebarTabPrefabs = GameObject.FindGameObjectsWithTag("SidebarTabPrefab");

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

        if (chatUiHeaderImage == null)
        {
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
                headerImage.color = blackColor1;
            }
            if (chatUiHeaderImage != null)
            {
                chatUiHeaderImage.color = blackColor1;
            }
            if (sourceImage != null)
            {
                sourceImage.color = new Color(0.84f, 0.84f, 0.84f);
            }
            if (sidebarImage != null)
            {
                sidebarImage.color = sidebarBlackColor;
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
                arChatSlideBtn.color = blackColor1;
            }
            if (arHeaderImage != null)
            {
                arHeaderImage.color = blackColor1;
            }

            if (chatlistBoxPrefab != null)
            {
                UpdateChatListBoxPrefabColor(blackColor2, Color.white, offWhiteColor);
            }
            if (friendRequestBoxPrefab != null)
            {
                UpdateFriendRequestBoxPrefabColor(blackColor2, Color.white, offWhiteColor, blackColor1);
            }
            if (contactsBoxPrefab != null)
            {
                UpdateContactsBoxPrefabColor(blackColor2, Color.white, offWhiteColor);
            }
            if (myTextBubblePrefab != null && theirTextBubblePrefab != null)
            {
                UpdateChatBubblePrefabColor(purpleColor, blackColor2, textBlackColor, offWhiteColor);
            }

            if (loadingUiPrefab1 != null)
            {
                if (loadingUiBackground1 != null)
                {
                    loadingUiBackground1.color = blackColor3;
                }
            }
            if (loadingUiPrefab2 != null)
            {
                if (loadingUiBackground2 != null)
                {
                    loadingUiBackground2.color = blackColor3;
                }
            }
            if (messageBarImage != null)
            {
                messageBarImage.color = blackColor1;
            }

            foreach (GameObject obj in sidebarTabPrefabs)
            {
                obj.GetComponent<Image>().color = blackColor2;
                obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = offWhiteColor;
            }
        }
        else
        {
            if (backgroundImage != null)
            {
                backgroundImage.sprite = originalSprite;
            }
            if (headerImage != null)
            {
                headerImage.color = newPinkColor;
            }
            if (chatUiHeaderImage != null)
            {
                chatUiHeaderImage.color = newPinkColor;
            }
            if (sourceImage != null)
            {
                sourceImage.color = Color.white;
            }
            if (sidebarImage != null)
            {
                sidebarImage.color = sidebarPurpleColor;
            }
            if (alternateText1 != null)
            {
                alternateText1.color = blackColor3;
            }
            if (alternateText2 != null)
            {
                alternateText2.color = blackColor3;
            }
            if (alternateText3 != null)
            {
                alternateText3.color = blackColor3;
            }
            if (alternateButton != null)
            {
                alternateButton.color = blackColor3;
            }
            if (arChatSlideBtn != null)
            {
                arChatSlideBtn.color = newPinkColor;
            }
            if (arHeaderImage != null)
            {
                arHeaderImage.color = newPinkColor;
            }

            if (chatlistBoxPrefab != null)
            {
                UpdateChatListBoxPrefabColor(offWhiteColor, textBlackColor, textGreyColor);
            }
            if (friendRequestBoxPrefab != null)
            {
                UpdateFriendRequestBoxPrefabColor(offWhiteColor, textBlackColor, textGreyColor, offWhiteColor);
            }
            if (contactsBoxPrefab != null)
            {
                UpdateContactsBoxPrefabColor(offWhiteColor, textBlackColor, textGreyColor);
            }
            if (myTextBubblePrefab != null && theirTextBubblePrefab != null)
            {
                UpdateChatBubblePrefabColor(newPinkColor, offWhiteColor, textBlackColor, textBlackColor);
            }

            if (loadingUiPrefab1 != null)
            {
                if (loadingUiBackground1 != null)
                {
                    loadingUiBackground1.color = Color.white;
                }
            }
            if (loadingUiPrefab2 != null)
            {
                if (loadingUiBackground2 != null)
                {
                    loadingUiBackground2.color = Color.white;
                }
            }

            if (messageBarImage != null)
            {
                messageBarImage.color = newPinkColor;
                //messageBarImage.color = new Color(0.9339623f, 0.8576292f, 0.849377f);
            }

            foreach (GameObject obj in sidebarTabPrefabs)
            {
                obj.GetComponent<Image>().color = offWhiteColor;
                obj.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = textBlackColor;
            }
        }

        void UpdateChatListBoxPrefabColor(Color bg, Color text1, Color text2)
        {
            // Box Color
            chatlistBoxPrefab.transform.GetChild(0).gameObject.GetComponent<Image>().color = bg;
            // Time color
            chatlistBoxPrefab.transform.GetChild(0).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().color = text2;
            // Username color
            chatlistBoxPrefab.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = text1;
            // status color
            chatlistBoxPrefab.transform.GetChild(0).GetChild(2).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().color = text2;
        }

        void UpdateFriendRequestBoxPrefabColor(Color bg, Color text1, Color text2, Color btn)
        {
            // Box Color
            friendRequestBoxPrefab.transform.GetChild(0).gameObject.GetComponent<Image>().color = bg;
            // Username Color
            friendRequestBoxPrefab.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = text1;
            // Email Color
            friendRequestBoxPrefab.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().color = text2;
            // Btn Colors
            friendRequestBoxPrefab.transform.GetChild(0).GetChild(2).GetChild(0).gameObject.GetComponent<Image>().color = btn;
            friendRequestBoxPrefab.transform.GetChild(0).GetChild(2).GetChild(1).gameObject.GetComponent<Image>().color = btn;
            // Btn text Colors
            friendRequestBoxPrefab.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = text2;
            friendRequestBoxPrefab.transform.GetChild(0).GetChild(2).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = text2;
        }

        void UpdateContactsBoxPrefabColor(Color bg, Color text1, Color text2)
        {
            // Box Color
            contactsBoxPrefab.transform.GetChild(0).gameObject.GetComponent<Image>().color = bg;
            // Username Color
            contactsBoxPrefab.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = text1;
            // Status Color
            contactsBoxPrefab.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().color = text2;
        }

        void UpdateChatBubblePrefabColor(Color bgMy, Color bgTh, Color text1, Color text2)
        {
            // user box color
            myTextBubblePrefab.transform.GetChild(0).gameObject.GetComponent<Image>().color = bgMy;
            // user text color
            myTextBubblePrefab.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = text1;

            // recipient box color
            theirTextBubblePrefab.transform.GetChild(0).gameObject.GetComponent<Image>().color = bgTh;
            // recipient text color
            theirTextBubblePrefab.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = text2;
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