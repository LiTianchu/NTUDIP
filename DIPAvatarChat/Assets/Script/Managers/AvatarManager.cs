using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Firestore;
using TMPro;

public class AvatarManager : Singleton<AvatarManager>
{
    public Dictionary<string, AvatarData> EmailToAvatarDict { get; set; }
    public GameObject avatarPrefab;

    //path for files
    public readonly string AVATAR_BODY_FILE_PATH = "Blender/Base_Avatar"; //"Blender/CatBaseTest2_v0_30";
    public readonly string CUSTOMISE_AVATAR_BODY_PATH = "/Canvas/AvatarContainer/Avatar";
    public readonly string AVATAR_HAT_PATH = "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End";
    public readonly string AVATAR_ARM_PATH = "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm";
    public readonly string AVATAR_SHOE_L_PATH = "Character_Rig/mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot/Empty_R_Shoes";
    public readonly string AVATAR_SHOE_R_PATH = "Character_Rig/mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot/Empty_L_Shoes";

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
    public readonly Vector3 SHOES_SCALE = new Vector3(0.011f, 0.011f, 0.011f);
    public readonly Quaternion SHOES_ROTATION = Quaternion.Euler(180f, 90f, 90f);

    public readonly Vector3 TEXT_BUBBLE_POS = new Vector3(1, 4.25f, 1.8f);

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

    public Dictionary<string, string> colourTo2dColourMap = new Dictionary<string, string>
    {
        { "Blender/Textures/ColorSchemes/Materials/1", "2D_assets/catcolor"},
        { "Blender/Textures/ColorSchemes/Materials/2", "2D_assets/catpurplecolor"},
        { "Blender/Textures/ColorSchemes/Materials/3", "2D_assets/catbluecolor"},
        { "Blender/Textures/ColorSchemes/Materials/4", "2D_assets/catpinkcolor"},
        { "Blender/Textures/ColorSchemes/Materials/5", "2D_assets/catorangecolor"},
        { "Blender/Textures/ColorSchemes/Materials/6", "2D_assets/catgreencolor"},
        { "Blender/Textures/ColorSchemes/Materials/7", "2D_assets/catredcolor"},
    };

    public Dictionary<string, string> dogColourTo2dColourMap = new Dictionary<string, string>
    {
        { "Blender/Textures/ColorSchemes/Materials/1", "2D_assets/dogcolor"},
        { "Blender/Textures/ColorSchemes/Materials/2", "2D_assets/dogpurplecolor"},
        { "Blender/Textures/ColorSchemes/Materials/3", "2D_assets/dogbluecolor"},
        { "Blender/Textures/ColorSchemes/Materials/4", "2D_assets/dogpinkcolor"},
        { "Blender/Textures/ColorSchemes/Materials/5", "2D_assets/dogorangecolor"},
        { "Blender/Textures/ColorSchemes/Materials/6", "2D_assets/doggreencolor"},
        { "Blender/Textures/ColorSchemes/Materials/7", "2D_assets/dogredcolor"},
    };

    public Dictionary<string, string> textureTo2dTextureMap = new Dictionary<string, string>
    {
        { "Blender/Textures/scars", "2D_assets/scars"},
        { "Blender/Textures/spots", "2D_assets/spots"},
        { "Blender/Textures/stripes", "2D_assets/stripes"},
    };

    public Dictionary<string, string> bodyPartToBodyPartPathMap = new Dictionary<string, string>
    {
        { "Blender/cattail", "Character_Rig/mixamorig:Hips/cattail"},
        { "Blender/dogtail", "Character_Rig/mixamorig:Hips/dogtail"},
        { "Blender/catear", "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/catear"},
        { "Blender/dogear", "Character_Rig/mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/dogear"},
    };

    void Start()
    {
        EmailToAvatarDict = new Dictionary<string, AvatarData>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Sprite> LoadAvatarSprite2d(string headFilePath, string skinFilePath, string hatFilePath, string textureFilePath, AvatarData avatarData)
    {
        List<Sprite> sprites = new List<Sprite>();

        Sprite head2d = Resources.Load<Sprite>(headFilePath);
        Sprite skin2d = null;
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

        switch (avatarData.ears)
        {
            case "Blender/catear":
                skin2d = Find2d(skin2d, skinFilePath, colourTo2dColourMap);
                break;
            case "Blender/dogear":
                skin2d = Find2d(skin2d, skinFilePath, dogColourTo2dColourMap);
                break;
            default:
                Debug.Log("invalid ear type: " + avatarData.ears);
                break;
        }

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

        List<Sprite> sprites = null;

        switch (avatarData.ears)
        {
            case "Blender/catear":
                sprites = LoadAvatarSprite2d("2D_assets/catbase", avatarData.colour, avatarData.hat, avatarData.texture, avatarData);
                break;
            case "Blender/dogear":
                sprites = LoadAvatarSprite2d("2D_assets/dogbase", avatarData.colour, avatarData.hat, avatarData.texture, avatarData);
                break;
            default:
                Debug.Log("invalid ear type: " + avatarData.ears);
                break;
        }

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
        GameObject shoesParentL = avatar.transform.Find(AVATAR_SHOE_L_PATH).gameObject;
        GameObject shoesParentR = avatar.transform.Find(AVATAR_SHOE_R_PATH).gameObject;

        // Load hat accessory
        LoadAccessory(avatarData.hat, avatar, HAT_POS, HAT_SCALE, HAT_ROTATION, hatParent, "HatAccessory");

        // Load arm accessory
        LoadAccessory(avatarData.arm, avatar, ARM_POS, ARM_SCALE, ARM_ROTATION, armParent, "ArmAccessory");

        // Load shoes accessory
        LoadAccessory(avatarData.shoes + "left", avatar, SHOES_POS, SHOES_SCALE, SHOES_ROTATION, shoesParentL, "ShoesAccessory");
        LoadAccessory(avatarData.shoes + "right", avatar, SHOES_POS, SHOES_SCALE, SHOES_ROTATION, shoesParentR, "ShoesAccessory");

        // Load ears and tail
        LoadBodyParts(avatarData.ears, avatarData.tail, avatar);

        LoadTexture(avatarData.texture, avatarData.colour, avatar);

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

    public GameObject LoadAvatarBody(GameObject prefab)
    {
        if (prefab != null)
        {
            GameObject fbx = Instantiate(prefab);
            BoxCollider collider = fbx.AddComponent<BoxCollider>(); //add collider to body to detect raycast
            collider.size = AVATAR_COLLIDER_SIZE;
            collider.center = AVATAR_COLLIDER_CENTER;

            // Check if the GameObject already has an Animator component
            Animator existingAnimator = fbx.GetComponent<Animator>();

            if (existingAnimator == null)
            {
                // If not, add a new Animator component
                fbx.AddComponent<Animator>();
            }
            else
            {
                // If an Animator component already exists, log a warning or handle it as needed
                Debug.LogWarning("Animator component already exists on the GameObject: " + fbx.name);
            }

            Animator animator = fbx.GetComponent<Animator>();
            animator.runtimeAnimatorController = AnimationManager.Instance.animatorController;
            return fbx;
        }

        return null;
    }


    public void LoadAccessory(string fbxFileName, GameObject AvatarBody, Vector3 itemPosition, Vector3 itemScale, Quaternion itemRotation, GameObject AccessoryParent, string tag)
    {
        if (fbxFileName != null && fbxFileName != "")
        {
            // Load the FBX asset from the Resources folder
            Debug.Log("Accessory Loaded: " + fbxFileName);
            GameObject loadedFBX = Resources.Load<GameObject>(fbxFileName); // Eg. Blender/porkpiehat.fbx
            if (loadedFBX != null)
            {
                // Instantiate the loaded FBX as a GameObject in the scene
                GameObject fbx = Instantiate(loadedFBX, itemPosition, itemRotation);
                fbx.transform.SetParent(AvatarBody.transform, false);
                fbx.transform.localScale = itemScale;
                fbx.tag = tag;

                // Manually rescale certain broken accessories
                ///////////////////////////////////////////////////////////////////////////////////////////
                // shoes not correct size and rotation
                if (fbxFileName == "Blender/Shoes/businessshoesleft" || fbxFileName == "Blender/Shoes/businessshoesright" || fbxFileName == "Blender/Shoes/laceupbootsleft" || fbxFileName == "Blender/Shoes/laceupbootsright")
                {
                    fbx = Instantiate(loadedFBX, itemPosition, Quaternion.Euler(0f, 0f, 180f));
                    fbx.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                }

                // porkpiehat not correct size
                if (fbxFileName == "Blender/porkpiehat")
                {
                    fbx.transform.localScale = new Vector3(0.004f, 0.004f, 0.004f);
                }

                // beret not correct size
                if (fbxFileName == "Blender/beret")
                {

                }
                ///////////////////////////////////////////////////////////////////////////////////////////


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
                Debug.LogWarning("FBX asset not found: " + fbxFileName);
            }
        }
    }


    public void LoadTexture(string textureName, string colourName, GameObject AvatarBody)
    {
        if (colourName != null && colourName != "")
        {
            // Load the FBX asset from the Resources folder
            // Eg. Blender/Materials/spots/Materials/body.fbx

            string BODY_COLOUR_PATH = colourName + "/bodycolour";
            string HEAD_COLOUR_PATH = colourName + "/headcolour";
            string TAIL_COLOUR_PATH = colourName + "/tailcolour";
            string DOG_TAIL_COLOUR_PATH = colourName + "/dogtailcolour";

            AddMaterial(BODY_COLOUR_PATH, "Body", "SkinnedMeshRenderer", true);
            AddMaterial(HEAD_COLOUR_PATH, "Head_Base", "SkinnedMeshRenderer", true);
            AddMaterial(TAIL_COLOUR_PATH, "Character_Rig/mixamorig:Hips/cattail", "MeshRenderer", true);
            AddMaterial(DOG_TAIL_COLOUR_PATH, "Character_Rig/mixamorig:Hips/dogtail", "MeshRenderer", true);

            if (textureName != null && textureName != "")
            {
                string BODY_MATERIAL_PATH = textureName + "/Materials/body";
                string HEAD_MATERIAL_PATH = textureName + "/Materials/head";
                string TAIL_MATERIAL_PATH = textureName + "/Materials/tail";

                AddMaterial(BODY_MATERIAL_PATH, "Body", "SkinnedMeshRenderer", false);
                AddMaterial(HEAD_MATERIAL_PATH, "Head_Base", "SkinnedMeshRenderer", false);
                AddMaterial(TAIL_MATERIAL_PATH, "Character_Rig/mixamorig:Hips/cattail", "MeshRenderer", false);
                AddMaterial(TAIL_MATERIAL_PATH, "Character_Rig/mixamorig:Hips/dogtail", "MeshRenderer", false);
            }
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
                if (SMR == null) { return; }

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
                if (MR == null) { return; }

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

    public void LoadBodyParts(string earType, string tailType, GameObject avatar)
    {
        if (earType == null || tailType == null) { return; }

        foreach (var kvp in bodyPartToBodyPartPathMap)
        {
            if (kvp.Key != earType && kvp.Key != tailType)
            {
                Destroy(avatar.transform.Find(kvp.Value).gameObject);
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

    public async void SetNametag(GameObject avatarObj, GameObject nametagPrefab, string email, Vector3 pos)
    {
        // Spawn the text bubble prefab and set its position to follow the avatar
        GameObject textBubble = Instantiate(nametagPrefab, avatarObj.transform);
        textBubble.transform.localPosition = pos; // Adjust position as needed
        textBubble.transform.rotation = Quaternion.identity;

        DocumentSnapshot userDoc = await UserBackendManager.Instance.GetUserByEmailTask(email);
        UserData userData = userDoc.ConvertTo<UserData>();

        textBubble.GetComponentInChildren<TMP_Text>().text = userData.username;
    }
}