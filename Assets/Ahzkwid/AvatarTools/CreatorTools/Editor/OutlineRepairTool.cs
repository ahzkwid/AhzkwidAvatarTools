
using UnityEngine;



#if UNITY_EDITOR


//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class OutlineRepairTool : EditorWindow
{
    public Object folder;
    public Object exceptionFolders;
    public float outlineWidth=0.01f;
    //public Object textureFolder;
    //public bool createBackup = true;

    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(OutlineRepairTool))]
    public static void Init()
    {
        var window = GetWindow<OutlineRepairTool>(utility: false, title: nameof(OutlineRepairTool));
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
    public static void ReplaceOutlineWidth(Object folder,float width)
    {
        var materials = GetFolderToMaterials(folder);
        Debug.Log($"materials.Length:{materials.Length}");

        {
            var material = materials[0];
            var properties = MaterialEditor.GetMaterialProperties(new Material[] { material });

            foreach (var property in properties)
            {
                Debug.Log($"property.name:{property.name}");

            }
        }

        foreach (var material in materials)
        {
            var properties = MaterialEditor.GetMaterialProperties(new Material[] { material });
            //Debug.Log($"properties.Length:{properties.Length}");
            //Debug.Log($"properties:{properties.Length}");
            foreach (var property in properties)
            {
                if (property.floatValue != 0)
                {
                    //Debug.Log($"property.name:{property.name}");
                }
                if (property.name != "_OutlineWidth")
                {
                    continue;
                }
                if (property.floatValue == 0)
                {
                    continue;
                }
                Debug.Log($"{material.name}.{property.name}:{property.floatValue}->{width}");
                property.floatValue = width;

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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(folder)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(outlineWidth)));
            EditorGUILayout.Space();
            if (folder == null)
            {
                allReady = false;
            }
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
            ReplaceOutlineWidth(folder, outlineWidth);
        }
        GUI.enabled = true;
    }
}
#endif