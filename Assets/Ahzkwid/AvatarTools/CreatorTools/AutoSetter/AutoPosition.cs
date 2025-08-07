

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{


    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine.Animations;

    //using VRC.SDK3.Avatars.Components;

    [CustomEditor(typeof(AutoPosition))]
    public class AutoPositionEditor : Editor
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


    [CustomPropertyDrawer(typeof(PositionDataAttribute))]
    public class PositionDataDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            float propertyCount = 9f;
            {
                var tracking = (AutoPosition.Tracking)property.FindPropertyRelative(nameof(AutoPosition.PositionData.tracking)).intValue;
                switch (tracking)
                {
                    case AutoPosition.Tracking.Path:
                        propertyCount += 1;
                        break;
                    case AutoPosition.Tracking.Humanoid:
                        break;
                    default:
                        break;
                }
            }
            {
                var target = (AutoPosition.Target)property.FindPropertyRelative(nameof(AutoPosition.PositionData.target)).intValue;
                switch (target)
                {
                    case AutoPosition.Target.Object:
                        break;
                    case AutoPosition.Target.Parent:
                        propertyCount += 1;
                        break;
                    default:
                        break;
                }
            }
            {
                var update = property.FindPropertyRelative(nameof(AutoPosition.PositionData.updatePosition)).boolValue;
                if (update)
                {
                    propertyCount += 1;
                }
            }
            {
                var update = property.FindPropertyRelative(nameof(AutoPosition.PositionData.updateRotation)).boolValue;
                if (update)
                {
                    propertyCount += 1;
                }
            }
            {
                var update = property.FindPropertyRelative(nameof(AutoPosition.PositionData.updateScale)).boolValue;
                if (update)
                {
                    propertyCount += 1;
                }
            }
            return EditorGUIUtility.singleLineHeight * propertyCount;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var fieldRect = rect;

            fieldRect.height = EditorGUIUtility.singleLineHeight;




            {
                var path = nameof(AutoPosition.PositionData.tracking);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }

            fieldRect.y += EditorGUIUtility.singleLineHeight;


            {

                var tracking = (AutoPosition.Tracking)property.FindPropertyRelative(nameof(AutoPosition.PositionData.tracking)).intValue;
                switch (tracking)
                {
                    case AutoPosition.Tracking.Path:
                        {
                            var propertyPath = nameof(AutoPosition.PositionData.pathTransform);
                            //EditorGUI.ObjectField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                            //var fieldReturn = EditorGUILayout.ObjectField("path", null, typeof(GameObject), true) as GameObject;
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(propertyPath), new GUIContent(propertyPath), true);
                            var fieldReturn = property.FindPropertyRelative(propertyPath).objectReferenceValue as Transform;
                            if (fieldReturn != null)
                            {
                                var path = ObjectPath.GetPath(fieldReturn, ObjectPath.GetVRCRoot(fieldReturn));
                                property.FindPropertyRelative(nameof(AutoPosition.PositionData.path)).stringValue = path;

                                property.FindPropertyRelative(propertyPath).objectReferenceValue = null;
                            }
                        }
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        {
                            var path = nameof(AutoPosition.PositionData.path);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        break;
                    case AutoPosition.Tracking.Humanoid:
                        {
                            var path = nameof(AutoPosition.PositionData.bone);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        break;
                    default:
                        break;
                }
                fieldRect.y += EditorGUIUtility.singleLineHeight;

            }


            fieldRect.y += EditorGUIUtility.singleLineHeight * 0.5f;






            {
                var path = nameof(AutoPosition.PositionData.target);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            //fieldRect.width /= 2;
            {
                var target = (AutoPosition.Target)property.FindPropertyRelative(nameof(AutoPosition.PositionData.target)).intValue;
                switch (target)
                {
                    case AutoPosition.Target.Object:
                        {
                            var path = nameof(AutoPosition.PositionData.targetObject);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        break;
                    case AutoPosition.Target.Parent:
                        var index = 0;
                        {
                            var path = nameof(AutoPosition.PositionData.parentIndex);
                            index = property.FindPropertyRelative(path).intValue;
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        {
                            var targetObject = property.serializedObject.targetObject as AutoPosition;
                            var parent = targetObject.GetParent(index);
                            GUI.enabled = false;
                            {
                                EditorGUI.ObjectField(fieldRect, parent.gameObject, typeof(GameObject), false);
                            }
                            GUI.enabled = true;
                        }
                        break;
                    default:
                        break;
                }
            }

            fieldRect.y += EditorGUIUtility.singleLineHeight * 0.5f;


            fieldRect.y += EditorGUIUtility.singleLineHeight;
            {
                {
                    var update = false;
                    {
                        var path = nameof(AutoPosition.PositionData.updatePosition);
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);

                        update = property.FindPropertyRelative(path).boolValue;
                    }
                    if (update)
                    {

                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        {
                            var path = nameof(AutoPosition.PositionData.position);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                    }
                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                }
                {
                    var update = false;
                    {
                        var path = nameof(AutoPosition.PositionData.updateRotation);
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);

                        update = property.FindPropertyRelative(path).boolValue;
                    }
                    if (update)
                    {

                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        {
                            var path = nameof(AutoPosition.PositionData.rotation);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                    }
                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                }
                {
                    var update = false;
                    {
                        var path = nameof(AutoPosition.PositionData.updateScale);
                        EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);

                        update = property.FindPropertyRelative(path).boolValue;
                    }
                    if (update)
                    {

                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        {
                            var path = nameof(AutoPosition.PositionData.scale);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                    }
                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                }
            }
            
        }
    }
    public class PositionDataAttribute : PropertyAttribute
    {

    }







    [ExecuteInEditMode]
    public class AutoPosition : MonoBehaviour
    {
        public enum MergeTrigger
        {
            OneShot, Runtime, Always
        }
        public MergeTrigger mergeTrigger = MergeTrigger.OneShot;
        public enum Tracking
        {
            Path, Humanoid
        }
        public enum Target
        {
            Parent, Object
        }
        [PositionDataAttribute]
        public List<PositionData> PositionDatas = new List<PositionData>() { new PositionData() };
        [System.Serializable]
        public class PositionData
        {
            public Tracking tracking;
            public HumanBodyBones bone = HumanBodyBones.Head;
            public string path = "";
            public Transform pathTransform;

            public Target target;
            public int parentIndex = 1;
            public Object targetObject;


            public bool updatePosition = true;
            public Vector3 position = Vector3.zero;
            public bool updateRotation = true;
            public Vector3 rotation = Vector3.zero;
            public bool updateScale = true;
            public Vector3 scale = Vector3.one;





            public Transform GetParentTarget(Transform root)
            {
                switch (tracking)
                {
                    case Tracking.Path:
                        {
                            return ObjectPath.Find(path, root.transform);
                        }
                    case Tracking.Humanoid:
                        {
                            return AhzkwidHumanoid.GetBoneTransform(root.gameObject, bone);
                        }
                    default:
                        break;
                }
                return null;
            }



        }
        public Transform GetParent(int index)
        {
            var parents = GetComponentsInParent<Transform>(true);
            index = Mathf.Clamp(index, 0, parents.Length - 1);
            Transform parent = null;
            if (parents.Length > 0)
            {
                parent = parents[index];
            }
            return parent;
        }
        /*
        public static Transform GetRoot(Transform transform)
        {

            var parents = transform.GetComponentsInParent<Transform>(true);
            Transform root = null;
            if (parents.Length == 1)
            {
                root = transform;
            }
            else
            {
                root = System.Array.Find(parents, parent => parent.GetComponentsInParent<Transform>(true).Length == 1);
            }
            return root;
        }
        */
        bool success = false;

        [HideInInspector]
        [System.Obsolete]
        public bool autoDestroy = true;
        void OnDrawGizmos()
        {
            switch (mergeTrigger)
            {
                case MergeTrigger.Always:
                    if (success == false)
                    {
                        UnityEditor.Handles.Label(transform.position, "Finding Character");
                    }
                    else
                    {
                        UnityEditor.Handles.Label(transform.position, "Success AutoSetting");
                    }
                    break;
                case MergeTrigger.Runtime:
                    break;
                default:
                    break;
            }
            Update();
        }
        // Start is called before the first frame update


        public void Run()
        {

            if (PrefabStageUtility.GetCurrentPrefabStage() != null) //������ ��������� �ߴ�
            {
                return ;
            }
            //foreach (var AutoPosition in FindObjectsOfType<AutoPosition>())
            {
                var AutoPosition = this;
                if (AutoPosition.success)
                {
                    return;
                }
                /*
                var root = GetRoot(AutoPosition.transform);
                if (root == null)
                {
                    return;
                }
                {
                    //var avatarDescriptor = root.GetComponentInChildren<VRCAvatarDescriptor>(true);
                    var components = root.GetComponentsInChildren<Component>(true);
                    var avatarDescriptor = System.Array.Find(components, x => x.GetType().Name.Contains("AvatarDescriptor"));
                    if (avatarDescriptor == null)
                    {
                        return;
                    }
                    root = avatarDescriptor.transform;
                }
                */
                var root = ObjectPath.GetVRCRoot(AutoPosition.transform, ObjectPath.VRCRootSearchOption.VRCRootOnly);
                if (root == null)
                {
                    return;
                }

                foreach (var positionData in AutoPosition.PositionDatas)
                {
                    //������Ʈ
                    Component component = null;
                    switch (positionData.target)
                    {
                        case Target.Object:
                            component = positionData.targetObject as Component;
                            break;
                        case Target.Parent:
                            break;
                        default:
                            break;
                    }
                    if (component == null)
                    {
                        continue;
                    }

                    var parent = positionData.GetParentTarget(root);
                    if (parent != null)
                    {

                        ObjectPath.ComponentMove(component, parent.gameObject);

                        //var newComponent = parent.gameObject.AddComponent(component.GetType());


                        //ObjectPath.ComponentMove(component,newComponent);

                        //DestroyImmediate(component);

                        //CopyClass(component,ref newComponent);
                        //var json = JsonUtility.ToJson(component);
                        //JsonUtility.FromJsonOverwrite(json, newComponent);

                        /*
                        foreach (var field in component.GetType().GetFields())
                        {
                            var value = field.GetValue(component);
                            field.SetValue(newComponent, value);
                        }
                        var properties = component.GetType().GetProperties();
                        foreach (var property in properties)
                        {
                            if (!property.CanWrite || !property.CanRead)
                            {
                                continue;
                            }
                            property.SetValue(newComponent, property.GetValue(component));
                        }
                        */
                        //var rc = newComponent as RotationConstraint;
                        //rc.GetSources
                    }

                }
                foreach (var positionData in AutoPosition.PositionDatas)
                {
                    //���ӿ�����Ʈ
                    Transform target = null;
                    switch (positionData.target)
                    {
                        case Target.Object:
                            if (positionData.targetObject == null)
                            {
                                target = AutoPosition.transform;
                            }
                            else
                            {
                                if (positionData.targetObject is Component)
                                {
                                    continue;
                                }
                                var gameObject = positionData.targetObject as GameObject;
                                target = gameObject.transform;
                            }
                            break;
                        case Target.Parent:
                            target = AutoPosition.GetParent(positionData.parentIndex);
                            break;
                        default:
                            break;
                    }
                    if (target == null)
                    {
                        continue;
                    }

                    var parent = positionData.GetParentTarget(root);
                    if (parent != null)
                    {
                        target.parent = parent;
                    }


                    /*
                    switch (positionData.tracking)
                    {
                        case Tracking.Path:
                            {
                                var parent = ObjectPath.Find(positionData.path, root.transform);
                                if (parent != null)
                                {
                                    target.parent = parent;
                                }
                            }
                            break;
                        case Tracking.Humanoid:
                            {
                                var parent = AhzkwidHumanoid.GetBoneTransform(root.gameObject, positionData.bone);
                                if (parent != null)
                                {
                                    target.parent = parent;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    */



                    if (positionData.updatePosition)
                    {
                        target.localPosition = positionData.position;
                    }

                    if (positionData.updateRotation)
                    {
                        target.localRotation = Quaternion.Euler(positionData.rotation);
                    }

                    if (positionData.updateScale)
                    {
                        target.localScale = positionData.scale;
                    }


                }

                AutoPosition.success = true;
                //if (success)
                {
                    //if (AutoPosition.autoDestroy)
                    {
                        switch (mergeTrigger)
                        {
                            case MergeTrigger.OneShot:
                                DestroyImmediate(AutoPosition);
                                break;
                            case MergeTrigger.Runtime:
                                break;
                            case MergeTrigger.Always:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        static void CopyClass<T>(T source,ref T target)
        {
            //lock (source)
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
        }


            // Update is called once per frame
        void Update()
        {
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