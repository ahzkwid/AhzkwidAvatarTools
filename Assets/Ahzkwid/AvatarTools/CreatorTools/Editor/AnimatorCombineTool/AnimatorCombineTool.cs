






#if UNITY_EDITOR

using UnityEngine;
using Ahzkwid.AvatarTool;


using UnityEditor.Animations;
using UnityEditor;
using System.Linq;
using UnityEditor.VersionControl;

[InitializeOnLoad]
class AnimatorCombineTool : EditorWindow
{

    public AnimatorController[] animators;

    public DefaultAsset exportForder;


    public static void Init()
    {
        var window = GetWindow<AnimatorCombineTool>(utility: false, title: nameof(AnimatorCombineTool));
        window.minSize = new Vector2(300, 200);
        //window.maxSize = window.minSize;
        window.Show();
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(animators)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(exportForder)));
            EditorGUILayout.Space();
            if (animators == null || animators.Length == 0) 
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
        if (GUILayout.Button("Run"))
        {
            AnimatorController animatorMain = null;
            string folderPath;
            if (exportForder != null)
            {
                folderPath = AssetDatabase.GetAssetPath(exportForder);
            }
            else
            {
                var fileOption = AssetManager.FileOptions.Normal;
                folderPath = AssetManager.GetFolderPath(fileOption);
            }
            foreach (var animator in animators)
            {
                if (animatorMain==null)
                {
                    animatorMain = animator;
                    continue;
                }
                if (string.IsNullOrWhiteSpace(folderPath)==false)
                {
                    animatorMain = AnimatorCombiner.CombineAnimators(animatorMain, animator, folderPath);
                }
            }
            AssetManager.SaveAsset(animatorMain, folderPath);
        }
        GUI.enabled = true;
    }

}
#endif