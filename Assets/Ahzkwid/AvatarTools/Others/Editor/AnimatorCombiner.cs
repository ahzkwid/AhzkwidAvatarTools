#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnimatorCombiner : MonoBehaviour
{
    public static RuntimeAnimatorController CombineAnimators(RuntimeAnimatorController runtimeControllerA, RuntimeAnimatorController runtimeControllerB)
    {
        var controllerA = runtimeControllerA as AnimatorController;
        var controllerB = runtimeControllerB as AnimatorController;


        if (controllerA == null)
        {
            controllerA = new AnimatorController();
        }
        if (controllerB == null)
        {
            controllerB = new AnimatorController();
        }
        if (controllerA == null && controllerB == null)
        {
            return null;
        }

        return CombineAnimators(controllerA, controllerB);
    }

    public static AnimatorController CombineAnimators(AnimatorController controllerA, AnimatorController controllerB)
    {
        var newController = new AnimatorController();
        newController.name = $"{controllerA.name}+{controllerB.name}";

        CopyParameters(controllerA, newController);
        CopyParameters(controllerB, newController);
        CopyLayers(controllerA, newController);
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
            var newLayer = new AnimatorControllerLayer
            {
                name = layer.name,
                avatarMask = layer.avatarMask,
                blendingMode = layer.blendingMode,
                defaultWeight = layer.defaultWeight, 
                syncedLayerAffectsTiming = layer.syncedLayerAffectsTiming,
                stateMachine = new AnimatorStateMachine() // 새 상태 머신 생성
            };
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
        var stateMap = new Dictionary<AnimatorState, AnimatorState>();

        foreach (var state in source.states)
        {
            var newState = new AnimatorState
            {
                name = state.state.name,
                motion = state.state.motion,
                speed = state.state.speed,
                cycleOffset = state.state.cycleOffset,
                mirror = state.state.mirror
            };
            destination.AddState(newState, state.position);
            stateMap[state.state] = newState;
        }

        foreach (var transition in source.anyStateTransitions)
        {
            var newTransition = destination.AddAnyStateTransition(stateMap[transition.destinationState]);
            newTransition.conditions = transition.conditions;
            newTransition.duration = transition.duration;
            newTransition.offset = transition.offset;
            newTransition.exitTime = transition.exitTime;
            newTransition.hasExitTime = transition.hasExitTime;
            newTransition.interruptionSource = transition.interruptionSource;
            newTransition.isExit = transition.isExit;
            newTransition.mute = transition.mute;
            newTransition.solo = transition.solo;
        }

        foreach (var state in source.states)
        {
            var newState = stateMap[state.state];

            foreach (var transition in state.state.transitions)
            {
                var newTransition = newState.AddTransition(stateMap[transition.destinationState]);
                newTransition.conditions = transition.conditions;
                newTransition.duration = transition.duration;
                newTransition.offset = transition.offset;
                newTransition.exitTime = transition.exitTime;
                newTransition.hasExitTime = transition.hasExitTime;
                newTransition.interruptionSource = transition.interruptionSource;
                newTransition.isExit = transition.isExit;
                newTransition.mute = transition.mute;
                newTransition.solo = transition.solo;
            }
        }
    }

    // 애니메이터 컨트롤러를 에셋 파일로 저장하는 함수
    public static void ExportAnimatorController(RuntimeAnimatorController controller, string path)
    {
        if (controller == null)
        {
            Debug.LogError("Invalid Animator Controller provided.");
            return;
        }

        // 경로에 파일이 이미 존재하면 삭제
        if (System.IO.File.Exists(path))
        {
            AssetDatabase.DeleteAsset(path);
        }

        var controllerCopy = Instantiate(controller);
        AssetDatabase.CreateAsset(controllerCopy, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
#endif