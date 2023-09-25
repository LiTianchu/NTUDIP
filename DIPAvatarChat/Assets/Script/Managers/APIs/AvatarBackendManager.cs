/* using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AvatarBackendManager : Singleton<AvatarBackendManager>
{
    FirebaseFirestore db;
    private string _userPath;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        _userPath = AuthManager.Instance.userPathData;
    }

    public async Task<DocumentSnapshot> GetAvatarByIDTask(string avatarID)
    {
        db = FirebaseFirestore.DefaultInstance;

        DocumentReference avatarDoc = db.Collection("avatars").Document(avatarID);
        DocumentSnapshot doc = await avatarDoc.GetSnapshotAsync();
        return doc;
    }

    public async Task<string> UploadAvatar(UserData userData, AvatarData avatarData)
    {
        try
        {
            // Create an instance of the AvatarData class with the provided data
            var avatar = new AvatarData
            {
                createdAt = FieldValue.ServerTimestamp,
                ears = avatarData.ears,
                eyes = avatarData.eyes,
                face = avatarData.face,
                hair = avatarData.hair,
                head = avatarData.head,
                mouth = avatarData.mouth,
                nose = avatarData.nose,
                userId = userData.userId
            };

            // Upload the avatar data to Firestore
            DocumentReference avatarRef = await db.Collection("avatars").AddAsync(avatar);
            string avatarID = avatarRef.Id;

            // Update the user's avatar ID in their profile
            Dictionary<string, object> userUpdate = new Dictionary<string, object>
            {
                { "avatarId", avatarID }
            };

            await db.Collection("users").Document(userData.userId).UpdateAsync(userUpdate);

            return avatarID;
        }
        catch (Exception e)
        {
            Debug.LogError("Error uploading avatar data: " + e.Message);
            return null;
        }
    }

    public async Task<AvatarData> GetAvatarData(string avatarID)
    {
        DocumentSnapshot avatarDocSnapshot = await GetAvatarByIDTask(avatarID);
        if (avatarDocSnapshot != null && avatarDocSnapshot.Exists)
        {
            Dictionary<string, object> avatarDict = avatarDocSnapshot.ToDictionary();
            AvatarData avatarData = new AvatarData
            {
                // Convert the server timestamp to a DateTime
                createdAt = ((DateTime)avatarDict["createdAt"]),
                ears = (string)avatarDict["ears"],
                eyes = (string)avatarDict["eyes"],
                face = (string)avatarDict["face"],
                hair = (string)avatarDict["hair"],
                head = (string)avatarDict["head"],
                mouth = (string)avatarDict["mouth"],
                nose = (string)avatarDict["nose"],
                userId = (string)avatarDict["userId"]
            };
            return avatarData;
        }
        else
        {
            Debug.LogError("Avatar data not found for ID: " + avatarID);
            return null;
        }
    }

    //updating avatardata
    public async Task<bool> UpdateAvatarData(string avatarID, AvatarData updatedAvatarData)
    {
        try
        {
            DocumentReference avatarRef = db.Collection("avatars").Document(avatarID);

            Dictionary<string, object> updatedData = new Dictionary<string, object>
        {
            { "ears", updatedAvatarData.ears },
            { "eyes", updatedAvatarData.eyes },
            { "face", updatedAvatarData.face },
            { "hair", updatedAvatarData.hair },
            { "head", updatedAvatarData.head },
            { "mouth", updatedAvatarData.mouth },
            { "nose", updatedAvatarData.nose },
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
            DocumentReference avatarRef = db.Collection("avatars").Document(avatarID);

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
    public async Task<List<AvatarData>> QueryAvatarsByUserId(string userId)
    {
        try
        {
            Query avatarQuery = db.Collection("avatar").WhereEqualTo("userId", userId);
            QuerySnapshot querySnapshot = await avatarQuery.GetSnapshotAsync();

            List<AvatarData> avatars = new List<AvatarData>();

            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                AvatarData avatarData = await GetAvatarData(documentSnapshot.Id);
                avatars.Add(avatarData);
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
*/ 


