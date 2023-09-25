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
