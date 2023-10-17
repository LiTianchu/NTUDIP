using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AvatarIconContainer : MonoBehaviour, IPointerDownHandler
{
    private GameObject _avatar;
    private Image _img;

    public GameObject AttachedAvatar { set { _avatar = value; } }
    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<Image>();
        _img.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Transform parent = transform.parent;
        foreach (AvatarIconContainer sibling in parent.GetComponentsInChildren<AvatarIconContainer>())
        {
            sibling.GetComponent<Image>().enabled = false;

        }
        _img.enabled = true;
       // ARChat.Instance.SelectedAvatar = this._avatar;
    }

}
