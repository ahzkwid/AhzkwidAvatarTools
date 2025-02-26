






#if UNITY_EDITOR

using UnityEngine;
using Ahzkwid.AvatarTool;


using UnityEditor;
[InitializeOnLoad]
class AnimationCreateTool : EditorWindow
{
    public enum Option
    {
        Simple, Extension
    }
    public Option option;
    public string header = "hash";
    public Object[] targets;


    public AnimationCreator.ToggleData[] toggleDatas;

    //public bool createVRCParameters = false;
    public bool createVRCMenus = true;
    public bool floatParameter = true;
    public bool inverse = false;


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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(option)));


            switch (option)
            {
                case Option.Simple:
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(header)));
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(targets)));
                    if (targets == null || targets.Length == 0)
                    {
                        allReady = false;
                    }
                    break;
                case Option.Extension:
                    EditorGUILayout.Space();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(toggleDatas)));
                    if (toggleDatas == null || toggleDatas.Length == 0)
                    {
                        allReady = false;
                    }
                    break;
                default:
                    break;
            }
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(exportForder)));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createVRCMenus)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(floatParameter)));
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(inverse)));
            EditorGUILayout.Space();
            //EditorGUILayout.Space();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createVRCParameters)));
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
            var datas= toggleDatas;
            switch (option)
            {
                case Option.Simple:
                    datas = new AnimationCreator.ToggleData[] { };
                    targets = System.Array.FindAll(targets, x => x != null);
                    datas = System.Array.ConvertAll(targets, target =>
                    {
                        var data = new AnimationCreator.ToggleData();
                        data.Parameter = header + target.name;
                        data.Parameter = data.Parameter.Replace(" ","_");
                        data.targets = new Object[] { target };
                        data.floatParameter = floatParameter;
                        data.inverse = inverse;
                        return data;
                    });
                    break;
                case Option.Extension:
                    break;
                default:
                    break;
            }
            var animatorController= AnimationCreator.CreateAnimator(datas,null,null,null);
            if (exportForder!=null)
            {
                animatorController.name = exportForder.name;
            }
            SaveAsset(animatorController);

            var expressionsMenu = AnimationCreator.CreateMenu(datas);
            var expressionParameters = expressionsMenu.Parameters;
            expressionParameters.name = "Parameters " + animatorController.name;
            expressionsMenu.name = "Menu " + animatorController.name;
            SaveAsset(expressionsMenu);
            /*
#if !YOUR_VRCSDK3_AVATARS && !YOUR_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#else
            if (createVRCMenus)
            {

                //파라미터 생성
                var expressionParameters = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters();
                var parameters = new List<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter>();
                foreach (var data in datas)
                {
                    if (data.targets.Length==0)
                    {
                        continue;
                    }
                    var parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter();
                    parameter.valueType = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType.Bool;
                    var target = data.targets.First();
                    if (target != null)
                    {
                        parameter.defaultValue = 1f;
                        if (target is GameObject gameObject)
                        {
                            parameter.defaultValue = gameObject.activeSelf ? 1f:0f;
                        }


                        if (target is Component component)
                        {
                            var enabled = typeof(Component).GetField("m_Enabled").GetValue(component);
                            if (enabled is bool _bool)
                            {
                                parameter.defaultValue = _bool ? 1f : 0f;

                            }
                        }

                    }
                    parameter.name = data.parameter;
                    parameters.Add(parameter);

                }
                expressionParameters.parameters = parameters.ToArray();
                expressionParameters.name = "Parameters " + animatorController.name;
                SaveAsset(expressionParameters);





                //메뉴 생성
                var expressionsMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu();
                expressionsMenu.Parameters= expressionParameters;
                var controls = new List<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control>();
                foreach (var data in datas)
                {
                    if (data.targets.Length == 0)
                    {
                        continue;
                    }
                    var control = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control();
                    control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle;
                    var target = data.targets.First();
                    if (target != null)
                    {
                        control.name = target.name;
                    }
                    if (expressionsMenu.Parameters!=null)
                    {
                        var parameter = System.Array.Find(expressionsMenu.Parameters.parameters, x => x.name == data.parameter);
                        control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter();
                        control.parameter.name = parameter.name;
                    }

                    controls.Add(control);

                }
                expressionsMenu.controls = controls;
                expressionsMenu.name = "Menu " + animatorController.name;
                SaveAsset(expressionsMenu);








            }

#endif
#endif
            */

        }
        GUI.enabled = true;
    }
    void SaveAsset(Object asset)
    {
        if (exportForder != null)
        {
            AssetManager.SaveAsset(asset, exportForder);
        }
        else
        {
            var fileOption = AssetManager.FileOptions.Normal;
            AssetManager.SaveAsset(asset, fileOption);
        }
    }

}
#endif