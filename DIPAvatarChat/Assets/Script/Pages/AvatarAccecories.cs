using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarAccessories : MonoBehaviour
{

    public GameObject AccessoryPrefab; // Reference to the prefab containing the image and TextMeshPro component
    public Transform AccessoryPanel; // Parent transform for the chat messages
    public Button selectAccessoryButton; // Reference to the send button in the Inspector

    

    // Start is called before the first frame update
    void Start()
    {

        // Attach an event handler to the send button's click event
        selectAccessoryButton.onClick.AddListener(InstantiateAccessory);
    }

    public void InstantiateAccessory()
    {        
            // Create a new chat message GameObject
            Instantiate(AccessoryPrefab, AccessoryPanel);
    }



    // Update is called once per frame
    void Update()
    {

    }
}

