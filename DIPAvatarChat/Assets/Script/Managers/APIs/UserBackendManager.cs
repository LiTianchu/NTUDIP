using System.Collections.Generic;
using UnityEngine;
using Firebase.Firestore;
using System;
using System.Threading.Tasks;

//This class contains API for Create/Update/Delete/Read(CRUD) database data
//This class should be called from other classes(page scripts) to perform CRUD operations
//Do not call this class directly from UI elements
public class UserBackendManager : Singleton<UserBackendManager>
{
    FirebaseFirestore db;

    //cache

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
        db = FirebaseFirestore.DefaultInstance;

        //Initialise null List<string> by default so that it wont cause an error
        var nullList = new List<string>();
        nullList.Add(null);

        var userData = new UserData
        {
            email = email,
            friendRequests = nullList,
            friends = nullList,
            conversations = nullList,
        };

        try
        {
            Debug.Log("Document Name: " + userData.email);
            Debug.Log("Document Link: " + AuthManager.Instance.userPathData);

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

    public async Task<DocumentSnapshot> GetUserByEmailTask(string email)
    {
        db = FirebaseFirestore.DefaultInstance;

        DocumentSnapshot doc = null;
        try
        {
            DocumentReference usernameDoc = db.Collection("user").Document(email);
            doc = await usernameDoc.GetSnapshotAsync();

        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);

        }
        return doc;
    }

    public void SendFriendRequestToThem(string myEmail, string theirEmail, List<string> theirFriendRequestsList)
    {
        theirFriendRequestsList.Add(myEmail);

        Dictionary<string, object> theirFriendRequestsDict = new Dictionary<string, object>
        {
            { "friendRequests", theirFriendRequestsList }
        };
        db.Document("user/" + theirEmail).UpdateAsync(theirFriendRequestsDict);

        Debug.Log("Friend Request Sent to " + theirEmail + "!");
    }

    public void AcceptFriendRequest(string myEmail, string theirEmail, List<string> myFriendRequestsList, List<string> theirFriendRequestsList, List<string> myFriendsList, List<string> theirFriendsList)
    {
        myFriendsList.Add(theirEmail);
        myFriendRequestsList.Remove(theirEmail);

        Dictionary<string, object> myUserData = new Dictionary<string, object>
        {
            { "friends", myFriendsList },
            { "friendRequests", myFriendRequestsList }
        };

        theirFriendsList.Add(myEmail);

        Dictionary<string, object> theirUserData = new Dictionary<string, object>
        {
            { "friends", theirFriendsList }
        };

        Debug.Log("Friend Request from " + theirEmail + " accepted! :)");

        db.Document("user/" + myEmail).UpdateAsync(myUserData);
        db.Document("user/" + theirEmail).UpdateAsync(theirUserData);
    }

    public void RejectFriendRequest(string myEmail, string theirEmail, List<string> myFriendRequestsList)
    {
        myFriendRequestsList.Remove(theirEmail);

        Dictionary<string, object> myUserData = new Dictionary<string, object>
        {
            { "friendRequests", myFriendRequestsList }
        };

        Debug.Log("Friend Request from " + theirEmail + " rejected! :<");

        db.Document("user/" + myEmail).UpdateAsync(myUserData);
    }

}