using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

//This class contains API for Create/Update/Delete/Read(CRUD) database data
//This class should be called from other classes(page scripts) to perform CRUD operations
//Do not call this class directly from UI elements
public class MessageBackendManager : Singleton<MessageBackendManager>
{

    FirebaseFirestore db;
    private string _userPath;

    //declare the event
    public event Action<List<MessageData>> MessageRetrieved;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;

    }

    
    public void GetAllMessages(string conversationID)
    {
        List<MessageData> messages = new List<MessageData>();
        Query messageQuery = db.Collection("message").WhereEqualTo("conversationID", conversationID);

        //this function is Async, so the return value does not work here.
        //one way is to use the C# event system to add a event listener that will be called once the message getting operation finished
        messageQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach(DocumentSnapshot documentSnapShot in snapshot.Documents)
            {
                Debug.Log(String.Format("Document data for {0} document:", documentSnapShot.Id));
                Dictionary<string, object> temp = documentSnapShot.ToDictionary();
               
                MessageData messageData = DictionaryToMessageData(temp);
                messages.Add(messageData);

                //invoke the event
                MessageRetrieved?.Invoke(messages);
            }
            
        });
       
        
    }

    public bool AddMessage(string message, string conversationID, string receiverEmail, string senderEmail)
    {
        //TODO:Implement ADD a message record to database
        return true;
    }

    public MessageData DictionaryToMessageData(Dictionary<string, object> firestorData) {
        MessageData messageData = new MessageData();

        firestorData.TryGetValue("message", out object message);
        messageData.message = (string)message;

        firestorData.TryGetValue("createdAt", out object createdAt);
        messageData.createdAt = (Timestamp)createdAt;

        firestorData.TryGetValue("conversationID", out object conversationID);
        messageData.conversationID = (string)conversationID;

        firestorData.TryGetValue("receiver", out object receiver);
        messageData.receiver = (string)receiver;

        firestorData.TryGetValue("sender", out object sender);
        messageData.sender = (string)sender;


        return messageData;
    
    }

}
