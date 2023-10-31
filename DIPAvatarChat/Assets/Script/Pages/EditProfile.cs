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
    public TMP_Text errorMessage;

    // Start is called before the first frame update
    void Start()
    {
        DisplayCurrentData();
    }

    // Update is called once per frame
    void Update()
    {

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

        if (AvatarBackendManager.Instance.currAvatarData == null)
        {
            DocumentSnapshot myAvatarDoc = await AvatarBackendManager.Instance.GetAvatarByEmailTask(AuthManager.Instance.currUser.email);
            AvatarBackendManager.Instance.currAvatarData = myAvatarDoc.ConvertTo<AvatarData>();
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
                AppManager.Instance.LoadScene("4-ChatList");
            }

        }
        else
        {
            errorMessage.text = "Field cannot be empty";
        }

    }

    public async void LoadAvatarCustomization()
    {
        AppManager.Instance.LoadScene("AvatarCustomisation");
    }

    public void LoadEditProfile()
    {
        AppManager.Instance.LoadScene("3-EditProfile");
    }
}
