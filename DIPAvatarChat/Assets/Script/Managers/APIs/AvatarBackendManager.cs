using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AvatarBackendManager : Singleton<AvatarBackendManager>
{
    FirebaseFirestore db;
    private string _userPath;

    public AvatarData currAvatarData = null;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;
    }

    public async Task<DocumentSnapshot> GetAvatarByIDTask(string avatarID)
    {
        db = FirebaseFirestore.DefaultInstance;

        DocumentReference avatarDoc = db.Collection("avatar").Document(avatarID);
        DocumentSnapshot doc = await avatarDoc.GetSnapshotAsync();
        return doc;
    }

    public async Task<bool> UploadAvatar()
    {
        try
        {
            // Upload the avatar data to Firestore
            DocumentReference avatarRef = await db.Collection("avatar").AddAsync(currAvatarData);
            string avatarID = avatarRef.Id;

            // Update the user's avatar ID in their profile
            Dictionary<string, object> userUpdate = new Dictionary<string, object>
            {
                { "currentAvatar", avatarID }
            };

            Dictionary<string, object> avatarUpdate = new Dictionary<string, object>
            {
                { "avatarId", avatarID }
            };

            await db.Collection("user").Document(currAvatarData.userEmail).UpdateAsync(userUpdate);
            await db.Collection("avatar").Document(avatarID).UpdateAsync(avatarUpdate);

            Debug.Log("Success uploading avatar data: " + avatarID);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error uploading avatar data: " + e.Message);
            return false;
        }
    }

    //updating avatardata
    public async Task<bool> UpdateAvatarData(string avatarID, AvatarData updatedAvatarData)
    {
        try
        {
            DocumentReference avatarRef = db.Collection("avatar").Document(avatarID);

            Dictionary<string, object> updatedData = new Dictionary<string, object>
        {
            { "colour", updatedAvatarData.colour },
            { "texture", updatedAvatarData.texture },
            { "expression", updatedAvatarData.expression },
            { "hat", updatedAvatarData.hat },
            { "arm", updatedAvatarData.arm },
            { "wings", updatedAvatarData.wings },
            { "tail", updatedAvatarData.tail },
        };

            await avatarRef.UpdateAsync(updatedData);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error updating avatar data: " + e.Message);
            return false;
        }
    }

    //deleting avatar
    public async Task<bool> DeleteAvatar(string avatarID)
    {
        try
        {
            DocumentReference avatarRef = db.Collection("avatar").Document(avatarID);

            await avatarRef.DeleteAsync();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error deleting avatar data: " + e.Message);
            return false;
        }
    }

    //querying avatar by userid for future use
    public async Task<List<string>> QueryAvatarsByUserEmail(string email)
    {
        try
        {
            Query avatarQuery = db.Collection("avatar").WhereEqualTo("userEmail", email);
            QuerySnapshot querySnapshot = await avatarQuery.GetSnapshotAsync();

            List<string> avatars = new List<string>();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                avatars.Add(documentSnapshot.Id);
            }

            return avatars;
        }
        catch (Exception e)
        {
            Debug.LogError("Error querying avatars by user ID: " + e.Message);
            return null;
        }
    }

}


