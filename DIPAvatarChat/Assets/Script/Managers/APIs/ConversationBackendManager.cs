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

    public void GetConversationByID(string conversationID) {
        db = FirebaseFirestore.DefaultInstance;
        
        DocumentReference conversationDoc = db.Collection("conversation").Document(conversationID);
        conversationDoc.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            ConversationData conversationData = ProcessConversationDocument(task.Result);
            ConversationDataRetrieved?.Invoke(conversationData);
        });
    }

    private ConversationData ProcessConversationDocument(DocumentSnapshot documentSnapShot) {
        
        Dictionary<string, object> temp = documentSnapShot.ToDictionary();
        return DictionaryToConversationData(temp,documentSnapShot.GetValue<List<string>>("members"), documentSnapShot.GetValue<List<string>>("messages"));
    }

    //Other backend APIs to be filled in by Backend people
    public bool AddConversation(List<String> members, string description)
    {
        //TODO:Implement ADD a conversation to database containing a description and initial members
        return true;
    }

    public bool UpdateConversationDesc(string conversationID, string description)
    {
        //TODO:Implement UPDATE a conversation description in database
        return true;
    }

    public bool AddConversationMember(string conversationID, string memberEmail)
    {
        //TODO:Implement ADD a conversation member to database
        return true;
    }

    public bool DeleteConversationMember(string conversationID, string memberEmail)
    {
        //TODO:Implement DELETE a conversation member from database
        return true;
    }

    public bool DeleteConversation(string conversationID)
    {
        //TODO :Implement DELETE a conversation from database
        return true;
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
}
