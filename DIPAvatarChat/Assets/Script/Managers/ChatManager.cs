using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    public List<MessageData> CurrentMessages { get; set; }
    public string CurrentRecipientName { get; set; }
    //public AvatarData MyAvatarData { get; set; }
    //public AvatarData TheirAvatarData { get; set; }
    public List<UserData> Friends { get; set; }
    public Dictionary<string, AvatarData> EmailToAvatar { get; set; }

    //rotation for avatar spawn
    public readonly Quaternion MY_AVATAR_ROTATION = Quaternion.Euler(0f, 75f, 0f);
    public readonly Quaternion THEIR_AVATAR_ROTAION = Quaternion.Euler(0f, -75f, 0f);

    //pos for avatar spawn pos
    public readonly Vector3 MY_AVATAR_POS = new Vector3(40f, 10f, -30f);
    public readonly Vector3 THEIR_AVATAR_POS = new Vector3(-30f, 10f, -30f);
    public readonly Vector3 POPUP_AVATAR_POS = new Vector3(0f, -60f, -30f);
    public readonly Vector3 HEAD_AVATAR_POS = new Vector3(15f, -95f, -30f);

    //pos for hat accessories
    public readonly Vector3 HAT_POS = new Vector3(0f, 3.6f, 10f);
    public readonly Vector3 HAT_SCALE = new Vector3(0.8f, 0.8f, 0.8f);

    //pos for arm accessories
    public readonly Vector3 ARM_POS1 = new Vector3(-1.087f, 1.953f, 0f);
    public readonly Vector3 ARM_POS2 = new Vector3(1.087f, 1.953f, 0f);
    public readonly Vector3 ARM_SCALE = new Vector3(0.08f, 0.08f, 0.08f);

    //pos for shoes accessories
    public readonly Vector3 SHOES_POS = new Vector3(0f, 0f, 0f);
    public readonly Vector3 SHOES_SCALE = new Vector3(1f, 1f, 1f);

    private Dictionary<string, Animation> emojiAnimations = new Dictionary<string, Animation>();

    // Define a dictionary that maps emojis to their corresponding .anim files
    private Dictionary<string, string> emojiToAnimMap = new Dictionary<string, string>
    {
        { "😀", "laughing.anim" },
        { "😂", "crying.anim" },
        // Add more emoji-to-animation mappings here
    };

    void Start()
    {
        CurrentMessages = new List<MessageData>();
        Friends = new List<UserData>();
        EmailToAvatar = new Dictionary<string, AvatarData>();
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
            foreach (var kvp in emojiToAnimMap)
            {
                if (messageInputField.text.Contains(kvp.Key))
                {
                    Debug.Log("Emoji Animation: " + kvp.Value);

                    // Play the animation for the emoji
                    /*if (TryGetEmojiAnimation(kvp.Value, out Animation animation))
                    {
                        animation.Play();
                    }*/
                }
            }

            // After processing the emojis, send the message
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
    // Function to get the animation for a specific .anim file
    private bool TryGetEmojiAnimation(string animFileName, out Animation animation)
    {
        // Check if the .anim file has already been loaded
        if (emojiAnimations.TryGetValue(animFileName, out animation))
        {
            return true;
        }
        else
        {
            // Load the .anim file (replace 'AnimationPath' with the correct path)
            Animation loadedAnimation = Resources.Load<Animation>("Animations/" + animFileName);
            if (loadedAnimation != null)
            {
                // Store the loaded animation for future use
                emojiAnimations[animFileName] = loadedAnimation;
                animation = loadedAnimation;
                return true;
            }
        }

        animation = null;
        return false;
    }

    public GameObject LoadAvatar(AvatarData avatarData) {
       GameObject avatar = LoadAvatarBody("Blender/CatBaseTest2_v0_30");

        // Load hat accessory
        LoadAccessory(avatarData.hat, avatar, HAT_POS, HAT_SCALE);

        // Load arm accessory
        LoadAccessory(avatarData.arm, avatar, ARM_POS1, ARM_SCALE);

        // Load shoes accessory
        LoadAccessory(avatarData.shoes, avatar, SHOES_POS, SHOES_SCALE);

        return avatar;
    }

    public GameObject LoadAvatar(string email)
    {
        if (EmailToAvatar.ContainsKey(email))
        {
            return LoadAvatar(this.EmailToAvatar[email]);
        }else
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