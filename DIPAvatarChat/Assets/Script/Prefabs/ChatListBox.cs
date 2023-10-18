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
        //display 2d avatar
        //string friendEmail = await AvatarBackendManager.Instance.GetAvatarForChatListBox(Box.name);
        //AvatarData avatarData = ChatManager.Instance.EmailToAvatarDict[friendEmail];
        DocumentSnapshot snapshot = await AvatarBackendManager.Instance.GetAvatarForChatListBox(Box.name);
        AvatarData avatarData = snapshot.ConvertTo<AvatarData>();

        List<Sprite> sprites = ChatManager.Instance.LoadAvatarSprite2d("2D_assets/catbase", "2D_assets/catcolor", avatarData.hat);
        
        Sprite skin2d = sprites[0];
        Sprite head2d = sprites[1];
        Sprite hat2d = sprites[2];

        if (skin2d != null)
        {
            Image imageComponent = AvatarSkinDisplayArea.GetComponent<Image>();
            imageComponent.sprite = skin2d;
        }
        else
        {
            Debug.Log("Skin sprite not found");
        }

        if (head2d != null)
        {
            Image imageComponent = AvatarHeadDisplayArea.GetComponent<Image>();
            imageComponent.sprite = head2d;
        }
        else
        {
            Debug.Log("Head sprite not found");
        }

        if (hat2d != null)
        {
            // Get the Image component attached to the GameObject
            Image imageComponent = AvatarHatDisplayArea.GetComponent<Image>();

            // Set the sprite
            imageComponent.sprite = hat2d;

            //LoadingUI.SetActive(false);
            AvatarHatDisplayArea.SetActive(true);
            AvatarSkinDisplayArea.SetActive(true);
            AvatarHeadDisplayArea.SetActive(true);
        }
        else
        {
            Debug.Log("No hat equipped");

            //LoadingUI.SetActive(false);
            AvatarSkinDisplayArea.SetActive(true);
            AvatarHeadDisplayArea.SetActive(true);
        }
    }
}
