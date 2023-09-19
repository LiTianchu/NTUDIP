using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NewChat : MonoBehaviour
{

  List<string> friendRequestsList;
  List<string> friendsList;
  string usernameData;
  string emailData;
  string statusData;
  string friendRequestData;

  // Start is called before the first frame update
  void Start()
  {
    //attach event listeners for user data
    UserBackendManager.Instance.SearchUserContactsReceived += DisplayAllContactsData;
  }

  // Update is called once per frame
  void Update()
  {

  }

  private void OnDisable()
  {
    UserBackendManager.Instance.SearchUserContactsReceived -= DisplayAllContactsData;
  }

  public void DisplayAllContactsData(UserData userData)
  {
    Debug.Log("User Data Retrieved");

    usernameData = userData.username;
    emailData = userData.email;
    statusData = userData.status;
    friendsList = userData.friends;
    int i = 0;

    foreach (string friendRequest in friendRequestsList)
    {
      if (friendRequest != null && friendRequest != "")
      {
        Debug.Log("Display friend: " + friendRequest);

        //Clone prefab for displaying friend request
        /*GameObject box = Instantiate(friendRequestBoxPrefab, new Vector3(0, -150 - (i - 1) * 80, 0), Quaternion.identity) as GameObject;
        box.transform.SetParent(GameObject.Find("FriendRequestsTab").transform, false);
        box.name = friendRequest;

        Debug.Log("Instantiated Friend Request: " + i);

        //Show the email of the friend request sender
        box.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = friendRequest;

        friendRequestData = friendRequest;*/
      }
      i++;
    }
  }
}
