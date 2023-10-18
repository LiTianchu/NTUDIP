using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public string CurrentAvatarUserEmail { get; set; }
    public GameObject AvatarSkinDisplayArea;
    public GameObject AvatarHeadDisplayArea;
    public GameObject AvatarHatDisplayArea;
    public GameObject LoadingUI;


    // Start is called before the first frame update
    void Start()
    {
        DisplayFriendAvatar2d();
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

    public async void DisplayFriendAvatar2d()
    {
        DocumentSnapshot snapshot = await AvatarBackendManager.Instance.GetAvatarByConversationIdTask(Box.name);  
        AvatarBackendManager.Instance.DisplayFriendAvatar2d(snapshot, AvatarHeadDisplayArea, AvatarSkinDisplayArea, AvatarHatDisplayArea);
    }
}
