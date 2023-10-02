using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarAccessories : MonoBehaviour
{
    public GameObject AccessoryPrefab;
    public Transform AccessoryPanel;
    public Button selectAccessoryButton;
    public string accessoryType;

    private GameObject previousClone;
    private int cloneCount = 0;

    private Coroutine instantiationCoroutine;
    private string accessoryPath = "Blender/";

    void Start()
    {
        //selectAccessoryButton.onClick.AddListener(InstantiateAccessory);
    }

    public void InstantiateAccessory()
    {
        Debug.Log("InstantiateAccessory called");

        // Stop any ongoing instantiation coroutine
        if (instantiationCoroutine != null)
        {
            StopCoroutine(instantiationCoroutine);
        }

        // Destroy the previous clone if it exists
        if (previousClone != null)
        {
            Debug.Log("Destroying previousClone");
            Destroy(previousClone);
        }

        // Start a new instantiation coroutine
        instantiationCoroutine = StartCoroutine(InstantiateAccessoryCoroutine());
    }

    public void UpdateAvatarData()
    {
        Debug.Log("Accessory Type: " + accessoryType + ", Accessory Name: " + AccessoryPrefab.name);
        switch (accessoryType)
        {
            case "colour":
                AvatarBackendManager.Instance.currAvatarData.colour = accessoryPath + AccessoryPrefab.name;
                break;
            case "texture":
                AvatarBackendManager.Instance.currAvatarData.texture = accessoryPath + AccessoryPrefab.name;
                break;
            case "expression":
                AvatarBackendManager.Instance.currAvatarData.expression = accessoryPath + AccessoryPrefab.name;
                break;
            case "hat":
                AvatarBackendManager.Instance.currAvatarData.hat = accessoryPath + AccessoryPrefab.name;
                break;
            case "arm":
                AvatarBackendManager.Instance.currAvatarData.arm = accessoryPath + AccessoryPrefab.name;
                break;
            case "wings":
                AvatarBackendManager.Instance.currAvatarData.wings = accessoryPath + AccessoryPrefab.name;
                break;
            case "tail":
                AvatarBackendManager.Instance.currAvatarData.tail = accessoryPath + AccessoryPrefab.name;
                break;
        }
    }

    private IEnumerator InstantiateAccessoryCoroutine()
    {
        // Yield one frame to ensure that the destruction has occurred
        yield return null;

        UpdateAvatarData();

        // Calculate the position for the new accessory clone based on the clone count
        Vector3 clonePosition = new Vector3(0, -cloneCount * 100f, 0); // Adjust the Y position as needed

        // Create a new accessory clone at the calculated position
        GameObject newClone = Instantiate(AccessoryPrefab, clonePosition, Quaternion.identity, AccessoryPanel);

        // Increment the clone count
        cloneCount++;

        // Assign the newClone to the previousClone variable
        previousClone = newClone;

        // Set the instantiation coroutine to null to allow for the next instantiation
        instantiationCoroutine = null;
    }

    void Update()
    {
        // You can add any update logic here if needed
    }
}