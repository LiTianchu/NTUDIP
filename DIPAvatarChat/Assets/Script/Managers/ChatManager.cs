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

    //pos for avatar spawn pos
    private readonly Vector3 MY_AVATAR_POS = new Vector3(40f, 10f, -30f);
    private readonly Vector3 THEIR_AVATAR_POS = new Vector3(-30f, 10f, -30f);
    private readonly Vector3 POPUP_AVATAR_POS = new Vector3(0f, -60f, -30f);

    //pos for hat accessories
    private readonly Vector3 HAT_POS = new Vector3(0f, 3.6f, 0f);
    private readonly Vector3 HAT_SCALE = new Vector3(0.2f, 0.2f, 0.2f);

    //pos for arm accessories
    private readonly Vector3 ARM_POS1 = new Vector3(-1.087f, 1.953f, 0f);
    private readonly Vector3 ARM_POS2 = new Vector3(1.087f, 1.953f, 0f);
    private readonly Vector3 ARM_SCALE = new Vector3(0.08f, 0.08f, 0.08f);

    //pos for shoes accessories
    private readonly Vector3 SHOES_POS = new Vector3(0f, 0f, 0f);
    private readonly Vector3 SHOES_SCALE = new Vector3(1f, 1f, 1f);

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
        GameObject box = Instantiate(_ChatBubblePrefab);
        box.transform.parent = _ChatBubbleParent.transform;
        box.transform.localPosition = new Vector3(box.transform.localPosition.x, box.transform.localPosition.y, 0);
        box.name = messageId;

        box.transform.GetComponentInChildren<TMP_Text>().text = msgText;
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

    public GameObject LoadMyAvatar()
    {
        // Spawn both avatar bodies
        GameObject myAvatar = LoadAvatarBody("Blender/CatBaseTest2_v0_30", MY_AVATAR_POS, Quaternion.Euler(0f, 75f, 0f));

        // Load hat accessory
        LoadAccessory(MyAvatarData.hat, myAvatar, HAT_POS, HAT_SCALE);

        // Load arm accessory
        LoadAccessory(MyAvatarData.arm, myAvatar, ARM_POS1, ARM_SCALE);

        // Load shoes accessory
        LoadAccessory(MyAvatarData.shoes, myAvatar, SHOES_POS, SHOES_SCALE);

        return myAvatar;
    }

    public GameObject LoadTheirAvatar()
    {
        // Spawn both avatar bodies
        GameObject theirAvatar = LoadAvatarBody("Blender/CatBaseTest2_v0_30", THEIR_AVATAR_POS, Quaternion.Euler(0f, -75f, 0f));

        // Load hat accessory
        LoadAccessory(TheirAvatarData.hat, theirAvatar, HAT_POS, HAT_SCALE);

        // Load arm accessory
        LoadAccessory(TheirAvatarData.arm, theirAvatar, ARM_POS2, ARM_SCALE);

        // Load shoes accessory
        LoadAccessory(TheirAvatarData.shoes, theirAvatar, SHOES_POS, SHOES_SCALE);

        return theirAvatar;
    }

    public GameObject LoadPopupAvatar()
    {
        // Spawn both avatar bodies
        GameObject popupAvatar = LoadAvatarBody("Blender/CatBaseTest2_v0_30", POPUP_AVATAR_POS, Quaternion.Euler(0f, 0f, 0f));

        // Load hat accessory
        LoadAccessory(TheirAvatarData.hat, popupAvatar, HAT_POS, HAT_SCALE);

        // Load arm accessory
        LoadAccessory(TheirAvatarData.arm, popupAvatar, ARM_POS1, ARM_SCALE);

        // Load shoes accessory
        LoadAccessory(TheirAvatarData.shoes, popupAvatar, SHOES_POS, SHOES_SCALE);

        return popupAvatar;
    }

    public GameObject LoadAvatarBody(string avatarBaseFbxFileName, Vector3 itemPosition, Quaternion itemRotation)
    {
        if (avatarBaseFbxFileName != null && avatarBaseFbxFileName != "")
        {
            GameObject loadedFBX = Resources.Load<GameObject>(avatarBaseFbxFileName); // Eg. Blender/catbasetest.fbx

            if (loadedFBX != null)
            {
                GameObject fbx = Instantiate(loadedFBX, itemPosition, itemRotation);
                return fbx;
            }
            else
            {
                Debug.LogError("FBX asset not found: " + avatarBaseFbxFileName);
                return null;
            }
        }
        return null;
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
