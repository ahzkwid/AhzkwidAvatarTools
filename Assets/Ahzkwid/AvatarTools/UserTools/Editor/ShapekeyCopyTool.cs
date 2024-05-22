
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
class ShapekeyCopyTool : EditorWindow
{
    public GameObject character;
    public GameObject shapekey;
    public bool createBackup = true;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/" + nameof(ShapekeyCopyTool))]
    public static void Init()
    {
        var window = GetWindow<ShapekeyCopyTool>(utility: false, title: nameof(ShapekeyCopyTool));
        window.minSize = new Vector2(300, 150);
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(character)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(shapekey)));
            EditorGUILayout.Space();
            if (character == null)
            {
                allReady = false;
            }
            if (shapekey == null)
            {
                allReady = false;
            }
            if (allReady)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
            }
        }
        serializedObject.ApplyModifiedProperties();


        GUI.enabled = allReady;
        if (GUILayout.Button("ShapekeyCopy"))
        {
            if (createBackup)
            {
                var characterCopy = Instantiate(character);
                characterCopy.name = character.name+" (Backup)";
                characterCopy.SetActive(false);
            }
            ShapekeyCopy(character, shapekey);
        }
        GUI.enabled = true;
    }

    public void ShapekeyCopy(GameObject character, GameObject shapekey)
    {
        var characterRenderers = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        var shapekeyRenderers = shapekey.GetComponentsInChildren<SkinnedMeshRenderer>();



        foreach (var characterRenderer in characterRenderers)
        {
            var shapekeyRenderer = System.Array.Find(shapekeyRenderers, x => x.sharedMesh == characterRenderer.sharedMesh);
            if (shapekeyRenderer == null)
            {
                continue;
            }    
            for (int i = 0; i < characterRenderer.sharedMesh.blendShapeCount; i++)
            {
                
                characterRenderer.SetBlendShapeWeight(i, shapekeyRenderer.GetBlendShapeWeight(i));
            }
        }

    }
}
#endif