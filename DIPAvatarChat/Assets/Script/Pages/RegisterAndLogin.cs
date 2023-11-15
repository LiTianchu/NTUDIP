using Firebase.Firestore;
using System.Collections;
using TMPro;
using UnityEngine;

public class RegisterAndLogin : MonoBehaviour, IPageTransition
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

    private CanvasGroup loginFormCG;
    private CanvasGroup registerFormCG;
    private CanvasGroup resetPasswordFormCG;

    private void OnEnable()
    {
        loginFormCG = loginForm.GetComponent<CanvasGroup>();
        registerFormCG = registerForm.GetComponent<CanvasGroup>();
        resetPasswordFormCG = resetPasswordForm.GetComponent<CanvasGroup>();

        FadeInUI();
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
        //front end validation
        if (emailLoginField.text.Trim().Length == 0 || passwordLoginField.text.Trim().Length == 0) {
            SetLoginWarning("Fields cannot be empty");
            return;
        }

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(emailLoginField.text);
        
        if(myUserDoc == null)
        {
            SetLoginWarning("Account does not exist"); return;
        }
        UserData myUserData = myUserDoc.ConvertTo<UserData>();

        if (myUserData == null)
        {
            SetLoginWarning("Account does not exist"); return;
        }
        if (myUserData.username != null && myUserData.status != null && myUserData.currentAvatar != null)
        {
            AuthManager.Instance.StartLogin(emailLoginField.text, passwordLoginField.text, "4-ChatList");
            return;
        }

        AuthManager.Instance.StartLogin(emailLoginField.text, passwordLoginField.text, "3-EditProfile");
        emailData = emailLoginField.text;
    }

    public void RegisterButton()
    {
        //front end validation
        if (emailRegisterField.text.Trim().Length == 0 || passwordRegisterField.text.Trim().Length == 0)
        {
            SetRegisterWarning("Fields cannot be empty");
            return;
        }
        AuthManager.Instance.StartRegistration(emailRegisterField.text, passwordRegisterField.text);
    }

    public void PasswordResetButton()
    {
        if (emailForPasswordResetField.text.Trim().Length == 0)
        {
            return;
        }
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
        if (registerForm.activeSelf)
        {
            //if login form is on, fade away the login and fade in the register
            UIManager.Instance.PanelFadeOut(loginFormCG, 0.2f, UIManager.UIMoveDir.Stay, loginFormCG.GetComponent<RectTransform>().anchoredPosition);
            UIManager.Instance.PanelFadeIn(registerFormCG, 0.3f, UIManager.UIMoveDir.Stay, registerFormCG.GetComponent<RectTransform>().anchoredPosition);
        }
        else if(loginForm.activeSelf)
        {
            //if register form is on fade away the register and fade in the login

            UIManager.Instance.PanelFadeOut(registerFormCG, 0.2f, UIManager.UIMoveDir.Stay, registerFormCG.GetComponent<RectTransform>().anchoredPosition);
            UIManager.Instance.PanelFadeIn(loginFormCG, 0.3f, UIManager.UIMoveDir.Stay, loginFormCG.GetComponent<RectTransform>().anchoredPosition);
        }
        
    }

    public void ToggleResetPasswordForm()
    {
        UIManager.Instance.ToggleGeneralTab(resetPasswordForm);
    }

    private void ClearText()
    {
        if (warningLoginText != null)
        {
            warningLoginText.text = "";
        }
        if (warningRegisterText != null)
        {
            warningRegisterText.text = "";
        }
        if (confirmLoginText != null)
        {
            confirmLoginText.text = "";
        }
    }

    public void FadeInUI()
    {
        UIManager.Instance.PanelFadeIn(registerFormCG, 0.5f, UIManager.UIMoveDir.FromLeft, registerFormCG.GetComponent<RectTransform>().anchoredPosition);
        
    }

    public IEnumerator ExitRoutine()
    {
        throw new System.NotImplementedException();
    }
}
