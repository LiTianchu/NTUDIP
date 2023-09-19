using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

//This class contains API for Create/Update/Delete/Read(CRUD) database data
//This class should be called from other classes(page scripts) to perform CRUD operations
//Do not call this class directly from UI elements
public class MessageBackendManager : Singleton<MessageBackendManager>
{

    FirebaseFirestore db;
    private string _userPath;

    //declare the event
    public event Action<List<MessageData>> MessageListRetrieved;
    public event Action<MessageData> MessageRetrieved;

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
                MessageListRetrieved?.Invoke(messages);
            }
            
        });
       
        
    }

    public void GetMessageByID(string messageID) { 
        DocumentReference messageDoc = db.Collection("message").Document(messageID);
        messageDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot messageSnapshot = task.Result;
            Dictionary<string, object> temp = messageSnapshot.ToDictionary();
            MessageData messageData = DictionaryToMessageData(temp);
            MessageRetrieved?.Invoke(messageData);
        });
    
    }

    public async Task<DocumentSnapshot> GetMessageByIDTask(string messageID) {
        DocumentReference messageDoc = db.Collection("message").Document(messageID);
        return await messageDoc.GetSnapshotAsync();
    }

    public bool AddMessage(string message, string conversationID, string receiverEmail, string senderEmail)
    {
        //TODO:Implement ADD a message record to database
        try
        {
            // Create a new message document in the "message" collection
            db = FirebaseFirestore.DefaultInstance;
            _userPath = AuthManager.Instance.userPathData;

            if (db == null)
            {
                Debug.LogError("Firestore instance (db) is null");
            }
            DocumentReference newMessageRef = db.Collection("message").Document();

            // Create a data object with the message details
            Dictionary<string, object> messageData = new Dictionary<string, object>
        {
            { "message", message },
            { "createdAt", FieldValue.ServerTimestamp }, 
            { "conversationID", conversationID },
            { "receiver", receiverEmail },
            { "sender", senderEmail }
        };

            // Set the data for the new message document
            newMessageRef.SetAsync(messageData);

            // The ID of the newly created message document
            string newMessageID = newMessageRef.Id;

            // You can perform additional actions here if needed, such as updating other parts of your app's data.

            return true; // Return true to indicate that the message was successfully added.
        }
        catch (Exception e)
        {
            Debug.LogError("Error adding message: " + e.Message);
            return false; // Return false to indicate that an error occurred while adding the message.
        }
    }

    public MessageData ProcessMessageDocument(DocumentSnapshot documentSnapShot)
    {

        Dictionary<string, object> temp = documentSnapShot.ToDictionary();
        return DictionaryToMessageData(temp);
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
