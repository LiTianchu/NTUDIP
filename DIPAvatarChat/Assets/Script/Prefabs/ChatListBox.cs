using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
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

public class ChatListBox : MonoBehaviour
{
    public GameObject Box;
    public string CurrentAvatarUserEmail {  get; set; }
    public GameObject AvatarDisplayArea;


    // Start is called before the first frame update
    void Start()
    {
        DisplayFriendAvatar();
        Debug.Log("Avatars loaded");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnterChat()
    {
        AuthManager.Instance.currConvId = Box.name;
        AppManager.Instance.LoadScene("6-ChatUI"); // 6-ChatUI | 6-ChatFrontEnd | Test-6
    }

    public async void DeleteChat()
    {
        if (await ConversationBackendManager.Instance.DeleteConversationTask(Box.name))
        {
            GameObject.Find("Canvas").GetComponent<ChatList>().RefreshConversation();
        }
    }

    //
    public async void DisplayFriendAvatar()
    {
        if (await AvatarBackendManager.Instance.GetAvatarForChatListBox(Box.name))
        {

            GameObject theirAvatarHead = ChatManager.Instance.LoadAvatar(CurrentAvatarUserEmail);
            theirAvatarHead.transform.position = ChatManager.Instance.HEAD_AVATAR_POS;
            theirAvatarHead.transform.rotation = Quaternion.identity;
            SetAvatar("TheirAvatarHead", theirAvatarHead, AvatarDisplayArea);
        }
    }

    private void SetAvatar(string name, GameObject avatarObj, GameObject avatarParent)
    {
        avatarObj.transform.SetParent(avatarParent.transform, false);
        avatarObj.name = name;

        float scale = 30f;
        avatarObj.transform.localScale = new Vector3(scale, scale, scale);
    }
}
