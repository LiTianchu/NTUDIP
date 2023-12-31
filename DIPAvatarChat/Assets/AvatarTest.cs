using UnityEngine;

public class AvatarTest : MonoBehaviour
{
    public string fbxFileName = "Blender/porkpiehat.fbx";

    void Start()
    {
        // Load the FBX asset from the Resources folder
        GameObject loadedFBX = Resources.Load<GameObject>(fbxFileName);

        if (loadedFBX != null)
        {
            // Instantiate the loaded FBX as a GameObject in the scene
            GameObject fbx = Instantiate(loadedFBX, transform.position, Quaternion.identity);
            fbx.transform.localScale = new Vector3(50f, 50f, 50f);
        }
        else
        {
            Debug.LogError("FBX asset not found: " + fbxFileName);
        }
    }
}
