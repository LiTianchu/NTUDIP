using UnityEngine;

public class ChatListBox : MonoBehaviour
{
    public GameObject Box;
    public string CurrentAvatarUserEmail { get; set; }

    public void EnterChat()
    {
        AuthManager.Instance.currConvId = Box.name;
        AppManager.Instance.LoadScene("6-ChatUI");
    }

    public async void DeleteChat()
    {
        if (await ConversationBackendManager.Instance.DeleteConversationTask(Box.name))
        {
            AppManager.Instance.ReloadScene();
        }
    }
}
