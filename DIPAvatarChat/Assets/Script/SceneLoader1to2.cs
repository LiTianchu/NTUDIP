using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader1to2 : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("2-MalvinRegisterAndLogin");
    }
}
