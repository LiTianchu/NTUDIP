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

public class NewChat : MonoBehaviour
{
  public GameObject ContactsBoxPrefab;

  List<string> friendsList;

  //string testEmail = "dipgrp6@gmail.com";


  // Start is called before the first frame update
  void Start()
  {
    DisplayAllContacts();
  }

  // Update is called once per frame
  void Update()
  {

  }

  private void OnDisable()
  {
    if (!this.gameObject.scene.isLoaded) return;
  }

  public void ChatList()
  {
    AppManager.Instance.LoadScene("4-ChatList");
  }

  async public void DisplayAllContacts()
  {
    ClearDisplay();
    Debug.Log(AuthManager.Instance.emailData); // AuthManager.Instance.emailData;

    DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.emailData);
    UserData myUserData = UserBackendManager.Instance.ProcessUserDocument(myUserDoc);

    foreach (string friend in myUserData.friends)
    {
      if (friend != null && friend != "")
      {
        Debug.Log("Display friend: " + friend);

        DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(friend);
        UserData theirUserData = UserBackendManager.Instance.ProcessUserDocument(theirUserDoc);

        //Clone prefab for displaying friend request
        GameObject box = Instantiate(ContactsBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        box.transform.SetParent(GameObject.Find("ContactsContent").transform, false);
        box.name = theirUserData.email;

        //Show the email of the friend request sender
        box.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = theirUserData.username;
        box.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TMP_Text>().text = theirUserData.status;
      }
    }
  }

  public async Task<string> GetCurrConvId(UserData currUserData, string recipientEmail)
  {
    //db = FirebaseFirestore.DefaultInstance;
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
      currConvId = await ConversationBackendManager.Instance.StartNewConversation(currUserData, theirUserData, currUserConversationsList, theirUserConversationsList);
    }

    return currConvId;
  }

  public void ClearDisplay()
  {
    if (friendsList != null)
    {
      friendsList.Clear();
    }

    GameObject[] tempPrefabs;

    tempPrefabs = GameObject.FindGameObjectsWithTag("TempPrefab");

    foreach (GameObject tempPrefab in tempPrefabs)
    {
      Destroy(tempPrefab);
    }
  }
}
