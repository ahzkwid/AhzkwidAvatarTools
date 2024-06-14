





#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;



[InitializeOnLoad]
class AssetsCloningTool : EditorWindow
{
    public Object fromFolder;
    public Object toFolder;
    public string[] ignores = new string[] { ".meta", ".asmdef", "MenuItems.cs" };

    public enum Option
    {
        Default, Add, Replace, Intersection
    }
    public Option option = Option.Default;

    public static void Init()
    {
        var window = GetWindow<AssetsCloningTool>(utility: false, title: nameof(AssetsCloningTool));
        window.minSize = new Vector2(300, 200);
        window.Show();
    }



    static string RelativePath(string folderPath, string filePath)
    {
        return filePath.Substring(folderPath.Length, filePath.Length - folderPath.Length);
    }
    public static string ReplaceWhiteList(string text, string regexPattern)//화이트리스트
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return text;
        }
        var convertText = new List<char>();
        for (int i = 0; i < text.Length; i++)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(text[i].ToString(), regexPattern))
            {
                convertText.Add(text[i]);
            }
        }
        if (convertText.Count == 0)
        {
            return "";
        }
        else
        {
            return new string(convertText.ToArray());
        }
    }

    void ReplaceFiles(Object folderA, Object folderB, string[] ignores, Option option)
    {
        //var folderAPath = $"{Application.dataPath}/{AssetDatabase.GetAssetPath(folderA)}";
        var folderAPath = AssetDatabase.GetAssetPath(folderA);
        Debug.Log(folderAPath);
        var folderBPath = AssetDatabase.GetAssetPath(folderB);
        Debug.Log(folderBPath);
        ReplaceFiles(folderAPath, folderBPath, ignores, option);
    }
    void ReplaceFiles(string folderAPath, string folderBPath, string[] ignores, Option option)
    {
        if (!Directory.Exists(folderAPath) || !Directory.Exists(folderBPath))
        {
            Debug.LogError($"folderAPath: {folderAPath}");
            Debug.LogError($"folderBPath: {folderBPath}");
            return;
        }
        if (option == Option.Default)
        {
            ignores = new string[] { ".meta", "MenuItems.cs" };
        }

        var filesA = Directory.GetFiles(folderAPath, "*", SearchOption.AllDirectories)
            .Where(file => System.Array.Find(ignores, ignore => Path.GetFileName(file).Contains(ignore)) == null)
            .Where(file => !ignores.Contains(Path.GetExtension(file)))
            .ToList();

        var filesB = Directory.GetFiles(folderBPath, "*", SearchOption.AllDirectories)
            .Where(file => System.Array.Find(ignores, ignore => Path.GetFileName(file).Contains(ignore)) == null)
            .Where(file => !ignores.Contains(Path.GetExtension(file)))
            .ToList();
        Debug.Log($"Directory.GetFiles(folderAPath).Length:{Directory.GetFiles(folderAPath).Length}");
        Debug.Log($"filesA.Count:{filesA.Count}");
        Debug.Log($"filesB.Count:{filesB.Count}");


        switch (option)
        {
            case Option.Default:
            case Option.Replace:
                foreach (var fileB in filesB)
                {
                    File.Delete(fileB);
                }
                break;
            case Option.Intersection:
            case Option.Add:
            default:
                break;
        }
        // A 폴더의 파일들을 B 폴더에 복사
        foreach (var filepathA in filesA)
        {
            var relativePath = "/" + Path.GetRelativePath(folderAPath, filepathA);
            //string fileName = Path.GetFileName(fileA);
            //string filePath = Path.GetDirectoryName(fileA);
            //Debug.Log(fileName);
            var newFilePath = folderBPath + relativePath;
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

            if (System.IO.Path.GetExtension(filepathA) == ".asmdef")
            {
                var text = System.IO.File.ReadAllText(filepathA);
                //var json = JsonUtility.FromJson<AssemblyDefinitionAsset>(text);
                var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                var newNamespace = System.IO.Path.GetDirectoryName(newFilePath); // 파일명제거
                newNamespace = newNamespace.Replace("Assets\\", ""); // Assets\ 제거
                newNamespace = ReplaceWhiteList(newNamespace, @"^[a-zA-Z]+$"); //알파벳만
                jsonObject["rootNamespace"] = newNamespace;
                jsonObject["name"] = newNamespace;
                var newJson = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                File.WriteAllText(newFilePath, newJson);
            }
            else
            {
                File.Copy(filepathA, newFilePath);
            }

            AssetDatabase.ImportAsset(newFilePath);
        }


        var prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { folderBPath });
        var prefabPaths = System.Array.ConvertAll(prefabGUIDs, guid => AssetDatabase.GUIDToAssetPath(guid));

        foreach (var prefabPath in prefabPaths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var components = prefab.GetComponentsInChildren<MonoBehaviour>(true);
            components = System.Array.FindAll(components, x => x != null);

            var dictionary = new Dictionary<string, string>();

            foreach (var component in components)
            {
                var script = MonoScript.FromMonoBehaviour(component);
                var scriptPath = AssetDatabase.GetAssetPath(script);
                if (scriptPath.StartsWith(folderAPath) == false)
                {
                    continue;
                }


                var newScriptPath = scriptPath.Replace(folderAPath, folderBPath);
                var newScript = AssetDatabase.LoadAssetAtPath<MonoScript>(newScriptPath);
                if (newScript == null)
                {
                    continue;
                }


                var scriptGUID = AssetDatabase.AssetPathToGUID(scriptPath);
                var newScriptGUID = AssetDatabase.AssetPathToGUID(newScriptPath);
                dictionary.Add(scriptGUID, newScriptGUID);
            }


            var text = System.IO.File.ReadAllText(prefabPath);
            foreach (var key in dictionary.Keys)
            {
                text = text.Replace(key, dictionary[key]);
                Debug.Log($"{prefabPath}.{key} -> {dictionary[key]}");
            }

            System.IO.File.WriteAllText(prefabPath, text);

        }
        foreach (var path in prefabPaths)
        {
            AssetDatabase.ImportAsset(path);
        }







        Debug.Log("Complete");
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
            if (option != Option.Default)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ignores)));
                EditorGUILayout.Space();
            }
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
