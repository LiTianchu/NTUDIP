using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationBackendManager : Singleton<ConversationBackendManager>
{
    FirebaseFirestore db;
    private string _userPath;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;

    }

    //register event for conversation data retrieved
    public event Action<ConversationData> ConversationDataRetrieved;

    public void GetConversationByID(string conversationID)
    {
        db = FirebaseFirestore.DefaultInstance;

        DocumentReference conversationDoc = db.Collection("conversation").Document(conversationID);
        conversationDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            ConversationData conversationData = ProcessConversationDocument(task.Result);
            ConversationDataRetrieved?.Invoke(conversationData);
        });
    }

    private ConversationData ProcessConversationDocument(DocumentSnapshot documentSnapShot)
    {

        Dictionary<string, object> temp = documentSnapShot.ToDictionary();
        return DictionaryToConversationData(temp, documentSnapShot.GetValue<List<string>>("members"), documentSnapShot.GetValue<List<string>>("messages"));
    }

    //Other backend APIs to be filled in by Backend people
    public bool AddConversation(List<String> members, string description)
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;
        if (db == null)
        {
            Debug.LogError("Firebase Firestore is not initialized. Make sure it's properly configured.");
            return false;
        }

        // Generate a new conversation document with an auto-generated ID
        DocumentReference conversationDoc = db.Collection("conversation").Document();
        object serverTimestamp = FieldValue.ServerTimestamp;
        // Define the data to be added to the document
        Dictionary<string, object> conversationData = new Dictionary<string, object>
    {
        { "description", description },
        { "members", members },
        { "timestamp", serverTimestamp } 
        // You can add more fields here if needed
    };

        // Set the data in the Firestore document
        conversationDoc.SetAsync(conversationData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Conversation added successfully.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error adding conversation: " + task.Exception);
                }
            });

        return true;
    }

    public bool UpdateConversationDesc(string conversationID, string description)
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;

        if (db == null)
        {
            Debug.LogError("Firebase Firestore is not initialized. Make sure it's properly configured.");
            return false;
        }

        try
        {
            // Get a reference to the conversation document using the provided conversation ID
            DocumentReference conversationRef = db.Collection("conversation").Document(conversationID);

            // Define the data to update (only the description field in this case)
            Dictionary<string, object> updateData = new Dictionary<string, object>
        {
            { "description", description }
        };

            // Update the conversation document with the new description
            conversationRef.UpdateAsync(updateData)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Conversation description updated successfully.");
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogError("Error updating conversation description: " + task.Exception);
                    }
                });

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error updating conversation description: " + e.Message);
            return false;
        }
    }

    public bool AddConversationMember(string conversationID, string memberEmail)
    {
        
            db = FirebaseFirestore.DefaultInstance;
            _userPath = AuthManager.Instance.userPathData;

            if (db == null)
            {
                Debug.LogError("Firebase Firestore is not initialized. Make sure it's properly configured.");
                return false;
            }

            try
            {
                // Get a reference to the conversation document using the provided conversation ID
                DocumentReference conversationRef = db.Collection("conversation").Document(conversationID);

                // Use FieldValue.ArrayUnion to add a member to the "members" array field
                Dictionary<string, object> updateData = new Dictionary<string, object>
        {
            { "members", FieldValue.ArrayUnion(memberEmail) }
        };

                // Update the conversation document to add the member
                conversationRef.UpdateAsync(updateData)
                    .ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCompleted)
                        {
                            Debug.Log("Member added to the conversation successfully.");
                        }
                        else if (task.IsFaulted)
                        {
                            Debug.LogError("Error adding member to the conversation: " + task.Exception);
                        }
                    });

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Error adding member to the conversation: " + e.Message);
                return false;
            }
     }
    

    public bool DeleteConversationMember(string conversationID, string memberEmail)
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;

        if (db == null)
        {
            Debug.LogError("Firebase Firestore is not initialized. Make sure it's properly configured.");
            return false;
        }

        try
        {
            // Get a reference to the conversation document using the provided conversation ID
            DocumentReference conversationRef = db.Collection("conversation").Document(conversationID);

            // Use FieldValue.ArrayRemove to remove the member from the "members" array field
            Dictionary<string, object> updateData = new Dictionary<string, object>
        {
            { "members", FieldValue.ArrayRemove(memberEmail) }
        };

            // Update the conversation document to remove the member
            conversationRef.UpdateAsync(updateData)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Member removed from the conversation successfully.");
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogError("Error removing member from the conversation: " + task.Exception);
                    }
                });

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error removing member from the conversation: " + e.Message);
            return false;
        }
    }


    public bool DeleteConversation(string conversationID)
    {
        try
        {
            db = FirebaseFirestore.DefaultInstance;
            _userPath = AuthManager.Instance.userPathData;
            // Get a reference to the conversation document using the provided conversation ID
            DocumentReference conversationRef = db.Collection("conversation").Document(conversationID);

            // Delete the conversation document
            conversationRef.DeleteAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("Conversation deleted successfully.");
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error deleting conversation: " + task.Exception.ToString());
                }
            });

            return true; // Return true to indicate that the deletion process has started.
        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting conversation: " + e.Message);
            return false; // Return false to indicate that an error occurred during deletion.
        }
    }


    public ConversationData DictionaryToConversationData(Dictionary<string, object> firestoreData, List<string> members, List<string> messages)
    {
        ConversationData conversationData = new ConversationData();
        firestoreData.TryGetValue("conversationID", out object conversationID);
        firestoreData.TryGetValue("description", out object description);
        conversationData.conversationID = (string)conversationID;
        conversationData.description = (string)description;
        conversationData.members = members;
        conversationData.messages = messages;
        return conversationData;

    }

    //For future use, to get the conversation ID based on description for the purpose of easy deletion of the conversation
    /*public void GetConversationIDByDescription(string description)
    {
        db = FirebaseFirestore.DefaultInstance;

        if (db == null)
        {
            Debug.LogError("Firebase Firestore is not initialized. Make sure it's properly configured.");
            return;
        }

        // Query the "conversation" collection to find the document with a specific description
        db.Collection("conversation")
            .WhereEqualTo("description", description)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    QuerySnapshot snapshot = task.Result;
                    if (snapshot.Documents.Count > 0)
                    {
                    // If there are matching documents, you can access their IDs
                    foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
                        {
                            string conversationID = documentSnapshot.Id;
                            Debug.Log("Conversation ID: " + conversationID);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("No matching conversation found.");
                    }
                }
                else if (task.IsFaulted)
                {
                    Debug.LogError("Error querying conversation: " + task.Exception);
                }
            });
    }*/
}

