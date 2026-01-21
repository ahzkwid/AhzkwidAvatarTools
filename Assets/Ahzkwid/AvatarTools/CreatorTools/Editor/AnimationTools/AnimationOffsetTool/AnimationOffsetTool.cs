



namespace Ahzkwid
{
#if UNITY_EDITOR

    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    public class AnimationOffsetTool : EditorWindow
    {
        public static void Init()
        {
            var window = GetWindow<AnimationOffsetTool>(utility: false, title: nameof(AnimationOffsetTool));
            window.minSize = new Vector2(300, 200);
            //window.maxSize = window.minSize;
            window.Show();
        }
        SerializedObject serializedObject;
        string[] characterPaths = new string[]
        {
        };
        public void ChangeOffset(bool multiplyMode)
        {
            foreach (var a in assets)
            {
                var path = AssetDatabase.GetAssetPath(a);
                var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                if (importer == null)
                {
                    continue;
                }

                var clips = importer.clipAnimations;
                if (clips == null || clips.Length == 0)
                {
                    clips = importer.defaultClipAnimations;
                }

                for (var i = 0; i < clips.Length; i++)
                {
                    var c = clips[i];
                    //c.keepOriginalPositionY = false;
                    if (multiplyMode)
                    {
                        c.heightOffset *= offset;
                    }
                    else
                    {
                        c.heightOffset = offset;
                    }
                    clips[i] = c;
                }

                importer.clipAnimations = clips;
                importer.SaveAndReimport();
            }
        }
        public float offset = 1f;
        public GameObject[] assets = new GameObject[] { };
        void OnGUI()
        {
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(this);
            }
            serializedObject.Update();
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(assets)));


                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(offset)));
            }
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            if (GUILayout.Button("MultiplyOffset"))
            {
                ChangeOffset(true);
            }
            if (GUILayout.Button("ChangeOffset"))
            {
                ChangeOffset(false);
            }
        }
        GameObject guidTarget;
    }

#endif
}