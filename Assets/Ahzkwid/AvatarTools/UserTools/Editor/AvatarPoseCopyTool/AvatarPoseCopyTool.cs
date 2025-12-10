


#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using UnityEditor.Search;
using UnityEditor;
using System.Linq;
using Ahzkwid;
using System.Collections.Generic;

[InitializeOnLoad]
class AvatarPoseCopyTool : EditorWindow
{
    public enum Mode
    {
        ApplyPose, Export
    }
    public Mode mode;

    public GameObject[] characters;
    public Object pose;
    public AvatarMask avatarMask;
    public float animationTime = 0;
    public bool poseBackup = false;
    public bool poseOnly = true;
    public bool xMirror = false;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/" + nameof(AvatarPoseCopyTool))]
    public static void Init()
    {
        var window = GetWindow<AvatarPoseCopyTool>(utility: false, title: nameof(AvatarPoseCopyTool));
        window.minSize = new Vector2(300, 150);
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
        var isHumanoid = true;



        serializedObject.Update();
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(mode)));

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(characters)));
            EditorGUILayout.Space();
            switch (mode)
            {
                case Mode.ApplyPose:
                    /*
                    if (characters == null || characters.Length == 0)
                    {
                        allReady = false;
                    }
                    else
                    {
                        foreach (var character in characters)
                        {
                            if (character != null)
                            {
                                var animator = character.GetComponent<Animator>();
                                if (animator == null)
                                {
                                    EditorGUILayout.HelpBox($"This character({character.name}) does not include an animator", MessageType.Info);
                                    //isHumanoid = false;
                                }
                                else if (animator.isHuman == false)
                                {
                                    EditorGUILayout.HelpBox($"This character({character.name}) is not a humanoid", MessageType.Info);
                                    //isHumanoid = false;
                                }
                            }
                        }
                    }
                    */
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(pose)));
                    if (pose == null)
                    {
                        var text = "Please insert a GameObject or AnimationClip.";
                        switch (Application.systemLanguage)
                        {
                            case SystemLanguage.Japanese:
                                text = "GameObjectやAnimationClipを入れてください。";
                                break;
                            case SystemLanguage.Korean:
                                text = "GameObject혹은 AnimationClip을 넣으십시오";
                                break;
                        }
                        EditorGUILayout.HelpBox(text, MessageType.Info);
                    }
                    else
                    {
                        if (pose is GameObject)
                        {
                        }
                        else if (pose is AnimationClip)
                        {
                            animationTime = EditorGUILayout.Slider(nameof(animationTime), animationTime, 0, 1);
                            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(poseOnly)));
                        }
                        else
                        {
                            pose = null;
                        }
                    }
                    EditorGUILayout.Space();

                    if (pose == null)
                    {
                        allReady = false;
                    }
                    else
                    {
                        if (pose is GameObject gameObject)
                        {
                            //Animator animator = null;
                            var animator = gameObject.GetComponent<Animator>();
                            if (animator == null)
                            {
                                EditorGUILayout.HelpBox("This pose character does not include an animator", MessageType.Info);
                                //allReady = false;
                            }
                            else if (animator.isHuman == false)
                            {
                                EditorGUILayout.HelpBox("This pose character is not a humanoid", MessageType.Info);
                                //allReady = false;
                            }
                        }
                    }




                    if (allReady)
                    {


                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(avatarMask)));

                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(poseBackup)));
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(xMirror)));
                    }
                    break;
                case Mode.Export:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(animationType)));
                    EditorGUILayout.Space();
                    break;
                default:
                    break;
            }
        }
        serializedObject.ApplyModifiedProperties();

        if (mode == Mode.ApplyPose)
        {
            GUI.enabled = allReady;
            if (GUILayout.Button("Apply Pose"))
            {
                foreach (var character in characters)
                {
                    if (poseBackup)
                    {
                        var characterCopy = Instantiate(character);
                        characterCopy.name = character.name + " (BackupPose)";
                        characterCopy.SetActive(false);
                    }
                    if (pose is GameObject gameObject)
                    {
                        //if (isHumanoid)
                        {
                            PoseCopy(character, gameObject,avatarMask);
                        }
                        PoseCopyPath(character, gameObject, avatarMask);
                    }
                    if (pose is AnimationClip animation)
                    {
                        if (poseOnly==false)
                        {
                            ApplyAnimationClips(character, avatarMask, animation);
                        }
                        ApplyAnimationClipsMuscle(character, avatarMask, animation);
                    }

                    if (xMirror)
                    {
                        var humanoid = new AhzkwidHumanoid(character);
                        humanoid.MirrorX();
                    }
                }


            }
            GUI.enabled = true;
        }
        else
        {
            if (GUILayout.Button("Export"))
            {
                foreach (var character in characters)
                {
                    CreateAnimation(character, animationType);
                }
            }
        }


    }





    public enum AnimationType
    {
        Generic, Human
    }
    public AnimationType animationType = AnimationType.Human;

    public static void CreateAnimation(GameObject root, AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.Generic:
                CreateGenericAnimation(root);
                break;
            case AnimationType.Human:
                CreateHumanoidAnimation(root);
                break;
            default:
                break;
        }
    }
    public static void CreateGenericAnimation(GameObject root)
    {
        if (root == null || root == false)
        {
            Debug.LogError("Root object is null.");
            return;
        }

        var humanoid = new AhzkwidHumanoid(root);
        var bones = humanoid.GetBoneTransforms();

        var path = EditorUtility.SaveFilePanelInProject("Save Animation", "Pose", "anim", "Select save location.");
        if (string.IsNullOrEmpty(path)) return;

        var clip = new AnimationClip();

        void WritePos(Transform transform, string curvePath)
        {
            var pos = transform.localPosition;
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalPosition.x", new AnimationCurve(new Keyframe(0, pos.x)));
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalPosition.y", new AnimationCurve(new Keyframe(0, pos.y)));
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalPosition.z", new AnimationCurve(new Keyframe(0, pos.z)));
        }

        void WriteRot(Transform transform, string curvePath)
        {
            var rot = transform.localRotation;
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalRotation.x", new AnimationCurve(new Keyframe(0, rot.x)));
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalRotation.y", new AnimationCurve(new Keyframe(0, rot.y)));
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalRotation.z", new AnimationCurve(new Keyframe(0, rot.z)));
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalRotation.w", new AnimationCurve(new Keyframe(0, rot.w)));
        }

        void WriteScale(Transform transform, string curvePath)
        {
            var scale = transform.localScale;
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalScale.x", new AnimationCurve(new Keyframe(0, scale.x)));
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalScale.y", new AnimationCurve(new Keyframe(0, scale.y)));
            clip.SetCurve(curvePath, typeof(Transform), "m_LocalScale.z", new AnimationCurve(new Keyframe(0, scale.z)));
        }

        void AddIfChanged(Transform transform, Transform prefabTransform, string curvePath)
        {
            bool posChanged = false, rotChanged = false, scaleChanged = false;
            bool forceRot = bones.Contains(transform);

            if (prefabTransform == null)
            {
                posChanged = rotChanged = scaleChanged = true;
            }
            else
            {
                if (transform.localPosition != prefabTransform.localPosition) posChanged = true;
                if (transform.localRotation != prefabTransform.localRotation) rotChanged = true;
                if (transform.localScale != prefabTransform.localScale) scaleChanged = true;
            }

            if (posChanged) WritePos(transform, curvePath);
            if (forceRot || rotChanged)
            {
                WriteRot(transform, curvePath);
            }
            if (scaleChanged) WriteScale(transform, curvePath);

            for (int i = 0; i < transform.childCount; i++)
            {
                var childTransform = transform.GetChild(i);
                Transform prefabChild = null;
                if (prefabTransform != null) prefabChild = prefabTransform.Find(childTransform.name);
                var childPath = curvePath == "" ? childTransform.name : curvePath + "/" + childTransform.name;
                AddIfChanged(childTransform, prefabChild, childPath);
            }
        }

        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(root);
        var prefabRoot = prefab != null ? (prefab).transform : null;

        AddIfChanged(root.transform, prefabRoot, "");

        var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        if (existing == null || existing == false)
        {
            AssetDatabase.CreateAsset(clip, path);
        }
        else
        {
            clip.name = existing.name;
            EditorUtility.CopySerialized(clip, existing); clip = existing;
        }

        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.keepOriginalOrientation = true;
        settings.keepOriginalPositionY = true;
        settings.keepOriginalPositionXZ = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }





    public static void CreateHumanoidAnimation(GameObject root)
    {
        /*
        {
            var text = "HumanTrait.MuscleName";
            for (int i = 0; i < HumanTrait.MuscleName.Length; i++)
            {
                //var muscleIndex = HumanTrait.MuscleFromBone(i, 0);
                var muscleName = HumanTrait.MuscleName[i];
                //text += $"\n[{i}]: {muscleName}";
                text += $"\n{muscleName}";
            }
            Debug.Log(text);
        }
        */
        var animator = root.GetComponent<Animator>();
        if (!animator || !animator.isHuman || !animator.avatar.isValid)
        {
            Debug.LogError("A humanoid avatar is required.");
            return;
        }

        var path = EditorUtility.SaveFilePanelInProject("Save Animation", "Pose", "anim", "Select a location to save the animation.");
        if (string.IsNullOrEmpty(path)) return;

        var clip = new AnimationClip();
        var poseHandler = new HumanPoseHandler(animator.avatar, root.transform);
        var pose = new HumanPose();
        poseHandler.GetHumanPose(ref pose);


        //Hips를 제외한 나머지
        //var fingerMuscleTypes = new[] { "Stretched", "Spread" };
        //var fingerMuscleKeywords = AhzkwidHumanoid.fingerMuscleKeywords;
        for (int i = 0; i < pose.muscles.Length; i++)
        {
            var muscleName = HumanTrait.MuscleName[i];
            /*
            if (fingerMuscleKeywords.Any(type => muscleName.Contains(type)))
            {
                var name = muscleName;
                name = name.Replace("Left ", "LeftHand.");
                name = name.Replace("Right ", "RightHand.");
                name = name.Replace("Thumb ", "Thumb.");
                name = name.Replace("Index ", "Index.");
                name = name.Replace("Middle ", "Middle.");
                name = name.Replace("Ring ", "Ring.");
                name = name.Replace("Little ", "Little.");
                muscleName = name;
            }
            */

            muscleName=AhzkwidHumanoid.MuscleToHumanoid(muscleName);

            var curve = new AnimationCurve(new Keyframe(0, pose.muscles[i]));
            clip.SetCurve("", typeof(Animator), muscleName, curve);
        }


        /*
        //손가락애니메이션
        for (var humanBodyBone = HumanBodyBones.LeftThumbProximal; humanBodyBone <= HumanBodyBones.RightLittleDistal; humanBodyBone++)
        {
            var transform = animator.GetBoneTransform(humanBodyBone);
            var humanBodyBoneName = humanBodyBone.ToString();
            var propertyName = "";
            if (humanBodyBoneName.Contains("Left"))
            {
                propertyName += "Left";
            }
            else
            {
                propertyName += "Right";
            }
            propertyName += "Hand.";

            {
                var fingers = new[] {
                            "Thumb",
                            "Index",
                            "Middle",
                            "Ring",
                            "Little" };
                foreach (var item in fingers)
                {
                    if (humanBodyBoneName.Contains(item))
                    {
                        propertyName += $"{item}.";
                        break;
                    }
                }
            }
            var prePropertyName = propertyName;
            {
                var segments = new[] {
                            "Proximal",
                            "Intermediate",
                            "Distal"};

                for (int i = 0; i < segments.Length+1; i++)
                {
                    var index = Mathf.Max(0,i-1);
                    if (humanBodyBoneName.Contains(segments[index]))
                    {
                        var rot = transform.localRotation.x;
                        if (i == 0)
                        {
                            rot = transform.localRotation.y;
                            propertyName = prePropertyName + "Spread";
                        }
                        else
                        {
                            propertyName = prePropertyName + i;
                            propertyName += " Stretched";
                        }
                        var curve = new AnimationCurve(new Keyframe(0, rot));
                        clip.SetCurve("", typeof(Animator), propertyName, curve);
                    }
                }
            }
        }
        */

        //Hips애니메이션
        {
            //var humanBodyBone = HumanBodyBones.Hips;
            //var transform = animator.GetBoneTransform(humanBodyBone);
            {

                var collection = new[] {
                    (pose.bodyRotation.x, "RootQ.x"),
                    (pose.bodyRotation.y, "RootQ.y"),
                    (pose.bodyRotation.z, "RootQ.z"),
                    (pose.bodyRotation.w, "RootQ.w"),
                    (pose.bodyPosition.x, "RootT.x"),
                    (pose.bodyPosition.y, "RootT.y"),
                    (pose.bodyPosition.z, "RootT.z")
                };
                foreach (var item in collection)
                {
                    var curve = new AnimationCurve(new Keyframe(0, item.Item1));
                    clip.SetCurve("", typeof(Animator), item.Item2, curve);
                }
            }
            /*
            {
                var curve = new AnimationCurve(new Keyframe(0, transform.rotation.x));
                clip.SetCurve("", typeof(Animator), "RootQ.x", curve);
            }
            */
        }

        var existingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

        if (existingClip == null)
        {
            AssetDatabase.CreateAsset(clip, path);
        }
        else
        {
            clip.name = existingClip.name;
            EditorUtility.CopySerialized(clip, existingClip);
            clip = existingClip;
        }
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.keepOriginalOrientation = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
    public void ApplyAnimationClips(GameObject root, AvatarMask avatarMask, params AnimationClip[] clips)
    {
        if (root == null)
        {
            return;
        }
        Undo.RegisterCompleteObjectUndo(root.gameObject, "Apply ObjectSetting");
        var animator = root.GetComponent<Animator>();
        Avatar originalAvatar = null;
        RuntimeAnimatorController originalController = null;
        if (animator)
        {
            originalAvatar = animator.avatar;
            originalController = animator.runtimeAnimatorController;
            animator.avatar = null;
            animator.runtimeAnimatorController = null;
        }
        var transforms = root.GetComponentsInChildren<Transform>(true);
        var preTRSs = new Dictionary<Transform, (Vector3 pos, Quaternion rot, Vector3 scale)>();

        foreach (var transform in transforms)
        {
            preTRSs.Add(transform, (transform.localPosition, transform.localRotation, transform.localScale));
        }
        foreach (var clip in clips)
        {
            clip.SampleAnimation(root.gameObject, clip.length);
        }
        if (avatarMask!=null)
        {
            var humanoid = new AhzkwidHumanoid(root);
            var childs = humanoid.GetBoneTransformChilds(avatarMask);
            foreach (var preTRS in preTRSs)
            {
                if (childs.Contains(preTRS.Key))
                {
                    continue;
                }

                var transform = preTRS.Key;
                var trs = preTRS.Value;
                transform.localPosition = trs.pos;
                transform.localRotation = trs.rot;
                transform.localScale = trs.scale;
                //롤백
            }
        }
        //EditorUtility.SetDirty(root.gameObject);

        if (animator)
        {
            animator.avatar = originalAvatar;
            animator.runtimeAnimatorController = originalController;
            //animator.avatar = null;
        }
        EditorUtility.SetDirty(root.gameObject);
    }
    public void ApplyAnimationClipsMuscle(GameObject root, AvatarMask avatarMask, params AnimationClip[] clips)
    {
        if (root==null)
        {
            return;
        }
        var animator = root.GetComponent<Animator>();
        if (!animator || !animator.isHuman || !animator.avatar.isValid)
        {
            Debug.LogError("A humanoid avatar is required.");
            return;
        }

        Undo.RegisterCompleteObjectUndo(root.gameObject, "Apply ObjectSetting");

        var transforms = root.GetComponentsInChildren<Transform>(true);
        var preTRSs = new Dictionary<Transform, (Vector3 pos, Quaternion rot, Vector3 scale)>();

        foreach (var transform in transforms)
        {
            preTRSs.Add(transform, (transform.localPosition, transform.localRotation, transform.localScale));
        }

        foreach (var clip in clips)
        {


            var poseHandler = new HumanPoseHandler(animator.avatar, root.transform);
            var pose = new HumanPose();
            poseHandler.GetHumanPose(ref pose);

            var bindings = AnimationUtility.GetCurveBindings(clip);

            /*
            {
                var text = "HumanoidAnimationPropertyNames";
                for (int i = 0; i < bindings.Length; i++)
                {
                    var binding = bindings[i];
                    //text += $"\n[{i}]: {binding.propertyName}";
                    text += $"\n{binding.propertyName}";
                }
                Debug.Log(text);
            }
            {
                var text = "HumanBodyBones";
                var enumNames = System.Enum.GetNames(typeof(HumanBodyBones));
                for (int i = 0; i < enumNames.Length; i++)
                {
                    var enumName = enumNames[i];
                    //text += $"\n[{i}]: {enumName}";
                    text += $"\n{enumName}";
                }
                Debug.Log(text);
            }
            {
                var text = "HumanTrait.MuscleName";
                for (int i = 0; i < HumanTrait.BoneCount; i++)
                {
                    //var muscleIndex = HumanTrait.MuscleFromBone(i, 0);
                    var muscleName = HumanTrait.MuscleName[i];
                    //text += $"\n[{i}]: {muscleName}";
                    text += $"\n{muscleName}";
                }
                Debug.Log(text);
            }
            */
            /*
            foreach (var binding in bindings)
            {
                Debug.Log($"binding.propertyName: {binding.propertyName}");
            }
            */
            for (int i = 0; i < pose.muscles.Length; i++)
            {
                var muscleName = HumanTrait.MuscleName[i];







                //Undo.RegisterCompleteObjectUndo(AhzkwidHumanoid.MuscleToBoneTransform(muscleName), "Apply ObjectSetting");









                //var humanoidName = AhzkwidHumanoid.HumanoidToMuscle(b.propertyName);

                //Debug.Log($"{muscleName} -> {humanoidName}");
                var binding = bindings.FirstOrDefault(b => AhzkwidHumanoid.HumanoidToMuscle(b.propertyName) == muscleName);
                if (binding == null)
                {
                    continue;
                }
                if (binding.propertyName == null)
                {
                    //Debug.Log(muscleName+"=null");
                    continue;
                }


                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (curve == null)
                {
                    continue;
                }
                if (curve.length == 0)
                {
                    continue;
                }
                float maxTime = 0;
                if (curve.length > 0)
                {
                    maxTime = curve.keys[curve.length - 1].time;
                }
                pose.muscles[i] = curve.Evaluate(maxTime*animationTime);
            }

            {
                /*
                var collection = new[] {
                    (pose.bodyRotation.x, "RootQ.x"),
                    (pose.bodyRotation.y, "RootQ.y"),
                    (pose.bodyRotation.z, "RootQ.z"),
                    (pose.bodyRotation.w, "RootQ.w"),
                    (pose.bodyPosition.x, "RootT.x"),
                    (pose.bodyPosition.y, "RootT.y"),
                    (pose.bodyPosition.z, "RootT.z")
                };
                */

                {
                    var bodyPosition = Vector3.zero;

                    var mapping = new (string, System.Action<float>)[]
                    {
                        ("RootT.x", v => bodyPosition.x = v),
                        ("RootT.y", v => bodyPosition.y = v),
                        ("RootT.z", v => bodyPosition.z = v)
                    };

                    for (int i = 0; i < mapping.Length; i++)
                    {
                        var kvp = mapping[i];
                        var binding = bindings.FirstOrDefault(b => b.propertyName == kvp.Item1);
                        if (binding == null)
                        {
                            break;
                        }
                        if (binding.propertyName == null)
                        {
                            break;
                        }
                        var curve = AnimationUtility.GetEditorCurve(clip, binding);

                        kvp.Item2(curve.Evaluate(0));
                        if (i == mapping.Length - 1)
                        {

                            pose.bodyPosition = bodyPosition;
                            //var transform = animator.GetBoneTransform(HumanBodyBones.Hips);
                            var position = bodyPosition;

                            var settings = AnimationUtility.GetAnimationClipSettings(clip);
                            if (settings.keepOriginalPositionXZ)
                            {
                                position.x = 0;
                                position.z = 0;
                            }
                            else
                            {
                                bodyPosition.x = 0;
                                bodyPosition.z = 0;
                            }
                            if (settings.keepOriginalPositionY)
                            {
                                position.y = 0;
                            }
                            else
                            {
                                bodyPosition.y = 0;
                            }
                            pose.bodyPosition = bodyPosition;
                            //animator.transform.position = position;
                        }
                    }
                }

                {
                    var bodyRotation = Quaternion.identity;

                    var mapping = new (string, System.Action<float>)[]
                    {
                        ("RootQ.x", v => bodyRotation.x = v),
                        ("RootQ.y", v => bodyRotation.y = v),
                        ("RootQ.z", v => bodyRotation.z = v),
                        ("RootQ.w", v => bodyRotation.w = v)
                    };

                    for (int i = 0; i < mapping.Length; i++)
                    {
                        var kvp = mapping[i];
                        var binding = bindings.FirstOrDefault(b => b.propertyName == kvp.Item1);
                        if (binding == null)
                        {
                            break;
                        }
                        if (binding.propertyName == null)
                        {
                            break;
                        }
                        var curve = AnimationUtility.GetEditorCurve(clip, binding);

                        float maxTime = 0;
                        if (curve.length>0)
                        {
                            maxTime = curve.keys[curve.length - 1].time;
                        }

                        kvp.Item2(curve.Evaluate(maxTime*animationTime));
                        if (i == mapping.Length - 1)
                        {
                            pose.bodyRotation = bodyRotation;
                        }
                    }
                }

            }










            poseHandler.SetHumanPose(ref pose);
        }


        if (avatarMask != null)
        {
            var humanoid = new AhzkwidHumanoid(root);
            var childs = humanoid.GetBoneTransformChilds(avatarMask);
            foreach (var preTRS in preTRSs)
            {
                if (childs.Contains(preTRS.Key))
                {
                    continue;
                }

                var transform = preTRS.Key;
                var trs = preTRS.Value;
                transform.localPosition = trs.pos;
                transform.localRotation = trs.rot;
                transform.localScale = trs.scale;
                //롤백
            }
        }

        EditorUtility.SetDirty(root.gameObject);
    }

    public static void SetToZeroPose(Animator animator)
    {
        if (animator == null || animator.avatar == null || !animator.avatar.isHuman)
        {
            Debug.LogWarning("Animator 또는 Avatar가 없거나 휴머노이드가 아닙니다.");
            return;
        }

        Undo.RegisterCompleteObjectUndo(animator.gameObject, "Set ZeroPose");

        var originalController = animator.runtimeAnimatorController;
        var originalUpdateMode = animator.updateMode;
        var originalApplyRootMotion = animator.applyRootMotion;

        animator.runtimeAnimatorController = null;
        animator.applyRootMotion = false;
        animator.updateMode = AnimatorUpdateMode.Normal;

        HumanPoseHandler poseHandler = new HumanPoseHandler(animator.avatar, animator.transform);
        HumanPose humanPose = new HumanPose();
        poseHandler.GetHumanPose(ref humanPose);
        humanPose.bodyPosition = Vector3.zero;
        humanPose.bodyRotation = Quaternion.identity;
        for (int i = 0; i < humanPose.muscles.Length; i++)
        {
            humanPose.muscles[i] = 0f; 
        }
        poseHandler.SetHumanPose(ref humanPose);

        animator.runtimeAnimatorController = originalController;
        animator.applyRootMotion = originalApplyRootMotion;
        animator.updateMode = originalUpdateMode;

        EditorUtility.SetDirty(animator.gameObject);
    }

    public void PoseCopyPath(GameObject character, GameObject pose, AvatarMask avatarMask)
    {


        var pathCharacter = SearchUtils.GetHierarchyPath(character.gameObject, false);
        var pathPose = SearchUtils.GetHierarchyPath(pose.gameObject, false);




        var characterChilds = character.GetComponentsInChildren<Transform>(true);
        var poseChilds = pose.GetComponentsInChildren<Transform>(true);


        if (avatarMask == null)
        {
            characterChilds = character.GetComponentsInChildren<Transform>(true);
            poseChilds = pose.GetComponentsInChildren<Transform>(true);
        }
        else 
        { 
            {
                var humanoid = new AhzkwidHumanoid(character);
                characterChilds = humanoid.GetBoneTransformChilds(avatarMask);
            }
            {
                var humanoid = new AhzkwidHumanoid(character);
                poseChilds = humanoid.GetBoneTransformChilds(avatarMask);
            }
        }




        var pathPoseChilds = System.Array.ConvertAll(poseChilds, x => SearchUtils.GetHierarchyPath(x.gameObject, false));
        var relativePathPose = System.Array.ConvertAll(pathPoseChilds, x => Path.GetRelativePath(pathPose, x));

        foreach (var characterChild in characterChilds)
        {
            var pathCharacterChild = SearchUtils.GetHierarchyPath(characterChild.gameObject, false);
            var relativePathCharacter = Path.GetRelativePath(pathCharacter, pathCharacterChild);


            var index = System.Array.FindIndex(relativePathPose, path => path == relativePathCharacter);
            if (index < 0)
            {
                return;
            }

            var poseChild = poseChilds[index];

            characterChild.localRotation = poseChild.localRotation;
            UnityEditor.EditorUtility.SetDirty(characterChild);
        }
    }
    public void PoseCopy(GameObject character, GameObject pose,AvatarMask avatarMask)
    {
        var characterAnimator = character.GetComponent<Animator>();
        if (characterAnimator==null)
        {
            return;
        }
        if (characterAnimator.isHuman == false)
        {
            return;
        }

        characterAnimator.runtimeAnimatorController = null;
        var poseAnimator = pose.GetComponent<Animator>();
        if (poseAnimator == null)
        {
            return;
        }
        if (poseAnimator.isHuman == false)
        {
            return;
        }

        var humanBodyBones = (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones));
        if (avatarMask!=null)
        {
            humanBodyBones=AhzkwidHumanoid.AvatarMaskToHumanBodyBones(avatarMask);
        }

        foreach (var humanBodyBone in humanBodyBones)
        {

            if (humanBodyBone < 0)
            {
                continue;
            }
            if (humanBodyBone >= HumanBodyBones.LastBone)
            {
                continue;
            }
            Transform characterTransform;
            try
            {
                characterTransform = characterAnimator.GetBoneTransform(humanBodyBone);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError(humanBodyBone);
                throw;
            }
            var poseTransform = poseAnimator.GetBoneTransform(humanBodyBone);
            //characterTransform.localPosition = poseTransform.localPosition;
            if (characterTransform == null)
            {
                continue;
            }
            if (poseTransform == null)
            {
                continue;
            }
            characterTransform.localRotation = poseTransform.localRotation;
            if (humanBodyBone >= HumanBodyBones.Hips)
            {
                characterTransform.localPosition = poseTransform.localPosition;
            }
        }
    }
}
#endif