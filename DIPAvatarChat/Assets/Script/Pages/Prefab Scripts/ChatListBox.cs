using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatListBox : MonoBehaviour
{
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
        AppManager.Instance.LoadScene("6-ChatFrontEnd");
    }
}
