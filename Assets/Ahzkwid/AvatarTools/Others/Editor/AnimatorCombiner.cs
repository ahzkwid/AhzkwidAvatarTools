#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using System.IO;

public class AnimatorCombiner : MonoBehaviour
{
    public static RuntimeAnimatorController CombineAnimators(RuntimeAnimatorController runtimeControllerA, RuntimeAnimatorController runtimeControllerB, Transform root)
    {
        var controllerA = runtimeControllerA as AnimatorController;
        var controllerB = runtimeControllerB as AnimatorController;

        return CombineAnimators(controllerA, controllerB, root);
    }

    public static AnimatorController CombineAnimators(AnimatorController controllerA, AnimatorController controllerB, Transform root)
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
        else
        {
            //controllerB = Instantiate(controllerB);
        }

        var newController = Instantiate(controllerA);
        var name = $"{controllerA.name}+{controllerB.name}";

        if (name.Length > 40)
        {
            name = $"{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks)}";
        }
        newController.name = name;
        CopyParameters(controllerB, newController);
        CopyLayers(controllerB, newController, root);

        return newController;
    }

    private static void CopyParameters(AnimatorController source, AnimatorController destination)
    {
        foreach (var parameter in source.parameters)
        {
            destination.AddParameter(parameter.name, parameter.type);
        }
    }

    private static void CopyLayers(AnimatorController source, AnimatorController destination, Transform root)
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

            CloneStateMachine(layer.stateMachine, newLayer.stateMachine, root);
            layers.Add(newLayer);
        }
        layers = layers.GroupBy(x => x.name).Select(x => x.Last()).ToList();
        destination.layers = layers.ToArray();
    }

    private static void CloneStateMachine(AnimatorStateMachine source, AnimatorStateMachine destination, Transform root)
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
        }
        foreach (var state in source.states)
        {
            if (state.state == null)
            {
                continue;
            }
            var newState = stateMap[state.state];
            ReplaceAnimations(newState, root);
        }
    }

    private static void ReplaceAnimations(AnimatorState state, Transform root)
    {
        if (state.motion is BlendTree blendTree)
        {
            Debug.Log("blendTree.name: " + state.motion.name);
            state.motion = ReplaceAnimationsInBlendTree(blendTree, root);
        }
        else if (state.motion is AnimationClip)
        {
            state.motion = ReplaceAnimationClipPaths((AnimationClip)state.motion, root);
        }
    }

    private static BlendTree ReplaceAnimationsInBlendTree(BlendTree blendTree, Transform root)
    {
        var newBlendTree = new BlendTree();
        //var newBlendTree = Instantiate(blendTree);

        EditorUtility.CopySerialized(blendTree, newBlendTree);



        //CopyClass(blendTree, newBlendTree);
        var childrens = newBlendTree.children;
        for (int i = 0; i < childrens.Length; i++)
        {
            var motion = childrens[i].motion;

            if (motion is BlendTree childBlendTree)
            {
                childrens[i].motion = ReplaceAnimationsInBlendTree(childBlendTree, root);
            }
            else if (motion is AnimationClip)
            {
                var newAnim = ReplaceAnimationClipPaths((AnimationClip)childrens[i].motion, root);
                Debug.Log($"newBlendTree: {newBlendTree.name},{i}:{childrens[i].motion}->{newAnim}");
                childrens[i].motion = newAnim;
                Debug.Log($"newBlendTree: {newBlendTree.name},{i}:{childrens[i].motion}");
            }
        }
        newBlendTree.children = childrens;
        SaveAsset(newBlendTree);
        return newBlendTree;
    }

    private static AnimationClip ReplaceAnimationClipPaths(AnimationClip clip, Transform root)
    {
        var transforms = root.GetComponentsInChildren<Transform>();


        var rootPath = SearchUtils.GetHierarchyPath(root.gameObject, false);
        var transformPaths = System.Array.ConvertAll(transforms, transform => SearchUtils.GetHierarchyPath(transform.gameObject, false));
        var transformNames = System.Array.ConvertAll(transforms, transform => transform.name);
        transformPaths = System.Array.ConvertAll(transformPaths, path => System.IO.Path.GetRelativePath(rootPath, path));


        var newClip = new AnimationClip();
        EditorUtility.CopySerialized(clip, newClip);


        foreach (var binding in AnimationUtility.GetCurveBindings(newClip))
        {
            var newBinding = binding;


            //var text = "";
            //text = binding.path;

            if (transformPaths.Contains(binding.path) == false)
            {
                var index = -1;
                //var index = System.Array.FindIndex(transformNames, name => name == binding.path);


                for (int i = 0; i < transformNames.Length; i++)
                {
                    if (transformNames[i] != binding.path)
                    {
                        continue;
                    }
                    if (transformPaths[i].ToLower().Contains("armature"))
                    {
                        continue;
                    }
                    index = i;
                }
                /*
                var paths = System.Array.FindAll(transformNames, name => name == binding.path);
                if (paths.Length > 0)
                {

                }
                */
                if (index >= 0)
                {
                    newBinding.path = transformPaths[index];
                    Debug.LogWarning($"{binding.path} -> {newBinding.path}");
                }
                else
                {
                    Debug.LogWarning($"{binding.path} is Nothing");
                }
            }
            //text = clothPath+text;
            //newBinding.path = text;


            var curve = AnimationUtility.GetEditorCurve(newClip, binding);
            AnimationUtility.SetEditorCurve(newClip, binding, null);//Á¦°Å

            try
            {
                AnimationUtility.SetEditorCurve(newClip, newBinding, curve);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError($"clip:{newClip}");
                Debug.LogError($"newBinding:{newBinding}");
                Debug.LogError($"curve:{curve}");
                Debug.LogError($"newBinding.type:{newBinding.type}");
                throw;
            }
        }

        SaveAsset(newClip);



        return newClip;
    }
    static void SaveAsset(Object asset)
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
        if ((asset is AnimationClip)|| (asset is BlendTree))
        {
            ext = ".anim";
        }
        var fileName = $"{asset.name}{(System.DateTime.Now.Ticks - new System.DateTime(2024, 1, 1).Ticks) / 1000}";
        var path = $"{folderPath}/{fileName}{ext}";
        asset.name = fileName;
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.Refresh();
#endif
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
    /*
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
    */
}
#endif
