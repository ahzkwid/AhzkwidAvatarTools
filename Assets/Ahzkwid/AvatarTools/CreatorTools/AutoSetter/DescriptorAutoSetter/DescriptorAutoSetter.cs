
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Ahzkwid
{
#if UNITY_EDITOR
    using UnityEditor;

    [CustomEditor(typeof(DescriptorAutoSetter))]
    public class DescriptorAutoSetterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();




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
#endif


    [ExecuteInEditMode]
    public class DescriptorAutoSetter : MonoBehaviour
    {
        public enum Target
        {
            PlayableLayersBase,
            PlayableLayersAddtive,
            PlayableLayersGesture,
            PlayableLayersAction,
            PlayableLayersFX,

            ExpressionsMenu,
            ExpressionsParameters
        }
        public static VRCAvatarDescriptor.AnimLayerType TargetToAnimLayerType(Target target)
        {
            var animLayerType = (VRCAvatarDescriptor.AnimLayerType)(-1);
            switch (target)
            {
                case Target.PlayableLayersBase:
                    animLayerType = VRCAvatarDescriptor.AnimLayerType.Base;
                    break;
                case Target.PlayableLayersAddtive:
                    animLayerType = VRCAvatarDescriptor.AnimLayerType.Additive;
                    break;
                case Target.PlayableLayersGesture:
                    animLayerType = VRCAvatarDescriptor.AnimLayerType.Gesture;
                    break;
                case Target.PlayableLayersAction:
                    animLayerType = VRCAvatarDescriptor.AnimLayerType.Action;
                    break;
                case Target.PlayableLayersFX:
                    animLayerType = VRCAvatarDescriptor.AnimLayerType.FX;
                    break;
            }
            return animLayerType;
        }
        public enum Operator
        {
            Equal,
            NotEqual
        }
        public enum TriggersOperator
        {
            AND
            , OR
        }




        [System.Serializable]
        public class Action
        {
            public enum Option
            {
                Replace
                , Merge
            }
            public Target target;
            public Option option;
            public Object value;
            public void Run(VRCAvatarDescriptor avatarDescriptor)
            {





                switch (target)
                {
                    case Target.PlayableLayersBase:
                    case Target.PlayableLayersAddtive:
                    case Target.PlayableLayersGesture:
                    case Target.PlayableLayersAction:
                    case Target.PlayableLayersFX:
#if UNITY_EDITOR
                        for (int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
                        {
                            var animLayerType = TargetToAnimLayerType(target);
                            var baseAnimationLayer = avatarDescriptor.baseAnimationLayers[i];
                            if (baseAnimationLayer.type == animLayerType)
                            {
                                avatarDescriptor.customizeAnimationLayers = true;
                                avatarDescriptor.baseAnimationLayers[i].isDefault = false;
                                {

                                    var animatorController = (RuntimeAnimatorController)value;
                                    if (option == Option.Merge)
                                    {
                                        animatorController = AnimatorCombiner.CombineAnimators(avatarDescriptor.baseAnimationLayers[i].animatorController, animatorController, avatarDescriptor.transform);
                                        //AnimatorCombiner.ExportAnimatorController(animatorController, "Assets/Temp/TempAnimator.controller");
                                        //animatorController=AnimatorCombiner.SaveAsset(animatorController) as RuntimeAnimatorController;
                                        AnimatorCombiner.SaveAsset(animatorController);
                                    }
                                    avatarDescriptor.baseAnimationLayers[i].animatorController = animatorController;
                                }
                            }
                        }
#endif

                        break;
                    case Target.ExpressionsMenu:
                        avatarDescriptor.customExpressions = true;
                        {
                            var expressionsMenuValue = (VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu)value;
                            var expressionsMenu = expressionsMenuValue;
                            if (option == Option.Merge)
                            {
                                //expressionsMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu();
                                if (avatarDescriptor.expressionsMenu != null)
                                {
                                    expressionsMenu = Instantiate(avatarDescriptor.expressionsMenu);
                                    var name = $"{avatarDescriptor.expressionsMenu?.name}+{expressionsMenuValue?.name}";
                                    if (name.Length > 40)
                                    {
                                        name = $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
                                    }
                                    expressionsMenu.name = name;

                                    //expressionsMenu.controls = new List<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control>();
                                    /*
                                    if (avatarDescriptor.expressionsMenu != null)
                                    {
                                        //expressionsMenu.controls.AddRange(avatarDescriptor.expressionsMenu.controls);
                                        foreach (var item in avatarDescriptor.expressionsMenu.controls)
                                        {
                                            var targetClass = item;
                                            var itemCopy = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control();
                                            var fields = targetClass.GetType().GetFields();
                                            foreach (var field in fields)
                                            {
                                                field.SetValue(itemCopy, field.GetValue(targetClass));
                                            }
                                            expressionsMenu.controls.Add(itemCopy);
                                        }
                                    }
                                    */
                                    if (expressionsMenuValue != null)
                                    {
                                        foreach (var item in expressionsMenuValue.controls)
                                        {
                                            var targetClass = item;
                                            var itemCopy = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control();
                                            var fields = targetClass.GetType().GetFields();
                                            foreach (var field in fields)
                                            {
                                                field.SetValue(itemCopy, field.GetValue(targetClass));
                                            }
                                            expressionsMenu.controls.Add(itemCopy);
                                        }
                                        //expressionsMenu.controls.AddRange(expressionsMenuValue.controls);
                                    }
                                    if (expressionsMenu.controls != null)
                                    {
                                        //expressionsMenu.controls = expressionsMenu.controls.GroupBy(x => x.name).Select(x => x.Last()).ToList();
                                    }
                                    //expressionsMenu = AnimatorCombiner.SaveAsset(expressionsMenu) as VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
                                    AnimatorCombiner.SaveAsset(expressionsMenu);
                                }
                            }
                            avatarDescriptor.expressionsMenu = expressionsMenu;
                        }
                        break;
                    case Target.ExpressionsParameters:
                        avatarDescriptor.customExpressions = true;
                        {
                            var expressionParametersValue = (VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters)value;
                            var expressionParameters = expressionParametersValue;
                            if (option == Option.Merge)
                            {
                                //expressionParameters = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters();
                                if (avatarDescriptor.expressionParameters != null)
                                {
                                    expressionParameters = Instantiate(avatarDescriptor.expressionParameters);
                                    var name = $"{avatarDescriptor.expressionParameters?.name}+{expressionParametersValue?.name}";
                                    if (name.Length > 40)
                                    {
                                        name = $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
                                    }
                                    expressionParameters.name = name;
                                    /*
                                    if (avatarDescriptor.expressionParameters?.parameters != null)
                                    {
                                        foreach (var item in avatarDescriptor.expressionParameters.parameters)
                                        {
                                            var targetClass = item;
                                            var itemCopy = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter();
                                            var fields = targetClass.GetType().GetFields();
                                            foreach (var field in fields)
                                            {
                                                field.SetValue(itemCopy, field.GetValue(targetClass));
                                            }
                                            expressionParameters.parameters.AddItem(itemCopy);
                                        }
                                    }
                                    */
                                    if (expressionParametersValue?.parameters != null)
                                    {
                                        //foreach (var item in expressionParametersValue.parameters)
                                        {
                                            /*
                                            var targetClass = item;
                                            var itemCopy = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters.Parameter();
                                            var fields = targetClass.GetType().GetFields();
                                            foreach (var field in fields)
                                            {
                                                field.SetValue(itemCopy, field.GetValue(targetClass));
                                            }
                                            */
                                            //expressionParameters.parameters= expressionParametersValue.parameters.Append(item).ToArray();
                                            //expressionParameters.parameters.AddItem(item);
                                        }
                                        expressionParameters.parameters = expressionParameters.parameters.Concat(expressionParametersValue.parameters).ToArray();
                                    }
                                    if (expressionParameters.parameters != null)
                                    {
                                        expressionParameters.parameters = expressionParameters.parameters.GroupBy(x => x.name).Select(x => x.Last()).ToArray();
                                    }
                                    //expressionParameters=AnimatorCombiner.SaveAsset(expressionParameters) as VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters;
                                    AnimatorCombiner.SaveAsset(expressionParameters);
                                }
                            }
                            if (avatarDescriptor.expressionParameters?.parameters != null)
                            {
                                Debug.Log($"parameters.Length: {avatarDescriptor.expressionParameters.parameters.Length}->{expressionParameters.parameters.Length}");
                            }
                            avatarDescriptor.expressionParameters = expressionParameters;
                        }
                        break;
                }
            }
        }
        [System.Serializable]
        public class Trigger
        {
            public Target target;
            public Operator _operator;
            public Object value;
            static Object GetDescriptorValue(VRCAvatarDescriptor avatarDescriptor, Target target)
            {
                Object targetValue = null;
                switch (target)
                {
                    case Target.PlayableLayersBase:
                    case Target.PlayableLayersAddtive:
                    case Target.PlayableLayersGesture:
                    case Target.PlayableLayersAction:
                    case Target.PlayableLayersFX:
                        if (avatarDescriptor.customizeAnimationLayers == false)
                        {
                            break;
                        }
                        foreach (var baseAnimationLayer in avatarDescriptor.baseAnimationLayers)
                        {
                            var animLayerType = TargetToAnimLayerType(target);

                            if (baseAnimationLayer.type == animLayerType)
                            {
                                if (baseAnimationLayer.isDefault)
                                {
                                    continue;
                                }
                                targetValue = baseAnimationLayer.animatorController;
                            }
                        }
                        break;
                    case Target.ExpressionsMenu:
                        if (avatarDescriptor.customExpressions == true)
                        {
                            targetValue = avatarDescriptor.expressionsMenu;
                        }
                        break;
                    case Target.ExpressionsParameters:
                        if (avatarDescriptor.customExpressions == true)
                        {
                            targetValue = avatarDescriptor.expressionParameters;
                        }
                        break;
                }
                return targetValue;
            }
            public bool Check(VRCAvatarDescriptor avatarDescriptor)
            {
                var targetValue = GetDescriptorValue(avatarDescriptor, target);
                switch (_operator)
                {
                    case Operator.Equal:
                        return targetValue == value;
                    case Operator.NotEqual:
                        return targetValue != value;
                }
                return false;
            }
        }
        [System.Serializable]
        public class ActionTrigger
        {
            public TriggersOperator triggersOperator;
            public Trigger[] triggers;
            public Action[] actions;

            public void CheckNActionStart(VRCAvatarDescriptor avatarDescriptor)
            {
                var triggerCheckReturns = System.Array.ConvertAll(triggers, x => x.Check(avatarDescriptor));

                var triggerCheckReturn = true;
                if (triggerCheckReturns.Length > 0)
                {
                    switch (triggersOperator)
                    {
                        case TriggersOperator.AND:
                            triggerCheckReturn = triggerCheckReturns.All(x => x);
                            break;
                        case TriggersOperator.OR:
                            triggerCheckReturn = triggerCheckReturns.Any(x => x);
                            break;
                    }
                }
                if (triggerCheckReturn)
                {
                    foreach (var action in actions)
                    {
                        action.Run(avatarDescriptor);
                    }
                }
            }
        }

        bool success = false;

        public bool autoDestroy = true;
        // List<BlendshapeTarget> blendshapeTargets = new List<BlendshapeTarget>();
        public List<ActionTrigger> actionTriggers = new List<ActionTrigger>();
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (success == false)
            {
                UnityEditor.Handles.Label(transform.position, "Finding Character");
            }
            else
            {
                UnityEditor.Handles.Label(transform.position, "Success AutoSetting");
            }
        }
#endif


        // Update is called once per frame
        void Update()
        {
            //foreach (var descriptorAutoSetter in FindObjectsOfType<DescriptorAutoSetter>())
            {
                var descriptorAutoSetter = this;
                if (descriptorAutoSetter.success)
                {
                    return ;
                }
                {

                    var parents = descriptorAutoSetter.transform.GetComponentsInParent<Transform>();
                    Transform root = null;
                    if (parents.Length == 1)
                    {
                        root = descriptorAutoSetter.transform;
                    }
                    else
                    {
                        root = System.Array.Find(parents, parent => parent.GetComponentsInParent<Transform>().Length == 1);
                    }
                    var avatarDescriptor = root.GetComponentInChildren<VRCAvatarDescriptor>();

                    if (avatarDescriptor == null)
                    {
                        return;
                    }


                    foreach (var actionTrigger in descriptorAutoSetter.actionTriggers)
                    {
                        actionTrigger.CheckNActionStart(avatarDescriptor);
                    }




                    if (descriptorAutoSetter.autoDestroy)
                    {
                        DestroyImmediate(descriptorAutoSetter);
                    }
                    descriptorAutoSetter.success = true;
                }
            }
        }
    }


}