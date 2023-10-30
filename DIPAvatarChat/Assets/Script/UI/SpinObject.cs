using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinObject : MonoBehaviour
{
    float rotationSpeed = 10f; // Speed at which the object spins (in degrees per second)

    void Update()
    {
        // Rotate the object around its up axis (Y-axis)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}
