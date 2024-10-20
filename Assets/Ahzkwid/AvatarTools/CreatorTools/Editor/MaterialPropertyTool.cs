
using UnityEngine;
using System.Linq;




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


    public Material from;
    public Object[] to= new Object[1];

    public string[] properties;
    public static readonly string[] propertyNamesOutline = new string[]
    {
        "_OutlineWidth"
    };

    public static readonly string[] propertyNamesLight = new string[]
    {
        "_MonochromeLighting",
        "_LightMinLimit", "_VertexLightStrength"
    };
    public static readonly string[] propertyNamesShadow = new string[]
    {
        "_UseShadow",
        "_ShadowStrength",
        "_ShadowColor", "_Shadow2ndColor", "_Shadow3rdColor",
        "_ShadowBorder", "_Shadow2ndBorder", "_Shadow3rdBorder",
        "_ShadowBlur", "_Shadow2ndBlur", "_Shadow3rdBlur",
        "_ShadowNormalStrength", "_Shadow2ndNormalStrength", "_Shadow3rdNormalStrength",
        "_ShadowReceive", "_Shadow2ndReceive", "_Shadow3rdReceive"
    };
    public static readonly string[] rimlightProperties = 
    {
        "_UseRim",
        "_RimColor",
        "_RimMainStrength",
        "_RimEnableLighting",
        "_RimShadowMask",
        "_RimBackfaceMask",
        "_RimBlendMode",
        "_RimDirStrength",
        "_RimDirRange",
        "_RimBorder",
        "_RimBlur",
        "_RimIndirStrength",
        "_RimIndirRange",
        "_RimIndirColor",
        "_RimIndirBorder",
        "_RimIndirBlur",
        "_RimNormalStrength",
        "_RimFresnelPower",
        "_RimVRParallaxStrength"
    };
    
    public static readonly string[] rimShadeProperties = 
    {
        "_UseRimShade",
        "_RimShadeColor",
        "_RimShadeNormalStrength",
        "_RimShadeBorder",
        "_RimShadeBlur",
        "_RimShadeFresnelPower"
    };

    public static readonly string[] backlightProperties = 
    {
        "_UseBacklight",
        "_BacklightColor",
        "_BacklightMainStrength",
        "_BacklightReceiveShadow",
        "_BacklightBackfaceMask",
        "_BacklightNormalStrength",
        "_BacklightBorder",
        "_BacklightBlur",
        "_BacklightDirectivity",
        "_BacklightViewStrength"
    };

    public static readonly string[] reflectionProperties = 
    {
        "_UseReflection",
        "_Smoothness",
        "_GSAAStrength",
        "_Metallic",
        "_ReflectionColor",
        "_Reflectance",
        "_SpecularNormalStrength",
        "_SpecularBorder",
        "_SpecularBlur",
        "_ApplySpecular",
        "_ApplySpecularFA",
        "_ApplyReflection",
        "_ReflectionBlendMode"
    };
    public static readonly string[] matCapProperties = 
    {
        "UseMatCap",
        "_MatCapTex",
        "_MatCapColor",
        "_MatCapColor.a",
        "_MatCapMainStrength",
        "_MatCapNormalStrength",
        "_MatCapBlendMask",
        "_MatCapBlend",
        "_MatCapEnableLighting",
        "_MatCapShadowMask",
        "_MatCapBackfaceMask",
        "_MatCapLod",
        "_MatCapBlendMode",
        "_MatCapApply Transparency",
        "_MatCapCustomNormal",
        "_UseMatCap2nd",
        "_MatCap2ndTex",
        "_MatCap2ndColor",
        "_MatCap2ndColor.a",
        "_MatCap2ndMainStrength",
        "_MatCap2ndNormalStrength",
        "_MatCap2ndBlendMask",
        "_MatCap2ndBlend",
        "_MatCap2ndEnableLighting",
        "_MatCap2ndShadowMask",
        "_MatCap2ndBackfaceMask",
        "_MatCap2ndLod",
        "_MatCap2ndBlendMode",
        "_MatCap2ndApply Transparency",
        "_MatCap2ndCustomNormal"
    };





    public Trigger trigger;
    public Action action;
    public TargetCopyPaste targetCP = (TargetCopyPaste)(-1);
    public TargetTriggerAction targetTA = (TargetTriggerAction)(-1);
    public Mode mode = Mode.CopyPaste;
    public enum TargetTriggerAction
    {
        Outline, LightMinLimit, Copy
    }
    public enum TargetCopyPaste
    {
        Light, Shadow, RimLight, RimShade, BackLight, Reflection, Matcap,Outline
    }
    public enum Mode
    {
        CopyPaste, TriggerAction
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
        if (folder is Material material)
        {
            return new Material[] { material };
        }
        var folderPath = AssetDatabase.GetAssetPath(folder);
        var filePaths = AssetDatabase.FindAssets($"t:{typeof(Material).Name}", new string[] { folderPath });
        var assets = new Material[filePaths.Length];
        for (int i = 0; i < assets.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(filePaths[i]));
        }
        return assets;
    }
    public static void ReplacePropertyValue(Object folder, Trigger trigger, Action action)
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
    public static void CopyPasteProperties(Material from, Object[] to, string[] properties)
    {

        var propertiesFrom = MaterialEditor.GetMaterialProperties(new Material[] { from });

        foreach (var folder in to)
        {
            if (folder is GameObject gameObject)
            {
                var renderers= gameObject.GetComponentsInChildren<Renderer>(true);

                foreach (var renderer in renderers)
                {
                    var materials = renderer.sharedMaterials;
                    //Debug.Log($"materials.Length:{materials.Length}");


                    CopyPasteProperties(from, materials, properties);
                }
                continue;
            }
            else
            {

                var materials = GetFolderToMaterials(folder);
                //Debug.Log($"materials.Length:{materials.Length}");


                CopyPasteProperties(from, materials, properties);
            }
        }

    }
    public static void CopyPasteProperties(Material from, Material[] toMaterials, string[] properties)
    {

        var propertiesFrom = MaterialEditor.GetMaterialProperties(new Material[] { from });

        Debug.Log($"materials.Length:{toMaterials.Length}");

        foreach (var material in toMaterials)
        {
            var materialProperties = MaterialEditor.GetMaterialProperties(new Material[] { material });
            foreach (var property in materialProperties)
            {
                if (System.Array.FindIndex(properties, x => x == property.name) < 0)
                {
                    continue;
                }


                switch (property.type)
                {
                    case MaterialProperty.PropType.Color:
                        {
                            var value = from.GetColor(property.name);
                            Debug.Log($"{material.name}.{property.name}:{property.colorValue}->{value}");
                            property.colorValue = value;
                        }
                        break;
                    case MaterialProperty.PropType.Vector:
                        break;
                    case MaterialProperty.PropType.Texture:
                        {

                            var value = from.GetTexture(property.name);
                            Debug.Log($"{material.name}.{property.name}:{property.textureValue}->{value}");
                            property.textureValue = value;
                        }
                        break;
                    case MaterialProperty.PropType.Int:
                        {

                            var value = from.GetInt(property.name);
                            Debug.Log($"{material.name}.{property.name}:{property.intValue}->{value}");
                            property.intValue = value;
                        }
                        break;
                    case MaterialProperty.PropType.Float:
                    case MaterialProperty.PropType.Range:
                    default:
                        {

                            var value = from.GetFloat(property.name);
                            Debug.Log($"{material.name}.{property.name}:{property.floatValue}->{value}");
                            property.floatValue = value;
                        }
                        break;
                }

            }
        }

    }
    static void CopyClass<T>(T source, T target)
    {
        //lock (source)
        {
            var fields = typeof(T).GetFields();
            foreach (var field in fields)
            {
                if (field.Name == "m_InstanceID")
                {
                    continue;
                }
                field.SetValue(target, field.GetValue(source));
            }

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (!property.CanWrite || !property.CanRead)
                {
                    continue;
                }
                property.SetValue(target, property.GetValue(source));
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



        var allReady = true;


        serializedObject.Update();
        {
            EditorGUILayout.Space();
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(mode)));
            }
        }
        serializedObject.ApplyModifiedProperties();

        switch (mode)
        {
            case Mode.CopyPaste:
                if (targetCP == (TargetCopyPaste)(-1))
                {
                    targetCP = TargetCopyPaste.Outline;
                    properties = propertyNamesOutline.ToArray();
                }
                EditorGUILayout.Space();
                serializedObject.Update();
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(from)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(to)));
                    EditorGUILayout.Space();

                    EditorGUILayout.Space();
                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(targetCP)));
                    }
                }
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        switch (targetCP)
                        {
                            case TargetCopyPaste.Outline:
                                properties = propertyNamesOutline.ToArray();
                                break;
                            case TargetCopyPaste.Light:
                                properties = propertyNamesLight.ToArray();
                                break;
                            case TargetCopyPaste.Shadow:
                                properties = propertyNamesShadow.ToArray();
                                break;
                            case TargetCopyPaste.RimLight:
                                properties = rimlightProperties.ToArray();
                                break;
                            case TargetCopyPaste.RimShade:
                                properties = rimShadeProperties.ToArray();
                                break;
                            case TargetCopyPaste.BackLight:
                                properties = backlightProperties.ToArray();
                                break;
                            case TargetCopyPaste.Reflection:
                                properties = reflectionProperties.ToArray();
                                break;
                            case TargetCopyPaste.Matcap:
                                properties = matCapProperties.ToArray();
                                break;
                            default:
                                break;
                        }
                    }
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(properties)));
                    if (from == null)
                    {
                        allReady = false;
                    }
                    if ((to != null)&& (System.Array.FindAll(to,x=> x != null).Length==0))
                    {
                        allReady = false;
                    }
                }
                serializedObject.ApplyModifiedProperties();


                GUI.enabled = allReady;
                if (GUILayout.Button("Paste"))
                {
                    CopyPasteProperties(from, to, properties);
                }



                break;
            case Mode.TriggerAction:
                if (targetTA == (TargetTriggerAction)(-1))
                {
                    targetTA = TargetTriggerAction.Outline;
                    trigger = outlineTA.trigger;
                    action = outlineTA.action;
                }
                serializedObject.Update();
                {
                    EditorGUILayout.Space();
                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(targetTA)));
                    }
                }
                serializedObject.ApplyModifiedProperties();

                serializedObject.Update();
                {
                    if (EditorGUI.EndChangeCheck())
                    {
                        var ta = new TriggerAction();
                        CopyClass(outlineTA, ta);
                        switch (targetTA)
                        {
                            case TargetTriggerAction.Outline:
                                break;
                            case TargetTriggerAction.LightMinLimit:
                                CopyClass(lightMinLimitTA, ta);
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
                    ReplacePropertyValue(folder, trigger, action);
                }
                break;
            default:
                break;
        }
        GUI.enabled = true;
    }
}
#endif