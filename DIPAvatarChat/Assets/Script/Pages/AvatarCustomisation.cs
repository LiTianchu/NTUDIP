using UnityEngine;

public class AvatarCustomisation : MonoBehaviour
{
    public GameObject FeaturesBox;
    public GameObject AvatarDisplayArea;
    public Notification notification;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseAvatarCustomisation();
    }

    public void LoadEditProfile()
    {
        AppManager.Instance.LoadScene("3-EditProfile");
    }

    public void DisplayPanel(string chooseFeatureName)
    {
        foreach (Transform child in FeaturesBox.transform)
        {
            Debug.Log("Child GameObject: " + child.gameObject.name);
            if (child.gameObject.name == chooseFeatureName)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                if (child.gameObject.name != "chooseFeature")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

    public async void SaveAvatarData()
    {
        // Call the AvatarBackendManager to update the avatar data
        
        bool updated = await AvatarBackendManager.Instance.UpdateAvatarData();

        if (updated)
        {
            Debug.Log("Avatar data updated successfully!");
            Instantiate(notification, transform);
        }
        else
        {
            Debug.LogError("Failed to update avatar data.");
        }
    }

    public void InitialiseAvatarCustomisation()
    {
        if (AvatarBackendManager.Instance.currAvatarData != null)
        {
            AvatarManager.Instance.InitialiseAvatarCustomisation(AvatarDisplayArea);
        }
    }
}
