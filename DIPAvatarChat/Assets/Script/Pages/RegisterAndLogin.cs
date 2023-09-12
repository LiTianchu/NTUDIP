using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

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
        //attach event method on enable
        AuthManager.Instance.RegisterWarning += SetRegisterWarning;
        AuthManager.Instance.LoginWarning += SetLoginWarning;
        AuthManager.Instance.RegisterConfirm += SetRegisterConfirm;
        AuthManager.Instance.LoginConfirm += SetLoginConfirm;
    }

    private void OnDisable()
    {
        //detach event method on disable
        AuthManager.Instance.RegisterWarning -= SetRegisterWarning;
        AuthManager.Instance.LoginWarning -= SetLoginWarning;
        AuthManager.Instance.RegisterConfirm -= SetRegisterConfirm;
        AuthManager.Instance.LoginConfirm -= SetLoginConfirm;
    }

    public void LoginButton()
    {
        AuthManager.Instance.StartLogin(emailLoginField.text, passwordLoginField.text);
    }

    public void RegisterButton()
    {
        AuthManager.Instance.StartRegistration(emailRegisterField.text, passwordRegisterField.text);
    }

    private void SetLoginWarning(string warning) {
        ClearText();
        warningLoginText.text = warning;
    }

    private void SetRegisterWarning(string warning)
    {
        ClearText();
        warningRegisterText.text = warning;
    }

    private void SetLoginConfirm(string confirm) {
        ClearText();
        confirmLoginText.text = confirm;
    }

    private void SetRegisterConfirm(string confirm) {
        ClearText();
    }

    private void ClearText() {
        warningLoginText.text = "";
        warningRegisterText.text = "";
        confirmLoginText.text = "";
    }
  
    
}
