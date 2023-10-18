using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class AuthManager : Singleton<AuthManager>
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth = null;
    public FirebaseUser user;

    //public string emailData { get; set; }
    //public string passwordData { get; set; }
    public string userPathData { get; set; }
    public string friendRequestPathData { get; set; }
    public UserData currUser { get; set; }
    public string currConvId { get; set; }

    //Login events
    //Events are used to notify pages that login is successful or failed
    //Events handlers are in RegisterAndLogin.cs
    public event Action<string> LoginWarning;
    public event Action<string> LoginConfirm;
    public event Action<string> RegisterWarning;
    public event Action<string> RegisterConfirm;
    public event Action ClearWarning;
    public event Action EmailVerificationSent;


    async void Start()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(async task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                Debug.Log("Setting up Firebase Auth");
                //Set the authentication instance object
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    //Function for the login button
    public void StartLogin(string email, string password, string landingScene)
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(email, password, landingScene));
    }
    //Function for the register button
    public void StartRegistration(string email, string password)
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(email, password));
    }

    //Function for the Reseet Password Function
    public void StartPasswordReset(string email)
    {
        // Call the password reset coroutine passing the email
        StartCoroutine(ResetPassword(email));
    }


    //Login Function
    private IEnumerator Login(string _email, string _password, string landingScene)
    {
        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }

            LoginWarning?.Invoke(message);
        }
        else
        {
            //User is now logged in
            //Now get the result
            //Verify result
            if (LoginTask.Result.User.IsEmailVerified)
            {
                // Email is verified, proceed with your logic
                userPathData = "user/" + _email;

                user = LoginTask.Result.User;
                Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);

                //raise event
                LoginConfirm?.Invoke("Logged In");

                SaveSession(_email, _password);

                UserBackendManager.Instance.GetUserByEmailTask(_email).ContinueWithOnMainThread(task =>
                {
                    DocumentSnapshot currUserDoc = task.Result;
                    this.currUser = currUserDoc.ConvertTo<UserData>();
                    AppManager.Instance.LoadScene(landingScene);
                });
            }
            else
            {
                // Email is not verified, show a message to the user          
                LoginWarning?.Invoke("Email is not verified. Please verify your email.");
            }
        }
    }

    public void SaveSession(string _email, string _password)
    {
        PlayerPrefs.SetString("email", _email);
        PlayerPrefs.SetString("password", _password);

        // Save the data to disk
        PlayerPrefs.Save();

        Debug.Log("Session saved! Email: " + _email);
    }

    public IEnumerator LoadSession(float delay, GameObject LoadingUI)
    {
        yield return new WaitForSecondsRealtime(delay);

        string _email = PlayerPrefs.GetString("email", null);
        string _password = PlayerPrefs.GetString("password", null);

        Debug.Log("Email: " + _email);

        if (_email != null && _password != null && _email != "" && _password != "")
        {
            if (LoadingUI != null)
            {
                LoadingUI.SetActive(true);
            }
            
            StartLogin(_email, _password, "4-ChatList");
            Debug.Log("Auto logging in...");
        }
        else
        {
            Debug.Log("No session found.");
        }
    }

    //Register Function
    private IEnumerator Register(string _email, string _password)
    {

        // Input validation
        if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password))
        {
            RegisterWarning?.Invoke("Please enter both email and password.");

            yield break; // Exit the method if input is invalid
        }

        if (!IsValidEmail(_email))
        {

            RegisterWarning?.Invoke("Invalid email format.");
            yield break; // Exit the method if email format is invalid
        }

        //Call the Firebase auth signin function passing the email and password
        Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

        if (RegisterTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Register Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
            }
            //raise event
            RegisterWarning?.Invoke(message);
        }
        else
        {
            //User has now been created
            //Now get the result
            user = RegisterTask.Result.User;

            //ps: I don't think we need to store password in database - Tianchu
            //passwordData = _password;
            userPathData = "user/" + _email;

            bool recordSaved = UserBackendManager.Instance.AddUser(_email);

            if (user != null && recordSaved)
            {
                // Send email verification
                Task emailVerificationTask = user.SendEmailVerificationAsync();
                RegisterConfirm?.Invoke("Registration successful. Please check your email to verify your account.");


                // Wait until the email verification task completes
                yield return new WaitUntil(() => emailVerificationTask.IsCompleted);

                if (emailVerificationTask.Exception != null)
                {
                    // Handle email verification error
                    Debug.LogWarning($"Failed to send email verification: {emailVerificationTask.Exception}");
                    RegisterWarning?.Invoke("Email verification failed!");

                }
                else
                {
                    // Email verification sent successfully
                    // You can provide a message to the user here or prompt them to check their email.

                    EmailVerificationSent?.Invoke();
                    ClearWarning?.Invoke();

                }
            }
        }
    }
    // Helper method to validate email format
    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    // Reset Password Function
    private IEnumerator ResetPassword(string _email)
    {
        // Input validation (you can customize this as needed)
        if (string.IsNullOrEmpty(_email) || !IsValidEmail(_email))
        {
            LoginWarning?.Invoke("Invalid email format.");
            yield break; // Exit the method if the email format is invalid
        }

        // Call the Firebase auth password reset function passing the email
        Task resetTask = auth.SendPasswordResetEmailAsync(_email);

        // Wait until the task completes
        yield return new WaitUntil(() => resetTask.IsCompleted);

        if (resetTask.Exception != null)
        {
            // Handle password reset errors
            Debug.LogWarning($"Failed to reset password with error: {resetTask.Exception.Message}");
            LoginWarning?.Invoke("Password reset failed. Check your email address.");
        }
        else
        {
            // Password reset email sent successfully
            // Display a confirmation message to the user
            LoginWarning?.Invoke("Password reset email sent. Check your inbox.");
        }
    }

    public void SignOut()
    {
        // Remove session
        PlayerPrefs.DeleteAll();

        auth.SignOut();
        Debug.Log("User signed out successfully");
    }
}
