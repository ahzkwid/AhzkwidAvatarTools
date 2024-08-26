




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
        UnityEditor.AssetDatabase.SaveAssetIfDirty(animationClip);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationClip));
    }
    public void Repair(AnimationClip clip, Action[] actions)
    {
        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
            var newBinding = binding;
            foreach (var action in actions)
            {
                //Debug.Log($"Path: {action.option}, Property: {binding.propertyName}");

                var text = "";
                switch (action.target)
                {
                    case Action.Target.Path:
                        text = binding.path;
                        break;
                    case Action.Target.Property:
                        text = binding.propertyName;
                        break;
                    case Action.Target.Type:
                        text = binding.type.ToString();
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
                        newBinding.path = text;
                        break;
                    case Action.Target.Property:
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
        UnityEditor.AssetDatabase.SaveAssetIfDirty(animationClip);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(animationClip));
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
                if (animationClip == null)
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(value)));
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