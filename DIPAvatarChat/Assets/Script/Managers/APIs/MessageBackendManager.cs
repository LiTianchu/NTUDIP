using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//This class contains API for Create/Update/Delete/Read(CRUD) database data
//This class should be called from other classes(page scripts) to perform CRUD operations
//Do not call this class directly from UI elements
public class MessageBackendManager : Singleton<MessageBackendManager>
{

    FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;

    }

    public async Task<QuerySnapshot> GetAllMessagesTask(string conversationID)
    {
        db = FirebaseFirestore.DefaultInstance;
        QuerySnapshot messagesDoc = null;
        try
        {
            Query messageQuery = db.Collection("message").WhereEqualTo("conversationID", conversationID).OrderBy("createdAt");
            messagesDoc = await messageQuery.GetSnapshotAsync();
        }
        catch (Exception e)
        {
            Debug.LogError("Error getting messages: " + e.Message);
        }
        if (messagesDoc == null)
        {
            Debug.LogError("Error getting messages: messagesDoc is null");
        }
        return messagesDoc;

    }

    public async Task<DocumentSnapshot> GetMessageByIDTask(string messageID)
    {
        DocumentReference messageDoc = db.Collection("message").Document(messageID);
        return await messageDoc.GetSnapshotAsync();
    }

    public async Task<bool> SendMessageTask(ConversationData currConvData, string message, string myEmail, string theirEmail)
    {
        try
        {
            db = FirebaseFirestore.DefaultInstance;

            List<string> messagesList = new List<string>(currConvData.messages);
            Debug.Log(messagesList.Count + " in SendMessageTask.");

            // Create a data object with the message details
            Dictionary<string, object> messageDict = new Dictionary<string, object>
            {
                { "message", message },
                { "createdAt", FieldValue.ServerTimestamp },
                { "conversationID", currConvData.conversationID },
                { "receiver", theirEmail },
                { "sender", myEmail }
            };

            DocumentReference messageDataRef = await db.Collection("message").AddAsync(messageDict);
            string currMessageId = messageDataRef.Id;
            messagesList.Add(currMessageId);
            Debug.Log(messagesList.Count + " in SendMessageTask After.");

            Dictionary<string, object> conversationDict = new Dictionary<string, object>
            {
                { "messages", messagesList },
                { "latestMessageCreatedAt", DateTime.Now }
            };

            db.Collection("conversation").Document(currConvData.conversationID).UpdateAsync(conversationDict);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error adding message: " + e.Message);
            return false; // Return false to indicate that an error occurred while adding the message.
        }
    }

    public void DeleteMessage(string msgID)
    {
        try
        {
            db = FirebaseFirestore.DefaultInstance;
            // Get a reference to the conversation document using the provided conversation ID
            DocumentReference msgRef = db.Collection("message").Document(msgID);

            // Delete the conversation document
            msgRef.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Message " + msgID + " deleted successfully.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error deleting msg: " + task.Exception.ToString());
                }
            });

        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting msg: " + e.Message);
        }
    }
}
