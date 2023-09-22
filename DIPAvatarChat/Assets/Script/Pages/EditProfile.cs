using System.Collections;
using System.Collections.Generic;
using TMPro;
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


    public void SaveProfile() {
        string username = usernameField.text;
        string status = statusField.text;


        if (!string.IsNullOrEmpty(username))
        {
            if (UserBackendManager.Instance.UpdateUsernameAndStatus(username, status))
            {

                AppManager.Instance.LoadScene("4-ChatList");
            }
            
        }
        else
        {
            errorMessage.text = "Field cannot be empty";
        }      
        
    }

    public void LoadAvatarCustomization(){

        AppManager.Instance.LoadScene("AvatarCustomisation");
    }

    public void LoadEditProfile(){

        AppManager.Instance.LoadScene("3-EditProfile");
    }


   
}
