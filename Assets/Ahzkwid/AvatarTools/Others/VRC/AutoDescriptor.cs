
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Ahzkwid
{
    using UnityEditor;

    [CustomEditor(typeof(AutoDescriptor))]
    public class AutoDescriptorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            var autoDescriptor= (AutoDescriptor)target;
            foreach (var actionTrigger in autoDescriptor.actionTriggers)
            {
                foreach (var action in actionTrigger.actions)
                {
                    if (action.value is GameObject)
                    {
                        var gameObject= action.value as GameObject;
                        action.value = gameObject.GetComponent<Animator>();
                        if (action.value==null)
                        {
                            action.value = gameObject.GetComponent<AnimationCreator>();
                        }
                    }



                    if ((action.value is Animator)
                        || (action.value is RuntimeAnimatorController)
                        || (action.value is AnimationCreator))
                    {
                        switch (action.target)
                        {
                            case AutoDescriptor.Target.PlayableLayersBase:
                            case AutoDescriptor.Target.PlayableLayersAddtive:
                            case AutoDescriptor.Target.PlayableLayersGesture:
                            case AutoDescriptor.Target.PlayableLayersAction:
                            case AutoDescriptor.Target.PlayableLayersFX:
                                break;
                            default:
                                var name = action.value.name.ToLower();
                                action.target = AutoDescriptor.Target.PlayableLayersFX;
                                if (name.Contains("base"))
                                {
                                    action.target = AutoDescriptor.Target.PlayableLayersBase;
                                }
                                if (name.Contains("addtive"))
                                {
                                    action.target = AutoDescriptor.Target.PlayableLayersAddtive;
                                }
                                if (name.Contains("gesture"))
                                {
                                    action.target = AutoDescriptor.Target.PlayableLayersGesture;
                                }
                                if (name.Contains("action"))
                                {
                                    action.target = AutoDescriptor.Target.PlayableLayersAction;
                                }
                                break;
                        }
                    }
                    if (action.value is VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionParameters)
                    {
                        action.target = AutoDescriptor.Target.ExpressionsParameters;
                    }
                    if (action.value is VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu)
                    {
                        action.target = AutoDescriptor.Target.ExpressionsMenu;
                    }
                }
            }


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


    [ExecuteInEditMode]
    public class AutoDescriptor : MonoBehaviour
    {

        bool debugMode = false;
        public bool isAwake = false;
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


        AvatarTool.AssetManager.FileOptions GetFileOptions()
        {
            var fileOption = AvatarTool.AssetManager.FileOptions.Normal;
            if (EditorApplication.isPlaying)
            {
                /*
                if (debugMode)
                {
                    fileOption = AvatarTool.AssetManager.FileOptions.TempSave;
                }
                else
                {
                    fileOption = AvatarTool.AssetManager.FileOptions.NoSave;
                }
                */
                fileOption = AvatarTool.AssetManager.FileOptions.NoSave;
                //fileOption = AvatarTool.AssetManager.FileOptions.TempSave;
            }
            else
            {
                switch (mergeTrigger)
                {
                    case MergeTrigger.Always:
                        break;
                    case MergeTrigger.Runtime:
                        //fileOption = AvatarTool.AssetManager.FileOptions.NoSave;
                        fileOption = AvatarTool.AssetManager.FileOptions.TempSave;
                        //fileOption = AvatarTool.AssetManager.FileOptions.NoSave;
                        break;
                    default:
                        break;
                }
            }
            return fileOption;
        }

        [System.Serializable]
        public class Action
        {
            public enum Option
            {
                Replace
                , Merge
                , Remove
            }
            public Target target;
            public Option option= Option.Merge;
            public Object value;

            void SaveAsset(Object asset, AutoDescriptor autoDescriptor)
            {
                AvatarTool.AssetManager.SaveAsset(asset, autoDescriptor.GetFileOptions());
            }

            public void End(VRCAvatarDescriptor avatarDescriptor)
            {

                Debug.Log($"{GetType().Name}.End()");



                switch (target)
                {
                    case Target.PlayableLayersBase:
                    case Target.PlayableLayersAddtive:
                    case Target.PlayableLayersGesture:
                    case Target.PlayableLayersAction:
                    case Target.PlayableLayersFX:
                        for (int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
                        {
                            var animLayerType = TargetToAnimLayerType(target);
                            var baseAnimationLayer = avatarDescriptor.baseAnimationLayers[i];
                            if (baseAnimationLayer.type == animLayerType)
                            {
                                avatarDescriptor.customizeAnimationLayers = true;
                                avatarDescriptor.baseAnimationLayers[i].isDefault = false;
                                {
                                    if (value is Animator)
                                    {
                                        var animator = value as Animator;
                                        DestroyImmediate(animator);
                                    }
                                }
                            }
                        }

                        break;
                    case Target.ExpressionsMenu:
                        break;
                    case Target.ExpressionsParameters:
                        break;
                }
            }
            public void Run(VRCAvatarDescriptor avatarDescriptor, AutoDescriptor autoDescriptor)
            {

                Debug.Log($"{GetType().Name}.Run()");



                switch (target)
                {
                    case Target.PlayableLayersBase:
                    case Target.PlayableLayersAddtive:
                    case Target.PlayableLayersGesture:
                    case Target.PlayableLayersAction:
                    case Target.PlayableLayersFX:
                        for (int i = 0; i < avatarDescriptor.baseAnimationLayers.Length; i++)
                        {
                            var animLayerType = TargetToAnimLayerType(target);
                            var baseAnimationLayer = avatarDescriptor.baseAnimationLayers[i];
                            if (baseAnimationLayer.type == animLayerType)
                            {
                                avatarDescriptor.customizeAnimationLayers = true;
                                avatarDescriptor.baseAnimationLayers[i].isDefault = false;
                                {
                                    RuntimeAnimatorController animatorController = null;
                                    Transform rootChild = null;
                                    if (value != null)
                                    {
                                        if (value is AnimationCreator animationCreator)
                                        {
                                            //animatorController = animationCreator.CreateAnimator(autoDescriptor.GetFileOptions());
                                            animatorController = animationCreator.CreateAnimator();
                                        }
                                        if (value is Animator)
                                        {
                                            var animator = value as Animator;
                                            animatorController = animator.runtimeAnimatorController;
                                            rootChild = animator.transform;
                                        }
                                        if (value is RuntimeAnimatorController)
                                        {
                                            animatorController = (RuntimeAnimatorController)value;
                                        }
                                    }
                                    if (value == null)
                                    {
                                        if (option == Option.Merge)
                                        {
                                            continue;
                                        }
                                        avatarDescriptor.baseAnimationLayers[i].animatorController = null;
                                    }
                                    else
                                    {
                                        if (option == Option.Merge)
                                        {
                                            /*
                                            var useSave = true;
                                            if (debugMode)
                                            {
                                            }
                                            else
                                            {
                                                if (EditorApplication.isPlaying)
                                                {
                                                    useSave = false;
                                                }
                                            }
                                            */
                                            animatorController = AnimatorCombiner.CombineAnimators(avatarDescriptor.baseAnimationLayers[i].animatorController, animatorController, autoDescriptor.GetFileOptions(), avatarDescriptor.transform, rootChild);
                                            //AnimatorCombiner.ExportAnimatorController(animatorController, "Assets/Temp/TempAnimator.controller");
                                            //animatorController=AnimatorCombiner.SaveAsset(animatorController) as RuntimeAnimatorController;

                                            /*
                                            var fileOption = AvatarTool.AssetManager.FileOptions.Normal;
                                            if (EditorApplication.isPlaying)
                                            {
                                                fileOption = AvatarTool.AssetManager.FileOptions.NoSave;
                                            }
                                            else
                                            {
                                                switch (autoDescriptor.mergeTrigger)
                                                {
                                                    case MergeTrigger.Always:
                                                        break;
                                                    case MergeTrigger.Runtime:
                                                        fileOption = AvatarTool.AssetManager.FileOptions.NoSave;
                                                        //fileOption = AvatarTool.AssetManager.FileOptions.TempSave;
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            if (fileOption!= AvatarTool.AssetManager.FileOptions.NoSave)
                                            {
                                                SaveAsset(animatorController, autoDescriptor);
                                            }
                                            */
                                            SaveAsset(animatorController, autoDescriptor);
                                        }
                                        avatarDescriptor.baseAnimationLayers[i].animatorController = animatorController;
                                    }
                                }
                            }
                        }

                        break;
                    case Target.ExpressionsMenu:
                        avatarDescriptor.customExpressions = true;
                        {
                            var expressionsMenuValue = (VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu)value;
                            var expressionsMenu = expressionsMenuValue;

                            switch (option)
                            {
                                case Option.Replace:
                                    break;
                                case Option.Merge:
                                case Option.Remove:
                                    /*
                                    static void CopyClass<T>(T source, T target)
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
                                    */
                                    void CreateOutsideMenu(VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu menu)
                                    {
                                        var controls = menu.controls;
                                        var maxCount = 8;
                                        var subMenuControlName = "More...";
                                        var typeSubMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
                                        if (controls.Count > maxCount)
                                        {


                                            var subMenuControl = controls.Find(x =>
                                            {
                                                if (x == null)
                                                {
                                                    return false;
                                                }
                                                if (x.type != typeSubMenu)
                                                {
                                                    return false;
                                                }
                                                if (x.name != subMenuControlName)
                                                {
                                                    return false;
                                                }
                                                return true;
                                            });
                                            //VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control subMenuControl = null;

                                            var takeCount = maxCount - 1;

                                            if (subMenuControl == null)
                                            {
                                                subMenuControl = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control
                                                {
                                                    name = subMenuControlName,
                                                    type = typeSubMenu
                                                };
                                            }
                                            else
                                            {
                                                controls.Remove(subMenuControl);
                                            }



                                            var mainControls = controls.Take(takeCount).ToList();
                                            var remainingControls = controls.Skip(takeCount).ToList();

                                            //var subMenu = ScriptableObject.CreateInstance<VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu>();
                                            //var subMenu = Instantiate(menu);
                                            var subMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu();
                                            subMenu.name = $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
                                            subMenu.controls = remainingControls;

                                            CreateOutsideMenu(subMenu);

                                            SaveAsset(subMenu, autoDescriptor);

                                            subMenuControl.subMenu = MergeMenu(subMenuControl.subMenu, subMenu);


                                            mainControls.Add(subMenuControl);
                                            menu.controls = mainControls;




                                        }
                                    }
                                    VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu MergeMenu(params VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu[] menus)
                                    {
                                        menus = System.Array.FindAll(menus, x => x != null);
                                        if (menus.Length == 0)
                                        {
                                            return null;
                                        }



                                        var menu = Instantiate(menus.First());
                                        var name = $"{menus.First()?.name}+{menus.Last()?.name}";
                                        if (name.Length > 40)
                                        {
                                            name = $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
                                        }
                                        menu.name = name;

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
                                        var controls = menu.controls.FindAll(x => x != null);

                                        for (int i = 1; i < menus.Length; i++)
                                        {
                                            foreach (var item in menus[i].controls)
                                            {
                                                var targetClass = item;
                                                var clone = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control();
                                                var fields = targetClass.GetType().GetFields();
                                                foreach (var field in fields)
                                                {
                                                    if (field.Name == "m_InstanceID")
                                                    {
                                                        continue;
                                                    }
                                                    field.SetValue(clone, field.GetValue(targetClass));
                                                }
                                                //var control = menu.controls.Find(x => x.name == item.name);
                                                VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control control = null;

                                                var typeSubMenu = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.SubMenu;
                                                var radialPuppet = VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu.Control.ControlType.RadialPuppet;
                                                /*
                                                var addMenu = true;
                                                if (control == null)
                                                {
                                                    addMenu = true;
                                                }
                                                else
                                                {
                                                    //if ((control.type != typeSubMenu) || (item.type != typeSubMenu))
                                                    if(item.type != typeSubMenu)
                                                    {
                                                        control = menu.controls.Find(x => x.name == item.name);
                                                        if (control.parameter.name != item.parameter.name)
                                                        {
                                                            addMenu = true;
                                                        }
                                                    }
                                                }
                                                */

                                                if (item.type == typeSubMenu)
                                                {
                                                    var subMenus = controls.FindAll(x => x.type == typeSubMenu);
                                                    control = subMenus.Find(x => x.name == item.name);

                                                }
                                                else
                                                {
                                                    var buttons = controls.FindAll(x => x.type != typeSubMenu);
                                                    if (string.IsNullOrWhiteSpace(item.parameter.name) == false)
                                                    {
                                                        control = buttons.Find(x => x.parameter.name == item.parameter.name);
                                                    }
                                                    if (item.type== radialPuppet)
                                                    {
                                                        var itemSubParameter = item.subParameters.FirstOrDefault();
                                                        if (itemSubParameter != null)
                                                        {
                                                            if (string.IsNullOrWhiteSpace(itemSubParameter.name)==false)
                                                            {
                                                                control = buttons.Find(x =>
                                                                {
                                                                    var subParameter = x.subParameters.FirstOrDefault();
                                                                    if (subParameter == null)
                                                                    {
                                                                        return false;
                                                                    }
                                                                    return subParameter.name == itemSubParameter.name;
                                                                });
                                                            }
                                                        }
                                                    }

                                                }
                                                switch (option)
                                                {
                                                    case Option.Replace:
                                                        break;
                                                    case Option.Merge:
                                                        if (item.type == typeSubMenu)
                                                        {
                                                            if (control != null)
                                                            {
                                                                control.subMenu = MergeMenu(control.subMenu, clone.subMenu);
                                                            }
                                                        }
                                                        if (control != null)
                                                        {
                                                            var icons = System.Array.FindAll(new Texture2D[] { control.icon, clone.icon }, x => x != null);
                                                            control.icon = icons.LastOrDefault();
                                                        }
                                                        else
                                                        {
                                                            menu.controls.Add(clone);
                                                        }
                                                        break;
                                                    case Option.Remove:
                                                        if (control != null)
                                                        {
                                                            if (item.type == typeSubMenu)
                                                            {
                                                                if (item.subMenu == null)
                                                                {
                                                                    menu.controls.Remove(control);
                                                                }
                                                                else
                                                                {
                                                                    control.subMenu = MergeMenu(control.subMenu, clone.subMenu);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                menu.controls.Remove(control);

                                                                /*
                                                                var controlIndex = controls.FindIndex(x => (x.parameter.name == item.parameter.name) && (x.type != typeSubMenu));
                                                                if (controlIndex >= 0)
                                                                {
                                                                    menu.controls.RemoveAt(controlIndex);
                                                                }
                                                                if (control != null)
                                                                {
                                                                    menu.controls.Remove(control);
                                                                }
                                                                */
                                                            }
                                                        }
                                                        break;
                                                    default:
                                                        break;
                                                }

                                            }
                                            //expressionsMenu.controls.AddRange(expressionsMenuValue.controls);
                                        }
                                        if (menu.controls != null)
                                        {
                                            //expressionsMenu.controls = expressionsMenu.controls.GroupBy(x => x.name).Select(x => x.Last()).ToList();
                                        }




                                        CreateOutsideMenu(menu);









                                        //expressionsMenu = AnimatorCombiner.SaveAsset(expressionsMenu) as VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu;
                                        SaveAsset(menu, autoDescriptor);
                                        return menu;
                                    }
                                    //expressionsMenu = new VRC.SDK3.Avatars.ScriptableObjects.VRCExpressionsMenu();
                                    expressionsMenu = MergeMenu(avatarDescriptor.expressionsMenu, expressionsMenuValue);
                                    break;
                                default:
                                    break;
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
                                    SaveAsset(expressionParameters, autoDescriptor);
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
            public string memo="";
            TriggersOperator triggersOperator=TriggersOperator.AND;
             Trigger[] triggers=new Trigger[] { };
            public Action[] actions;

             bool Check(VRCAvatarDescriptor avatarDescriptor)
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
                return triggerCheckReturn;
            }
            public void CheckNActionStart(VRCAvatarDescriptor avatarDescriptor, AutoDescriptor autoDescriptor)
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
                if (Check(avatarDescriptor))
                {
                    foreach (var action in actions)
                    {
                        action.Run(avatarDescriptor, autoDescriptor);
                    }
                }
            }
            public void End(VRCAvatarDescriptor avatarDescriptor)
            {
                if (Check(avatarDescriptor))
                {
                    foreach (var action in actions)
                    {
                        action.End(avatarDescriptor);
                    }
                }
            }
        }

        bool success = false;

        public enum MergeTrigger
        {
            Always, Runtime
        }
        public MergeTrigger mergeTrigger= MergeTrigger.Runtime;
        //public bool autoDestroy = true;
        // List<BlendshapeTarget> blendshapeTargets = new List<BlendshapeTarget>();
        public List<ActionTrigger> actionTriggers = new List<ActionTrigger>();
        void OnDrawGizmos()
        {
            if (mergeTrigger!=MergeTrigger.Runtime)
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
            Update();
        }

        public void Run()
        {
            var autoDescriptor = this;

            if (autoDescriptor.success)
            {
                return;
            }

            /*
            var parents = autoDescriptor.transform.GetComponentsInParent<Transform>();
            Transform root = null;
            if (parents.Length == 1)
            {
                root = autoDescriptor.transform;
            }
            else
            {
                root = System.Array.Find(parents, parent => parent.GetComponentsInParent<Transform>().Length == 1);
            }

            */
            var root = ObjectPath.GetVRCRoot(autoDescriptor.transform);

            var avatarDescriptor = root.GetComponentInChildren<VRCAvatarDescriptor>();




            if (avatarDescriptor == null)
            {
                return;
            }


            foreach (var actionTrigger in autoDescriptor.actionTriggers)
            {
                actionTrigger.CheckNActionStart(avatarDescriptor, this);
            }

            foreach (var actionTrigger in autoDescriptor.actionTriggers)
            {
                actionTrigger.End(avatarDescriptor);
            }



            //if (descriptorAutoSetter.autoDestroy)
            {
                DestroyImmediate(autoDescriptor);
            }
            autoDescriptor.success = true;
        }
        // Update is called once per frame
        void Update()
        {
            //foreach (var descriptorAutoSetter in FindObjectsOfType<DescriptorAutoSetter>())
            switch (mergeTrigger)
            {
                case MergeTrigger.Always:
                    {
                        Run();
                    }
                    break;
                case MergeTrigger.Runtime:
                    if (EditorApplication.isPlaying)
                    {
                        Run();
                    }
                    break;
                default:
                    break;
            }
        }
    }


}
#endif