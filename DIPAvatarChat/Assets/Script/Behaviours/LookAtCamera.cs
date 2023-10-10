using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System;

public class LookAtCamera : MonoBehaviour
{
    private Camera _mainCam;
    // Start is called before the first frame update
    void Start()
    {
        _mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = _mainCam.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.x,direction.z) * Mathf.Rad2Deg;
        this.transform.rotation = Quaternion.Euler(0, angle, 0);
    }
}
