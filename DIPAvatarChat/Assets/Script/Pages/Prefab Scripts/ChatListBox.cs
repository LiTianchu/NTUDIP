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

    public void OpenChatFrontEnd()
    {
        Chat.currConvId = Box.name;
        
        AppManager.Instance.LoadScene("6-ChatFrontEnd");
        //AppManager.Instance.LoadScene("Test-6");
    }
}
