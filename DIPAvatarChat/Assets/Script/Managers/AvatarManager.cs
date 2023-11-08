using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;

public class AvatarManager : Singleton<AvatarManager>
{
    public Dictionary<string, AvatarData> EmailToAvatarDict { get; set; }
    public GameObject avatarPrefab;

    //path for files
    public readonly string AVATAR_BODY_FILE_PATH = "Blender/Base_Avatar"; //"Blender/CatBaseTest2_v0_30";
    public readonly string CUSTOMISE_AVATAR_BODY_PATH = "/Canvas/AvatarContainer/Avatar";
    public readonly string AVATAR_HAT_PATH = "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End";
    public readonly string AVATAR_ARM_PATH = "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm";

    //rotation for avatar spawn
    public readonly Vector3 AVATAR_COLLIDER_SIZE = new Vector3(2f, 4f, 2f);
    public readonly Vector3 AVATAR_COLLIDER_CENTER = new Vector3(0f, 2f, 0f);

    //pos for hat accessories
    public readonly Vector3 HAT_POS = new Vector3(-0.0005419354f, -0.0004f, 0.0008415249f);
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

    public Dictionary<string, string> textureTo2dTextureMap = new Dictionary<string, string>
    {
        { "Blender/Textures/scars", "2D_assets/scars"},
        { "Blender/Textures/spots", "2D_assets/spots"},
        { "Blender/Textures/stripes", "2D_assets/stripes"},
    };

    void Start()
    {
        EmailToAvatarDict = new Dictionary<string, AvatarData>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Sprite> LoadAvatarSprite2d(string headFilePath, string skinFilePath, string hatFilePath, string textureFilePath)
    {
        List<Sprite> sprites = new List<Sprite>();

        Sprite head2d = Resources.Load<Sprite>(headFilePath);
        Sprite skin2d = Resources.Load<Sprite>(skinFilePath);
        Sprite hat2d = null;
        Sprite texture2d = null;

        /*if (hatFilePath != null && hatFilePath != "")
        {
            foreach (var kvp in hatTo2dHatMap)
            {
                if (hatFilePath.Contains(kvp.Key))
                {
                    Debug.Log("2d Hat file path: " + kvp.Value);
                    hat2d = Resources.Load<Sprite>(kvp.Value);
                }
            }
        }*/

        hat2d = Find2d(hat2d, hatFilePath, hatTo2dHatMap);
        texture2d = Find2d(texture2d, textureFilePath, textureTo2dTextureMap);

        sprites.Add(skin2d);
        sprites.Add(head2d);
        sprites.Add(hat2d);
        sprites.Add(texture2d);

        return sprites;

        Sprite Find2d(Sprite sprite2d, string filePath, Dictionary<string, string> Map2d)
        {
            if (filePath != null && filePath != "")
            {
                foreach (var kvp in Map2d)
                {
                    if (filePath.Contains(kvp.Key))
                    {
                        Debug.Log("2d Hat file path: " + kvp.Value);
                        sprite2d = Resources.Load<Sprite>(kvp.Value);
                        return sprite2d;
                    }
                }
            }
            return null;
        }
    }

    public async void DisplayFriendAvatar2d(DocumentSnapshot snapshot, GameObject AvatarHeadDisplayArea, GameObject AvatarSkinDisplayArea, GameObject AvatarHatDisplayArea, GameObject AvatarTextureDisplayArea)
    {
        if (snapshot == null)
        {
            Debug.Log("Friend Avatar Data Not Found");

            return;
        }
        AvatarData avatarData = snapshot.ConvertTo<AvatarData>();



        List<Sprite> sprites = LoadAvatarSprite2d("2D_assets/catbase", "2D_assets/catcolor", avatarData.hat, avatarData.texture);


        Sprite skin2d = sprites[0];
        Sprite head2d = sprites[1];
        Sprite hat2d = sprites[2];
        Sprite texture2d = sprites[3];

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

        AvatarSkinDisplayArea.SetActive(true);
        AvatarHeadDisplayArea.SetActive(true);

        if (hat2d != null)
        {
            // Get the Image component attached to the GameObject
            Image imageComponent = AvatarHatDisplayArea.GetComponent<Image>();

            // Set the sprite
            imageComponent.sprite = hat2d;

            //LoadingUI.SetActive(false);
            AvatarHatDisplayArea.SetActive(true);
        }
        else
        {
            Debug.Log("No hat equipped");
        }

        if (texture2d != null)
        {
            // Get the Image component attached to the GameObject
            Image imageComponent = AvatarTextureDisplayArea.GetComponent<Image>();

            // Set the sprite
            imageComponent.sprite = texture2d;

            //LoadingUI.SetActive(false);
            AvatarTextureDisplayArea.SetActive(true);
        }
        else
        {
            Debug.Log("No texture equipped");
        }
    }

    public GameObject LoadAvatar(AvatarData avatarData, string tag = null)
    {
        GameObject avatar = LoadAvatarBody(avatarPrefab);
        GameObject hatParent = avatar.transform.Find(AVATAR_HAT_PATH).gameObject;
        GameObject armParent = avatar.transform.Find(AVATAR_ARM_PATH).gameObject;
        GameObject shoesParent = null;

        // Load hat accessory
        LoadAccessory(avatarData.hat, avatar, HAT_POS, HAT_SCALE, HAT_ROTATION, hatParent, "HatAccessory");

        // Load arm accessory
        LoadAccessory(avatarData.arm, avatar, ARM_POS, ARM_SCALE, ARM_ROTATION, armParent, "ArmAccessory");

        // Load shoes accessory
        LoadAccessory(avatarData.shoes, avatar, SHOES_POS, SHOES_SCALE, SHOES_ROTATION, shoesParent, "ShoesAccessory");

        LoadTexture(avatarData.texture, avatar);

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

    /*public GameObject LoadAvatarBody(string avatarBaseFbxFileName)
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
    }*/

    public GameObject LoadAvatarBody(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject avatar = Instantiate(prefab);

            if (avatar != null)
            {
                GameObject fbx = Instantiate(avatar);
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
                Debug.LogError("FBX asset not found: ");
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

    public void LoadTexture(string fbxFileName, GameObject AvatarBody)
    {
        if (fbxFileName != null && fbxFileName != "")
        {
            // Load the FBX asset from the Resources folder
            // Eg. Blender/Materials/spots/Materials/body.fbx

            string BODY_MATERIAL_PATH = fbxFileName + "/Materials/body";
            string HEAD_MATERIAL_PATH = fbxFileName + "/Materials/head";
            string TAIL_MATERIAL_PATH = fbxFileName + "/Materials/tail";


            AddMaterial(BODY_MATERIAL_PATH, "Body", "SkinnedMeshRenderer", false);
            AddMaterial(HEAD_MATERIAL_PATH, "Head_Base", "SkinnedMeshRenderer", false);
            AddMaterial(TAIL_MATERIAL_PATH, "Character_Rig/mixamorig:Hips/Cat_Tail", "MeshRenderer", false);

        }
        //add default materials
        string IRIS_MATERIAL_PATH = "Blender/Materials/Eye_Sclera_Mat";
        AddMaterial(IRIS_MATERIAL_PATH, "Eye_L_Sclera", "SkinnedMeshRenderer", true);
        AddMaterial(IRIS_MATERIAL_PATH, "Eye_R_Sclera", "SkinnedMeshRenderer", true);

        void AddMaterial(string MATERIAL_PATH, string bodyPart, string rendererType, bool replaceTexture)
        {
            Material mat = Resources.Load<Material>(MATERIAL_PATH);

            if (mat == null) { return; }
            if (rendererType == "SkinnedMeshRenderer")
            {
                SkinnedMeshRenderer SMR = AvatarBody.transform.Find(bodyPart).gameObject.GetComponent<SkinnedMeshRenderer>();

                if (replaceTexture)
                {
                    SMR.sharedMaterials = new Material[1] { mat };
                }
                else
                {
                    // Clone the current materials array and add the new material to it
                    Material[] mats = new Material[SMR.sharedMaterials.Length + 1];
                    SMR.sharedMaterials.CopyTo(mats, 0);
                    mats[mats.Length - 1] = mat;

                    // Assign the updated materials array to the Skinned Mesh Renderer
                    SMR.sharedMaterials = mats;
                }
                return;
            }

            if (rendererType == "MeshRenderer")
            {
                MeshRenderer MR = AvatarBody.transform.Find(bodyPart).gameObject.GetComponent<MeshRenderer>();

                if (replaceTexture)
                {
                    MR.sharedMaterials = new Material[1] { mat };
                }
                else
                {
                    // Clone the current materials array and add the new material to it
                    Material[] mats = new Material[MR.sharedMaterials.Length + 1];
                    MR.sharedMaterials.CopyTo(mats, 0);
                    mats[mats.Length - 1] = mat;

                    // Assign the updated materials array to the Skinned Mesh Renderer
                    MR.sharedMaterials = mats;
                }
                return;
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
        avatar.AddComponent<SpinObject>();
        SetAvatar("Avatar", avatar, AvatarDisplayArea, AVATAR_POS, Quaternion.Euler(0f, 180f, 0f), AVATAR_SCALE);
    }
}