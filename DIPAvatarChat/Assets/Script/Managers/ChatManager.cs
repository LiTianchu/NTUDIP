using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{
    public List<MessageData> CurrentMessages { get; set; }
    public string CurrentRecipientName { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        CurrentMessages = new List<MessageData>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
