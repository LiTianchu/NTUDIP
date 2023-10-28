using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AvatarAccessories : MonoBehaviour
{
    public GameObject AccessoryName;
    public string accessoryType;
    GameObject AvatarDisplayArea;

    void Start()
    {
        AvatarDisplayArea = GameObject.Find("/Canvas/AvatarContainer");
    }

    void Update()
    {
        // You can add any update logic here if needed
    }

    public void InstantiateAccessory()
    {
        Debug.Log("InstantiateAccessory called");
        string path = "Blender/" + AccessoryName.name;

        UpdateAvatarData();

        GameObject avatar = GameObject.Find("/Canvas/AvatarContainer/Avatar");
        if (avatar != null)
        {
            ClearAvatar();
            AvatarManager.Instance.InitialiseAvatarCustomisation(AvatarDisplayArea);
        }
    }

    public void UpdateAvatarData()
    {
        Debug.Log("Accessory Type: " + accessoryType + ", Accessory Name: " + AccessoryPrefab.name);
        string path = null;

        if (AccessoryName.name != "nothing")
        {
            path = "Blender/" + AccessoryName.name;
        }

        if (AvatarBackendManager.Instance.currAvatarData != null)
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
    }

    public void ClearAvatar()
    {
        GameObject[] Avatar = GameObject.FindGameObjectsWithTag("AvatarCustomise");

        foreach (GameObject a in Avatar)
        {
            Destroy(a);
        }
    }
}