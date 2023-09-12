using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayText : MonoBehaviour
{
    // Using enter key:
    
    /*public GameObject chatMessagePrefab; // Reference to the prefab containing the image and TextMeshPro component
    public Transform chatPanel; // Parent transform for the chat messages
    public TMP_InputField chatBubble;

    private const string ChatMessageKeyPrefix = "ChatMessage_";

    // Start is called before the first frame update
    void Start()
    {
        // Load any previously saved chat messages (if needed)
        LoadChatMessages();
    }

    public void Create()
    {
        string text = chatBubble.text;

        // Check if the input is not empty
        if (!string.IsNullOrWhiteSpace(text))
        {
            // Create a new chat message GameObject
            GameObject chatMessage = Instantiate(chatMessagePrefab, chatPanel);

            // Access the TextMeshPro component within the chat message GameObject
            TMP_Text messageText = chatMessage.GetComponentInChildren<TMP_Text>();

            // Set the text
            messageText.text = text;

            // Save the text to PlayerPrefs (if needed)
            SaveChatMessage(text);

            // Clear the input field
            chatBubble.text = "";
        }
    }*/

    // Using send button:

    public GameObject chatMessagePrefab; // Reference to the prefab containing the image and TextMeshPro component
    public Transform chatPanel; // Parent transform for the chat messages
    public TMP_InputField chatBubble;
    public Button sendButton; // Reference to the send button in the Inspector

    private const string ChatMessageKeyPrefix = "ChatMessage_";

    // Start is called before the first frame update
    void Start()
    {
        // Load any previously saved chat messages (if needed)
        LoadChatMessages();

        // Attach an event handler to the send button's click event
        sendButton.onClick.AddListener(Create);
    }

    public void Create()
    {
        string text = chatBubble.text;

        // Check if the input is not empty
        if (!string.IsNullOrWhiteSpace(text))
        {
            // Create a new chat message GameObject
            GameObject chatMessage = Instantiate(chatMessagePrefab, chatPanel);

            // Access the TextMeshPro component within the chat message GameObject
            TMP_Text messageText = chatMessage.GetComponentInChildren<TMP_Text>();

            // Set the text
            messageText.text = text;

            // Save the text to PlayerPrefs (if needed)
            SaveChatMessage(text);

            // Clear the input field
            chatBubble.text = "";
        }
    }

    private void LoadChatMessages()
    {
        // Load and display previously saved chat messages (if any)
        // You can implement this part based on how you store and retrieve chat messages
    
        int messageIndex = 0;
        List<string> chatMessages = new List<string>();

        while (true)
        {
            string message = PlayerPrefs.GetString(ChatMessageKeyPrefix + messageIndex);
            if (string.IsNullOrEmpty(message))
            {
                break; // Stop loading when no more messages are found
            }

        chatMessages.Add(message);
        messageIndex++;
        }

        // Now you have the chatMessages list containing all the loaded chat messages
        // You can display them in your UI as needed
    }

    private void SaveChatMessage(string text)
    {
        // Save the chat message (if needed)
        // You can implement this part based on how you want to save chat messages

        // Find the index for the next chat message
        int messageIndex = 0;
        while (PlayerPrefs.HasKey(ChatMessageKeyPrefix + messageIndex))
        {
            messageIndex++;
        }

        // Save the chat message with a unique key
        PlayerPrefs.SetString(ChatMessageKeyPrefix + messageIndex, text);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
