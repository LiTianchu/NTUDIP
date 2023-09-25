using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public TMP_InputField MessageInputField;
    public TMP_Text RecipientName;
    public GameObject MyChatBubblePrefab;
    public GameObject TheirChatBubblePrefab;
    public GameObject ChatBubbleParent;

    //public static string currConvId { get; set; }
    ConversationData currConvData;
    UserData recipientUserData;

    // Start is called before the first frame update
    async void Start()
    {
        PopulateMessage(AuthManager.Instance.currConvId);
        SetRecipientName();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void SendMessage()
    {
        string myEmail = AuthManager.Instance.currUser.email;
        string theirEmail = null;

        foreach (string member in currConvData.members)
        {
            if (member != myEmail)
            {
                theirEmail = member;
            }
        }

        if (MessageInputField.text != null && MessageInputField.text != "")
        {
            bool IsMessageSent = await MessageBackendManager.Instance.SendMessageTask(currConvData, MessageInputField.text, myEmail, theirEmail);
            if (IsMessageSent)
            {
                PopulateMessage(AuthManager.Instance.currConvId);
            }
        }
    }

    private async void PopulateMessage(string conversationID)
    {
        ClearDisplay();

        //Populate the data onto the UI
        QuerySnapshot messages = await MessageBackendManager.Instance.GetAllMessagesTask(conversationID);
        foreach (DocumentSnapshot message in messages.Documents)
        {
            MessageData msg = message.ConvertTo<MessageData>();
            string msgText = msg.message;
            string msgSender = msg.sender;
            string msgReceiver = msg.receiver;
            Timestamp msgTime = msg.createdAt;

            if (msgSender.Equals(AuthManager.Instance.emailData))
            { //message is sent by me
                string username = AuthManager.Instance.currUser.username; // AuthManager.Instance.currUser.username
                string avatar = AuthManager.Instance.currUser.currentAvatar;
                Debug.Log(username + ": " + msgText + "   " + msgTime.ToString());

                //Spawn text bubble at right side of the chat
                InstantiateChatBubble(MyChatBubblePrefab, msgText, message.Id);

            }
            else
            { //message is sent by other user
                DocumentSnapshot otherUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(msgSender);
                UserData otherUser = otherUserDoc.ConvertTo<UserData>();
                string otherUserName = otherUser.username;
                string otherUserAvatar = otherUser.currentAvatar;
                Debug.Log(otherUserName + ": " + msgText + "   " + msgTime.ToString());

                //Spawn text bubble at left side of the chat
                InstantiateChatBubble(TheirChatBubblePrefab, msgText, message.Id);
            }
        }
    }

    public void InstantiateChatBubble(GameObject ChatBubblePrefab, string msgText, string messageId)
    {
        GameObject box = Instantiate(ChatBubblePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        box.transform.SetParent(ChatBubbleParent.transform, false);
        box.name = messageId;

        box.transform.GetChild(0).GetChild(0).gameObject.GetComponent<TMP_Text>().text = msgText;
    }

    public async void SetRecipientName()
    {
        recipientUserData = await GetRecipientData();
        RecipientName.text = recipientUserData.username;
    }

    public async Task<UserData> GetRecipientData()
    {
        DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(AuthManager.Instance.currConvId);
        currConvData = ConversationBackendManager.Instance.ProcessConversationDocument(conversationDoc);

        string recipientEmail = null;

        foreach (string member in currConvData.members)
        {
            if (member != AuthManager.Instance.currUser.email)
            {
                recipientEmail = member;
            }
        }

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(recipientEmail);
        UserData userData = UserBackendManager.Instance.ProcessUserDocument(userDoc);

        return userData;
    }

    public void ClearDisplay()
    {
        GameObject[] tempPrefabs;

        tempPrefabs = GameObject.FindGameObjectsWithTag("TempPrefab");

        foreach (GameObject tempPrefab in tempPrefabs)
        {
            Destroy(tempPrefab);
        }
    }
}
