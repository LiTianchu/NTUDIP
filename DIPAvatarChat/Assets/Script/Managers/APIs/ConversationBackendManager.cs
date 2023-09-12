using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//This class contains API for Create/Update/Delete/Read(CRUD) database data
//This class should be called from other classes(page scripts) to perform CRUD operations
//Do not call this class directly from UI elements
public class ConversationBackendManager : Singleton<UserBackendManager>
{
    FirebaseFirestore db;
    private string _userPath;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;

    }

    public bool GetAllConversations()
    {
        //TODO:Implement GET all conversations from database
        return true;
    }

    public bool AddConversation(List<String> members,string description)
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

    public bool DeleteConversation(string conversationID) { 
        //TODO :Implement DELETE a conversation from database
        return true;
    }


}
