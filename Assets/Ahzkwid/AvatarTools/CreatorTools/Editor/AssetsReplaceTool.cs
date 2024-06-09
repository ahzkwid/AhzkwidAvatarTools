using UnityEngine;
using System.IO;
using System.Linq;






#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
class AssetsReplaceTool : EditorWindow
{
    public Object fromFolder;
    public Object toFolder;
    public string[] ignores= new string[] { ".meta", ".asmdef" };

    public enum Option
    {
        Add, Replace, Intersection
    }
    public Option option = Option.Add;

    public static void Init()
    {
        var window = GetWindow<AssetsReplaceTool>(utility: false, title: nameof(AssetsReplaceTool));
        window.minSize = new Vector2(300, 200);
        window.Show();
    }



    static string RelativePath(string folderPath, string filePath)
    {
        return filePath.Substring(folderPath.Length, filePath.Length - folderPath.Length);
    }


    void ReplaceFiles(Object folderA, Object folderB, string[] excludeExtensions, Option option)
    {
        //var folderAPath = $"{Application.dataPath}/{AssetDatabase.GetAssetPath(folderA)}";
        var folderAPath = AssetDatabase.GetAssetPath(folderA);
        Debug.Log(folderAPath);
        var folderBPath = AssetDatabase.GetAssetPath(folderB);
        Debug.Log(folderBPath);
        ReplaceFiles(folderAPath, folderBPath, excludeExtensions, option);
    }
    void ReplaceFiles(string folderAPath, string folderBPath, string[] excludeExtensions, Option option)
    {
        if (!Directory.Exists(folderAPath) || !Directory.Exists(folderBPath))
        {
            Debug.LogError($"folderAPath: {folderAPath}");
            Debug.LogError($"folderBPath: {folderBPath}");
            return;
        }

        var filesA = Directory.GetFiles(folderAPath, "*", SearchOption.AllDirectories)
            .Where(file => !excludeExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToList();

        var filesB = Directory.GetFiles(folderBPath, "*", SearchOption.AllDirectories)
            .Where(file => !excludeExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToList();
        Debug.Log($"Directory.GetFiles(folderAPath).Length:{Directory.GetFiles(folderAPath).Length}");
        Debug.Log($"filesA.Count:{filesA.Count}");
        Debug.Log($"filesB.Count:{filesB.Count}");

        if (option == Option.Replace)
        {
            foreach (var fileB in filesB)
            {
                File.Delete(fileB);
            }
        }
        // A 폴더의 파일들을 B 폴더에 복사
        foreach (var fileA in filesA)
        {
            var relativePath = RelativePath(folderAPath, fileA);
            //string fileName = Path.GetFileName(fileA);
            //string filePath = Path.GetDirectoryName(fileA);
            //Debug.Log(fileName);
            var newFilePath = folderBPath+ relativePath;
            Debug.Log(newFilePath);




            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }
            else
            {
                if (option == Option.Intersection)
                {
                    continue;
                }
            }
            var folderPath = System.IO.Path.GetDirectoryName(newFilePath);
            if (System.IO.Directory.Exists(folderPath) == false)
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            File.Copy(fileA, newFilePath);
            AssetDatabase.ImportAsset(newFilePath);
        }


        Debug.Log("파일 교체 완료");
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(fromFolder)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(toFolder)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ignores)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(option)));
            EditorGUILayout.Space();
            if (fromFolder == null)
            {
                allReady = false;
            }
            if (toFolder == null)
            {
                allReady = false;
            }

            if (allReady)
            {
            }
        }
        serializedObject.ApplyModifiedProperties();

        GUI.enabled = allReady;
        if (GUILayout.Button("Run"))
        {
            ReplaceFiles(fromFolder, toFolder, ignores, option);
        }
        GUI.enabled = true;
    }
}
#endif
