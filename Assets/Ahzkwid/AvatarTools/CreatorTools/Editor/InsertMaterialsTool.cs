



#if UNITY_EDITOR

using UnityEngine;

//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[InitializeOnLoad]
class InsertMaterialsTool : EditorWindow
{
    public enum MaterialSelectMode
    {
        Materials, MaterialFolder
    }
    public MaterialSelectMode materialSelectMode;
    public DefaultAsset[] materialFolders;
    public Material[] materials;
    public GameObject root;
    public GameObject[] roots;


    public bool repairMode=false;
    public bool nameMerge = false;
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
    void InsertMaterials()
    {
        if (materialSelectMode == MaterialSelectMode.MaterialFolder)
        {
            if (roots.Length==1)
            {
                for (int i = 0; i < materialFolders.Length; i++)
                {
                    var root = roots.First();
                    var materialFolder = materialFolders[i];
                    InsertMaterials(root, materialFolder);
                }
            }
            else
            {
                for (int i = 0; i < roots.Length; i++)
                {
                    if (i> materialFolders.Length)
                    {
                        continue;
                    }
                    var root = roots[i];
                    var materialFolder = materialFolders[i % roots.Length];
                    InsertMaterials(root, materialFolder);
                }
            }
        }
        else
        {
            InsertMaterials(root, materials);
        }
    }
    void InsertMaterials(GameObject root, DefaultAsset materialFolder)
    {
        materials = GetFolderToMaterials(materialFolder);
        if (nameMerge)
        {
            root.name += $" {materialFolder.name}";
        }
        InsertMaterials(root, materials);
    }
    public void InsertMaterials(GameObject root, Material[] materials)
    {
        if (materials == null)
        {
            Debug.LogWarning("materials == null");
            return;
        }
        if (materials.Length == 0)
        {
            Debug.LogWarning("materials.Length == 0");
            return;
        }
        string path = AssetDatabase.GetAssetPath(root);
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log($"NotAsset");
            var renders = root.GetComponentsInChildren<Renderer>();
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
        else
        {
            Debug.Log($"IsAsset");
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
            {
                Debug.LogWarning("importer == null");
                return;
            }
            var serializedObject = new SerializedObject(importer);
            var materialProperty = serializedObject.FindProperty("m_Materials");
            var materialNames = new List<string>();

            for (int i = 0; i < materialProperty.arraySize; i++)
            {
                var element = materialProperty.GetArrayElementAtIndex(i);
                var materialName = element.FindPropertyRelative("name").stringValue;
                materialNames.Add(materialName);
            }

            foreach (var importerMaterial in importer.GetExternalObjectMap())
            {
                if (importerMaterial.Value==null)
                {
                    continue;
                }
                Debug.Log($"{importerMaterial.Value.name}");

                materialNames.Remove(importerMaterial.Value.name);

                var material = System.Array.FindLast(materials, x => x.name == importerMaterial.Value.name);
                if (material == null)
                {
                    material = System.Array.FindLast(materials, x => x.name == importerMaterial.Key.name);
                }
                if (material == null)
                {
                    Debug.LogWarning($"material == null");
                    continue;
                }
                Debug.Log($"{importerMaterial.Value.name}->{material.name}");
                importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), importerMaterial.Key.name), material);
            }

            foreach (var materialName in materialNames)
            {
                Debug.Log($"Material name in model: {materialName}");
                var material = System.Array.FindLast(materials, x => x.name == materialName);
                if (material == null)
                {
                    Debug.LogWarning($"material == null for {materialName}");
                    continue;
                }
                Debug.Log($"{materialName} -> {material.name}");
                importer.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), materialName), material);
            }


            AssetDatabase.WriteImportSettingsIfDirty(path);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            Debug.Log("Materials updated for model: " + path);
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

                EditorGUILayout.Space();
                GUI.enabled = allReady;
                if (GUILayout.Button("ResetMaterials"))
                {
                    materialFolders = null;
                    materials = null;
                }
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materialFolders)));
                if ((materialFolders == null)||(System.Array.FindAll(materialFolders,x=>x!=null).Length==0))
                {
                    allReady = false;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (materialSelectMode == MaterialSelectMode.MaterialFolder)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(roots)));
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(root)));
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(repairMode)));
            if (materialSelectMode == MaterialSelectMode.MaterialFolder)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(nameMerge)));
            }
            EditorGUILayout.Space();

            if (materialSelectMode == MaterialSelectMode.MaterialFolder)
            {
                if ((roots == null) || (System.Array.FindAll(roots, x => x != null).Length == 0))
                {
                    allReady = false;
                }
            }
            else
            {
                if (root == null)
                {
                    allReady = false;
                }
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
            InsertMaterials();
        }
        GUI.enabled = true;
    }

}
#endif