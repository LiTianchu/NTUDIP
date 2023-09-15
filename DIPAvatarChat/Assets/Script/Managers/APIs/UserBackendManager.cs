using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;
using Random = System.Random;

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
        db = FirebaseFirestore.DefaultInstance;

        //Initialise null List<string> by default so that it wont cause an error
        var nullList = new List<string>();
        nullList.Add(null);

        var userData = new UserData
        {
            email = email,
            friendRequests = nullList,
            friends = nullList,
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

    public void SearchUserByEmail(string email)
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

                var friendRequestsList = new List<string>();
                var friendsList = new List<string>();

                friendRequestsList = documentSnapShot.GetValue<List<string>>("friendRequests");
                friendsList = documentSnapShot.GetValue<List<string>>("friends");

                Dictionary<string, object> temp = documentSnapShot.ToDictionary();

                userData = DictionaryToUserData(temp, friendRequestsList, friendsList);

                UserDataReceived?.Invoke(userData);

                // Newline to separate entries
                Debug.Log("");
            }
        });
    }

    public bool SendFriendRequest(List<string> friendRequests, string receiverEmail, string senderEmail, string description = "Hi, I would like to be your friend!")
    {
        /*Dictionary<string, object> friendRequestData = new Dictionary<string, object>
            {
                { "createdAt", FieldValue.ServerTimestamp },
                { "description", description },
                { "receiverID", receiverEmail },
                { "senderID", senderEmail }
            };*/

        string UniqueID = "friendRequest/" + senderEmail + "->" + receiverEmail;

        var friendRequestData = new FriendRequestData
        {
            //createdAt = FieldValue.ServerTimestamp,
            description = description,
            receiverID = receiverEmail,
            senderID = senderEmail,
        };

        List<string> friendRequestsList = new List<string>(friendRequests);

        //checks if friend request already sent by the user
        bool duplicateFriendRequest = false;

        foreach (string friendRequest in friendRequests)
        {
            if (senderEmail == friendRequest)
            {
                duplicateFriendRequest = true;
                Debug.Log("You already sent this user a friend request...");
            }
        }

        try
        {
            if (receiverEmail != senderEmail && receiverEmail != null && !duplicateFriendRequest)
            {
                friendRequestsList.Add(senderEmail);

                Dictionary<string, object> userFriendRequests = new Dictionary<string, object>
                {
                    { "friendRequests", friendRequestsList }
                };

                db.Document(UniqueID).SetAsync(friendRequestData);
                Debug.Log("Friend Request Sent to " + receiverEmail + "!");

                db.Document("user/" + receiverEmail).UpdateAsync(userFriendRequests);
            }
            else
            {
                Debug.Log("Friend Request cannot be sent...");
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);
            return false;
        }
        return true;
    }

    public bool DisplayFriendRequests()
    {
        return true;
    }

    public bool AcceptFriendRequest()
    {
        return true;
    }

    public bool RejectFriendRequest()
    {
        return true;
    }

    public UserData DictionaryToUserData(Dictionary<string, object> firestorData, List<string> friendRequests, List<string> friends)
    {
        UserData userData = new UserData();

        firestorData.TryGetValue("username", out object username);
        userData.username = (string)username;

        firestorData.TryGetValue("email", out object email);
        userData.email = (string)email;

        firestorData.TryGetValue("status", out object status);
        userData.status = (string)status;

        //firestorData.TryGetValue("friendRequests", out object friendRequests);
        userData.friendRequests = (List<string>)friendRequests;

        //firestorData.TryGetValue("friends", out object friends);
        userData.friends = (List<string>)friends;

        return userData;

    }

    //Random ID generator
    /*public static string GenerateRandomID(int length)
    {
        Random random = new Random();

        var stringChars = new char[length];
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);

        return finalString;
    }*/

}