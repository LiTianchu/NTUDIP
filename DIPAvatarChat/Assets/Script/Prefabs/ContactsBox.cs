using UnityEngine;

public class ContactsBox : MonoBehaviour
{
    public GameObject Box;

    public async void EnterChat()
    {
        AuthManager.Instance.currConvId = await GameObject.Find("Canvas").GetComponent<NewChat>().GetCurrConvId(Box.name);
        AppManager.Instance.LoadScene("6-ChatUI");
    }
}
