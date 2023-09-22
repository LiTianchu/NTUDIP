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

    //cache
    public UserData currentUser { get; set; }

    //events
    public event Action<UserData> SearchUserFriendRequestsReceived;
    public event Action<UserData> CurrentUserRetrieved;
    public event Action<UserData> SearchUserContactsReceived;
    public event Action<UserData> OtherUserDataReceived;

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

    /*public void GetCurrentUser()
    {
        db = FirebaseFirestore.DefaultInstance;

        UserData userData;
        DocumentReference userDoc = db.Collection("user").Document(AuthManager.Instance.emailData);

        //this function is Async, so the return value does not work here.
        //one way is to use the C# event system to add a event listener that will be called once the message getting operation finished
        userDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            userData = ProcessUserDocument(task.Result);
            this.currentUser = userData;
            CurrentUserRetrieved?.Invoke(userData);

        });

    }*/

    public async Task<DocumentSnapshot> GetUserByEmailTask(string email)
    {
        DocumentSnapshot doc = null;
        try
        {
            DocumentReference usernameDoc = db.Collection("user").Document(email);
            doc = await usernameDoc.GetSnapshotAsync();
            currentUser = ProcessUserDocument(doc);
        }catch(Exception ex) {             
            Debug.LogError("Firestore Error: " + ex.Message);
               
        }
        return doc;
    }


    public void GetOtherUser(string email)
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

                userData = ProcessUserDocument(documentSnapShot);
                OtherUserDataReceived?.Invoke(userData);
            }
        });
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

    public void SearchFriendRequests(string myEmail)
    {
        UserData userData;
        Query usernameQuery = db.Collection("user").WhereEqualTo("email", myEmail);

        //this function is Async, so the return value does not work here.
        //one way is to use the C# event system to add a event listener that will be called once the message getting operation finished
        usernameQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapShot in snapshot.Documents)
            {
                userData = ProcessUserDocument(documentSnapShot);

                SearchUserFriendRequestsReceived?.Invoke(userData);

                // Newline to separate entries
                Debug.Log("");
            }
        });
    }

    public void SearchContacts(string myEmail)
    {
        UserData userData;
        Query usernameQuery = db.Collection("user").WhereEqualTo("email", myEmail);

        //this function is Async, so the return value does not work here.
        //one way is to use the C# event system to add a event listener that will be called once the message getting operation finished
        usernameQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot documentSnapShot in snapshot.Documents)
            {
                userData = ProcessUserDocument(documentSnapShot);

                SearchUserContactsReceived?.Invoke(userData);

                // Newline to separate entries
                Debug.Log("");
            }
        });
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

    public UserData ProcessUserDocument(DocumentSnapshot documentSnapShot)
    {
        Debug.Log(String.Format("Document data for {0} document:", documentSnapShot.Id));

        List<string> friendRequestsList = new List<string>();
        List<string> friendsList = new List<string>();
        List<string> conversationList = new List<string>();

        documentSnapShot.TryGetValue("friendRequests", out friendRequestsList);
        documentSnapShot.TryGetValue("friends", out friendsList);
        documentSnapShot.TryGetValue("conversations", out conversationList);

        Dictionary<string, object> temp = documentSnapShot.ToDictionary();
        if (temp == null)
        {
            return null;
        }
        return DictionaryToUserData(temp, friendRequestsList, friendsList, conversationList);

    }

    public UserData DictionaryToUserData(Dictionary<string, object> firestoreData, List<string> friendRequests, List<string> friends, List<string> conversations)
    {
        UserData userData = new UserData();

        firestoreData.TryGetValue("username", out object username);
        userData.username = (string)username;

        firestoreData.TryGetValue("email", out object email);
        userData.email = (string)email;

        firestoreData.TryGetValue("status", out object status);
        userData.status = (string)status;

        //firestorData.TryGetValue("friendRequests", out object friendRequests);
        userData.friendRequests = (List<string>)friendRequests;

        //firestorData.TryGetValue("friends", out object friends);
        userData.friends = (List<string>)friends;

        userData.conversations = (List<string>)conversations;

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