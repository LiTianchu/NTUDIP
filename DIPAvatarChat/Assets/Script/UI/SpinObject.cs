using UnityEngine;

public class SpinObject : MonoBehaviour
{
    float rotationSpeed = 20f; // Increased speed at which the object spins (in degrees per second)

    void Update()
    {
        // Rotate the object around its up axis (Y-axis)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
