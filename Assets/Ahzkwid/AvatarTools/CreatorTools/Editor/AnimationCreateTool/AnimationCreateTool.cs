﻿






#if UNITY_EDITOR

using UnityEngine;
using Ahzkwid.AvatarTool;


using UnityEditor;

[InitializeOnLoad]
class AnimationCreateTool : EditorWindow
{

    public AnimationCreator.ToggleData[] toggleDatas;

    public DefaultAsset exportForder;


    public static void Init()
    {
        var window = GetWindow<AnimationCreateTool>(utility: false, title: nameof(AnimationCreateTool));
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(toggleDatas)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(exportForder)));
            EditorGUILayout.Space();
            if (toggleDatas == null || toggleDatas.Length == 0) 
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
            var animatorController= AnimationCreator.CreateAnimator(toggleDatas);
            if (exportForder!=null)
            {
                animatorController.name = exportForder.name;
                AssetManager.SaveAsset(animatorController, exportForder);
            }
            else
            {
                var fileOption = AssetManager.FileOptions.Normal;
                AssetManager.SaveAsset(animatorController, fileOption);
            }
        }
        GUI.enabled = true;
    }

}
#endif