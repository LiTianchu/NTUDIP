using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class contains API for Create/Update/Delete/Read(CRUD) database data
//This class should be called from other classes(page scripts) to perform CRUD operations
//Do not call this class directly from UI elements
public class MessageBackendManager : Singleton<MessageBackendManager>
{

    FirebaseFirestore db;
    private string _userPath;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;

    }

    public bool GetAllMessages(string conversationID)
    {
        //TODO:Implement GET all messages from database for a conversation ID
        return true;

    }

    public bool AddMessage(string message, string conversationID, string receiverEmail, string senderEmail)
    {
        //TODO:Implement ADD a message record to database
        return true;
    }


}
