using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

[FirestoreData]
public class AvatarData
{
    [FirestoreProperty]
    public DateTime createdAt { get; set; }

    [FirestoreProperty]
    public string ears { get; set; }

    [FirestoreProperty]
    public string eyes { get; set; }

    [FirestoreProperty]
    public string face { get; set; }

    [FirestoreProperty]
    public string hair { get; set; }

    [FirestoreProperty]
    public string head { get; set; }

    [FirestoreProperty]
    public string mouth { get; set; }

    [FirestoreProperty]
    public string nose { get; set; }

    [FirestoreProperty]
    public string userId { get; set; }
}