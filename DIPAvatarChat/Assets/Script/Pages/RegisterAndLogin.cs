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

public class RegisterAndLogin : MonoBehaviour
{
    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_Text warningRegisterText;

    [Header("Reset Password")]
    public TMP_InputField emailForPasswordResetField;

    [Header("Form")]
    public GameObject loginForm;
    public GameObject registerForm;
    public GameObject resetPasswordForm;

    public static string emailData { get; set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        //attach event listeners on enable
        AuthManager.Instance.RegisterWarning += SetRegisterWarning;
        AuthManager.Instance.LoginWarning += SetLoginWarning;
        AuthManager.Instance.RegisterConfirm += SetRegisterConfirm;
        AuthManager.Instance.LoginConfirm += SetLoginConfirm;
        AuthManager.Instance.ClearWarning += ClearText;
        AuthManager.Instance.EmailVerificationSent += ToggleUI;
    }

    private void OnDisable()
    {
        //detach event listeners on disable
        if (!this.gameObject.scene.isLoaded) return;
        AuthManager.Instance.RegisterWarning -= SetRegisterWarning;
        AuthManager.Instance.LoginWarning -= SetLoginWarning;
        AuthManager.Instance.RegisterConfirm -= SetRegisterConfirm;
        AuthManager.Instance.LoginConfirm -= SetLoginConfirm;
        AuthManager.Instance.ClearWarning -= ClearText;
        AuthManager.Instance.EmailVerificationSent += ToggleUI;
    }

    public async void LoginButton()
    {
        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(emailLoginField.text);
        UserData myUserData = myUserDoc.ConvertTo<UserData>();

        if (myUserData.username != null && myUserData.status != null)
        {
            AuthManager.Instance.StartLogin(emailLoginField.text, passwordLoginField.text, "4-ChatList");
            return;
        }

        AuthManager.Instance.StartLogin(emailLoginField.text, passwordLoginField.text, "3-EditProfile");
        emailData = emailLoginField.text;
    }

    public void RegisterButton()
    {
        AuthManager.Instance.StartRegistration(emailRegisterField.text, passwordRegisterField.text);
    }

    public void PasswordResetButton()
    {
        AuthManager.Instance.StartPasswordReset(emailForPasswordResetField.text);
    }

    private void SetLoginWarning(string warning)
    {
        ClearText();
        warningLoginText.text = warning;
    }

    private void SetRegisterWarning(string warning)
    {
        ClearText();
        warningRegisterText.text = warning;
    }

    private void SetLoginConfirm(string confirm)
    {
        ClearText();
        confirmLoginText.text = confirm;
    }

    private void SetRegisterConfirm(string confirm)
    {
        ClearText();
    }
    public void ToggleUI()
    {
        UIManager.Instance.ToggleLoginRegister(loginForm, registerForm);
    }

    public void ToggleResetPasswordForm()
    {
        UIManager.Instance.ToggleGeneralTab(resetPasswordForm);
    }

    private void ClearText()
    {
        warningLoginText.text = "";
        warningRegisterText.text = "";
        confirmLoginText.text = "";
    }


}
