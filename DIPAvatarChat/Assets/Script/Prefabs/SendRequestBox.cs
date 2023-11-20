using UnityEngine;

public class SendRequestBox : MonoBehaviour
{
    public GameObject Box;

    public void SendFriendRequest()
    {
        GameObject.Find("Canvas").GetComponent<ChatList>().SendFriendRequest();
    }
}
