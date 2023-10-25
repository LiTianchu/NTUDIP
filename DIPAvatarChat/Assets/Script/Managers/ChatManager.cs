using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
public class ChatManager : Singleton<ChatManager>
{
    public RuntimeAnimatorController animatorController;
    public string CurrentRecipientName { get; set; }
    public Dictionary<string, HashSet<MessageData>> ConvIDToMessageDataDict { get; set; }
    public Dictionary<string, ConversationData> EmailToConversationDict { get; set; }
    public Dictionary<string, UserData> EmailToUsersDict { get; set; }
    public Dictionary<string, AvatarData> EmailToAvatarDict { get; set; }

    //path for files
    public readonly string AVATAR_BODY_PATH = "Blender/Cat_Base_v3_3"; //"Blender/CatBaseTest2_v0_30";
    public readonly string MY_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/MyAvatarBody";
    public readonly string THEIR_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/TheirAvatarBody";
    public readonly string AVATAR_HAT_PATH = "/Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head";

    //rotation for avatar spawn
    public readonly Quaternion MY_AVATAR_ROTATION = Quaternion.Euler(0f, 180f, 0f);
    public readonly Quaternion THEIR_AVATAR_ROTATION = Quaternion.Euler(0f, 180f, 0f);
    public readonly Vector3 AVATAR_COLLIDER_SIZE = new Vector3(2f, 4f, 2f);
    public readonly Vector3 AVATAR_COLLIDER_CENTER = new Vector3(0f, 2f, 0f);

    //pos for avatar spawn pos
    public readonly Vector3 MY_AVATAR_POS = new Vector3(55f, 10f, -30f);
    public readonly Vector3 THEIR_AVATAR_POS = new Vector3(-55f, 10f, -30f);
    public readonly Vector3 POPUP_AVATAR_POS = new Vector3(0f, -60f, -30f);
    public readonly Vector3 HEAD_AVATAR_POS = new Vector3(15f, -95f, -30f);

    //pos for hat accessories
    private readonly Vector3 HAT_POS = new Vector3(-0.0005419353f, 0.01470897f, -5.878706e-05f);
    private readonly Vector3 HAT_SCALE = new Vector3(0.01f, 0.01f, 0.01f);
    private readonly Quaternion HAT_ROTATION = Quaternion.Euler(0.156f, -2.057f, 2.777f);

    //pos for arm accessories
    public readonly Vector3 ARM_POS1 = new Vector3(-1.087f, 1.953f, 0f);
    public readonly Vector3 ARM_POS2 = new Vector3(1.087f, 1.953f, 0f);
    public readonly Vector3 ARM_SCALE = new Vector3(0.08f, 0.08f, 0.08f);
    private readonly Quaternion ARM_ROTATION = Quaternion.Euler(0f, 0f, 0f);

    //pos for shoes accessories
    public readonly Vector3 SHOES_POS = new Vector3(0f, 0f, 0f);
    public readonly Vector3 SHOES_SCALE = new Vector3(1f, 1f, 1f);
    private readonly Quaternion SHOES_ROTATION = Quaternion.Euler(0f, 0f, 0f);

    private Dictionary<string, Animation> emojiAnimations = new Dictionary<string, Animation>();

    // Define a dictionary that maps emojis to their corresponding .anim files
    private Dictionary<string, string> emojiToAnimMap = new Dictionary<string, string>
    {
        { ">:(", "Angry"},
        { ":angry:", "Angry"},
    };

    // E71 angry
    private Dictionary<string, int> emojiToImageMap = new Dictionary<string, int>
    {
        { ":)", 0},
        { ":smile:", 0},
        { ":O", 10},
        { ":shocked:", 10},
        { "XD", 12},
        { ":P", 16},
        { ":nerd:", 18},
        { ":sus:", 19},
        { ">:(", 21},
        { ":angry:", 21},
        { ":flushed:", 24},
        { ":laughing:", 24},
        { "T.T", 26},
        { ":crying:", 26},
        { ":ok:", 39},
        { ":oops:", 53},
    };

    public Dictionary<string, string> hatTo2dHatMap = new Dictionary<string, string>
    {
        { "Blender/beret", "2D_assets/beret"},
        { "Blender/crown", "2D_assets/crown"},
        { "Blender/horns", "2D_assets/horns"},
        { "Blender/nightcap", "2D_assets/sleepcap"},
        { "Blender/partyhat", "2D_assets/partyhat"},
        { "Blender/porkpiehat", "2D_assets/porkpiehat"},
        { "Blender/starclip", "2D_assets/starclip"},
        { "Blender/strawboater", "2D_assets/strawboater"},
        { "Blender/sunflower", "2D_assets/flowers"},
    };

    void Start()
    {
        ConvIDToMessageDataDict = new Dictionary<string, HashSet<MessageData>>();
        EmailToUsersDict = new Dictionary<string, UserData>();
        EmailToAvatarDict = new Dictionary<string, AvatarData>();
        EmailToConversationDict = new Dictionary<string, ConversationData>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject InstantiateChatBubble(GameObject _ChatBubbleParent, GameObject _ChatBubblePrefab, string msgText, string messageId)
    {
        GameObject box = Instantiate(_ChatBubblePrefab);
        box.transform.parent = _ChatBubbleParent.transform;
        box.transform.localPosition = new Vector3(box.transform.localPosition.x, box.transform.localPosition.y, 0);
        box.name = messageId;

        foreach (var kvp in emojiToImageMap)
        {
            if (msgText.Contains(kvp.Key))
            {
                msgText = msgText.Replace(kvp.Key, $"<size=36><sprite={kvp.Value}></size>");
            }
        }

        box.transform.GetComponentInChildren<TMP_Text>().text = msgText;
        return box;
    }

    public void SendMessage(TMP_InputField messageInputField)
    {
        SendMessage(messageInputField, AuthManager.Instance.currConvId);
    }

    public async void SendMessage(TMP_InputField messageInputField, string convID)
    {
        string myEmail = AuthManager.Instance.currUser.email;
        string theirEmail = null;

        DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(convID);
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

    public void CacheMessage(string convID, MessageData msg)
    {
        if (!ConvIDToMessageDataDict.ContainsKey(convID))
        {
            ConvIDToMessageDataDict[convID] = new HashSet<MessageData>();
        }
        ConvIDToMessageDataDict[convID].Add(msg);
    }

    public void PlayAnimation(GameObject avatar, string msgText)
    {
        Animator _animator = avatar.GetComponent<Animator>();

        try
        {
            foreach (var kvp in emojiToAnimMap)
            {
                if (msgText.Contains(kvp.Key))
                {
                    Debug.Log("Animation: " + kvp.Value);

                    _animator.SetBool(kvp.Value, true);
                    _animator.SetBool("Default", true);
                    return;
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Error playing animation: " + e);
        }
    }

    public Sprite LoadEmojiSprite(string emojiFilePath)
    {
        if (emojiFilePath != null && emojiFilePath != "")
        {
            Sprite sprite = Resources.Load<Sprite>(emojiFilePath);

            return sprite;
        }
        return null;
    }

    public List<Sprite> LoadAvatarSprite2d(string headFilePath, string skinFilePath, string hatFilePath)
    {
        List<Sprite> sprites = new List<Sprite>();

        Sprite head2d = Resources.Load<Sprite>(headFilePath);
        Sprite skin2d = Resources.Load<Sprite>(skinFilePath);
        Sprite hat2d = null;

        if (hatFilePath != null && hatFilePath != "")
        {
            foreach (var kvp in hatTo2dHatMap)
            {
                if (hatFilePath.Contains(kvp.Key))
                {
                    Debug.Log("2d Hat file path: " + kvp.Value);
                    hat2d = Resources.Load<Sprite>(kvp.Value);
                }
            }
        }

        sprites.Add(skin2d);
        sprites.Add(head2d);
        sprites.Add(hat2d);

        return sprites;
    }

    public GameObject LoadAvatar(AvatarData avatarData)
    {
        GameObject avatar = LoadAvatarBody(AVATAR_BODY_PATH);

        // Load hat accessory
        LoadAccessory(avatarData.hat, avatar, HAT_POS, HAT_SCALE, HAT_ROTATION);

        // Load arm accessory
        LoadAccessory(avatarData.arm, avatar, ARM_POS1, ARM_SCALE, ARM_ROTATION);

        // Load shoes accessory
        LoadAccessory(avatarData.shoes, avatar, SHOES_POS, SHOES_SCALE, SHOES_ROTATION);

        return avatar;
    }

    public GameObject LoadAvatar(string email)
    {
        if (EmailToAvatarDict.ContainsKey(email))
        {
            return LoadAvatar(this.EmailToAvatarDict[email]);
        }
        else
        {
            throw new KeyNotFoundException(email);
        }
    }

    public GameObject LoadAvatarBody(string avatarBaseFbxFileName)
    {
        if (avatarBaseFbxFileName != null && avatarBaseFbxFileName != "")
        {
            GameObject loadedFBX = Resources.Load<GameObject>(avatarBaseFbxFileName); // Eg. Blender/catbasetest.fbx

            if (loadedFBX != null)
            {
                GameObject fbx = Instantiate(loadedFBX);
                BoxCollider collider = fbx.AddComponent<BoxCollider>(); //add collider to body to detect raycast
                collider.size = AVATAR_COLLIDER_SIZE;
                collider.center = AVATAR_COLLIDER_CENTER;

                fbx.AddComponent<Animator>();

                Animator animator = fbx.GetComponent<Animator>();
                animator.runtimeAnimatorController = animatorController;
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

    public void LoadAccessory(string fbxFileName, GameObject AvatarBody, Vector3 itemPosition, Vector3 itemScale, Quaternion itemRotation)
    {
        if (fbxFileName != null && fbxFileName != "")
        {
            // Load the FBX asset from the Resources folder
            GameObject loadedFBX = Resources.Load<GameObject>(fbxFileName); // Eg. Blender/porkpiehat.fbx
            if (loadedFBX != null)
            {
                // Instantiate the loaded FBX as a GameObject in the scene
                GameObject fbx = Instantiate(loadedFBX, itemPosition, itemRotation);

                fbx.transform.SetParent(AvatarBody.transform, false);
                fbx.transform.localScale = itemScale;
            }
            else
            {
                Debug.LogError("FBX asset not found: " + fbxFileName);
            }
        }
    }

    public void EquipAccessory(GameObject accessory, string avatarBodyPath, string accessoryPath)
    {
        if (avatarBodyPath != null && accessoryPath != null)
        {
            Debug.Log("Filepath: " + avatarBodyPath + accessoryPath);
            GameObject parent = GameObject.Find(avatarBodyPath + accessoryPath);
            accessory.transform.SetParent(parent.transform, false);
        }
    }
}