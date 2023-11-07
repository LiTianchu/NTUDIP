using UnityEngine;

public class ChatListBox : MonoBehaviour
{
    public GameObject Box;
    public string CurrentAvatarUserEmail { get; set; }

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
        AppManager.Instance.LoadScene("6-ChatUI"); // 6-ChatUI | 6-ChatFrontEnd | Test-6
    }

    public async void DeleteChat()
    {
        if (await ConversationBackendManager.Instance.DeleteConversationTask(Box.name))
        {
            AppManager.Instance.ReloadScene();
        }
    }
}
