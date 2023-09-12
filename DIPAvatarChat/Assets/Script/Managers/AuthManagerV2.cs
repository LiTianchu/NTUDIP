using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AuthManagerV2 : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

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

    //For Firestore Database
    FirebaseFirestore db;

    public static string emailData;
    public static string passwordData;
    public static string userPathData;

    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();

            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password)
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
            warningLoginText.text = message;
            confirmLoginText.text = "";
        }
        else
        {
            //User is now logged in
            //Now get the result
            userPathData = "user/" + _email;

            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            AppManager.Instance.LoadScene("3-RegisterUsername");

        }
    }

    private IEnumerator Register(string _email, string _password)
    {

        // Input validation
        if (string.IsNullOrEmpty(_email) || string.IsNullOrEmpty(_password))
        {
            warningRegisterText.text = "Please enter both email and password.";
            yield break; // Exit the method if input is invalid
        }

        if (!IsValidEmail(_email))
        {
            warningRegisterText.text = "Invalid email format.";
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
            warningRegisterText.text = message;
        }
        else
        {
            //User has now been created
            //Now get the result
            User = RegisterTask.Result.User;
            emailData = _email;
            passwordData = _password;
            userPathData = "user/" + _email;

            var userData = new UserData
            {
                email = emailData,
                password = passwordData,
            };

            db = FirebaseFirestore.DefaultInstance;
            db.Document(userPathData).SetAsync(userData);

            if (User != null)
            {
                // Send email verification
                Task emailVerificationTask = User.SendEmailVerificationAsync();

                // Wait until the email verification task completes
                yield return new WaitUntil(() => emailVerificationTask.IsCompleted);

                if (emailVerificationTask.Exception != null)
                {
                    // Handle email verification error
                    Debug.LogWarning($"Failed to send email verification: {emailVerificationTask.Exception}");
                    warningRegisterText.text = "Email verification failed!";
                }
                else
                {
                    // Email verification sent successfully
                    // You can provide a message to the user here or prompt them to check their email.
                    warningRegisterText.text = "Registration successful. Please check your email to verify your account.";
                }
            
            /*//Create a user profile and set the username
            UserProfile profile = new UserProfile { DisplayName = _username };

            //Call the Firebase auth update user profile function passing the profile with the username
            Task ProfileTask = User.UpdateUserProfileAsync(profile);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                warningRegisterText.text = "Username Set Failed!";
            }
            else
            {
                //Username is now set
                //Now return to login screen
                UIManager.instance.LoginScreen();
                warningRegisterText.text = "";
            }*/
            UIManager.Instance.LoginScreen();
                warningRegisterText.text = "";
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
}