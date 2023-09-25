using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ContactsBox : MonoBehaviour
{
    FirebaseFirestore db;
    public GameObject Box;
    string id;
    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void EnterChat()
    {
        AuthManager.Instance.currConvId = await GetCurrConvId(AuthManager.Instance.currUser, Box.name);
        AppManager.Instance.LoadScene("6-ChatFrontEnd");
    }

    public async Task<string> GetCurrConvId(UserData currUserData, string recipientEmail)
    {
        db = FirebaseFirestore.DefaultInstance;
        string currConvId = null;

        DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(recipientEmail);
        UserData theirUserData = UserBackendManager.Instance.ProcessUserDocument(theirUserDoc);
        Debug.Log("Current user email: " + currUserData.email);

        List<string> currUserConversationsList = new List<string>(currUserData.conversations);
        List<string> theirUserConversationsList = new List<string>(theirUserData.conversations);

        foreach (string conversation in currUserData.conversations)
        {
            if (conversation != null && conversation != "")
            {
                Debug.Log("conversation: " + conversation);
                DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(conversation);
                ConversationData currConversation = ConversationBackendManager.Instance.ProcessConversationDocument(conversationDoc);

                foreach (string member in currConversation.members)
                {
                    if (recipientEmail == member)
                    {
                        // If conversation already exists
                        currConvId = conversationDoc.Id;
                        Debug.Log(currConvId);
                    }
                }
            }
        }

        // if conversation does not exist, start new conversation
        if (currConvId == null)
        {
            Debug.Log("Start new conversation");
            
            Dictionary<string, object> convData = new Dictionary<string, object>
            {
                { "description", "This is a chat with " + theirUserData.username },
                { "members", new List<string>() { currUserData.email, recipientEmail } },
                { "messages", new List<string>() { null } }
            };

            DocumentReference convDataRef = await db.Collection("conversation").AddAsync(convData);
            currConvId = convDataRef.Id;

            currUserConversationsList.Add(currConvId);
            theirUserConversationsList.Add(currConvId);

            Dictionary<string, object> currUserConversationsDict = new Dictionary<string, object>
            {
                { "conversations", currUserConversationsList },
            };

            Dictionary<string, object> theirUserConversationsDict = new Dictionary<string, object>
            {
                { "conversations", theirUserConversationsList },
            };

            db.Document("user/" + currUserData.email).UpdateAsync(currUserConversationsDict);
            db.Document("user/" + theirUserData.email).UpdateAsync(theirUserConversationsDict);
        }

        return currConvId;
    }
}
