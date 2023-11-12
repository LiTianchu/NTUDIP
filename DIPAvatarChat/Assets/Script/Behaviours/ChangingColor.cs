using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ChangingColor : MonoBehaviour
{
    [SerializeField]
    private float colorChangingSpeed = 5f;
    private Image image;

    private float h;
    private float s;
    private float v;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        Color.RGBToHSV(image.color, out h, out s, out v);
    }

    // Update is called once per frame
    void Update()
    {
        
        h += colorChangingSpeed * Time.deltaTime;
        if (h > 360)
        {
            h = h - 360.0f;
        }
        image.color = Color.HSVToRGB(h/360, s, v);
    }
}
