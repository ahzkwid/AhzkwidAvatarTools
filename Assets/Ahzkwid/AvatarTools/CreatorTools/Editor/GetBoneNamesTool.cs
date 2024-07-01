



#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


[InitializeOnLoad]
class GetBoneNamesTool : EditorWindow
{
    public Transform[] characters;
    public List<string> texts=new List<string>();

    void GetBoneNames(Transform character=null)
    {
        string ToLowerFirst(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            return char.ToLower(text[0]) + text.Substring(1);
        }
        {
            var boneTransformEnumNames = "";
            var boneTransformNames = "";
            Animator animator = null;
            if (character != null)
            {
                animator=character.GetComponentInChildren<Animator>(true);
            }
            foreach (var humanBodyBone in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
            {

                if (humanBodyBone < 0)
                {
                    continue;
                }
                if (humanBodyBone >= HumanBodyBones.LastBone)
                {
                    continue;
                }
                var humanBodyBoneToString = ToLowerFirst(humanBodyBone.ToString());
                if (!string.IsNullOrWhiteSpace(boneTransformEnumNames))
                {
                    boneTransformEnumNames += ",";
                }
                boneTransformEnumNames += $"\"{humanBodyBoneToString}\": None";
                //boneTransformEnumNames += $"\n";

                if (animator == null)
                {
                    continue;
                }
                var boneTransform = animator.GetBoneTransform(humanBodyBone);
                if (boneTransform == null)
                {
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(boneTransformNames))
                {
                    boneTransformNames += ",";
                }
                boneTransformNames += $"{humanBodyBoneToString}=\"{boneTransform.name}\"";
                //boneTransformNames += $"\n";
            }
            if (character==null)
            {
                //var text = "self.bones = {\n" + boneTransformEnumNames + "\n}";
                var text = "self.bones = {" + boneTransformEnumNames + "}";
                texts.Add(text);
                Debug.Log(text);
            }
            else
            {
                //var text = $"{ToLowerFirst(character.name)}Names = Humanoid(\n{boneTransformNames}\n)";
                var text = $"{(character.name).ToLower()}BoneNames = Humanoid({boneTransformNames})";
                texts.Add(text);
                Debug.Log(text);
            }
        }
    }

    public static void Init()
    {
        var window = GetWindow<GetBoneNamesTool>(utility: false, title: nameof(GetBoneNamesTool));
        window.minSize = new Vector2(300, 200);
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
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(characters)));
            }
            if (EditorGUI.EndChangeCheck())
            {
                //serializedObject.ApplyModifiedProperties();
                //textures = GetFolderToTextureAssets(textureFolder);
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(texts)));
            EditorGUILayout.Space();
            GUI.enabled = false;
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(textures)));
            GUI.enabled = true;
            EditorGUILayout.Space();
            if (characters == null)
            {
                allReady = false;
            }

            if (allReady)
            {
            }
        }
        serializedObject.ApplyModifiedProperties();

        GUI.enabled = allReady;

        if (GUILayout.Button("GetBoneNames"))
        {
            texts.Clear();
            GetBoneNames();
            foreach (var character in characters)
            {
                GetBoneNames(character.transform);
            }
        }
        GUI.enabled = true;
    }
}
#endif
