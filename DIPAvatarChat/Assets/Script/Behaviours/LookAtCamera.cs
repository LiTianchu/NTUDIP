using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtDirection = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(lookAtDirection);
    }
}
