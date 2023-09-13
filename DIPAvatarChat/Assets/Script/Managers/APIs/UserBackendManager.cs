using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System;

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
    // API for retrieving username by email
    // Returns the username if found, or null if not found
    // Takes in an email as a parameter
    /*public async Task<string> GetUsernameByEmailAsync(string email)
    {
        try
        {
            // Query the database for the user document with the given email
            QuerySnapshot querySnapshot = await db.Collection("user")
                .WhereEqualTo("email", email)
                .Limit(1)
                .GetSnapshotAsync();

            // Check if a document was found
            if (querySnapshot.Documents.Count > 0)
            {
                // Extract the username from the document
                string username = querySnapshot.Documents[0].GetString("username");
                return username;
            }
            else
            {
                // No document with the specified email was found
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);
            return null;
        }

    }*/






}