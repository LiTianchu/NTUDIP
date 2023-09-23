using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //hard coded id, need to replace with a dynamic id
        PopulateMessage("7SNpvqQwHcr6TWOn4n34");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
  
    }

    private async void PopulateMessage(string conversationID) {
        //TODO: Populate the data onto the UI
        QuerySnapshot messages = await MessageBackendManager.Instance.GetAllMessagesTask(conversationID);
        foreach(DocumentSnapshot message in messages.Documents)
        {
            MessageData msg = message.ConvertTo<MessageData>();
            Debug.Log(message.Id);
            Debug.Log(msg.message);
        }

        
    }
}
