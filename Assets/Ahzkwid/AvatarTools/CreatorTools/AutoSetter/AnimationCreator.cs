
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

using Ahzkwid;
using Ahzkwid.AvatarTool;
using UnityEditor;
using UnityEngine.Animations;



using UnityEditor;
using VRC.SDK3.Avatars.Components;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Data.SqlClient;


[CustomEditor(typeof(AnimationCreator))]
public class AutoDescriptorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        /*

        var AnimationCreator = (AnimationCreator)target;
        foreach (var actionTrigger in AnimationCreator.actionTriggers)
        {
            foreach (var action in actionTrigger.actions)
            {
                if (action.value is GameObject)
                {
                    var gameObject = action.value as GameObject;
                    action.value = gameObject.GetComponent<Animator>();
                }



                if ((action.value is Animator) || (action.value is RuntimeAnimatorController))
                {
                    switch (action.target)
                    {
                        case AnimationCreator.Target.PlayableLayersBase:
                        case AnimationCreator.Target.PlayableLayersAddtive:
                        case AnimationCreator.Target.PlayableLayersGesture:
                        case AnimationCreator.Target.PlayableLayersAction:
                        case AnimationCreator.Target.PlayableLayersFX:
                            break;
                        default:
                            var name = action.value.name.ToLower();
                            action.target = AnimationCreator.Target.PlayableLayersFX;
                            if (name.Contains("base"))
                            {
                                action.target = AnimationCreator.Target.PlayableLayersBase;
                            }
                            if (name.Contains("addtive"))
                            {
                                action.target = AnimationCreator.Target.PlayableLayersAddtive;
                            }
                            if (name.Contains("gesture"))
                            {
                                action.target = AnimationCreator.Target.PlayableLayersGesture;
                            }
                            if (name.Contains("action"))
                            {
                                action.target = AnimationCreator.Target.PlayableLayersAction;
                            }
                            break;
                    }
                }
                if (action.value is VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters)
                {
                    action.target = AnimationCreator.Target.ExpressionsParameters;
                }
                if (action.value is VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu)
                {
                    action.target = AnimationCreator.Target.ExpressionsMenu;
                }
            }
        }

        */
        serializedObject.Update();
        {
            GUI.enabled = false;
            {
                EditorGUILayout.LabelField("you want download this tool ?");
                EditorGUILayout.LabelField("https://ahzkwid.booth.pm/items/5463945");
            }
            GUI.enabled = true;
        }
        serializedObject.ApplyModifiedProperties();
    }
}




public class AnimationCreator : MonoBehaviour
{
    public enum Option
    {
        Simple, Extension
    }
    //public string header = "";
    //public string allParameter = "";
    public All all;
    //public Option option;
    //public VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters prameters;
    public Transform animationRoot = null;
    //public Object[] targets;

    //public bool createVRCMenus = true;
    //public DefaultAsset exportForder;
    //public bool floatParameter = true;
    //public bool inverse = false;

    [System.Serializable]
    public class All
    {
        public string parameter = "";
        public bool inverse = false;
    }


    AssetManager.FileOptions GetFileOptions()
    {
        var fileOption = AssetManager.FileOptions.Normal;
        if (EditorApplication.isPlaying)
        {
            fileOption = AssetManager.FileOptions.NoSave;
        }
        else
        {
            fileOption = AssetManager.FileOptions.TempSave;
        }
        return fileOption;
    }
    void SaveAsset(Object asset)
    {
        AssetManager.SaveAsset(asset, GetFileOptions());
        /*
        if (exportForder != null)
        {
            AssetManager.SaveAsset(asset, exportForder);
        }
        else
        {
            //var fileOption = AssetManager.FileOptions.Normal;
            AssetManager.SaveAsset(asset, GetFileOptions());
        }
        */
    }
    [System.Serializable]
    public class ToggleData
    {
        public string[] parameters = null;
        public Object[] targets;
        public AnimationClip clip;
        public Shirink shirink;

        /*
        public AnimationClip clipShrink;
#if !YOUR_VRCSDK3_AVATARS && !YOUR_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#else
        public VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer shrinkLayer = VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer.FX;
#endif
#endif
        */

        public bool floatParameter = true;
        public bool motionTime = false;
        public bool inverse = false;


        [System.Serializable]
        public class Shirink
        {
            public AnimationClip clip;
            /*
#if !YOUR_VRCSDK3_AVATARS && !YOUR_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#else
            public VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer blendableLayer = VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer.FX;
#endif
#endif
            */
        }


        /*
        public AnimationClip CreateAnimationClip(AssetManager.FileOptions fileOption)
        {
            return AnimationCreateTool.CreateAnimationClip(targets, fileOption);
        }
        public AnimationClip CreateAnimationClip(Object forder)
        {
            return AnimationCreateTool.CreateAnimationClip(targets, forder);
        }
        */
        /*
        public AnimationClip CreateAnimationClip()
        {
            if (floatParameter)
            {
                return AnimationCreator.CreateAnimationClip(targets,ClipValue.Double);
            }
            return AnimationCreator.CreateAnimationClip(targets, ClipValue.Zero);
        }
        */
        public string Parameter
        {
            get
            {
                return parameters.FirstOrDefault();
            }
            set
            {
                if (parameters == null)
                {
                    parameters = new string[1];
                }
                if (parameters.Length == 0)
                {
                    parameters = new string[1];
                }

                parameters[0] = value;
            }
        }
        public AnimatorControllerLayer CreateLayer(AnimationClip clip, Transform rootParent, Transform rootChild, int layerIndex, All all)
        {
            targets = System.Array.FindAll(targets, x => x != null);
            float GetThreshold(bool isAllParameter)
            {
                var threshold = 0f;
                if (isAllParameter)
                {
                    return threshold;
                }
                if (floatParameter)
                {
                    threshold = 0.5f;
                }
                return threshold;
            }
            AnimatorConditionMode GetMode(int i,bool isAllParameter)
            {
                var inverse =this.inverse;
                if (isAllParameter)
                {
                    inverse = all.inverse;
                }

                var mode = AnimatorConditionMode.If;
                if (i > 0)
                {
                    mode = AnimatorConditionMode.IfNot;
                }
                if (inverse)
                {
                    if (mode == AnimatorConditionMode.If)
                    {
                        mode = AnimatorConditionMode.IfNot;
                    }
                    else
                    {
                        mode = AnimatorConditionMode.If;
                    }
                }
                if (isAllParameter)
                {
                    return mode;
                }
                if (floatParameter)
                {
                    mode = AnimatorConditionMode.Greater;
                    if (i > 0)
                    {
                        mode = AnimatorConditionMode.Less;
                    }
                    if (inverse)
                    {
                        if (mode == AnimatorConditionMode.Greater)
                        {
                            mode = AnimatorConditionMode.Less;
                        }
                        else
                        {
                            mode = AnimatorConditionMode.Greater;
                        }
                    }
                }
                return mode;
            }
            var parameters = this.parameters.ToArray();
            if (string.IsNullOrWhiteSpace(all.parameter)==false)
            {
                parameters = parameters.Append(all.parameter).ToArray();
            }
            if (parameters.Length == 0)
            {
                return null;
            }
            if (clip == null)
            {
                if (targets.Length == 0)
                {
                    return null;
                }
            }

            var newLayer = new AnimatorControllerLayer();
            {
                var parameter = parameters.FirstOrDefault();
                newLayer.name = parameter;
            }
            newLayer.defaultWeight = 1;

            newLayer.stateMachine = new AnimatorStateMachine();

            /*
            if (floatParameter)
            {
                var parameter = parameters.First();
                var newState = new AnimatorState();
                newState.motion = CreateAnimationClip( ClipValue.Double);
                newState.name = parameter;
                //controller.AddParameter(toggleData.parameter, AnimatorControllerParameterType.Float);
                newState.timeParameterActive = true;
                newState.timeParameter = parameter;

                newLayer.stateMachine.AddState(newState, Vector3.right * 300);
                newLayer.stateMachine.name = parameter;
                newLayer.stateMachine.defaultState = newState;
            }
            else
            */



            var useMosionTime = motionTime;


            bool isShirink = false;
            if (clip != null)
            {
                if (clip == shirink.clip)
                {
                    isShirink = true;
                }
            }
            if (isShirink)
            {
                useMosionTime = false;
            }
            if (useMosionTime)
            {
                var parameter = parameters.FirstOrDefault();
                var newState = new AnimatorState();
                if (clip == null)
                {
                    newState.motion = CreateAnimationClip(ClipValue.Double);
                }
                else
                {
                    newState.motion = AnimatorCombiner.ReplaceAnimations(clip, rootParent, rootChild);
                }
                newState.name = parameter;
                //controller.AddParameter(toggleData.parameter, AnimatorControllerParameterType.Float);
                newState.timeParameterActive = true;
                newState.timeParameter = parameter;

                newLayer.stateMachine.AddState(newState, Vector3.right * 300);
                newLayer.stateMachine.name = parameter;
                newLayer.stateMachine.defaultState = newState;
            }
            else
            {
                //var parameter = parameters.First();
                var newStates = new List<AnimatorState>();
                for (int i = 0; i < 2; i++)
                {
                    var name = parameters.First();
                    if (i > 0)
                    {
                        name += " On";
                    }
                    else
                    {
                        name += " Off";
                    }

                    var newState = new AnimatorState();
                    if (clip==null)
                    {
                        newState.motion = CreateAnimationClip((ClipValue)i);
                    }
                    else
                    {
                        newState.motion = AnimatorCombiner.ReplaceAnimations(clip, rootParent, rootChild);
                        if (i==0)
                        {
                            newState.motion = InverseAnimationClip(newState.motion as AnimationClip, isShirink);
                            
                        }
                    }
                    newState.name = name;

                    newLayer.stateMachine.AddState(newState, Vector3.right * 300 + Vector3.up * 100 * i);
                    newLayer.stateMachine.name = name;
                    newLayer.stateMachine.defaultState = newState;


#if !YOUR_VRCSDK3_AVATARS && !YOUR_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
#if UDON
#else

                    if (layerIndex >= 0)
                    {
                        var behaviour = newState.AddStateMachineBehaviour(typeof(VRCAnimatorLayerControl));
                        if (behaviour is VRC.SDK3.Avatars.Components.VRCAnimatorLayerControl layerControl)
                        {
                            layerControl.layer = layerIndex;
                            //layerControl.playable = shirink.blendableLayer;
                            layerControl.playable = VRC.SDKBase.VRC_AnimatorLayerControl.BlendableLayer.FX;
                            layerControl.goalWeight = i;
                        }
                    }

#endif
#endif



                    newStates.Add(newState);
                }
                for (int i = 0; i < 2; i++)
                {
                    /*
                    var mode = AnimatorConditionMode.If;
                    if (i > 0)
                    {
                        mode = AnimatorConditionMode.IfNot;
                    }
                    if (inverse)
                    {
                        if (mode == AnimatorConditionMode.If)
                        {
                            mode = AnimatorConditionMode.IfNot;
                        }
                        else
                        {
                            mode = AnimatorConditionMode.If;
                        }
                    }
                    if (floatParameter)
                    {
                        mode = AnimatorConditionMode.Greater;
                        if (i > 0)
                        {
                            mode = AnimatorConditionMode.Less;
                        }
                        if (inverse)
                        {
                            if (mode == AnimatorConditionMode.Greater)
                            {
                                mode = AnimatorConditionMode.Less;
                            }
                            else
                            {
                                mode = AnimatorConditionMode.Greater;
                            }
                        }
                    }
                    */

                    /*
                    var threshold = 0f;
                    if (floatParameter)
                    {
                        threshold = 0.5f;
                    }
                    */


                    {
                        var from = newStates[i];
                        var to = newStates[1 - i];

                        //or은 트랜지션이 여러개
                        //and는 컨디션이 여러개
                        var and = false;
                        if (i==0)
                        {
                            and = true;
                        }
                        if (and)
                        {
                            var newTransition = from.AddTransition(to);
                            newTransition.exitTime = 0;
                            newTransition.duration = 0;

                            foreach (var parameter in parameters)
                            {
                                var isAllParameter = parameter == all.parameter;
                                var mode = GetMode(i, isAllParameter);
                                var threshold = GetThreshold(isAllParameter);
                                newTransition.AddCondition(mode, threshold, parameter);
                            }
                        }
                        else
                        {
                            foreach (var parameter in parameters)
                            {
                                var newTransition = from.AddTransition(to);
                                newTransition.exitTime = 0;
                                newTransition.duration = 0;


                                var isAllParameter = parameter == all.parameter;
                                var mode = GetMode(i, isAllParameter);
                                var threshold = GetThreshold(isAllParameter);
                                newTransition.AddCondition(mode, threshold, parameter);
                            }
                        }
                    }
                }
            }

            return newLayer;
        }
        public AnimationClip InverseAnimationClip(AnimationClip clip,bool isShrink=false)
        {

            if (clip == null)
            {
                return null;
            }
            var newClip = new AnimationClip();
            newClip.name = clip.name + " inverse";



            var bindings = AnimationUtility.GetCurveBindings(clip);
            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);

                var newCurve = new AnimationCurve();
                /*
                foreach (var key in curve.keys)
                {
                    newCurve.AddKey(new Keyframe(key.time, 0f, 0f, 0f));
                }
                */
                if (curve.keys.Length > 0)
                {
                    var lastKey = curve.keys.Last();
                    var value = 0f;
                    if (lastKey.value==0f)
                    {
                        value = 100f;
                        if (isShrink)
                        {
                            if (binding.propertyName.Contains("blendShape"))
                            {
                                if (binding.type==typeof(SkinnedMeshRenderer))
                                {
                                    value = 0;
                                }
                            }
                        }
                    }
                    newCurve.AddKey(new Keyframe(0f, value, 0f, 0f));
                }

                AnimationUtility.SetEditorCurve(newClip, binding, newCurve);
            }

            return newClip;
        }

        public AnimationClip CreateAnimationClip(ClipValue clipValue)
        {

            targets = System.Array.FindAll(targets, target => target != null);
            if (targets.Length == 0)
            {
                return null;
            }
            var newClip = new AnimationClip();
            newClip.name = targets.First().name;

            switch (clipValue)
            {
                case ClipValue.Zero:
                    newClip.name += " Off";
                    break;
                case ClipValue.One:
                    newClip.name += " On";
                    break;
                case ClipValue.Double:
                    break;
                default:
                    break;
            }




            foreach (var target in targets)
            {
                var transform = target as Transform;
                if (transform == null)
                {
                    var property = target.GetType().GetProperty("transform");
                    if (property == null)
                    {
                        continue;
                    }
                    transform = property.GetValue(target) as Transform;
                }
                if (transform == null)
                {
                    continue;
                }



                var root = ObjectPath.GetVRCRoot(transform, ObjectPath.VRCRootSearchOption.IncludeVRCRoot);
                //var text = "";
                //text = binding.path;
                if (root != null)
                {
                    var newBinding = new EditorCurveBinding();
                    //newBinding.type = typeof(Behaviour);
                    newBinding.type = target.GetType();
                    newBinding.path = ObjectPath.GetPath(transform, root);
                    newBinding.propertyName = "m_IsActive";
                    if (target is Component)
                    {
                        newBinding.propertyName = "m_Enabled";
                    }
                    if (target is RotationConstraint)
                    {
                        //newBinding.propertyName = "m_Weight";
                    }
                    var curve = new AnimationCurve();
                    switch (clipValue)
                    {
                        case ClipValue.Zero:
                            curve.AddKey(0f, 0f);
                            break;
                        case ClipValue.One:
                            curve.AddKey(0f, 1f);
                            break;
                        case ClipValue.Double:
                            if (inverse)
                            {
                                curve.AddKey(0f, 1f);
                                curve.AddKey(1f / 60f, 0f);
                            }
                            else
                            {
                                curve.AddKey(0f, 0f);
                                curve.AddKey(1f / 60f, 1f);
                            }
                            break;
                        default:
                            break;
                    }
                    AnimationUtility.SetEditorCurve(newClip, newBinding, curve);
                }


            }
            return newClip;
        }

    }
    public ToggleData[] toggleDatas;







    public enum ClipValue
    {
        Zero, One, Double
    }


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AnimationCreateTool))]


    /*
    public static AnimationClip CreateAnimationClip(Object[] targets, Object forder)
    {
        var newClip = CreateAnimationClip(targets);
        AssetManager.SaveAsset(newClip, forder);
        return newClip;
    }
    public static AnimationClip CreateAnimationClip(Object[] targets, AssetManager.FileOptions fileOption)
    {
        var newClip = CreateAnimationClip(targets);
        AssetManager.SaveAsset(newClip, fileOption);
        return newClip;
    }
    */
    //public static AnimatorControllerLayer[] CreateLayers(ToggleData[] toggleDatas, AssetManager.FileOptions fileOption)
    //{

    //    var layers = new List<AnimatorControllerLayer>();

    //    foreach (var toggleData in toggleDatas)
    //    {
    //        /*
    //        var newLayer = new AnimatorControllerLayer();
    //        newLayer.name = toggleData.parameter;
    //        newLayer.defaultWeight = 1;
    //        newLayer.stateMachine = new AnimatorStateMachine();
    //        var newState = new AnimatorState();
    //        newState.motion = toggleData.CreateAnimationClip(fileOption);
    //        newState.name = toggleData.parameter;
    //        //controller.AddParameter(toggleData.parameter, AnimatorControllerParameterType.Float);
    //        newState.timeParameterActive = true;
    //        newState.timeParameter = toggleData.parameter;

    //        newLayer.stateMachine.AddState(newState, Vector3.right * 100);

    //        newLayer.stateMachine.defaultState = newState;
    //        layers.Add(newLayer);
    //        */
    //        var layer = toggleData.CreateLayer();
    //        /*
    //        foreach (var state in layer.stateMachine.states)
    //        {
    //            AssetManager.SaveAsset(state.state.motion, fileOption);
    //        }
    //        */

    //        layers.Add(layer);
    //    }

    //    return layers.ToArray();


    //}
    public static AnimatorControllerLayer[] CreateLayers(ToggleData[] toggleDatas, Transform rootParent, Transform rootChild,All all)
    {
        var layers = new List<AnimatorControllerLayer>();


        foreach (var toggleData in toggleDatas)
        {

            for (int i = 0; i < 3; i++)
            {
                AnimatorControllerLayer layer = null;
                switch (i)
                {
                    case 0:
                        layer = toggleData.CreateLayer(null, null, null,-1, all);
                        break;
                    case 1:
                        if (toggleData.clip != null)
                        {
                            layer = toggleData.CreateLayer(toggleData.clip, rootParent, rootChild,-1, all);
                            layer.name += " clip";
                        }
                        break;
                    case 2:
                        if (toggleData.shirink.clip != null)
                        {
                            layer = toggleData.CreateLayer(toggleData.shirink.clip, null, null, layers.Count, all);
                            layer.name += " clipShrink";
                        }
                        break;
                    default:
                        break;
                }
                if (layer != null)
                {
                    layers.Add(layer);
                }
            }
        }

        return layers.ToArray();
    }
    /*
    public AnimatorController CreateAnimator(AssetManager.FileOptions fileOption)
    {
        return CreateAnimator(toggleDatas, fileOption);
    }
    */
//    public void Run()
//    {
//        var datas = toggleDatas;
//        //switch (option)
//        //{
//        //    case Option.Simple:
//        //        datas = new AnimationCreator.ToggleData[] { };
//        //        targets = System.Array.FindAll(targets, x => x != null);
//        //        datas = System.Array.ConvertAll(targets, target =>
//        //        {
//        //            var data = new AnimationCreator.ToggleData();
//        //            data.parameter = header + target.name;
//        //            data.parameter = data.parameter.Replace(" ", "_");
//        //            data.targets = new Object[] { target };
//        //            data.floatParameter = floatParameter;
//        //            data.inverse = inverse;
//        //            return data;
//        //        });
//        //        break;
//        //    case Option.Extension:
//        //        break;
//        //    default:
//        //        break;
//        //}
//        var animatorController = AnimationCreator.CreateAnimator(datas, animationRoot);
//        //if (exportForder != null)
//        //{
//        //    animatorController.name = exportForder.name;
//        //}
//        if (animationRoot != null)
//        {
//            animatorController.name = animationRoot.name;
//        }
//        SaveAsset(animatorController);

//        /*
//#if !YOUR_VRCSDK3_AVATARS && !YOUR_VRCSDK3_WORLDS && VRC_SDK_VRCSDK3
//#if UDON
//#else
//        if (createVRCMenus)
//        {

//            //파라미터 생성
//            var expressionParameters = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters();
//            var parameters = new List<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter>();
//            foreach (var data in datas)
//            {
//                if (data.targets.Length == 0)
//                {
//                    continue;
//                }
//                var parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter();
//                parameter.valueType = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.ValueType.Bool;
//                var target = data.targets.First();
//                if (target != null)
//                {
//                    parameter.defaultValue = 1f;
//                    if (target is GameObject gameObject)
//                    {
//                        parameter.defaultValue = gameObject.activeSelf ? 1f : 0f;
//                    }


//                    if (target is Component component)
//                    {
//                        var enabled = typeof(Component).GetField("m_Enabled").GetValue(component);
//                        if (enabled is bool _bool)
//                        {
//                            parameter.defaultValue = _bool ? 1f : 0f;

//                        }
//                    }

//                }
//                parameter.name = data.parameter;
//                parameters.Add(parameter);

//            }
//            expressionParameters.parameters = parameters.ToArray();
//            expressionParameters.name = "Parameters " + animatorController.name;
//            SaveAsset(expressionParameters);





//            //메뉴 생성
//            var expressionsMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu();
//            expressionsMenu.Parameters = expressionParameters;
//            var controls = new List<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control>();
//            foreach (var data in datas)
//            {
//                if (data.targets.Length == 0)
//                {
//                    continue;
//                }
//                var control = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control();
//                control.type = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.Toggle;
//                var target = data.targets.First();
//                if (target != null)
//                {
//                    control.name = target.name;
//                }
//                if (expressionsMenu.Parameters != null)
//                {
//                    var parameter = System.Array.Find(expressionsMenu.Parameters.parameters, x => x.name == data.parameter);
//                    control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter();
//                    control.parameter.name = parameter.name;
//                }

//                controls.Add(control);

//            }
//            expressionsMenu.controls = controls;
//            expressionsMenu.name = "Menu " + animatorController.name;
//            SaveAsset(expressionsMenu);







//        }

//#endif
//#endif
//        */
//    }
    public static VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu CreateMenu(ToggleData[] datas)
    {

        var expressionParameters = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters();
        var parameters = new List<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter>();
        foreach (var data in datas)
        {
            if (data.targets.Length == 0)
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
                    parameter.defaultValue = gameObject.activeSelf ? 1f : 0f;
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
            parameter.name = data.parameters.First();
            parameters.Add(parameter);

        }
        expressionParameters.parameters = parameters.ToArray();
        //expressionParameters.name = "Parameters " + animatorController.name;
        //SaveAsset(expressionParameters);




        var expressionsMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu();
        expressionsMenu.Parameters = expressionParameters;
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
            if (expressionsMenu.Parameters != null)
            {
                var parameter = System.Array.Find(expressionsMenu.Parameters.parameters, x => x.name == data.Parameter);
                control.parameter = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.Parameter();
                control.parameter.name = parameter.name;
            }

            controls.Add(control);
        }
        expressionsMenu.controls = controls;
        //expressionsMenu.name = "Menu " + animatorController.name;
        //SaveAsset(expressionsMenu);
        return expressionsMenu;
    }
    public AnimatorController CreateAnimator()
    {
        return this.CreateAnimator(toggleDatas);
    }
    public AnimatorController CreateAnimator(ToggleData[] toggleDatas)
    {
        var root = ObjectPath.GetVRCRoot(transform);
        var avatarDescriptor = root.GetComponentInChildren<VRCAvatarDescriptor>();

        return CreateAnimator(toggleDatas, avatarDescriptor.transform, animationRoot, all);
    }
    public static AnimatorController CreateAnimator(ToggleData[] toggleDatas, Transform rootParent, Transform rootChild,All all)
    {
        var controller = new AnimatorController();



        foreach (var toggleData in toggleDatas)
        {
            var parameters = toggleData.parameters;
            if (parameters == null)
            {
                if (toggleData.targets?.Length > 0)
                {
                    parameters = new string[] { toggleData.targets.First().ToString() };
                }

                if (parameters?.Length > 0)
                {
                }
                else
                {
                    parameters = new string[] { toggleData.clip.ToString() };
                }
            }
            foreach (var parameter in parameters)
            {
                if (controller.parameters.Any(p => p.name == parameter))
                {
                    continue; //이미 존재하면 스킵
                }
                if (toggleData.floatParameter)
                {
                    controller.AddParameter(parameter, AnimatorControllerParameterType.Float);
                }
                else
                {
                    controller.AddParameter(parameter, AnimatorControllerParameterType.Bool);
                }
            }
        }
        if (string.IsNullOrWhiteSpace(all.parameter) == false)
        {
            if (controller.parameters.Any(p => p.name == all.parameter)==false)
            {
                controller.AddParameter(all.parameter, AnimatorControllerParameterType.Bool);
            }
        }

        var layers = new List<AnimatorControllerLayer>();

        /*
        foreach (var toggleData in toggleDatas)
        {
            var newLayer = new AnimatorControllerLayer();
            newLayer.name = toggleData.parameter;
            newLayer.defaultWeight = 1;
            newLayer.stateMachine = new AnimatorStateMachine();
            var newState = new AnimatorState();
            newState.motion = toggleData.CreateAnimationClip(fileOption);
            newState.name = toggleData.parameter;
            controller.AddParameter(toggleData.parameter, AnimatorControllerParameterType.Float);
            newState.timeParameterActive = true;
            newState.timeParameter = toggleData.parameter;

            newLayer.stateMachine.AddState(newState, Vector3.right * 100);

            newLayer.stateMachine.defaultState = newState;
            layers.Add(newLayer);
        }
        controller.layers = layers.ToArray();

        */

        controller.layers = CreateLayers(toggleDatas, rootParent, rootChild, all);

        //var clip = CreateAnimationClip(targets, fileOption);


        return controller;
    }
    /*
    public static AnimatorController CreateAnimator(ToggleData[] toggleDatas, DefaultAsset forder)
    {
        var animatorController = CreateAnimator(toggleDatas);
        animatorController.name = forder.name;
        //SaveAnimator(animatorController, forder);
        return animatorController;
    }
    public static AnimatorController CreateAnimator(ToggleData[] toggleDatas, AssetManager.FileOptions fileOption)
    {
        var animatorController = CreateAnimator(toggleDatas);
        //SaveAnimator(animatorController, fileOption);
        return animatorController;
    }
    */
    /*
    public static void SaveAnimator(AnimatorController controller, DefaultAsset forder)
    {
        foreach (var layer in controller.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                AssetManager.SaveAsset(state.state.motion, forder);
            }
        }
        AssetManager.SaveAsset(controller, forder);
    }
    public static void SaveAnimator(AnimatorController controller, AssetManager.FileOptions fileOption)
    {
        AssetManager.SaveAsset(controller, fileOption);

    }



    */














    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif