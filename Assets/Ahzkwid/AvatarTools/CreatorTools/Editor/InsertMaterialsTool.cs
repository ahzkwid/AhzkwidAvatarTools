



#if UNITY_EDITOR

using UnityEngine;

//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine.WSA;

[InitializeOnLoad]
class InsertMaterialsTool : EditorWindow
{
    public enum MaterialSelectMode
    {
        Materials, MaterialFolder, CreateMaterialsVariant
    }
    public MaterialSelectMode materialSelectMode;
    public DefaultAsset[] materialFolders;
    public DefaultAsset[] toFolders;
    public Material[] materials;
    public GameObject root;
    public GameObject[] roots;


    public bool repairMode=false;
    public bool nameMerge = false;
    public bool onlyNameContains = false;
    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(InsertMaterialTool))]
    public static void Init()
    {
        var window = GetWindow<InsertMaterialsTool>(utility: false, title: nameof(InsertMaterialsTool));
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
    void InsertMaterials()
    {
        switch (materialSelectMode)
        {
            case MaterialSelectMode.Materials:
                InsertMaterials(root, materials);
                break;
            case MaterialSelectMode.MaterialFolder:
                if (roots.Length == 1)
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
                        if (i > materialFolders.Length)
                        {
                            continue;
                        }
                        var root = roots[i];
                        var materialFolder = materialFolders[i % roots.Length];
                        InsertMaterials(root, materialFolder);
                    }
                }
                break;
            case MaterialSelectMode.CreateMaterialsVariant:

                if (materialFolders.Length == 1)
                {
                    var materialFolder = materialFolders.First();

                    foreach (var toFolder in toFolders)
                    {
                        CreateMaterialsVariant(materialFolder, toFolder);
                    }
                }
                else
                {
                    for (int i = 0; i < materialFolders.Length; i++)
                    {
                        var materialFolder = materialFolders[i];
                        var toFolder = toFolders.FirstOrDefault(x => x.name == materialFolder.name);

                        if (toFolder == null)
                        {
                            continue;
                        }
                        CreateMaterialsVariant(materialFolder, toFolder);
                    }
                }
                break;
            default:
                break;
        }
    }
    void CreateMaterialsVariant(DefaultAsset fromFolder, DefaultAsset toFolder)
    {
        var folderPathFrom = AssetDatabase.GetAssetPath(fromFolder);
        var folderPathTo = AssetDatabase.GetAssetPath(toFolder);

        var filePathsFrom = AssetDatabase.FindAssets($"t:{typeof(Material).Name}", new string[] { folderPathFrom });
        var assetsFrom = new Material[filePathsFrom.Length];
        for (int i = 0; i < assetsFrom.Length; i++)
        {
            var fromMaterial = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(filePathsFrom[i]));



            var toPath = Path.Combine(folderPathTo, fromMaterial.name)+ ".mat";


            if (File.Exists(toPath))
            {
                Debug.Log($"File.Exists {toPath}");
                continue;
            }

            var variant = new Material(fromMaterial);
            variant.parent = fromMaterial;


            AssetDatabase.CreateAsset(variant, toPath);
        }
    }
    void InsertMaterials(GameObject root, DefaultAsset materialFolder)
    {
        if (onlyNameContains)
        {
            if (root.name.Contains(materialFolder.name)==false)
            {
                return;
            }
        }
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
            var renders = root.GetComponentsInChildren<Renderer>(true);
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
            switch (materialSelectMode)
            {
                case MaterialSelectMode.Materials:
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
                    break;
                case MaterialSelectMode.MaterialFolder:
                case MaterialSelectMode.CreateMaterialsVariant:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materialFolders)));
                    if ((materialFolders == null) || (System.Array.FindAll(materialFolders, x => x != null).Length == 0))
                    {
                        allReady = false;
                    }
                    break;
                default:
                    break;
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            switch (materialSelectMode)
            {
                case MaterialSelectMode.Materials:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(root)));
                    break;
                case MaterialSelectMode.MaterialFolder:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(roots)));
                    break;
                case MaterialSelectMode.CreateMaterialsVariant:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(toFolders)));
                    break;
                default:
                    break;
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            switch (materialSelectMode)
            {
                case MaterialSelectMode.Materials:
                case MaterialSelectMode.MaterialFolder:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(repairMode)));
                    break;
                case MaterialSelectMode.CreateMaterialsVariant:
                    break;
                default:
                    break;
            }
            if (materialSelectMode == MaterialSelectMode.MaterialFolder)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(nameMerge)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(onlyNameContains)));
            }
            EditorGUILayout.Space();

            switch (materialSelectMode)
            {
                case MaterialSelectMode.Materials:
                    if (root == null)
                    {
                        allReady = false;
                    }
                    break;
                case MaterialSelectMode.MaterialFolder:
                    if ((roots == null) || (System.Array.FindAll(roots, x => x != null).Length == 0))
                    {
                        allReady = false;
                    }
                    break;
                case MaterialSelectMode.CreateMaterialsVariant:
                    break;
                default:
                    break;
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