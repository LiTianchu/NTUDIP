using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //hard coded id, need to replace with a dynamic id
        MessageBackendManager.Instance.GetAllMessages("q12HSJSAG712HAHS1223swSD");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        //add event listener
        MessageBackendManager.Instance.MessageRetrieved += PopulateMessage;
    }

    private void OnDisable()
    {
        MessageBackendManager.Instance.MessageRetrieved -= PopulateMessage;
    }

    private void PopulateMessage(List<MessageData> messages) {
        //TODO: Populate the data onto the UI
        Debug.Log("Messages retrieved");
        foreach (MessageData item in messages)
        {
            Debug.Log(item.message);
        }
    }
}
