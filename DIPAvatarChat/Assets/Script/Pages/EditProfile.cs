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


    public void SaveProfile()
    {
        string username = usernameField.text;
        string status = statusField.text;
        AvatarData avatarData = AvatarBackendManager.Instance.currAvatarData;

        if (!string.IsNullOrEmpty(username) && avatarData != null)
        {
            if (UserBackendManager.Instance.UpdateUsernameAndStatus(username, status) && AvatarBackendManager.Instance.UploadAvatar(avatarData) != null)
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
        if (AvatarBackendManager.Instance.currAvatarData == null)
        {
            AvatarData avatarData = new AvatarData
            {
                createdAt = DateTime.Now,
                colour = null,
                texture = null,
                expression = null,
                hat = null,
                arm = null,
                wings = null,
                tail = null,
                userEmail = AuthManager.Instance.currUser.email,
            };

            AvatarBackendManager.Instance.currAvatarData = avatarData;
        }

        AppManager.Instance.LoadScene("AvatarCustomisation");
    }

    public void LoadEditProfile()
    {

        AppManager.Instance.LoadScene("3-EditProfile");
    }



}
