using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class NewChat : MonoBehaviour, IPageTransition
{
    public GameObject ContactsBoxPrefab;
    public Transform contactBoxContainer;

    [Header("UI Transition")]
    public CanvasGroup topBar;
    public CanvasGroup chatList;

    List<string> friendsList;

    // Start is called before the first frame update
    void Start()
    {
        FadeInUI();
        DisplayAllContacts();
    }

    private void OnDisable()
    {
        if (!this.gameObject.scene.isLoaded) return;
    }

    public void ChatList()
    {

        StartCoroutine(ExitRoutine());
    }

    async public void DisplayAllContacts()
    {
        ClearDisplay();
        Debug.Log(AuthManager.Instance.currUser.email);

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.currUser.email);
        UserData myUserData = myUserDoc.ConvertTo<UserData>();

        foreach (string friend in myUserData.friends)
        {
            if (friend != null && friend != "")
            {
                Debug.Log("Display friend: " + friend);

                DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(friend);
                UserData theirUserData = theirUserDoc.ConvertTo<UserData>();

                //Clone prefab for displaying friend request
                GameObject box = Instantiate(ContactsBoxPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
                box.transform.SetParent(contactBoxContainer, false);
                box.name = theirUserData.email;

                //Show the email of the friend request sender
                box.transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = theirUserData.username;
                box.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<TMP_Text>().text = theirUserData.status;
            }
        }

        contactBoxContainer.GetComponent<VerticalResizable>().Resize();
        
    }

    public async Task<string> GetCurrConvId(string recipientEmail)
    {
        string currConvId = null;

        DocumentSnapshot theirUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(recipientEmail);
        UserData theirUserData = theirUserDoc.ConvertTo<UserData>();

        DocumentSnapshot myUserDoc = await UserBackendManager.Instance.GetUserByEmailTask(AuthManager.Instance.currUser.email);
        AuthManager.Instance.currUser = myUserDoc.ConvertTo<UserData>();
        Debug.Log("Current user email: " + AuthManager.Instance.currUser.email);


        List<string> currUserConversationsList = new List<string>(AuthManager.Instance.currUser.conversations);
        List<string> theirUserConversationsList = new List<string>(theirUserData.conversations);

        foreach (string conversation in AuthManager.Instance.currUser.conversations)
        {
            if (conversation != null && conversation != "")
            {
                Debug.Log("conversation: " + conversation);
                DocumentSnapshot conversationDoc = await ConversationBackendManager.Instance.GetConversationByIDTask(conversation);
                ConversationData currConversation = conversationDoc.ConvertTo<ConversationData>();

                if(currConversation == null)
                {
                    Debug.LogWarning("Conversation with ID: " +  conversation + " not found in database");
                    continue;
                }
                foreach (string member in currConversation.members)
                {
                    if (recipientEmail == member)
                    {
                        // If conversation already exists
                        currConvId = conversationDoc.Id;
                        Debug.Log(currConvId);
                    }
                }
            }
        }

        // if conversation does not exist, start new conversation
        if (currConvId == null)
        {
            Debug.Log("Start new conversation");
            currConvId = await ConversationBackendManager.Instance.StartNewConversation(AuthManager.Instance.currUser, theirUserData, currUserConversationsList, theirUserConversationsList);
        }

        return currConvId;
    }

    public void ClearDisplay()
    {
        if (friendsList != null)
        {
            friendsList.Clear();
        }

        GameObject[] tempPrefabs;

        tempPrefabs = GameObject.FindGameObjectsWithTag("TempPrefab");

        foreach (GameObject tempPrefab in tempPrefabs)
        {
            Destroy(tempPrefab);
        }
    }

    public void FadeInUI()
    {
        UIManager.Instance.PanelFadeIn(topBar, 0.5f, UIManager.UIMoveDir.FromTop, topBar.GetComponent<RectTransform>().anchoredPosition);
        UIManager.Instance.PanelFadeIn(chatList, 0.5f, UIManager.UIMoveDir.FromLeft, chatList.GetComponent<RectTransform>().anchoredPosition);
    }

    public IEnumerator ExitRoutine()
    {
        UIManager.Instance.PanelFadeOut(topBar, 0.5f, UIManager.UIMoveDir.FromTop, topBar.GetComponent<RectTransform>().anchoredPosition); //fade out all UI
        UIManager.Instance.PanelFadeOut(chatList, 0.5f, UIManager.UIMoveDir.FromLeft, chatList.GetComponent<RectTransform>().anchoredPosition); //fade out all UI
        yield return new WaitForSeconds(0.5f);
        AppManager.Instance.LoadScene("4-ChatList");
    }


}
