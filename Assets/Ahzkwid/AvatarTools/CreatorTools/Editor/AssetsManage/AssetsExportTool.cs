

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetsExportTool : EditorWindow
{
    public DefaultAsset[] fromFolders;
    public string toFolder="";
    public string projectName = "";
    public string version = "";



    public static void Init()
    {
        var window = GetWindow<AssetsExportTool>(utility: false, title: nameof(AssetsExportTool));
        window.minSize = new Vector2(300, 200);
        window.Show();
    }


    SerializedObject serializedObject;
    void Export()
    {
        foreach (var fromFolder in fromFolders)
        {
            var assetPath = AssetDatabase.GetAssetPath(fromFolder);
            var fromFolderName = Path.GetFileNameWithoutExtension(assetPath);
            var packagePath = Path.Combine(toFolder, $"{fromFolderName} {projectName} {version}.unitypackage");


            AssetDatabase.ExportPackage(assetPath, packagePath, ExportPackageOptions.Recurse);
        }
    }
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
            {
                var property = serializedObject.FindProperty(nameof(fromFolders));
                EditorGUILayout.PropertyField(property);
            }
            EditorGUILayout.Space();
            {
                var property = serializedObject.FindProperty(nameof(toFolder));
                EditorGUILayout.PropertyField(property);
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(projectName)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(version)));


            if ((fromFolders == null) || (fromFolders.Length == 0))
            {
                allReady = false;
            }

            if (string.IsNullOrWhiteSpace(toFolder))
            {
                allReady = false;
            }
        }
        serializedObject.ApplyModifiedProperties();

        GUI.enabled = allReady;
        if (GUILayout.Button("Run"))
        {
            Export();
            AssetDatabase.Refresh();
        }
        GUI.enabled = true;
    }
}

#endif
