using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    public string CurrentRecipientName { get; set; }
    public Dictionary<string, HashSet<MessageData>> ConvIDToMessageDataDict { get; set; }
    public Dictionary<string, ConversationData> EmailToConversationDict { get; set; }
    public Dictionary<string, UserData> EmailToUsersDict { get; set; }

    public readonly int EMOJI_SIZE = 24;
    public readonly string MY_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/MyAvatarBody";
    public readonly string THEIR_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/TheirAvatarBody";
    public readonly string POPUP_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/PopUpTheirAvatar/AvatarPopupArea/PopupAvatarBody";

    //pos for avatar spawn pos
    public readonly Vector3 MY_AVATAR_POS = new Vector3(100f, 40f, -30f);
    public readonly Vector3 THEIR_AVATAR_POS = new Vector3(-100f, 40f, -30f);
    public readonly Vector3 POPUP_AVATAR_POS = new Vector3(0f, -60f, -30f);
    public readonly Vector3 HEAD_AVATAR_POS = new Vector3(15f, -95f, -30f);
    public readonly Quaternion MY_AVATAR_ROTATION = Quaternion.Euler(0f, 210f, 0f);
    public readonly Quaternion THEIR_AVATAR_ROTATION = Quaternion.Euler(0f, 150f, 0f);

    // Map commands to custom emotes
    private Dictionary<string, int> emojiToImageMap = new Dictionary<string, int>
    {
        { ":shocked:", 10},
        { ":xdface:", 12},
        { ":sus:", 19},
        { ":angry:", 21},
        { ":laugh:", 24},
        { ":cry:", 26 },
        { ":wave:", 9},
    };

    void Start()
    {
        ConvIDToMessageDataDict = new Dictionary<string, HashSet<MessageData>>();
        EmailToUsersDict = new Dictionary<string, UserData>();
        EmailToConversationDict = new Dictionary<string, ConversationData>();
    }

    public GameObject InstantiateChatBubble(GameObject _ChatBubbleParent, GameObject _ChatBubblePrefab, string msgText, string messageId)
    {
        GameObject box = Instantiate(_ChatBubblePrefab);
        box.transform.SetParent(_ChatBubbleParent.transform, false);
        box.transform.localPosition = new Vector3(box.transform.localPosition.x, box.transform.localPosition.y, 0);
        box.name = messageId;

        msgText = EmojiUpdate(msgText);

        box.transform.GetComponentInChildren<TMP_Text>().text = msgText;
        return box;
    }

    // converts emoji code to picture
    public string EmojiUpdate(string text)
    {
        foreach (var kvp in emojiToImageMap)
        {
            if (text.Contains(kvp.Key))
            {
                text = text.Replace(kvp.Key, $"<size={EMOJI_SIZE}><sprite={kvp.Value}></size>");
            }
        }

        return text;
    }

    // converts the emoji back to code (so that it wont store html to the database)
    public string ReverseEmojiUpdate(string text)
    {
        foreach (var kvp in emojiToImageMap)
        {
            if (text.Contains($"<size={EMOJI_SIZE}><sprite={kvp.Value}></size>"))
            {
                text = text.Replace($"<size={EMOJI_SIZE}><sprite={kvp.Value}></size>", kvp.Key);
            }
        }

        return text;
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
            string updText = ReverseEmojiUpdate(messageInputField.text);
            bool IsMessageSent = await MessageBackendManager.Instance.SendMessageTask(currConvData, updText, myEmail, theirEmail);
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

    public Sprite LoadEmojiSprite(string emojiFilePath)
    {
        if (emojiFilePath != null && emojiFilePath != "")
        {
            Sprite sprite = Resources.Load<Sprite>(emojiFilePath);

            return sprite;
        }
        return null;
    }
}