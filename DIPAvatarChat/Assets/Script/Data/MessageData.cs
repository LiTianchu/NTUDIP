using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[FirestoreData]
public class MessageData
{
    public string messageID { get; set; }

    //message content
    [FirestoreProperty]
    public string message { get; set; }

    //receiver's email
    [FirestoreProperty]
    public string receiver { get; set; }

    //sender's email
    [FirestoreProperty]
    public string sender { get; set; }

    [FirestoreProperty]
    public string conversationID { get; set; }

    [FirestoreProperty]
    public Timestamp createdAt {  get; set; }
}
