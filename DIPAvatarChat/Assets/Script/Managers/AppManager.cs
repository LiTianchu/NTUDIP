using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : Singleton<AppManager>
{
    public void LoadScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            
            Debug.Log("Loaded Scene " + sceneName);
        }
    }

    public void LoadSceneAdditive(string sceneName) {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            Debug.Log("Loaded Scene " + sceneName + " additively");
        }
    }

    public void UnloadScene(string sceneName) {
        if (SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            SceneManager.UnloadSceneAsync(sceneName);
            Debug.Log("Unloaded Scene " + sceneName);
        }
    }

    public Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
}
