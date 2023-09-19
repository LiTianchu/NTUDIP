using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendRequestBox : MonoBehaviour
{
    public GameObject Box;
    public static string id;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcceptFriendRequest()
    {
        id = Box.name;
        GameObject.Find("Canvas").GetComponent<ChatList>().AcceptFriendRequest();
    }

    public void RejectFriendRequest()
    {
        id = Box.name;
        GameObject.Find("Canvas").GetComponent<ChatList>().RejectFriendRequest();
    }
}
