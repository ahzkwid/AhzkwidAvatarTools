#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using System.IO;
using Ahzkwid.AvatarTool;

public class AnimatorCombiner : MonoBehaviour
{
    public static RuntimeAnimatorController CombineAnimators(RuntimeAnimatorController runtimeControllerA, RuntimeAnimatorController runtimeControllerB, AssetManager.FileOptions fileOptions, Transform rootParent, Transform rootChild=null)
    {
        var controllerA = runtimeControllerA as AnimatorController;
        var controllerB = runtimeControllerB as AnimatorController;

        return CombineAnimators(controllerA, controllerB, rootParent, rootChild, fileOptions);
    }

    public static AnimatorController CombineAnimators(AnimatorController controllerA, AnimatorController controllerB, Transform rootParent, Transform rootChild, AssetManager.FileOptions fileOptions)
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
        CopyLayers(controllerB, newController, rootParent, rootChild, fileOptions);

        return newController;
    }

    private static void CopyParameters(AnimatorController source, AnimatorController destination)
    {
        foreach (var parameter in source.parameters)
        {
            var newParameter = new UnityEngine.AnimatorControllerParameter();
            CopyClass(parameter, newParameter);

            var index = System.Array.FindIndex(destination.parameters, x => x.name == parameter.name);
            if (index >= 0)
            {
                destination.parameters[index]= newParameter;
            }
            else
            {
                //destination.AddParameter(parameter.name, parameter.type);
                destination.AddParameter(newParameter);
            }
        }
    }

    private static void CopyLayers(AnimatorController source, AnimatorController destination, Transform rootParent, Transform rootChild, AssetManager.FileOptions fileOptions)
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

            CloneStateMachine(layer.stateMachine, newLayer.stateMachine, rootParent, rootChild, fileOptions);
            layers.Add(newLayer);
        }
        //layers = layers.GroupBy(x => x.name).Select(x => x.Last()).ToList();
        destination.layers = layers.ToArray();
    }

    private static void CloneStateMachine(AnimatorStateMachine source, AnimatorStateMachine destination, Transform rootParent, Transform rootChild, AssetManager.FileOptions fileOptions)
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

        var stateMachineMap = new Dictionary<AnimatorStateMachine, AnimatorStateMachine>();
        foreach (var stateMachine in source.stateMachines)
        {
            if (stateMachine.stateMachine == null)
            {
                continue;
            }
            var newStateMachine = new AnimatorStateMachine();

            CopyClass(stateMachine.stateMachine, newStateMachine);
            destination.AddStateMachine(newStateMachine, stateMachine.position);

            stateMachineMap[stateMachine.stateMachine] = newStateMachine;
        }

        foreach (var state in source.states)
        {
            if (state.state == null)
            {
                continue;
            }
            foreach (var transition in state.state.transitions)
            {
                AnimatorState newDestinationState = null;
                AnimatorStateMachine newDestinationStateMachine = null;


                if (transition.destinationState != null)
                {
                    newDestinationState = stateMap[transition.destinationState];
                }
                if (transition.destinationStateMachine != null)
                {
                    newDestinationStateMachine = stateMachineMap[transition.destinationStateMachine];
                }

                var newState = stateMap[state.state];
                var newTransition = newState.AddTransition(newDestinationState);

                CopyClass(transition, newTransition);

                newTransition.destinationStateMachine = newDestinationStateMachine;
                newTransition.destinationState = newDestinationState;


            }
            {

                var newState = stateMap[state.state];


                var oldTransitions = state.state.transitions;
                newState.transitions = System.Array.FindAll(newState.transitions,x=> oldTransitions.Contains(x) == false);
                //���� Ʈ������ ����
            }
        }


        foreach (var state in source.states)
        {
            if (state.state == null)
            {
                continue;
            }
            var newState = stateMap[state.state];
            ReplaceAnimations(newState, rootParent, rootChild, fileOptions);
        }
    }

    private static void ReplaceAnimations(AnimatorState state, Transform rootParent, Transform rootChild, AssetManager.FileOptions fileOptions)
    {
        if (state.motion is BlendTree blendTree)
        {
            Debug.Log("blendTree.name: " + state.motion.name);
            state.motion = ReplaceAnimationsInBlendTree(blendTree, rootParent, rootChild, fileOptions);
        }
        else if (state.motion is AnimationClip)
        {
            state.motion = ReplaceAnimationClipPaths((AnimationClip)state.motion, rootParent, rootChild, fileOptions);
        }
    }

    private static BlendTree ReplaceAnimationsInBlendTree(BlendTree blendTree, Transform rootParent, Transform rootChild, AssetManager.FileOptions fileOptions)
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
                childrens[i].motion = ReplaceAnimationsInBlendTree(childBlendTree, rootParent, rootChild, fileOptions);
            }
            else if (motion is AnimationClip)
            {
                var newAnim = ReplaceAnimationClipPaths((AnimationClip)childrens[i].motion, rootParent, rootChild, fileOptions);
                Debug.Log($"newBlendTree: {newBlendTree.name},{i}:{childrens[i].motion}->{newAnim}");
                childrens[i].motion = newAnim;
                Debug.Log($"newBlendTree: {newBlendTree.name},{i}:{childrens[i].motion}");
            }
        }
        newBlendTree.children = childrens;
        //newBlendTree= SaveAsset(newBlendTree) as BlendTree;
        AssetManager.SaveAsset(newBlendTree, fileOptions);
        return newBlendTree;
    }

    private static AnimationClip ReplaceAnimationClipPaths(AnimationClip clip, Transform rootParent, Transform rootChild, AssetManager.FileOptions fileOptions)
    {
        var transforms = rootParent.GetComponentsInChildren<Transform>();


        var rootParentPath = SearchUtils.GetHierarchyPath(rootParent.gameObject, false);
        var rootChildPath = "";

        if (rootChild != null)
        {
            rootChildPath = SearchUtils.GetHierarchyPath(rootChild.gameObject, false);
            rootChildPath = System.IO.Path.GetRelativePath(rootParentPath, rootChildPath);
        }

        var transformNames = System.Array.ConvertAll(transforms, transform => transform.name);

        var transformPaths = System.Array.ConvertAll(transforms, transform => SearchUtils.GetHierarchyPath(transform.gameObject, false));
        transformPaths = System.Array.ConvertAll(transformPaths, path => System.IO.Path.GetRelativePath(rootParentPath, path));





        /*

        string[] transformChildPaths = null;
        if (rootChild != null)
        {
            transformChildPaths = System.Array.ConvertAll(transformFullPaths, path => System.IO.Path.GetRelativePath(rootChildPath, path));
        }
        */


        var newClip = new AnimationClip();
        EditorUtility.CopySerialized(clip, newClip);


        foreach (var binding in AnimationUtility.GetCurveBindings(newClip))
        {
            var newBinding = binding;


            //var text = "";
            //text = binding.path;
            if (rootChild != null)
            {
                newBinding.path = Path.Join(rootChildPath, binding.path).Replace("\\", "/");
                //newBinding.path = rootChildPath + "/" + binding.path;
                Debug.Log($"{binding.path} -> {newBinding.path}");
            }
            else
            {
                if (transformPaths.Contains(binding.path) == false)
                {
                    //�ڵ�����
                    var index = -1;
                    //var index = System.Array.FindIndex(transformNames, name => name == binding.path);



                    for (int i = 0; i < transformNames.Length; i++)
                    {
                        if (transformNames[i] != binding.path)
                        {
                            continue;
                        }
                        //if (transformPaths[i].ToLower().Contains("armature"))
                        if (transformPaths[i].ToLower().StartsWith("armature"))
                        {
                            //�ǻ� ����
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
            }

            //text = clothPath+text;
            //newBinding.path = text;


            var curve = AnimationUtility.GetEditorCurve(newClip, binding);
            AnimationUtility.SetEditorCurve(newClip, binding, null);//����

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

        //newClip = SaveAsset(newClip) as AnimationClip;
            AssetManager.SaveAsset(newClip, fileOptions);


        return newClip;
    }

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
