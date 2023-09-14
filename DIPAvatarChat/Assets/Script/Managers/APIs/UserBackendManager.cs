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

        var userData = new UserData
        {
            email = email,
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

                Debug.Log("Dictionary username: " + temp["username"]);
                Debug.Log("Dictionary friendRequests: " + temp["friendRequests"]);
                
                userData = DictionaryToUserData(temp);
                Debug.Log(userData.username);

                UserDataReceived?.Invoke(userData);

                // Newline to separate entries
                Debug.Log("");

            }
        });
    }

    public bool SendFriendRequest(string receiverEmail, string senderEmail, string description = "Hi, I would like to be your friend!")
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

        var list = new List<string>();
        list.Add(senderEmail);

        Dictionary<string, object> userFriendRequests = new Dictionary<string, object>
            {
                { "friendRequests", list }
            };

        try
        {
            if (receiverEmail != senderEmail && receiverEmail != null)
            {
                db.Document(UniqueID).SetAsync(friendRequestData);
                Debug.Log("Friend Request Sent to " + receiverEmail + "!");

                db.Document("user/" + receiverEmail).UpdateAsync(userFriendRequests);
            }
            else
            {
                Debug.Log("You cannot send a friend request to yourself!");
            }

        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);
            return false;
        }
        return true;
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

        /*firestorData.TryGetValue("friendRequests", out object friendRequests);
        userData.friendRequests = (List<string>)friendRequests.ToList();

        firestorData.TryGetValue("friends", out object friends);
        userData.friends = (List<string>)friends.ToList();*/

        return userData;

    }

    //Random ID generator
    public static string GenerateRandomID(int length)
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
    }

}