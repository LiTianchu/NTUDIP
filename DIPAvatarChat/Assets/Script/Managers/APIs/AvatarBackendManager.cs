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

    private void Start()
    {
        // Initialize Firestore instance
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;
    }
   
    // A private method to upload avatar data and return the document reference
    private async Task<DocumentReference> UploadAvatarData(AvatarData avatarData)
    {
        try
        {
            // Add the avatar data to the "avatar" collection in Firestore
            DocumentReference avatarRef = await db.Collection("avatar").AddAsync(avatarData);
            return avatarRef;
        }
        catch (Exception e)
        {
            // Handle and log any exceptions that occur during the upload
            Debug.LogError("Error uploading avatar data: " + e.Message);
            return null;
        }
    }

    public async Task<bool> UploadAvatar()
    {
        try
        {
            Debug.Log("Current Avatar: " + AuthManager.Instance.currUser.currentAvatar);
            
            if (AuthManager.Instance.currUser.currentAvatar == null)
            {
                // If user does not have an avatar
                DocumentReference avatarRef = await UploadAvatarData(currAvatarData);

                if (avatarRef != null)
                {
                    string avatarId = avatarRef.Id;

                    // Update the user's avatar ID in their profile
                    Dictionary<string, object> userUpdate = new Dictionary<string, object>
                    {
                        { "currentAvatar", avatarId }
                    };

                    Dictionary<string, object> avatarUpdate = new Dictionary<string, object>
                    {
                        { "avatarId", avatarId }
                    };

                    // Update the user's profile and the avatar document in Firestore
                    await db.Collection("user").Document(currAvatarData.email).UpdateAsync(userUpdate);
                    await db.Collection("avatar").Document(avatarId).UpdateAsync(avatarUpdate);

                    AuthManager.Instance.currUser.currentAvatar = avatarId;

                    Debug.Log("Success creating new avatar data: " + AuthManager.Instance.currUser.currentAvatar);
                }
            }
            else
            {
                // If user already has an avatar & trying to change it
                DocumentReference avatarRef = db.Collection("avatar").Document(AuthManager.Instance.currUser.currentAvatar);

                Dictionary<string, object> updatedData = new Dictionary<string, object>
                {
                    { "colour", currAvatarData.colour },
                    { "texture", currAvatarData.texture },
                    { "expression", currAvatarData.expression },
                    { "hat", currAvatarData.hat },
                    { "arm", currAvatarData.arm },
                    { "wings", currAvatarData.wings },
                    { "tail", currAvatarData.tail },
                    { "lastUpdatedAt", DateTime.Now },
                };

                // Update the avatar data in Firestore
                await avatarRef.UpdateAsync(updatedData);

                Debug.Log("Success updating avatar data: " + AuthManager.Instance.currUser.currentAvatar);
            }
            return true;
        }
        catch (Exception e)
        {
            // Handle and log any exceptions that occur during the avatar upload/update
            Debug.LogError("Error uploading avatar data: " + e.Message);
            return false;
        }
    }

    //updating avatardata
    /*public async Task<bool> UpdateAvatarData(string avatarID, AvatarData updatedAvatarData)
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
    }*/

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
            Query avatarQuery = db.Collection("avatar").WhereEqualTo("email", email);
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


