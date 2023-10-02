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

    }

    void Update()
    {
        // You can add any update logic here if needed
    }

    public void InstantiateAccessory()
    {
        Debug.Log("InstantiateAccessory called");
        
        foreach (Transform child in AccessoryPanel.transform)
        {
            Debug.Log("Child GameObject: " + child.gameObject.name);
            if (child.gameObject == AccessoryPrefab)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);

                UpdateAvatarData(child.gameObject.activeSelf);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }

    public void UpdateAvatarData(bool isActive)
    {
        Debug.Log("Accessory Type: " + accessoryType + ", Accessory Name: " + AccessoryPrefab.name);
        Debug.Log("IsActive: " + isActive);

        if (isActive)
        {
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
        else
        {
            switch (accessoryType)
            {
                case "colour":
                    AvatarBackendManager.Instance.currAvatarData.colour = null;
                    break;
                case "texture":
                    AvatarBackendManager.Instance.currAvatarData.texture = null;
                    break;
                case "expression":
                    AvatarBackendManager.Instance.currAvatarData.expression = null;
                    break;
                case "hat":
                    AvatarBackendManager.Instance.currAvatarData.hat = null;
                    break;
                case "arm":
                    AvatarBackendManager.Instance.currAvatarData.arm = null;
                    break;
                case "wings":
                    AvatarBackendManager.Instance.currAvatarData.wings = null;
                    break;
                case "tail":
                    AvatarBackendManager.Instance.currAvatarData.tail = null;
                    break;
            }
        }
    }

    /*private IEnumerator InstantiateAccessoryCoroutine()
    {
        // Yield one frame to ensure that the destruction has occurred
        yield return null;

        //UpdateAvatarData();

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

    private void DisplayAccessory()
    {
        foreach (Transform child in AccessoryPanel.transform)
        {
            Debug.Log("Child GameObject: " + child.gameObject.name);
            if (child.gameObject == AccessoryPrefab)
            {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
    }*/
}