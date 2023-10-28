using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarAccessories : MonoBehaviour
{
    public GameObject AccessoryPrefab;
    public GameObject AvatarBody;
    public Transform AccessoryPanel;
    public Button selectAccessoryButton;
    public string accessoryType;

    //private GameObject previousClone;
    //private int cloneCount = 0;
    //private Coroutine instantiationCoroutine;

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
        string path = "Blender/" + AccessoryPrefab.name;

        /*foreach (Transform child in AccessoryPanel.transform)
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
        }*/

        switch (accessoryType)
        {
            case "colour":

                break;
            case "texture":

                break;
            case "expression":

                break;
            case "hat":
                // Load hat accessory

                break;
            case "arm":
                // Load arm accessory

                break;
            case "wings":

                break;
            case "tail":

                break;
            case "shoes":
                // Load shoes accessory

                break;
        }
    }

    public void UpdateAvatarData(bool isActive)
    {
        Debug.Log("Accessory Type: " + accessoryType + ", Accessory Name: " + AccessoryPrefab.name);
        Debug.Log("Accessory is active: " + isActive);

        string path = "Blender/" + AccessoryPrefab.name;

        if (AvatarBackendManager.Instance.currAvatarData != null)
        {
            if (isActive)
            {
                switch (accessoryType)
                {
                    case "colour":
                        AvatarBackendManager.Instance.currAvatarData.colour = path;
                        break;
                    case "texture":
                        AvatarBackendManager.Instance.currAvatarData.texture = path;
                        break;
                    case "expression":
                        AvatarBackendManager.Instance.currAvatarData.expression = path;
                        break;
                    case "hat":
                        AvatarBackendManager.Instance.currAvatarData.hat = path;
                        break;
                    case "arm":
                        AvatarBackendManager.Instance.currAvatarData.arm = path;
                        break;
                    case "wings":
                        AvatarBackendManager.Instance.currAvatarData.wings = path;
                        break;
                    case "tail":
                        AvatarBackendManager.Instance.currAvatarData.tail = path;
                        break;
                    case "shoes":
                        AvatarBackendManager.Instance.currAvatarData.shoes = path;
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
                    case "shoes":
                        AvatarBackendManager.Instance.currAvatarData.shoes = null;
                        break;
                }
            }
        }
    }

    public void ClearAccessoryType(GameObject[] AccessoryType, string accessoryType)
    {
        AccessoryType = GameObject.FindGameObjectsWithTag(accessoryType);

        foreach (GameObject accessory in AccessoryType)
        {
            Destroy(accessory);
        }
    }
}