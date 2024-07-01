
using UnityEngine;



#if UNITY_EDITOR


//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class MaterialPropertyTool : EditorWindow
{
    public Object folder;
    public Object exceptionFolders;
    public enum Operator
    {
        Equal,
        NotEqual
    }
    [System.Serializable]
    public class Trigger
    {
        public Operator _operator;
        public float value;
    }
    [System.Serializable]
    public class Action
    {
        public string propertyName;
        public float value;
    }
    [System.Serializable]
    public class TriggerAction
    {
        public Trigger trigger;
        public Action action;
    }
    public static readonly TriggerAction outlineTA = new TriggerAction
    {
        trigger = new Trigger { _operator = Operator.NotEqual, value = 0 },
        action = new Action { propertyName = "_OutlineWidth", value = 0.01f }
    };
    public static readonly TriggerAction lightMinLimitTA = new TriggerAction
    {
        trigger = new Trigger { _operator = Operator.Equal, value = 0.05f },
        action = new Action { propertyName = "_LightMinLimit", value = 0.2f }
    };




    public Trigger trigger;
    public Action action;
    public Target target = (Target)(-1);
    public enum Target
    {
        Outline, LightMinLimit
    }
    //public Object textureFolder;
    //public bool createBackup = true;

    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(MaterialPropertyTool))]
    public static void Init()
    {
        var window = GetWindow<MaterialPropertyTool>(utility: false, title: nameof(MaterialPropertyTool));
        window.minSize = new Vector2(300, 200);
        //window.maxSize = window.minSize;
        window.Show();
    }
    static Material[] GetFolderToMaterials(Object folder)
    {
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var filePaths = AssetDatabase.FindAssets($"t:{typeof(Material).Name}", new string[] { folderPath });
        var assets = new Material[filePaths.Length];
        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(filePaths[i]));
        }
        return assets;
    }
    public static void ReplacePropertyValue(Object folder,Trigger trigger, Action action)
    {


        var materials = GetFolderToMaterials(folder);
        Debug.Log($"materials.Length:{materials.Length}");
        /*
        {
            var material = materials[0];
            var properties = MaterialEditor.GetMaterialProperties(new Material[] { material });

            foreach (var property in properties)
            {
                Debug.Log($"property.name:{property.name}");

            }
        }
        */

        foreach (var material in materials)
        {
            var properties = MaterialEditor.GetMaterialProperties(new Material[] { material });
            //Debug.Log($"properties.Length:{properties.Length}");
            //Debug.Log($"properties:{properties.Length}");
            foreach (var property in properties)
            {
                if (property.floatValue != 0)
                {
                    //Debug.Log($"property.name:{property.name}");
                }
                if (property.name != action.propertyName)
                {
                    continue;
                }
                switch (trigger._operator)
                {
                    case Operator.Equal:
                        if (property.floatValue != trigger.value)
                        {
                            continue;
                        }
                        break;
                    case Operator.NotEqual:
                        if (property.floatValue == trigger.value)
                        {
                            continue;
                        }
                        break;
                    default:
                        break;
                }
                
                Debug.Log($"{material.name}.{property.name}:{property.floatValue}->{action.value}");
                property.floatValue = action.value;

            }
        }
    }
    SerializedObject serializedObject;
    void OnGUI()
    {
        if (serializedObject == null)
        {
            serializedObject = new SerializedObject(this);
        }

        if (target == (Target)(-1))
        {
            target = Target.Outline;
            trigger = outlineTA.trigger;
            action = outlineTA.action;
        }


        var allReady = true;




        serializedObject.Update();
        {
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(target)));
            }
        }
        serializedObject.ApplyModifiedProperties();

        serializedObject.Update();
        {
            if (EditorGUI.EndChangeCheck())
            {
                var ta = outlineTA;
                switch (target)
                {
                    case Target.Outline:
                        break;
                    case Target.LightMinLimit:
                        ta = lightMinLimitTA;
                        break;
                    default:
                        break;
                }
                {
                    trigger._operator = ta.trigger._operator;
                    trigger.value = ta.trigger.value;
                    action.propertyName = ta.action.propertyName;
                    action.value = ta.action.value;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(folder)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(trigger)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(action)));
            EditorGUILayout.Space();
            if (folder == null)
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
        if (GUILayout.Button("Replace"))
        {
            ReplacePropertyValue(folder, trigger,action);
        }
        GUI.enabled = true;
    }
}
#endif