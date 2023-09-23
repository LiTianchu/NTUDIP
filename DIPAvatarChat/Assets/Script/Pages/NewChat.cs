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
  string usernameData;
  string emailData;
  string statusData;
  string friendData;
  string friendUsernameData;
  string friendEmailData;
  string friendStatusData;

  string testEmail = "dipgrp6@gmail.com";


  // Start is called before the first frame update
  void Start()
  {
    //attach event listeners for user data
    //UserBackendManager.Instance.SearchUserContactsReceived += DisplayAllContactsData;
    //UserBackendManager.Instance.OtherUserDataReceived += ContactsData;
    DisplayAllContacts();
  }

  // Update is called once per frame
  void Update()
  {

  }

  private void OnDisable()
  {
    if (!this.gameObject.scene.isLoaded) return;
    //UserBackendManager.Instance.SearchUserContactsReceived -= DisplayAllContactsData;
    //UserBackendManager.Instance.OtherUserDataReceived -= ContactsData;
  }

  public void ChatList()
  {
    AppManager.Instance.LoadScene("4-ChatList");
  }

  async public void DisplayAllContacts()
  {
    ClearDisplay();
    Debug.Log(testEmail); // AUthManager.Instance.emailData;

    DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(testEmail);
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

  public void DisplayAllContactsData(UserData userData)
  {
    Debug.Log("User Data Retrieved");

    usernameData = userData.username;
    emailData = userData.email;
    statusData = userData.status;
    friendsList = userData.friends;
    int i = 0;

    foreach (string friend in friendsList)
    {
      if (friend != null && friend != "")
      {
        Debug.Log("friend id: " + friend);

        UserBackendManager.Instance.GetOtherUser(friend);

      }
      i++;
    }
  }

  public void ContactsData(UserData userData)
  {
    //friendUsernameData = userData.username;
    //friendEmailData = userData.email;
    //friendStatusData = userData.status;

    //Clone prefab for displaying contacts
    GameObject box = Instantiate(ContactsBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
    box.transform.SetParent(GameObject.Find("ContactsContent").transform, false);
    box.name = userData.username;

    //Show the name and status
    box.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = userData.username;
    box.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TMP_Text>().text = userData.status;
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
