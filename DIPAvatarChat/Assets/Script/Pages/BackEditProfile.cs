using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackEditProfile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Back()
    {
        AppManager.Instance.LoadScene("3-RegisterUsername");
    }
}
