




#if UNITY_EDITOR

using UnityEngine;

using UnityEditor;

/*

[CustomPropertyDrawer(typeof(AnimationRepairToolActionAttribute))]
public class AnimationRepairToolActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var fieldRect = rect;
        fieldRect.width /= 2;
        {
            var path = nameof(AnimationRepairTool.Action.key);
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
        }


        fieldRect.x += fieldRect.width;
        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(nameof(AnimationRepairTool.Action.value)), label, true);
    }
}
public class AnimationRepairToolActionAttribute : PropertyAttribute
{

}

*/

[InitializeOnLoad]
class AnimationRepairTool : EditorWindow
{
    public AnimationClip animationClip;
    public Action[] actions= new Action[1];

    public AnimationData[] animationDatas;



    public void SaveSettingToJson(string path)
    {
        var json = JsonUtility.ToJson(this);
        System.IO.File.WriteAllText(path, json);
        AssetDatabase.Refresh();
    }

    public void LoadSettingFromJson(string path)
    {
        if (System.IO.File.Exists(path))
        {
            var preAnimationClip= animationClip;
            var json = System.IO.File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, this);

            animationClip= preAnimationClip;
        }
    }







    [System.Serializable]
    public class AnimationData
    {
        public string path = "";
        public string propertyName = "";
        public string type = "";
    }


    [System.Serializable]
    public class Action
    {
        public enum Target
        {
            Path, Property, Type
        }
        public enum Option
        {
            Replace, Add,AddFirst
        }
        public Target target;
        public Option option;
        public string key = "";
        public string value = "";
    }
    [System.Serializable]
    public class Value
    {
        public enum Option
        {
            Multiply, Add
        }
        public Option option;
        public float value = 1f;
    }
    public Value value;

    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AnimationRepairTool))]
    public static void Init()
    {
        var window = GetWindow<AnimationRepairTool>(utility: false, title: nameof(AnimationRepairTool));
        window.minSize = new Vector2(300, 200);
        //window.maxSize = window.minSize;
        window.Show();
    }
    public void ChangeValue(AnimationClip clip, Value value)
    {
        var curveBindings = AnimationUtility.GetCurveBindings(clip);

        foreach (var curveBinding in curveBindings)
        {
            var curve = AnimationUtility.GetEditorCurve(clip, curveBinding);
            if (curve != null)
            {
                for (int i = 0; i < curve.keys.Length; i++)
                {
                    Keyframe keyframe = curve.keys[i];
                    switch(value.option)
                    {
                        case Value.Option.Multiply:
                            keyframe.value *= value.value;
                            break;
                        case Value.Option.Add:
                            keyframe.value += value.value;
                            break;
                    }
                    curve.MoveKey(i, keyframe);
                }
                AnimationUtility.SetEditorCurve(clip, curveBinding, curve);
            }
        }
        UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clip));
    }
    public void Repair(AnimationClip clip, Action[] actions)
    {
        var bindings = AnimationUtility.GetCurveBindings(clip);
        foreach (var binding in bindings)
        {
            var newBinding = binding;
            foreach (var action in actions)
            {
                //Debug.Log($"Path: {action.option}, Property: {binding.propertyName}");

                var text = "";
                switch (action.target)
                {
                    case Action.Target.Path:
                        text = newBinding.path;
                        break;
                    case Action.Target.Property:
                        text = newBinding.propertyName;
                        break;
                    case Action.Target.Type:
                        text = newBinding.type.ToString();
                        break;
                }
                switch (action.option)
                {
                    case Action.Option.Replace:
                        switch (action.target)
                        {
                            case Action.Target.Path:
                            case Action.Target.Property:
                                if (action.key == "")
                                {
                                    text = action.value;
                                }
                                else
                                {
                                    text = text.Replace(action.key, action.value);
                                }
                                break;
                            case Action.Target.Type:
                                if (action.key == "")
                                {
                                    text = action.value;
                                }
                                else
                                {
                                    if (text == action.key)
                                    {
                                        text = action.value;
                                    }
                                }
                                break;

                        }

                        break;
                    case Action.Option.Add:
                        text += action.value;
                        break;
                    case Action.Option.AddFirst:
                        text = action.value+ text;
                        break;
                    default:
                        break;
                }
                switch (action.target)
                {
                    case Action.Target.Path:
                        if (newBinding.path != text)
                        {
                            Debug.Log($"Path: {newBinding.path} -> {text}");
                        }
                        newBinding.path = text;
                        break;
                    case Action.Target.Property:
                        if (newBinding.propertyName != text)
                        {
                            Debug.Log($"propertyName: {newBinding.propertyName} -> {text}");
                        }
                        newBinding.propertyName = text;
                        break;
                    case Action.Target.Type:
                        var type = System.Type.GetType(text);
                        if (type == null)
                        {
                            type = System.Type.GetType(value + ", UnityEngine");
                        }
                        if (type==null)
                        {
                            Debug.LogWarning($"type: {type}");
                        }
                        if (type != null)
                        {
                            if (newBinding.type != type)
                            {
                                Debug.Log($"type: {newBinding.type} -> {text}");
                            }
                            newBinding.type = type;
                        }
                        break;

                }

            }
            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            AnimationUtility.SetEditorCurve(clip, binding, null);//제거

            try
            {
                AnimationUtility.SetEditorCurve(clip, newBinding, curve);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError($"clip:{clip}");
                Debug.LogError($"newBinding:{newBinding}");
                Debug.LogError($"curve:{curve}");
                Debug.LogError($"newBinding.type:{newBinding.type}");
                throw;
            }
        }
        UnityEditor.AssetDatabase.SaveAssetIfDirty(clip);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(clip));
        //UnityEditor.EditorUtility.SetDirty(animationClip);

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
            //EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(animationClip)));
                EditorGUILayout.Space();
            }
            //if (EditorGUI.EndChangeCheck())
            {
                if ((animationClip == null)||(animationClip.Equals(null)))
                {
                    animationDatas = null;
                }
                else
                {
                    var bindings = AnimationUtility.GetCurveBindings(animationClip);
                    animationDatas = System.Array.ConvertAll(bindings, x =>
                    {
                        var animationData = new AnimationData();
                        animationData.path = x.path;
                        animationData.propertyName = x.propertyName;
                        animationData.type = $"{x.type}, {x.type.Namespace}";
                        return animationData;
                    });
                }
            }
            //EditorGUI.BeginChangeCheck();
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(animationDatas)));
                GUI.enabled = true;
            }
            //if (EditorGUI.EndChangeCheck())
            {
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(actions)));
            EditorGUILayout.Space();

            EditorGUILayout.Space();


            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(value)));
            EditorGUILayout.Space();



            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("SettingData");
                if (GUILayout.Button("Save"))
                {
                    var path = EditorUtility.SaveFilePanel("Save Setting", "", "AnimRepairToolSetting.json", "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        SaveSettingToJson(path);
                    }
                }

                if (GUILayout.Button("Load"))
                {
                    var path = EditorUtility.OpenFilePanel("Load Setting", "", "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        LoadSettingFromJson(path);
                    }
                }
            }
            GUILayout.EndHorizontal();


            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (animationClip == null)
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
        if (GUILayout.Button("Repair"))
        {
            Repair(animationClip, actions);
            ChangeValue(animationClip, value);
        }
        GUI.enabled = true;
    }

}
#endif