
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

using Ahzkwid;
using Ahzkwid.AvatarTool;
using UnityEditor;
using UnityEngine.Animations;
public class AnimationCreator : MonoBehaviour
{
    [System.Serializable]
    public class ToggleData
    {
        public string parameter;
        public Object[] targets;
        public bool floatParameter = true;
        public bool inverse = false;
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
        public AnimatorControllerLayer CreateLayer()
        {

            var newLayer = new AnimatorControllerLayer();
            newLayer.name = parameter;
            newLayer.defaultWeight = 1;

            newLayer.stateMachine = new AnimatorStateMachine();


            if (floatParameter)
            {
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
            {
                var newStates = new List<AnimatorState>();
                for (int i = 0; i < 2; i++)
                {
                    var name = parameter;
                    if (i > 0)
                    {
                        name += " On";
                    }
                    else
                    {
                        name += " Off";
                    }

                    var newState = new AnimatorState();
                    newState.motion = CreateAnimationClip( (ClipValue)i);
                    newState.name = name;

                    newLayer.stateMachine.AddState(newState, Vector3.right * 300 + Vector3.up * 100 * i);
                    newLayer.stateMachine.name = name;
                    newLayer.stateMachine.defaultState = newState;

                    newStates.Add(newState);
                }
                for (int i = 0; i < 2; i++)
                {
                    var newTransition = newStates[i].AddTransition(newStates[1-i]);
                    newTransition.exitTime = 0;
                    newTransition.duration = 0;


                    var mode = AnimatorConditionMode.If;
                    if (i > 0)
                    {
                        mode=AnimatorConditionMode.IfNot;
                    }
                    if (inverse)
                    {
                        if (mode==AnimatorConditionMode.If)
                        {
                            mode = AnimatorConditionMode.IfNot;
                        }
                        else
                        {
                            mode = AnimatorConditionMode.If;
                        }
                    }
                    newTransition.AddCondition(mode, 0f, parameter);
                }
            }

            return newLayer;
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
    public static AnimatorControllerLayer[] CreateLayers(ToggleData[] toggleDatas)
    {
        var layers = new List<AnimatorControllerLayer>();

        foreach (var toggleData in toggleDatas)
        {
            var layer = toggleData.CreateLayer();

            layers.Add(layer);
        }

        return layers.ToArray();
    }
    /*
    public AnimatorController CreateAnimator(AssetManager.FileOptions fileOption)
    {
        return CreateAnimator(toggleDatas, fileOption);
    }
    */
    public AnimatorController CreateAnimator()
    {
        return CreateAnimator(toggleDatas);
    }
    public static AnimatorController CreateAnimator(ToggleData[] toggleDatas)
    {
        var controller = new AnimatorController();


        foreach (var toggleData in toggleDatas)
        {
            if (toggleData.floatParameter)
            {
                controller.AddParameter(toggleData.parameter, AnimatorControllerParameterType.Float);
            }
            else
            {
                controller.AddParameter(toggleData.parameter, AnimatorControllerParameterType.Bool);
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

        controller.layers = CreateLayers(toggleDatas);

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