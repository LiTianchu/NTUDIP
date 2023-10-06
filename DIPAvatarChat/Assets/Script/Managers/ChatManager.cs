using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    public List<MessageData> CurrentMessages { get; set; }
    public string CurrentRecipientName { get; set; }
    public AvatarData MyAvatarData { get; set; }
    public AvatarData TheirAvatarData { get; set; }

    void Start()
    {
        CurrentMessages = new List<MessageData>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

   

    public void InstantiateChatBubble(GameObject _ChatBubbleParent, GameObject _ChatBubblePrefab, string msgText, string messageId)
    {
        GameObject box = Instantiate(_ChatBubblePrefab, _ChatBubbleParent.transform) as GameObject;
        box.name = messageId;

        box.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = msgText;
    }

    public async void SendMessage(TMP_InputField messageInputField)
    {
        string myEmail = AuthManager.Instance.currUser.email;
        string theirEmail = null;

        DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
        ConversationData currConvData = conversationDoc.ConvertTo<ConversationData>();

        Debug.Log(currConvData.messages.Count + " in SendMessage.");

        foreach (string member in currConvData.members)
        {
            if (member != myEmail)
            {
                theirEmail = member;
            }
        }

        if (messageInputField.text != null && messageInputField.text != "")
        {
            bool IsMessageSent = await MessageBackendManager.Instance.SendMessageTask(currConvData, messageInputField.text, myEmail, theirEmail);
            if (IsMessageSent)
            {
                messageInputField.text = "";
            }
        }
        else
        {
            Debug.Log("message is null...");
        }
    }

    public void LoadAvatar(GameObject avatarContainer) {
        Vector3 myAvatarSpawnPosition = new Vector3(-30f, 10f, -30f);
        Vector3 theirAvatarSpawnPosition = new Vector3(40f, 10f, -30f);

        // Spawn both avatar bodies
        LoadAvatarBody("Blender/CatBaseTest2_v0_30", avatarContainer, myAvatarSpawnPosition, Quaternion.Euler(0f, -75f, 0f), "MyAvatarBody");
        LoadAvatarBody("Blender/CatBaseTest2_v0_30", avatarContainer, theirAvatarSpawnPosition, Quaternion.Euler(0f, 75f, 0f), "TheirAvatarBody");

        // Load hat accessory
        Vector3 hatPosition = new Vector3(0f, 3.6f, 0f);
        Vector3 hatScale = new Vector3(0.2f, 0.2f, 0.2f);
        LoadAccessory(MyAvatarData.hat, avatarContainer.transform.GetChild(0).gameObject, hatPosition, hatScale);
        LoadAccessory(TheirAvatarData.hat, avatarContainer.transform.GetChild(1).gameObject, hatPosition, hatScale);

        // Load arm accessory
        Vector3 armPosition = new Vector3(-1.087f, 1.953f, 0f);
        Vector3 armPosition2 = new Vector3(1.087f, 1.953f, 0f);
        Vector3 armScale = new Vector3(0.08f, 0.08f, 0.08f);
        LoadAccessory(MyAvatarData.arm, avatarContainer.transform.GetChild(0).gameObject, armPosition, armScale);
        LoadAccessory(TheirAvatarData.arm, avatarContainer.transform.GetChild(1).gameObject, armPosition2, armScale);

    }

    public void LoadAvatarBody(string avatarBaseFbxFileName, GameObject AvatarDisplayArea, Vector3 itemPosition, Quaternion itemRotation, string avatarName)
    {
        if (avatarBaseFbxFileName != null && avatarBaseFbxFileName != "")
        {
            GameObject loadedFBX = Resources.Load<GameObject>(avatarBaseFbxFileName); // Eg. Blender/catbasetest.fbx

            if (loadedFBX != null)
            {
                GameObject fbx = Instantiate(loadedFBX, itemPosition, itemRotation);
                fbx.transform.SetParent(AvatarDisplayArea.transform, false);
                fbx.name = avatarName;

                float scale = 30f;
                fbx.transform.localScale = new Vector3(scale, scale, scale);
            }
            else
            {
                Debug.LogError("FBX asset not found: " + avatarBaseFbxFileName);
            }
        }
    }

    public void LoadAccessory(string fbxFileName, GameObject AvatarBody, Vector3 itemPosition, Vector3 itemScale)
    {
        if (fbxFileName != null && fbxFileName != "")
        {
            // Load the FBX asset from the Resources folder
            GameObject loadedFBX = Resources.Load<GameObject>(fbxFileName); // Eg. Blender/porkpiehat.fbx

            if (loadedFBX != null)
            {
                // Instantiate the loaded FBX as a GameObject in the scene
                GameObject fbx = Instantiate(loadedFBX, itemPosition, Quaternion.identity);
                fbx.transform.SetParent(AvatarBody.transform, false);
                fbx.transform.localScale = itemScale;
            }
            else
            {
                Debug.LogError("FBX asset not found: " + fbxFileName);
            }
        }
    }
}
