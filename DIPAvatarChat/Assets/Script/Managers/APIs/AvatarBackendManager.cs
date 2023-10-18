using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;

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
    // A private method to create a new avatar data and return the document reference

    private async Task<DocumentReference> CreateNewAvatarData()
    {
        try
        {
            // Add the avatar data to the "avatar" collection in Firestore
            DocumentReference avatarRef = await db.Collection("avatar").AddAsync(currAvatarData);
            return avatarRef;
        }
        catch (Exception e)
        {
            // Handle and log any exceptions that occur during the creation
            Debug.LogError("Error creating new avatar data: " + e.Message);
            return null;
        }
    }

    public async Task<bool> UploadAvatarTask()
    {
        try
        {
            Debug.Log("Current Avatar: " + AuthManager.Instance.currUser.currentAvatar);

            if (AuthManager.Instance.currUser.currentAvatar == null)
            {
                // If user does not have an avatar, create a new one
                DocumentReference avatarRef = await CreateNewAvatarData();

                if (avatarRef != null)
                {
                    string avatarId = avatarRef.Id;

                    // Update the user's avatar ID in their profile
                    Dictionary<string, object> userUpdate = new Dictionary<string, object>
                    {
                        { "currentAvatar", avatarId }
                    };

                    // Update the user's profile in Firestore
                    await db.Collection("user").Document(currAvatarData.email).UpdateAsync(userUpdate);

                    AuthManager.Instance.currUser.currentAvatar = avatarId;

                    Debug.Log("Success creating new avatar data: " + AuthManager.Instance.currUser.currentAvatar);
                }
            }
            else
            {
                // If user already has an avatar, update the existing avatar data
                bool updated = await UpdateAvatarData();

                if (updated)
                {
                    Debug.Log("Success updating avatar data: " + AuthManager.Instance.currUser.currentAvatar);
                }
                else
                {
                    Debug.LogError("Failed to update avatar data.");
                }
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

    public async Task<bool> UpdateAvatarData()
    {
        try
        {
            if (currAvatarData != null)
            {
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
                    { "shoes", currAvatarData.shoes },
                    { "lastUpdatedAt", DateTime.Now },
                };

                // Update the avatar data in Firestore
                await avatarRef.UpdateAsync(updatedData);

                return true;
            }
            else
            {
                Debug.LogError("Current avatar data is null. Cannot update avatar data.");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error updating avatar data: " + e.Message);
            return false;
        }
    }

    public async void DisplayFriendAvatar2d(DocumentSnapshot snapshot, GameObject AvatarHeadDisplayArea, GameObject AvatarSkinDisplayArea, GameObject AvatarHatDisplayArea)
    {
        AvatarData avatarData = snapshot.ConvertTo<AvatarData>();

        List<Sprite> sprites = ChatManager.Instance.LoadAvatarSprite2d("2D_assets/catbase", "2D_assets/catcolor", avatarData.hat);

        Sprite skin2d = sprites[0];
        Sprite head2d = sprites[1];
        Sprite hat2d = sprites[2];

        if (skin2d != null)
        {
            Image imageComponent = AvatarSkinDisplayArea.GetComponent<Image>();
            imageComponent.sprite = skin2d;
        }
        else
        {
            Debug.Log("Skin sprite not found");
        }

        if (head2d != null)
        {
            Image imageComponent = AvatarHeadDisplayArea.GetComponent<Image>();
            imageComponent.sprite = head2d;
        }
        else
        {
            Debug.Log("Head sprite not found");
        }

        if (hat2d != null)
        {
            // Get the Image component attached to the GameObject
            Image imageComponent = AvatarHatDisplayArea.GetComponent<Image>();

            // Set the sprite
            imageComponent.sprite = hat2d;

            //LoadingUI.SetActive(false);
            AvatarHatDisplayArea.SetActive(true);
            AvatarSkinDisplayArea.SetActive(true);
            AvatarHeadDisplayArea.SetActive(true);
        }
        else
        {
            Debug.Log("No hat equipped");

            //LoadingUI.SetActive(false);
            AvatarSkinDisplayArea.SetActive(true);
            AvatarHeadDisplayArea.SetActive(true);
        }
    }

    /*public async Task<DocumentSnapshot> GetAvatarByEmail(string email)
    {
        try
        {
            DocumentSnapshot theirAvatarDoc = await GetAvatarByEmailTask(email);
            return theirAvatarDoc;
        }
        catch (Exception e)
        {
            Debug.LogError("Avatar Display Error: " + e.Message);
            return null;
        }
    }*/

    public async Task<bool> GetAvatarsForChat()
    {
        try
        {
            DocumentSnapshot currConvDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
            ConversationData currConvData = currConvDoc.ConvertTo<ConversationData>();

            if (currConvData != null)
            {
                foreach (string member in currConvData.members)
                {
                    DocumentSnapshot avatarDoc = await GetAvatarByEmailTask(member);

                    ChatManager.Instance.EmailToAvatarDict[member] = avatarDoc.ConvertTo<AvatarData>();
                }
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Avatar Display Error: " + e.Message);
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

    public async Task<DocumentSnapshot> GetAvatarByEmailTask(string email)
    {
        db = FirebaseFirestore.DefaultInstance;

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(email);
        UserData userData = userDoc.ConvertTo<UserData>();
        if (userData.currentAvatar == null)
        {
            Debug.Log("Theres no avatar fir user: " + email);
            return null;
        }

        DocumentSnapshot doc = null;
        try
        {
            DocumentReference usernameDoc = db.Collection("avatar").Document(userData.currentAvatar);
            doc = await usernameDoc.GetSnapshotAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError("Firestore Error: " + ex.Message);

        }
        return doc;
    }

    public async Task<DocumentSnapshot> GetAvatarByConversationIdTask(string currConvId)
    {
        DocumentSnapshot doc = null;

        try
        {
            string email = null;

            DocumentSnapshot currConvDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(currConvId);
            ConversationData currConvData = currConvDoc.ConvertTo<ConversationData>();

            if (currConvData != null)
            {
                foreach (string member in currConvData.members)
                {
                    if (member != AuthManager.Instance.currUser.email)
                    {
                        email = member;
                        doc = await GetAvatarByEmailTask(member);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Avatar Display Error: " + e.Message);
        }

        return doc;
    }

    //querying avatar by userid for future use
    public async Task<List<string>> QueryAvatarsByUserEmailTask(string email)
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


