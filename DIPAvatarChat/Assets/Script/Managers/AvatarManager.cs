using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class AvatarManager : Singleton<AvatarManager>
{
    public Dictionary<string, AvatarData> EmailToAvatarDict { get; set; }

    //path for files
    public readonly string AVATAR_BODY_FILE_PATH = "Blender/Cat_Base_v3_3"; //"Blender/CatBaseTest2_v0_30";
    public readonly string CUSTOMISE_AVATAR_BODY_PATH = "/Canvas/AvatarContainer/Avatar";
    public readonly string AVATAR_HAT_PATH = "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End";
    public readonly string AVATAR_ARM_PATH = "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm";

    //rotation for avatar spawn
    public readonly Vector3 AVATAR_COLLIDER_SIZE = new Vector3(2f, 4f, 2f);
    public readonly Vector3 AVATAR_COLLIDER_CENTER = new Vector3(0f, 2f, 0f);

    //pos for hat accessories
    public readonly Vector3 HAT_POS = new Vector3(-0.0005419354f, 0.0004996881f, 0.0008415249f);
    public readonly Vector3 HAT_SCALE = new Vector3(0.01f, 0.01f, 0.01f);
    public readonly Quaternion HAT_ROTATION = Quaternion.Euler(0.156f, 180f, 2.777f);

    //pos for arm accessories  
    public readonly Vector3 ARM_POS = new Vector3(0.000391079f, 0.002983337f, -0.0007772499f);
    public readonly Vector3 ARM_SCALE = new Vector3(0.0007f, 0.0007f, 0.0007f);
    public readonly Quaternion ARM_ROTATION = Quaternion.Euler(-181.103f, 51.586f, -96.85901f);

    //pos for shoes accessories
    public readonly Vector3 SHOES_POS = new Vector3(0f, 0f, 0f);
    public readonly Vector3 SHOES_SCALE = new Vector3(1f, 1f, 1f);
    public readonly Quaternion SHOES_ROTATION = Quaternion.Euler(0f, 180f, 0f);

    public Dictionary<string, string> hatTo2dHatMap = new Dictionary<string, string>
    {
        { "Blender/beret", "2D_assets/beret"},
        { "Blender/crown", "2D_assets/crown"},
        { "Blender/horns", "2D_assets/horns"},
        { "Blender/nightcap", "2D_assets/sleepcap"},
        { "Blender/partyhat", "2D_assets/partyhat"},
        { "Blender/porkpiehat", "2D_assets/porkpiehat"},
        { "Blender/starclip", "2D_assets/starclip"},
        { "Blender/strawboater", "2D_assets/strawboater"},
        { "Blender/sunflower", "2D_assets/flowers"},
    };

    void Start()
    {
        EmailToAvatarDict = new Dictionary<string, AvatarData>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Sprite> LoadAvatarSprite2d(string headFilePath, string skinFilePath, string hatFilePath)
    {
        List<Sprite> sprites = new List<Sprite>();

        Sprite head2d = Resources.Load<Sprite>(headFilePath);
        Sprite skin2d = Resources.Load<Sprite>(skinFilePath);
        Sprite hat2d = null;

        if (hatFilePath != null && hatFilePath != "")
        {
            foreach (var kvp in hatTo2dHatMap)
            {
                if (hatFilePath.Contains(kvp.Key))
                {
                    Debug.Log("2d Hat file path: " + kvp.Value);
                    hat2d = Resources.Load<Sprite>(kvp.Value);
                }
            }
        }

        sprites.Add(skin2d);
        sprites.Add(head2d);
        sprites.Add(hat2d);

        return sprites;
    }

    public async void DisplayFriendAvatar2d(DocumentSnapshot snapshot, GameObject AvatarHeadDisplayArea, GameObject AvatarSkinDisplayArea, GameObject AvatarHatDisplayArea)
    {
        AvatarData avatarData = snapshot.ConvertTo<AvatarData>();

        List<Sprite> sprites = LoadAvatarSprite2d("2D_assets/catbase", "2D_assets/catcolor", avatarData.hat);

        Sprite skin2d = sprites[0];
        Sprite head2d = sprites[1];
        Sprite hat2d = sprites[2];

        if (skin2d != null)
        {
            Image imageComponent = AvatarSkinDisplayArea.GetComponent<Image>();
            imageComponent.sprite = skin2d;
        }
        else
        {
            Debug.Log("Skin sprite not found");
        }

        if (head2d != null)
        {
            Image imageComponent = AvatarHeadDisplayArea.GetComponent<Image>();
            imageComponent.sprite = head2d;
        }
        else
        {
            Debug.Log("Head sprite not found");
        }

        if (hat2d != null)
        {
            // Get the Image component attached to the GameObject
            Image imageComponent = AvatarHatDisplayArea.GetComponent<Image>();

            // Set the sprite
            imageComponent.sprite = hat2d;

            //LoadingUI.SetActive(false);
            AvatarHatDisplayArea.SetActive(true);
            AvatarSkinDisplayArea.SetActive(true);
            AvatarHeadDisplayArea.SetActive(true);
        }
        else
        {
            Debug.Log("No hat equipped");

            //LoadingUI.SetActive(false);
            AvatarSkinDisplayArea.SetActive(true);
            AvatarHeadDisplayArea.SetActive(true);
        }
    }

    public GameObject LoadAvatar(AvatarData avatarData, string tag = null)
    {
        GameObject avatar = LoadAvatarBody(AVATAR_BODY_FILE_PATH);
        GameObject hatParent = avatar.transform.Find(AVATAR_HAT_PATH).gameObject;
        GameObject armParent = avatar.transform.Find(AVATAR_ARM_PATH).gameObject;
        GameObject shoesParent = null;

        // Load hat accessory
        LoadAccessory(avatarData.hat, avatar, HAT_POS, HAT_SCALE, HAT_ROTATION, hatParent, "HatAccessory");

        // Load arm accessory
        LoadAccessory(avatarData.arm, avatar, ARM_POS, ARM_SCALE, ARM_ROTATION, armParent, "ArmAccessory");

        // Load shoes accessory
        LoadAccessory(avatarData.shoes, avatar, SHOES_POS, SHOES_SCALE, SHOES_ROTATION, shoesParent, "ShoesAccessory");

        if (tag != null)
        {
            avatar.tag = tag;
        }
        return avatar;
    }

    public GameObject LoadAvatar(string email)
    {
        if (EmailToAvatarDict.ContainsKey(email))
        {
            return LoadAvatar(this.EmailToAvatarDict[email]);
        }
        else
        {
            throw new KeyNotFoundException(email);
        }
    }

    public GameObject LoadAvatarBody(string avatarBaseFbxFileName)
    {
        if (avatarBaseFbxFileName != null && avatarBaseFbxFileName != "")
        {
            GameObject loadedFBX = Resources.Load<GameObject>(avatarBaseFbxFileName); // Eg. Blender/catbasetest.fbx

            if (loadedFBX != null)
            {
                GameObject fbx = Instantiate(loadedFBX);
                BoxCollider collider = fbx.AddComponent<BoxCollider>(); //add collider to body to detect raycast
                collider.size = AVATAR_COLLIDER_SIZE;
                collider.center = AVATAR_COLLIDER_CENTER;

                fbx.AddComponent<Animator>();

                Animator animator = fbx.GetComponent<Animator>();
                animator.runtimeAnimatorController = AnimationManager.Instance.animatorController;
                return fbx;
            }
            else
            {
                Debug.LogError("FBX asset not found: " + avatarBaseFbxFileName);
                return null;
            }
        }
        return null;
    }

    public void LoadAccessory(string fbxFileName, GameObject AvatarBody, Vector3 itemPosition, Vector3 itemScale, Quaternion itemRotation, GameObject AccessoryParent, string tag)
    {
        if (fbxFileName != null && fbxFileName != "")
        {
            // Load the FBX asset from the Resources folder
            GameObject loadedFBX = Resources.Load<GameObject>(fbxFileName); // Eg. Blender/porkpiehat.fbx
            if (loadedFBX != null)
            {
                // Instantiate the loaded FBX as a GameObject in the scene
                GameObject fbx = Instantiate(loadedFBX, itemPosition, itemRotation);

                fbx.transform.SetParent(AvatarBody.transform, false);
                fbx.transform.localScale = itemScale;
                fbx.tag = tag;

                if (AccessoryParent != null)
                {
                    fbx.transform.SetParent(AccessoryParent.transform, false);
                }
                else
                {
                    Debug.Log("Parent not found. ");
                }
            }
            else
            {
                Debug.LogError("FBX asset not found: " + fbxFileName);
            }
        }
    }

    public void SetAvatar(string name, GameObject avatarObj, GameObject avatarParent, Vector3 pos, Quaternion rot, float scale)
    {
        avatarObj.transform.SetParent(avatarParent.transform, false);
        avatarObj.name = name;
        avatarObj.transform.localPosition = pos;
        avatarObj.transform.localRotation = rot;

        avatarObj.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void InitialiseAvatarCustomisation(GameObject AvatarDisplayArea)
    {
        Vector3 AVATAR_POS = new Vector3(0f, -20f, -30f);
        float AVATAR_SCALE = 7.5f;

        GameObject avatar = LoadAvatar(AvatarBackendManager.Instance.currAvatarData, "AvatarCustomise");
        SetAvatar("Avatar", avatar, AvatarDisplayArea, AVATAR_POS, ChatManager.Instance.MY_AVATAR_ROTATION, AVATAR_SCALE);
    }
}