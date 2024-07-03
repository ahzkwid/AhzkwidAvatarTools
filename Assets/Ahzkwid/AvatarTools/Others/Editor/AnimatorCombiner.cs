#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static AnimationRepairTool;

public class AnimatorCombiner : MonoBehaviour
{
    void SaveAsset(Object asset)
    {
#if UNITY_EDITOR
        var folderPath = $"Assets/EasyWearDatas/";
        if (System.IO.Directory.Exists(folderPath) == false)
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        var ext = ".asset";
        if (asset is RuntimeAnimatorController)
        {
            ext = ".controller";
        }
        if (asset is AnimationClip)
        {
            ext = ".clip";
        }
        var fileName = $"{asset.name}{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks) / 1000}";
        var path = $"{folderPath}/{fileName}{ext}";
        asset.name = fileName;
        AssetDatabase.CreateAsset(asset, path);
        //AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    public static RuntimeAnimatorController CombineAnimators(RuntimeAnimatorController runtimeControllerA, RuntimeAnimatorController runtimeControllerB)
    {
        var controllerA = runtimeControllerA as AnimatorController;
        var controllerB = runtimeControllerB as AnimatorController;

        return CombineAnimators(controllerA, controllerB);
    }

    public static AnimatorController CombineAnimators(AnimatorController controllerA, AnimatorController controllerB)
    {
        if (controllerA == null && controllerB == null)
        {
            return null;
        }
        if (controllerA == null)
        {
            controllerA = new AnimatorController();
        }
        if (controllerB == null)
        {
            controllerB = new AnimatorController();
        }

        var newController = Instantiate(controllerA);
        newController.name = $"{controllerA.name}+{controllerB.name}";

        CopyParameters(controllerB, newController);
        CopyLayers(controllerB, newController);

        return newController;
    }

    private static void CopyParameters(AnimatorController source, AnimatorController destination)
    {
        foreach (var parameter in source.parameters)
        {
            destination.AddParameter(parameter.name, parameter.type);
        }
    }

    private static void CopyLayers(AnimatorController source, AnimatorController destination)
    {
        var layers = new List<AnimatorControllerLayer>(destination.layers);

        for (int i = 0; i < source.layers.Length; i++)
        {
            var layer = source.layers[i];
            var newLayer = new AnimatorControllerLayer();

            CopyClass(layer, newLayer);

            newLayer.stateMachine = new AnimatorStateMachine();

            if (i == 0)
            {
                newLayer.defaultWeight = 1f;
            }

            CloneStateMachine(layer.stateMachine, newLayer.stateMachine);
            layers.Add(newLayer);
        }
        layers = layers.GroupBy(x => x.name).Select(x => x.Last()).ToList();
        destination.layers = layers.ToArray();
    }

    private static void CloneStateMachine(AnimatorStateMachine source, AnimatorStateMachine destination)
    {
        if (source == null || destination == null)
        {
            return;
        }

        var stateMap = new Dictionary<AnimatorState, AnimatorState>();

        foreach (var state in source.states)
        {
            if (state.state == null)
            {
                continue;
            }

            var newState = new AnimatorState();

            CopyClass(state.state, newState);
            destination.AddState(newState, state.position);

            stateMap[state.state] = newState;
        }

        foreach (var state in source.states)
        {
            if (state.state == null)
            {
                continue;
            }
            foreach (var transition in state.state.transitions)
            {
                var newState = stateMap[state.state];
                AnimatorState newDestinationState = null;
                var newTransition = newState.AddTransition(newDestinationState);

                CopyClass(transition, newTransition);

                if (transition.destinationState != null)
                {
                    newTransition.destinationState = stateMap[transition.destinationState];
                }
            }

            ReplaceAnimations(state.state);
        }
    }

    private static void ReplaceAnimations(AnimatorState state)
    {
        if (state.motion is BlendTree blendTree)
        {
            ReplaceAnimationsInBlendTree(blendTree);
        }
        else if (state.motion is AnimationClip)
        {
            state.motion = ReplaceAnimationClipPaths((AnimationClip)state.motion);
        }
    }

    private static void ReplaceAnimationsInBlendTree(BlendTree blendTree)
    {
        for (int i = 0; i < blendTree.children.Length; i++)
        {
            var child = blendTree.children[i];

            if (child.motion is BlendTree childBlendTree)
            {
                ReplaceAnimationsInBlendTree(childBlendTree);
            }
            else if (child.motion is AnimationClip)
            {
                blendTree.children[i].motion = ReplaceAnimationClipPaths((AnimationClip)child.motion);
            }
        }
    }

    private static AnimationClip ReplaceAnimationClipPaths(AnimationClip clip)
    {
        var newClip = new AnimationClip();
        EditorUtility.CopySerialized(clip, newClip);













        foreach (var binding in AnimationUtility.GetCurveBindings(clip))
        {
            var newBinding = binding;


            var text = "";
            text = binding.path;
            //text = clothPath+text;
            //newBinding.path = text;


            var curve = AnimationUtility.GetEditorCurve(clip, binding);
            AnimationUtility.SetEditorCurve(clip, binding, null);//Á¦°Å

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

        SaveAsset(newClip);

        void SaveAsset(Object asset)
        {
#if UNITY_EDITOR
            var folderPath = $"Assets/EasyWearDatas/";
            if (System.IO.Directory.Exists(folderPath) == false)
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }
            var ext = ".asset";
            if (asset is RuntimeAnimatorController)
            {
                ext = ".controller";
            }
            if (asset is RuntimeAnimatorController)
            {
                ext = ".anim";
            }
            var fileName = $"{asset.name}{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks) / 1000}";
            var path = $"{folderPath}/{fileName}{ext}";
            asset.name = fileName;
            AssetDatabase.CreateAsset(asset, path);
#endif
        }




        return newClip;
    }

    static void CopyClass<T>(T source, T target)
    {
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
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

    public static void ExportAnimatorController(RuntimeAnimatorController controller, string path)
    {
        if (controller == null)
        {
            Debug.LogError("Invalid Animator Controller provided.");
            return;
        }

        if (System.IO.File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
        }

        var controllerCopy = Instantiate(controller);
        AssetDatabase.CreateAsset(controllerCopy, path);
        //AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif
