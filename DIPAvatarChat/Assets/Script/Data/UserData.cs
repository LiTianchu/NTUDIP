using Firebase.Firestore;

[FirestoreData]

public struct UserData
{
    [FirestoreProperty]
    public string avatars { get; set; }

    [FirestoreProperty]
    public string blacklist { get; set; }

    [FirestoreProperty]
    public string conversations { get; set; }

    [FirestoreProperty]
    public Timestamp createdAt { get; set; }

    [FirestoreProperty]
    public string currentAvatar { get; set; }

    [FirestoreProperty]
    public string email { get; set; }

    [FirestoreProperty]
    public string friendRequests { get; set; }

    [FirestoreProperty]
    public string friends { get; set; }

    [FirestoreProperty]
    public string name { get; set; }

   // [FirestoreProperty]
   // public string password { get; set; }

    [FirestoreProperty]
    public string phone { get; set; }

    [FirestoreProperty]
    public string settings { get; set; }

    [FirestoreProperty]
    public string status { get; set; }

    [FirestoreProperty]
    public string username { get; set; }
}
