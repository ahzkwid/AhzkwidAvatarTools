

#if UNITY_EDITOR
namespace Ahzkwid
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class UnitypackageManager : EditorWindow
    {

        [System.Serializable]
        public class Exporter
        {
            public DefaultAsset[] fromFolders;
            public string toFolder = "";
            public string projectName = "";
            public string version = "";

            public void Export()
            {
                foreach (var fromFolder in fromFolders)
                {
                    var assetPath = AssetDatabase.GetAssetPath(fromFolder);
                    var fromFolderName = Path.GetFileNameWithoutExtension(assetPath);
                    var packagePath = Path.Combine(toFolder, $"{fromFolderName} {projectName} {version}.unitypackage");


                    AssetDatabase.ExportPackage(assetPath, packagePath, ExportPackageOptions.Recurse);
                }
                AssetDatabase.Refresh();
            }
            public void Draw(SerializedObject serializedObject)
            {
                var serializedProperty = serializedObject.FindProperty(nameof(exporter));
                EditorGUILayout.LabelField("Exporter", EditorStyles.boldLabel);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    var allReady = true;

                    EditorGUILayout.Space();
                    {
                        var property = serializedProperty.FindPropertyRelative(nameof(fromFolders));
                        EditorGUILayout.PropertyField(property);
                    }
                    EditorGUILayout.Space();
                    {
                        var property = serializedProperty.FindPropertyRelative(nameof(toFolder));
                        EditorGUILayout.PropertyField(property);
                    }
                    EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative(nameof(projectName)));
                    EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative(nameof(version)));


                    if ((fromFolders == null) || (fromFolders.Length == 0))
                    {
                        allReady = false;
                    }

                    if (string.IsNullOrWhiteSpace(toFolder))
                    {
                        allReady = false;
                    }




                    GUI.enabled = allReady;
                    if (GUILayout.Button("Export"))
                    {
                        Export();
                    }
                    GUI.enabled = true;
                }
                GUILayout.EndVertical();
            }
        }
        [System.Serializable]
        public class Importer
        {
            public string rootFolder = "";

            public void Import()
            {
                var files = Directory.GetFiles(rootFolder, "*.unitypackage", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    AssetDatabase.ImportPackage(file, false);
                }
                AssetDatabase.Refresh();
            }
            public void Draw(SerializedObject serializedObject)
            {
                var serializedProperty = serializedObject.FindProperty(nameof(importer));
                EditorGUILayout.LabelField("Importer", EditorStyles.boldLabel);
                GUILayout.BeginVertical(EditorStyles.helpBox);
                {
                        var allReady = true;

                    EditorGUILayout.Space();
                    {

                        var property = serializedProperty.FindPropertyRelative(nameof(rootFolder));
                        EditorGUILayout.PropertyField(property);
                    }
                    if (System.IO.Directory.Exists(rootFolder) ==false)
                    {
                        allReady = false;
                    }

                    GUI.enabled = allReady;
                    if (GUILayout.Button("Import"))
                    {
                        Import();
                    }
                    GUI.enabled = true;

                }
                GUILayout.EndVertical();
            }
        }

        public static void Init()
        {
            var window = GetWindow<UnitypackageManager>(utility: false, title: nameof(UnitypackageManager));
            window.minSize = new Vector2(300, 200);
            window.Show();
        }
        public Exporter exporter = new();
        public Importer importer = new();

        SerializedObject serializedObject;
        void OnGUI()
        {
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(this);
            }

            {
                serializedObject.Update();
                {
                    importer.Draw(serializedObject);
                    exporter.Draw(serializedObject);
                }
                serializedObject.ApplyModifiedProperties();

            }

        }
    }

}
#endif
