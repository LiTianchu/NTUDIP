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

public class StartingPage : MonoBehaviour
{
  public GameObject LoadingUI;

  // Start is called before the first frame update
  void Start()
  {
    // Auto log in if session is stored
    LoadSession();
  }

  // Update is called once per frame
  void Update()
  {

  }

  void LoadSession()
  {
    StartCoroutine(AuthManager.Instance.LoadSession(0.25f, LoadingUI));
  }
}
