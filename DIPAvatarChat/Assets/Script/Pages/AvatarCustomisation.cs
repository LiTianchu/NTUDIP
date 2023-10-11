using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AvatarCustomisation : MonoBehaviour
{
  public GameObject FeatureCustomisation;

  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void LoadEditProfile()
  {
    AppManager.Instance.LoadScene("3-EditProfile");
  }

  public void DisplayPanel(string chooseFeatureName)
  {
    foreach (Transform child in FeatureCustomisation.transform)
    {
      Debug.Log("Child GameObject: " + child.gameObject.name);
      if (child.gameObject.name == chooseFeatureName)
      {
        child.gameObject.SetActive(true);
      }
      else
      {
        if (child.gameObject.name != "chooseFeature")
        {
          child.gameObject.SetActive(false);
        }
      }
    }
  }

  public async void SaveAvatarData()
  {
    // Call the AvatarBackendManager to update the avatar data
    bool updated = await AvatarBackendManager.Instance.UpdateAvatarData();

    if (updated)
    {
      Debug.Log("Avatar data updated successfully!");
    }
    else
    {
      Debug.LogError("Failed to update avatar data.");
    }
  }
}
