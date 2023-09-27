using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatListBox : MonoBehaviour
{
    public GameObject Box;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void EnterChat()
    {
        AuthManager.Instance.currConvId = Box.name;
        AppManager.Instance.LoadScene("6-ChatUI"); // 6-ChatUI | 6-ChatFrontEnd
    }

    public async void DeleteChat()
    {
        if (await ConversationBackendManager.Instance.DeleteConversationTask(Box.name))
        {
            GameObject.Find("Canvas").GetComponent<ChatList>().RefreshConversation();
        }
    }
}
