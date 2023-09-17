using Firebase.Firestore;
using System.Collections.Generic;

[FirestoreData]

public class FriendRequestData {

    [FirestoreProperty]
    public Timestamp createdAt { get; set; }

    [FirestoreProperty]
    public string description { get; set; }

    [FirestoreProperty]
    public string receiverID { get; set; }

    [FirestoreProperty]
    public string senderID { get; set; }
}
