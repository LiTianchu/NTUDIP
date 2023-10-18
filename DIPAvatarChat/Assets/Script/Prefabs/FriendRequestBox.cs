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

public class FriendRequestBox : MonoBehaviour
{
    public GameObject Box;
    public GameObject AvatarSkinDisplayArea;
    public GameObject AvatarHeadDisplayArea;
    public GameObject AvatarHatDisplayArea;

    // Start is called before the first frame update
    void Start()
    {
        DisplayFriendAvatar2d();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AcceptFriendRequest()
    {
        GameObject.Find("Canvas").GetComponent<ChatList>().AcceptFriendRequest(Box.name);
    }

    public void RejectFriendRequest()
    {
        GameObject.Find("Canvas").GetComponent<ChatList>().RejectFriendRequest(Box.name);
    }

    public async void DisplayFriendAvatar2d()
    {
        //display 2d avatar
        DocumentSnapshot snapshot = await AvatarBackendManager.Instance.GetAvatarByEmailTask(Box.name);
        AvatarBackendManager.Instance.DisplayFriendAvatar2d(snapshot, AvatarHeadDisplayArea, AvatarSkinDisplayArea, AvatarHatDisplayArea);
    }
}
