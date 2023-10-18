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

    public async void EnterChat()
    {
        AuthManager.Instance.currConvId = await GameObject.Find("Canvas").GetComponent<NewChat>().GetCurrConvId(AuthManager.Instance.currUser, Box.name);
        AppManager.Instance.LoadScene("6-ChatUI");
    }

    public async void DisplayFriendAvatar2d()
    {
        //display 2d avatar
        DocumentSnapshot snapshot = await AvatarBackendManager.Instance.GetAvatarByEmailTask(Box.name);
        AvatarBackendManager.Instance.DisplayFriendAvatar2d(snapshot, AvatarHeadDisplayArea, AvatarSkinDisplayArea, AvatarHatDisplayArea);
    }
}
