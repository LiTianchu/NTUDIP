using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EditProfile : MonoBehaviour
{
    [Header("UserInfo")]
    public TMP_InputField usernameField;
    public TMP_InputField statusField;
    public Button submitButton;
    public TMP_Text errorMessage;

    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(SaveProfile);
    }

    // Update is called once per frame
    void Update()
    {

    }


    public async void SaveProfile()
    {
        string username = usernameField.text;
        string status = statusField.text;

        if (!string.IsNullOrEmpty(username) && AvatarBackendManager.Instance.currAvatarData != null)
        {
            if (UserBackendManager.Instance.UpdateUsernameAndStatus(username, status) && await AvatarBackendManager.Instance.UploadAvatar())
            {
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
        AvatarData avatarData = new AvatarData
        {
            createdAt = DateTime.Now,
            userEmail = AuthManager.Instance.currUser.email,
        };

        AvatarBackendManager.Instance.currAvatarData = avatarData;
        AppManager.Instance.LoadScene("AvatarCustomisation");
    }

    public void LoadEditProfile()
    {
        AppManager.Instance.LoadScene("3-EditProfile");
    }
}
