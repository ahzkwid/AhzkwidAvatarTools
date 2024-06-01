



#if UNITY_EDITOR

using UnityEngine;

//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class InsertMaterialsTool : EditorWindow
{
    public enum MaterialSelectMode
    {
        Materials, MaterialFolder
    }
    public MaterialSelectMode materialSelectMode;
    public Object materialFolder;
    public Material[] materials;
    public GameObject root;


    public bool repairMode=false;
    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(InsertMaterialTool))]
    public static void Init()
    {
        var window = GetWindow<InsertMaterialsTool>(utility: false, title: nameof(InsertMaterialsTool));
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
    public void InsertMaterials(GameObject root, Material[] materials)
    {
        var renders=root.GetComponentsInChildren<Renderer>();
        foreach (var render in renders)
        {
            if (repairMode)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(render))
                {
                    var originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(render);
                    render.sharedMaterials = originalPrefab.sharedMaterials;
                }
            }
            var sharedMaterials = render.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                var material = System.Array.FindLast(materials, x => x.name == sharedMaterials[i].name);
                if (material != null)
                {
                    sharedMaterials[i] = material;
                }
            }
            render.sharedMaterials = sharedMaterials;

            UnityEditor.EditorUtility.SetDirty(render);
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materialSelectMode)));
            EditorGUILayout.Space();
            if (materialSelectMode == MaterialSelectMode.Materials)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materials)));
                if (materials == null)
                {
                    allReady = false;
                }
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materialFolder)));
                if (materialFolder == null)
                {
                    allReady = false;
                }
            }
            EditorGUILayout.Space();
            GUI.enabled = allReady;
            if (GUILayout.Button("ResetMaterials"))
            {
                materialFolder = null;
                materials = null;
            }
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(root)));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(repairMode)));
            EditorGUILayout.Space();
            
            if (root == null)
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
        if (GUILayout.Button("Insert"))
        {
            if (materialSelectMode==MaterialSelectMode.MaterialFolder)
            {
                materials = GetFolderToMaterials(materialFolder);
            }
            if ((materials != null)&& (materials.Length > 0))
            {
                InsertMaterials(root, materials);
            }
        }
        GUI.enabled = true;
    }

}
#endif