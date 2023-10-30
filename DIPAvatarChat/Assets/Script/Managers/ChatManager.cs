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
    public string CurrentRecipientName { get; set; }
    public Dictionary<string, HashSet<MessageData>> ConvIDToMessageDataDict { get; set; }
    public Dictionary<string, ConversationData> EmailToConversationDict { get; set; }
    public Dictionary<string, UserData> EmailToUsersDict { get; set; }

    public readonly string MY_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/MyAvatarBody";
    public readonly string THEIR_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/AvatarMask/AvatarArea/TheirAvatarBody";
    public readonly string POPUP_AVATAR_BODY_PATH = "/UserSelected/ChatUI/Canvas/PopUpTheirAvatar/AvatarPopupArea/PopupAvatarBody";

    //pos for avatar spawn pos
    public readonly Vector3 MY_AVATAR_POS = new Vector3(55f, 10f, -30f);
    public readonly Vector3 THEIR_AVATAR_POS = new Vector3(-55f, 10f, -30f);
    public readonly Vector3 POPUP_AVATAR_POS = new Vector3(0f, -60f, -30f);
    public readonly Vector3 HEAD_AVATAR_POS = new Vector3(15f, -95f, -30f);
    public readonly Quaternion MY_AVATAR_ROTATION = Quaternion.Euler(0f, 180f, 0f);
    public readonly Quaternion THEIR_AVATAR_ROTATION = Quaternion.Euler(0f, 180f, 0f);

    private Dictionary<string, Animation> emojiAnimations = new Dictionary<string, Animation>();

    // Map commands to custom emotes
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
        { ":flushed:", 22},
        { ":laughing:", 24},
        { "T.T", 26},
        { ":crying:", 26},
        { ":ok:", 39},
        { ":oops:", 53},
    };

    void Start()
    {
        ConvIDToMessageDataDict = new Dictionary<string, HashSet<MessageData>>();
        EmailToUsersDict = new Dictionary<string, UserData>();
        EmailToConversationDict = new Dictionary<string, ConversationData>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject InstantiateChatBubble(GameObject _ChatBubbleParent, GameObject _ChatBubblePrefab, string msgText, string messageId)
    {
        GameObject box = Instantiate(_ChatBubblePrefab);
        //box.transform.parent = _ChatBubbleParent.transform;
        box.transform.SetParent(_ChatBubbleParent.transform, false);
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