using System.Collections;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class DatabaseManager : MonoBehaviour
{

    [Header("User")]
    public TMP_InputField usernameEnterField;
    public TMP_InputField statusEnterField;

    // Start is called before the first frame update
    void Start()
    {
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class User
    {
        public string username;
        public string email;
        public string status;

        public User()
        {
        }

        public User(string username, string email, string status)
        {
            this.username = username;
            this.email = email;
            this.status = status;
        }

        private void writeNewUser(string userId, string name, string email, string status)
        {
            User user = new User(name, email, status);
            string json = JsonUtility.ToJson(user);

            //_database.Child("users").Child(userId).SetRawJsonValueAsync(json);
        }
    }
}
