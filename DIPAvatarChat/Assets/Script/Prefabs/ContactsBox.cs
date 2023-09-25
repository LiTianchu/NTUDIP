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
        AuthManager.Instance.currConvId = await GameObject.Find("Canvas").GetComponent<NewChat>().GetCurrConvId(AuthManager.Instance.currUser, Box.name);
        AppManager.Instance.LoadScene("6-ChatFrontEnd");
    }
}
