using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//same as singleton, just that it does not persist through out scenes
public class PageSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    //load singleton into instance
                    _instance = new GameObject().AddComponent<T>();
                }
                else
                {
                }
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        //if there is already a instance, destroy
        if (_instance != null)
        {
            Destroy(this);

        }
        else
        {
            _instance = this as T;
        }


    }

}
