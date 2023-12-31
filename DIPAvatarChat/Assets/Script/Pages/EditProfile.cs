using Firebase.Firestore;
using System;
using TMPro;
using UnityEngine;

public class EditProfile : MonoBehaviour
{
    [Header("UserInfo")]
    public TMP_InputField usernameField;
    public TMP_InputField statusField;
    public TMP_Text errorMessage;

    // Start is called before the first frame update
    void Start()
    {
        DisplayCurrentData();
    }

    public async void DisplayCurrentData()
    {
        if (AuthManager.Instance.currUser.username != null)
        {
            usernameField.text = AuthManager.Instance.currUser.username;
        }

        if (AuthManager.Instance.currUser.status != null)
        {
            statusField.text = AuthManager.Instance.currUser.status;
        }

        DocumentSnapshot myAvatarDoc = await AvatarBackendManager.Instance.GetAvatarByEmailTask(AuthManager.Instance.currUser.email);
        if (myAvatarDoc != null)
        {
            AvatarBackendManager.Instance.currAvatarData = myAvatarDoc.ConvertTo<AvatarData>();
        }
        else
        {
            InitialiseNewAvatar();
            if (await AvatarBackendManager.Instance.UpdateAvatarData())
            {
                myAvatarDoc = await AvatarBackendManager.Instance.GetAvatarByEmailTask(AuthManager.Instance.currUser.email);
                AvatarBackendManager.Instance.currAvatarData = myAvatarDoc.ConvertTo<AvatarData>();
            }
        }
    }

    public async void SaveProfile()
    {
        string username = usernameField.text;
        string status = statusField.text;

        if (!string.IsNullOrEmpty(username) && AvatarBackendManager.Instance.currAvatarData != null)
        {
            if (UserBackendManager.Instance.UpdateUsernameAndStatus(username, status) && await AvatarBackendManager.Instance.UploadAvatarTask())
            {
                Debug.Log("Profile saved! ^.^");
                AuthManager.Instance.currUser.username = username;
                AuthManager.Instance.currUser.status = status;
                AppManager.Instance.LoadScene("4-ChatList");
            }
        }
        else
        {
            errorMessage.text = "Field cannot be empty";
        }

    }

    public void LoadAvatarCustomization()
    {
        AuthManager.Instance.currUser.username = usernameField.text;
        AuthManager.Instance.currUser.status = statusField.text;
        AppManager.Instance.LoadScene("AvatarCustomisation");
    }

    public void LoadEditProfile()
    {
        AppManager.Instance.LoadScene("3-EditProfile");
    }

    public async void InitialiseNewAvatar()
    {
        if (AvatarBackendManager.Instance.currAvatarData == null)
        {
            AvatarData newAvatarData = new AvatarData
            {
                createdAt = DateTime.Now,
                lastUpdatedAt = DateTime.Now,
                email = AuthManager.Instance.currUser.email,
                arm = null,
                colour = "Blender/Textures/ColorSchemes/Materials/1",
                expression = null,
                hat = null,
                shoes = null,
                tail = "Blender/cattail",
                ears = "Blender/catear",
                texture = null,
                wings = null,
            };
            AvatarBackendManager.Instance.currAvatarData = newAvatarData;
        }
    }

}
