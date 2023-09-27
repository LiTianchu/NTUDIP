using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : Singleton<AppManager>
{
    public void LoadScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName("YourSceneName").isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            Debug.Log("Loaded Scene " + sceneName);
        }
    }

    public Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
}
