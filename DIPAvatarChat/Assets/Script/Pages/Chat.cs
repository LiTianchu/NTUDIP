using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //TODO: hard coded id, need to replace with a dynamic id
        PopulateMessage("7SNpvqQwHcr6TWOn4n34");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private async void PopulateMessage(string conversationID) {
        //TODO: Populate the data onto the UI
        QuerySnapshot messages = await MessageBackendManager.Instance.GetAllMessagesTask(conversationID);
        foreach(DocumentSnapshot message in messages.Documents)
        {
            MessageData msg = message.ConvertTo<MessageData>();
            string msgText = msg.message;
            string msgSender = msg.sender;
            string msgReceiver = msg.receiver;
            Timestamp msgTime = msg.createdAt;

            if (msgSender.Equals(AuthManager.Instance.emailData))
            { //message is sent by me
                string username = AuthManager.Instance.currUser.username;
                string avatar = AuthManager.Instance.currUser.currentAvatar;
                Debug.Log(username + ": " + msgText + "   " + msgTime.ToString());
                
                //TODO: spawn text bubble at right side of the chat

            }
            else 
            { //message is sent by other user
               DocumentSnapshot otherUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(msgSender);
               UserData otherUser = otherUserDoc.ConvertTo<UserData>();
               string otherUserName = otherUser.username;
               string otherUserAvatar = otherUser.currentAvatar;
               Debug.Log(otherUserName + ": " + msgText + "   " + msgTime.ToString());

               //TODO: spawn text bubble at left side of the chat

            }
        }

        
    }
}
