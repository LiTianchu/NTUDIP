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
  public DateTime lastUpdatedAt { get; set; }

  [FirestoreProperty]
  public string colour { get; set; }

  [FirestoreProperty]
  public string texture { get; set; }

  [FirestoreProperty]
  public string expression { get; set; }

  [FirestoreProperty]
  public string hat { get; set; }

  [FirestoreProperty]
  public string arm { get; set; }

  [FirestoreProperty]
  public string wings { get; set; }

  [FirestoreProperty]
  public string tail { get; set; }

  [FirestoreProperty]
  public string userEmail { get; set; }

  [FirestoreProperty]
  public string avatarId { get; set; }
}