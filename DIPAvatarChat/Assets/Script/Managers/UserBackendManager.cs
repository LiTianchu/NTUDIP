using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;

public class UserBackendManager : MonoBehaviour
{
    FirebaseFirestore db;

    public string _userPath;

    [Header("UserInfo")]
    public TMP_InputField usernameField;
    public TMP_InputField statusField;
    public Button submitButton;

    // Start is called before the first frame update
    void Start()
    {
        submitButton.onClick.AddListener(() =>
        {
            _userPath = AuthManagerV2.userPathData;

            /*var userData = new UserData
            {
                username = usernameField.text,
                status = statusField.text,
            };*/

            Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "username", usernameField.text },
                { "status", statusField.text },
                { "createdAt", FieldValue.ServerTimestamp }
            };

            db = FirebaseFirestore.DefaultInstance;
            db.Document(_userPath).UpdateAsync(userData);
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}