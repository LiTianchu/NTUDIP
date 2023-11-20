using UnityEngine;
using Firebase.Firestore;

public class DisplayPicture : MonoBehaviour
{
    public GameObject Box;
    public GameObject AvatarSkinDisplayArea;
    public GameObject AvatarHeadDisplayArea;
    public GameObject AvatarHatDisplayArea;
    public GameObject AvatarTextureDisplayArea;
    public bool isConv = false;

    // Start is called before the first frame update
    void Start()
    {
        DisplayFriendAvatar2d();
    }

    public async void DisplayFriendAvatar2d()
    {
        //display 2d avatar
        DocumentSnapshot snapshot = isConv ? await AvatarBackendManager.Instance.GetAvatarByConversationIdTask(Box.name) : await AvatarBackendManager.Instance.GetAvatarByEmailTask(Box.name);


        AvatarManager.Instance.DisplayFriendAvatar2d(snapshot, AvatarHeadDisplayArea, AvatarSkinDisplayArea, AvatarHatDisplayArea, AvatarTextureDisplayArea);
    }
}
