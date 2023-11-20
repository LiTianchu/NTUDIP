using UnityEngine;

public class FriendRequestBox : MonoBehaviour
{
    public GameObject Box;

    public void AcceptFriendRequest()
    {
        GameObject.Find("Canvas").GetComponent<ChatList>().AcceptFriendRequest(Box.name);
    }

    public void RejectFriendRequest()
    {
        GameObject.Find("Canvas").GetComponent<ChatList>().RejectFriendRequest(Box.name);
    }
}
