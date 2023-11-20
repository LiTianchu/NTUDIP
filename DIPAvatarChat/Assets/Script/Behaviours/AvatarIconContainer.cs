using UnityEngine;
using UnityEngine.UI;

public class AvatarIconContainer : MonoBehaviour
{
    private Avatar _avatar;
    private Image _img;

    public Avatar AttachedAvatar { set { _avatar = value; } }
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
    public void OnImgButtonDown()
    {
        Transform parent = transform.parent;
        foreach (AvatarIconContainer sibling in parent.GetComponentsInChildren<AvatarIconContainer>())
        {
            sibling.GetComponent<Image>().enabled = false;

        }
        _img.enabled = true;
        ARChat.Instance.SelectedAvatar = this._avatar;
        GameObject.Find("AR_Session").GetComponent<ARChat>().ClearChatDisplay();
    }

    public void ToggleARDefaultPlane()
    {
        GameObject.Find("AR_Session").GetComponent<ARChat>().SetTrackablesVisibility(true);
    }

}
