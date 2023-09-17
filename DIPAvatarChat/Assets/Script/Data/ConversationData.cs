using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationData
{
    [FirestoreProperty]
    public string conversationID { get; set; }

    [FirestoreProperty]
    public string description { get; set; }

    //list of user record IDs
    [FirestoreProperty]
    public List<string> members { get; set; }

    //list of message record IDs
    [FirestoreProperty]
    public List<string> messages { get; set; }
}
