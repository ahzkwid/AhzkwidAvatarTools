




#if UNITY_EDITOR
using UnityEngine;


//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class MaterialPathRepairTool : EditorWindow
{
    public DefaultAsset materialFolder;
    public DefaultAsset[] materialFolders;
    public DefaultAsset parentFolder;
    public DefaultAsset textureFolderFrom;
    public DefaultAsset textureFolder;
    public DefaultAsset folder;
    public DefaultAsset[] removefolders;
    //public bool createBackup = true;

    public enum Option
    {
        Simple , DuelPath, ParentOnly //, TripplePath
    }
    public Option option;

    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(MaterialPathRepairTool))]
    public static void Init()
    {
        var window = GetWindow<MaterialPathRepairTool>(utility: false, title: nameof(MaterialPathRepairTool));
        window.minSize = new Vector2(300, 200);
        //window.maxSize = window.minSize;
        window.Show();
    }
    static Material[] GetFolderToMaterials(DefaultAsset folder)
    {
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var filePaths = AssetDatabase.FindAssets($"t:{typeof(Material).Name}", new string[] { folderPath });
        var assets = new Material[filePaths.Length];
        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(filePaths[i]));
        }
        return assets;
    }
    static Texture[] GetFolderToTextures(DefaultAsset folder)
    {
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var filePaths = AssetDatabase.FindAssets($"t:{typeof(Texture).Name}", new string[] { folderPath });
        var assets = new Texture[filePaths.Length];
        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(filePaths[i]));
        }
        return assets;
    }

    public static void ReplaceParent(DefaultAsset materialFolder, DefaultAsset parentFolder)
    {
        var materials = GetFolderToMaterials(materialFolder);
        Debug.Log($"materials.Length:{materials.Length}");

        var parents = GetFolderToMaterials(parentFolder);

        var materialFolderPath = AssetDatabase.GetAssetPath(materialFolder);


        var parentFolderPath = AssetDatabase.GetAssetPath(parentFolder);


        var paths = System.Array.ConvertAll(parents, material => AssetDatabase.GetAssetPath(material));
        paths = System.Array.ConvertAll(paths, path => path.Substring(parentFolderPath.Length, path.Length - parentFolderPath.Length));

        foreach (var material in materials)
        {
            if (material.parent == null)
            {
                continue;
            }
            var path = AssetDatabase.GetAssetPath(material.parent);


            if (path.StartsWith(materialFolderPath + "/"))
            {
                //정상
                continue;
            }



            var parentIndex = System.Array.FindIndex(paths, parentPath =>
            {
                if (path.Length <= parentPath.Length)//경로가 parent경로보다 짧으면 비교 불가
                {
                    return false;
                }
                return path.Substring(path.Length - parentPath.Length) == parentPath;//마지막 문자열이 같은지
            });

            if (parentIndex >= 0)
            {
                var parent = parents[parentIndex];
                var parentPath = AssetDatabase.GetAssetPath(parent);
                Debug.Log($"{path}->{parentPath}");
                material.parent = parent;
            }

            if (material.parent == null)
            {
                continue;
            }

            if (parentIndex < 0)
            {
                Debug.LogWarning($"{path}->Fail");
            }

        }
    }


    public static void ReplaceTextures(DefaultAsset materialFolder, DefaultAsset textureToFolder, DefaultAsset[] removefolders)
    {
        ReplaceTextures(materialFolder, null,textureToFolder, removefolders);
    }
    public static void ReplaceTextures(DefaultAsset materialFolder, DefaultAsset textureFromFolder, DefaultAsset textureToFolder, DefaultAsset[] removefolders)
    {
        var removefolderPaths = System.Array.ConvertAll(removefolders, removefolder=>AssetDatabase.GetAssetPath(removefolder));




        //var materialFolderPath = AssetDatabase.GetAssetPath(materialFolder);
        //var textureFolderFromPath = AssetDatabase.GetAssetPath(textureFromFolder);
        var textureFolderToPath = AssetDatabase.GetAssetPath(textureToFolder);
        var materials = GetFolderToMaterials(materialFolder);
        Debug.Log($"materials.Length:{materials.Length}");


        //var texturesFrom = GetFolderToTextures(textureFromFolder);
        //Debug.Log($"textures.Length:{texturesFrom.Length}");

        var texturesTo = GetFolderToTextures(textureToFolder);
        Debug.Log($"textures.Length:{texturesTo.Length}");


        var paths = System.Array.ConvertAll(texturesTo, texture => AssetDatabase.GetAssetPath(texture));
         paths = System.Array.ConvertAll(paths, path => path.Substring(textureFolderToPath.Length, path.Length- textureFolderToPath.Length));


        foreach (var material in materials)
        {
            var properties = MaterialEditor.GetMaterialProperties(new Material[] { material });
            //Debug.Log($"properties.Length:{properties.Length}");
            //Debug.Log($"properties:{properties.Length}");
            properties = System.Array.FindAll(properties, property => property.textureValue != null);
            //Debug.Log($"properties:{properties.Length}");

            foreach (var property in properties)
            {
                var path = AssetDatabase.GetAssetPath(property.textureValue);
                /*
                if (path.Length >= textureFolderToPath.Length)
                {
                    if (path.Substring(0, textureFolderToPath.Length) == textureFolderToPath)
                    {
                        //Debug.Log($"{path}->정상");
                        continue;
                    }
                }
                */
                if (path.StartsWith(textureFolderToPath+"/"))
                {
                    //정상
                    continue;
                }


                var textureIndex = System.Array.FindIndex(paths, x =>
                {
                    if (path.Length <= x.Length)//경로가 대상경로보다 짧으면 비교 불가
                    {
                        return false;
                    }
                    return path.Substring(path.Length - x.Length) == x;//마지막 문자열이 같은지
                });

                if (textureIndex >= 0)
                {
                    var texture = texturesTo[textureIndex];
                    var texturePath = AssetDatabase.GetAssetPath(texture);
                    Debug.Log($"{path}->{texturePath}");
                    property.textureValue = texture;
                }


                path = AssetDatabase.GetAssetPath(property.textureValue);
                foreach (var removefolderPath in removefolderPaths)
                {
                    if (path.Length >= removefolderPath.Length)
                    {
                        if (path.Substring(0, removefolderPath.Length) == removefolderPath)
                        {
                            property.textureValue = null;
                            Debug.Log($"{path}->Delete");
                            continue;
                        }
                    }
                }
                if (property.textureValue==null)
                {
                    continue;
                }

                if (textureIndex < 0)
                {
                    Debug.LogWarning($"{path}->Fail");
                }

            }
        }

    }
    SerializedObject serializedObject;
    void OnGUI()
    {
        if (serializedObject == null)
        {
            serializedObject = new SerializedObject(this);
        }



        var allReady = true;


        serializedObject.Update();
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(option)));
            EditorGUILayout.Space();
            switch (option)
            {
                case Option.Simple:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(folder)));
                    materialFolder = folder;
                    if (folder == null)
                    {
                        allReady = false;
                    }
                    break;
                case Option.DuelPath:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materialFolder)));
                    folder = materialFolder;
                    if (materialFolder == null)
                    {
                        allReady = false;
                    }
                    //if (option==Option.TripplePath)
                    //{
                    //    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(textureFolderFrom)));
                    //    if (textureFolderFrom == null)
                    //    {
                    //        allReady = false;
                    //    }
                    //}
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(textureFolder)));
                    if (textureFolder == null)
                    {
                        allReady = false;
                    }
                    break;
                case Option.ParentOnly:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materialFolders)));
                    if (GUILayout.Button("ResetMaterialFolders"))
                    {
                        materialFolders = null;
                    }
                    if ((materialFolders == null)||(materialFolders.Length==0))
                    {
                        allReady = false;
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(parentFolder)));
                    if (parentFolder == null)
                    {
                        allReady = false;
                    }
                    break;
                //case Option.TripplePath:
                //    break;
                default:
                    break;
            }
            switch (option) 
            {
                case Option.Simple:
                case Option.DuelPath:
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(removefolders)));
                    break;
                case Option.ParentOnly:
                    break;
                default:
                    break;
            }
            EditorGUILayout.Space();
            //if (textureFolder == null)
            {
                //allReady = false;
            }
            if (allReady)
            {
                //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
            }
        }
        serializedObject.ApplyModifiedProperties();


        GUI.enabled = allReady;
        if (GUILayout.Button("수리"))
        {
            switch (option)
            {
                case Option.Simple:
                    ReplaceParent(folder, folder);
                    ReplaceTextures(folder, folder, removefolders);
                    break;
                case Option.DuelPath:
                    ReplaceParent(materialFolder, materialFolder);
                    ReplaceTextures(materialFolder, textureFolder, removefolders);
                    break;
                case Option.ParentOnly:
                    foreach (var materialFolder in materialFolders)
                    {
                        ReplaceParent(materialFolder, parentFolder);
                    }
                    break;
                //case Option.TripplePath:
                //ReplaceTextures(materialFolder, textureFolderFrom,textureFolderTo, removefolders);
                //break;
                default:
                    break;
            }
            /*
            if (createBackup)
            {
                var characterCopy = Instantiate(character);
                characterCopy.name = character.name+" (Backup)";
                characterCopy.SetActive(false);
            }
            ShapekeyCopy(character, shapekey);
            */
        }
        GUI.enabled = true;
    }

    public void ShapekeyCopy(GameObject character, GameObject shapekey)
    {
        var characterRenderers = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        var shapekeyRenderers = shapekey.GetComponentsInChildren<SkinnedMeshRenderer>();



        foreach (var characterRenderer in characterRenderers)
        {
            var shapekeyRenderer = System.Array.Find(shapekeyRenderers, x => x.sharedMesh == characterRenderer.sharedMesh);
            if (shapekeyRenderer == null)
            {
                continue;
            }    
            for (int i = 0; i < characterRenderer.sharedMesh.blendShapeCount; i++)
            {
                
                characterRenderer.SetBlendShapeWeight(i, shapekeyRenderer.GetBlendShapeWeight(i));
            }
        }

    }
}
#endif