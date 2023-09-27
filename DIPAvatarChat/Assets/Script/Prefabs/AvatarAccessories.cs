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

    private GameObject previousClone;
    

    // Start is called before the first frame update
    void Start()
    {
        // Attach an event handler to the send button's click event
        selectAccessoryButton.onClick.AddListener(InstantiateAccessory);
    }

    public void InstantiateAccessory()
    {   
        Debug.Log("InstantiateAccessory called");

        // Destroy the previous clone if it exists
        if (previousClone != null)
        {
            Debug.Log("Destroying previousClone");
            Destroy(previousClone);
        }

        // Create a new chat message GameObject
        GameObject newClone = Instantiate(AccessoryPrefab, AccessoryPanel);

        // Assign the newClone to the previousClone variable
        previousClone = newClone;

    }



    // Update is called once per frame
    void Update()
    {

    }
}

