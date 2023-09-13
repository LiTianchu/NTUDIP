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
    public bool UpdateUsernameAndStatus(string username, string status) {
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
        catch (Exception ex) { 
            Debug.LogError("Firestore Error: " + ex.Message);
            return false;
        }
        return true;

    }
   
    public async Task<string> GetUsernameByEmail(string email)
    {
        try
        {
            // Query the database for the user document with the given email
            var querySnapshot = await db.Collection("user")
                .WhereEqualTo("user", email)
                .Limit(1)
                .GetSnapshotAsync();

            // Check if a document was found
            foreach (var document in querySnapshot.Documents)
            {
                // Extract the username from the document using Get method
                if (document.TryGetValue("username", out object usernameObj) && usernameObj is string username)
                {
                    return username;
                }
            }

            // No document with the specified email was found
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);
            return null;
        }
    }








}