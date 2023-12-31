using Firebase.Firestore;
using System;

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
  public string ears { get; set; }

  [FirestoreProperty]
  public string shoes { get; set; }

  [FirestoreProperty]
  public string email { get; set; }

  [FirestoreProperty]
  public string avatarId { get; set; }
}