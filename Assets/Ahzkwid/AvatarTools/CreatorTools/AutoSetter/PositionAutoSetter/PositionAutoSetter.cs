

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{


#if UNITY_EDITOR
    using UnityEditor;
    using VRC.SDK3.Avatars.Components;

    [CustomEditor(typeof(PositionAutoSetter))]
    public class PositionAutoSetterEditor : Editor
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
            var option = (PositionAutoSetter.Option)property.FindPropertyRelative(nameof(PositionAutoSetter.PositionData.option)).intValue;
            switch (option)
            {
                case PositionAutoSetter.Option.GameObject:
                    break;
                case PositionAutoSetter.Option.Parent:
                    return EditorGUIUtility.singleLineHeight * 6;
                default:
                    break;
            }
            return  EditorGUIUtility.singleLineHeight*5;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var fieldRect = rect;

            fieldRect.height = EditorGUIUtility.singleLineHeight;
            {
                var path = nameof(PositionAutoSetter.PositionData.option);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            //fieldRect.width /= 2;
            {
                var option = (PositionAutoSetter.Option)property.FindPropertyRelative(nameof(PositionAutoSetter.PositionData.option)).intValue;
                switch (option)
                {
                    case PositionAutoSetter.Option.GameObject:
                        {
                            var path = nameof(PositionAutoSetter.PositionData.gameObject);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        break;
                    case PositionAutoSetter.Option.Parent:
                        var index = 0;
                        {
                            var path = nameof(PositionAutoSetter.PositionData.parentIndex);
                            index = property.FindPropertyRelative(path).intValue;
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        {
                            var target = property.serializedObject.targetObject as PositionAutoSetter;
                            var parent = target.GetParent(index);
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


            fieldRect.y += EditorGUIUtility.singleLineHeight;
            {
                var path = nameof(PositionAutoSetter.PositionData.position);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            {
                var path = nameof(PositionAutoSetter.PositionData.rotation);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            {
                var path = nameof(PositionAutoSetter.PositionData.scale);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }
        }
    }
    public class PositionDataAttribute : PropertyAttribute
    {

    }




    public class PositionAutoSetter : MonoBehaviour
    {
        public enum Option
        {
            Parent, GameObject
        }
        [PositionDataAttribute]
        public List<PositionData> PositionDatas = new List<PositionData>();
        [System.Serializable]
        public class PositionData
        {
            public Option option;
            public int parentIndex = 1;
            public GameObject gameObject;
            public Vector3 position = Vector3.zero;
            public Vector3 rotation = Vector3.zero;
            public Vector3 scale = Vector3.one;
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
        Transform GetRoot(Transform transform)
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
        bool success = false;

        public bool autoDestroy = true;
        void OnDrawGizmos()
        {
            if (success == false)
            {
                UnityEditor.Handles.Label(transform.position, "Finding Character");
                {
                    var root = GetRoot(transform);
                    if (root == null)
                    {
                        return;
                    }
                    var avatarDescriptor = root.GetComponentInChildren<VRCAvatarDescriptor>(true);
                    if (avatarDescriptor == null)
                    {
                        return;
                    }
                    root = avatarDescriptor.transform;

                    foreach (var positionData in PositionDatas)
                    {
                        Transform target = null;
                        switch (positionData.option)
                        {
                            case Option.GameObject:
                                if (positionData.gameObject == null)
                                {
                                    target = transform;
                                }
                                else
                                {
                                    target = positionData.gameObject.transform;
                                }
                                break;
                            case Option.Parent:
                                target = GetParent(positionData.parentIndex);
                                break;
                            default:
                                break;
                        }
                        target.localPosition = positionData.position;
                        target.localRotation = Quaternion.Euler(positionData.rotation);
                        target.localScale = positionData.scale;
                    }

                    success = true;
                    //if (success)
                    {
                        if (autoDestroy)
                        {
                            DestroyImmediate(this);
                        }
                    }
                }
            }
            else
            {
                UnityEditor.Handles.Label(transform.position, "Success AutoSetting");
            }
        }
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {

        }
    }
#endif
}