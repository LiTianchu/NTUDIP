using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

//This class contains API for Create/Update/Delete/Read(CRUD) database data
//This class should be called from other classes(page scripts) to perform CRUD operations
//Do not call this class directly from UI elements
public class UserBackendManager : Singleton<UserBackendManager>
{
    FirebaseFirestore db;

    public event Action<UserData> UserDataReceived;

    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

    }

    //API for creating user record
    //Returns true if update is successful
    //Takes in email as parameter
    public bool AddUser(string email)
    {

        var userData = new UserData
        {
            email = email,
        };

        try
        {
            db.Document(AuthManager.Instance.userPathData).SetAsync(userData);
        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);
            return false;
        }
        return true;
    }

    //API for updating username and status
    //Returns true if update is successful
    //Takes in username and status as parameters
    public bool UpdateUsernameAndStatus(string username, string status)
    {
        Dictionary<string, object> userData = new Dictionary<string, object>
            {
                { "username", username },
                { "status", status },
                { "createdAt", FieldValue.ServerTimestamp }
            };

        try
        {
            db.Document(AuthManager.Instance.userPathData).UpdateAsync(userData);

        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);
            return false;
        }
        return true;

    }
    
    public void GetUsernameByEmail(string email)
    {
        UserData userData;
        Query usernameQuery = db.Collection("user").WhereEqualTo("email", email);

        //this function is Async, so the return value does not work here.
        //one way is to use the C# event system to add a event listener that will be called once the message getting operation finished
        usernameQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapShot in snapshot.Documents)
            {
                Debug.Log(String.Format("Document data for {0} document:", documentSnapShot.Id));
                Dictionary<string, object> temp = documentSnapShot.ToDictionary();

                /*foreach (KeyValuePair<string, object> pair in temp)
                {
                    Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                    
                }*/

                userData = DictionaryToUserData(temp);
                Debug.Log(userData.username);

                // This line is not working, the event is not invoked when I run
                UserDataReceived?.Invoke(userData);

                // Newline to separate entries
                Debug.Log("");

            }
        });
    }

    public UserData DictionaryToUserData(Dictionary<string, object> firestorData)
    {
        UserData userData = new UserData();

        firestorData.TryGetValue("username", out object username);
        userData.username = (string)username;

        firestorData.TryGetValue("email", out object email);
        userData.email = (string)email;

        firestorData.TryGetValue("status", out object status);
        userData.status = (string)status;

        return userData;

    }

}