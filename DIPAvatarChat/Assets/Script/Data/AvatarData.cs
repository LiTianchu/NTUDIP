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
    public string backgroundColor { get; set; }

    [FirestoreProperty]
    public string face { get; set; }

    [FirestoreProperty]
    public string hat { get; set; }

    [FirestoreProperty]
    public string watch { get; set; }

    [FirestoreProperty]
    public string wings { get; set; }

    [FirestoreProperty]
    public string tail { get; set; }

    [FirestoreProperty]
    public string userId { get; set; }
}