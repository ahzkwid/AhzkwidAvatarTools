
using UnityEngine;



#if UNITY_EDITOR


//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class MaterialPathRepairTool : EditorWindow
{
    public Object materialFolder;
    public Object textureFolder;
    public Object folder;
    public Object[] removefolders;
    //public bool createBackup = true;

    public enum Option
    {
        Simple , DuelPath
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
    static Material[] GetFolderToMaterials(Object folder)
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
    static Texture[] GetFolderToTextures(Object folder)
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
    public static void ReplaceTextures(Object materialFolder, Object textureFolder, Object[] removefolders)
    {
        var removefolderPaths = System.Array.ConvertAll(removefolders, removefolder=>AssetDatabase.GetAssetPath(removefolder));




        var materialFolderPath = AssetDatabase.GetAssetPath(materialFolder);
        var textureFolderPath = AssetDatabase.GetAssetPath(textureFolder);
        var materials = GetFolderToMaterials(materialFolder);
        Debug.Log($"materials.Length:{materials.Length}");


        var textures = GetFolderToTextures(textureFolder);
        Debug.Log($"textures.Length:{textures.Length}");


        var paths = System.Array.ConvertAll(textures, texture => AssetDatabase.GetAssetPath(texture));
         paths = System.Array.ConvertAll(paths, path => path.Substring(textureFolderPath.Length, path.Length- textureFolderPath.Length));


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
                if (path.Length >= textureFolderPath.Length)
                {
                    if (path.Substring(0, textureFolderPath.Length) == textureFolderPath)
                    {
                        //Debug.Log($"{path}->정상");
                        continue;
                    }
                }

                var textureIndex = System.Array.FindIndex(paths, x =>
                {
                    if (path.Length <= x.Length)
                    {
                        return false;
                    }
                    return path.Substring(path.Length - x.Length) == x;
                });
                if (textureIndex >= 0)
                {
                    var texture = textures[textureIndex];
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
            if (option == Option.Simple)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(folder)));
                materialFolder = folder;
                if (folder == null)
                {
                    allReady = false;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materialFolder)));
                folder = materialFolder;
                if (materialFolder == null)
                {
                    allReady = false;
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(textureFolder)));
                if (textureFolder == null)
                {
                    allReady = false;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(removefolders)));
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
            if (option==Option.Simple)
            {
                ReplaceTextures(folder, folder, removefolders);
            }
            else
            {
                ReplaceTextures(materialFolder, textureFolder, removefolders);
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