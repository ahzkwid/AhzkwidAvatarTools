using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{ 


#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(BlendshapeAutoSetter))]
    public class AutoBlendshapeSetterEditor : Editor
    {
        //    Hashtable reorderableListTable = new Hashtable();
        //    public void DrawStageDatas(string propertyPath)
        //    {

        //        var reorderableListProperty = serializedObject.FindProperty(propertyPath);

        //        if (reorderableListTable[propertyPath] == null)
        //        {
        //            reorderableListTable[propertyPath] = new ReorderableList(serializedObject, reorderableListProperty);
        //        }
        //        var reorderableList = (ReorderableList)reorderableListTable[propertyPath];

        //        serializedObject.Update();
        //        {

        //            //헤더명
        //            reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, $"{propertyPath} ({reorderableListProperty.arraySize})");

        //            //요소크기
        //            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 5;

        //            reorderableList.drawElementCallback =
        //            (Rect rect, int index, bool isActive, bool isFocused) =>
        //            {
        //                var elementProperty = reorderableListProperty.GetArrayElementAtIndex(index);

        //                var fieldRect = rect;
        //                fieldRect.height = EditorGUIUtility.singleLineHeight;
        //                EditorGUI.PropertyField(fieldRect, elementProperty.FindPropertyRelative(nameof(AutoBlendshapeSetter.BlendshapeTarget.mesh)));


        //                fieldRect.y += EditorGUIUtility.singleLineHeight;
        //                EditorGUI.PropertyField(fieldRect, elementProperty.FindPropertyRelative(nameof(AutoBlendshapeSetter.BlendshapeTarget.blendshapeValues)));
        //            };
        //            reorderableList.DoLayoutList();

        //            //선택된 필드
        //            if (reorderableList.index >= 0)
        //            {
        //                var index = reorderableList.index;
        //                if ((reorderableListProperty != null) && (reorderableListProperty.arraySize > 0) && (index < reorderableListProperty.arraySize))
        //                {
        //                    var elementAtIndex = reorderableListProperty.GetArrayElementAtIndex(index);

        //                    var mesh=elementAtIndex.FindPropertyRelative(nameof(AutoBlendshapeSetter.BlendshapeTarget.mesh)).objectReferenceValue;

        //                    /*
        //                    //기본표시
        //                    foreach (var field in typeof(AutoBlendshapeSetter.BlendshapeTarget).GetFields())
        //                    {
        //                        if ((field.IsStatic) || (field.IsNotSerialized))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (field.Name != elementName)
        //                            {
        //                                EditorGUILayout.PropertyField(elementAtIndex.FindPropertyRelative(field.Name));
        //                            }
        //                        }
        //                    }
        //                    */
        //                }
        //            }

        //            /*
        //            //요소
        //            var elementName = nameof(AutoBlendshapeSetter.BlendshapeTarget.mesh);
        //            reorderableList.drawElementCallback =
        //            (Rect rect, int index, bool isActive, bool isFocused) =>
        //            {
        //                EditorGUI.PropertyField(rect, reorderableListProperty.GetArrayElementAtIndex(index).FindPropertyRelative(elementName));
        //                //기본표시
        //                foreach (var field in typeof(AutoBlendshapeSetter.BlendshapeTarget).GetFields())
        //                {
        //                    if (field.Name != elementName)
        //                    {
        //                        EditorGUILayout.PropertyField(reorderableListProperty.GetArrayElementAtIndex(index).FindPropertyRelative(field.Name));
        //                    }
        //                }
        //            };
        //            reorderableList.DoLayoutList();
        //            */

        //            /*
        //            //선택된 필드
        //            if (reorderableList.index >= 0)
        //            {
        //                var index = reorderableList.index;
        //                if ((reorderableListProperty != null) && (reorderableListProperty.arraySize > 0) && (index < reorderableListProperty.arraySize))
        //                {
        //                    var elementAtIndex = reorderableListProperty.GetArrayElementAtIndex(index);

        //                    //기본표시
        //                    foreach (var field in typeof(AutoBlendshapeSetter.BlendshapeTarget).GetFields())
        //                    {
        //                        if ((field.IsStatic) || (field.IsNotSerialized))
        //                        {
        //                        }
        //                        else
        //                        {
        //                            if (field.Name != elementName)
        //                            {
        //                                EditorGUILayout.PropertyField(elementAtIndex.FindPropertyRelative(field.Name));
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            */

        //        }
        //        serializedObject.ApplyModifiedProperties();
        //    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //DrawStageDatas(nameof(AutoBlendshapeSetter.blendshapeTargets));

        var blendshapeAutoSetter = target as BlendshapeAutoSetter;
        foreach (var blendshapeTarget in blendshapeAutoSetter.blendshapeTargets)
        {

            var mesh = blendshapeTarget.mesh;
            if (EditorGUILayout.DropdownButton(new GUIContent($"{blendshapeTarget.mesh.name}.blendshapeNames"), FocusType.Passive))
            {
                GenericMenu menu = new GenericMenu();

                for (int i = 0; i < mesh.blendShapeCount; i++)
                {
                    menu.AddItem(new GUIContent($"{i}: {mesh.GetBlendShapeName(i)}"), false, Callback, i);
                }
                menu.ShowAsContext();


                void Callback(object obj)
                {
                    //sampleClass.gameObject.layer = (int)obj;
                }
            }
        }
        }


    }
#endif
public class BlendshapeAutoSetter : MonoBehaviour
{
    [System.Serializable]
    public class BlendshapeSettingData
    {
        public string name;
        [Range(0,100)]
        public float value = 100;
    }
    [System.Serializable]
    public class BlendshapeTarget
    {
        public Mesh mesh;
        public List<BlendshapeSettingData> blendshapeValues;
            //public Dictionary<string,float> keyValuePairs = new Dictionary<string,float>();
    }

        bool success = false;

        public bool autoDestroy = true;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var blendshapeAutoSetter = this;
            var transform = blendshapeAutoSetter.transform;
            if (success == false)
            {
                UnityEditor.Handles.Label(transform.position, "Finding Character");
                {

                    var parents = transform.GetComponentsInParent<Transform>();
                    Transform root = null;
                    if (parents.Length == 1)
                    {
                        root = transform;
                    }
                    else
                    {
                        root = System.Array.Find(parents, parent => parent.GetComponentsInParent<Transform>().Length == 1);
                    }
                    var skinnedMeshRenderers = root.GetComponentsInChildren<SkinnedMeshRenderer>();
                    var meshs = System.Array.ConvertAll(skinnedMeshRenderers, x => x.sharedMesh);
                    meshs = System.Array.FindAll(meshs, x => x != null);


                    foreach (var blendshapeTarget in blendshapeAutoSetter.blendshapeTargets)
                    {
                        var renderers = System.Array.FindAll(skinnedMeshRenderers, renderer => renderer.sharedMesh == blendshapeTarget.mesh);
                        foreach (var renderer in renderers)
                        {
                            foreach (var blendshapeValue in blendshapeTarget.blendshapeValues)
                            {
                                var index = blendshapeTarget.mesh.GetBlendShapeIndex(blendshapeValue.name);
                                renderer.SetBlendShapeWeight(index, blendshapeValue.value);
                                success = true;
                            }
                        }
                    }
                }
                if (success)
                {
                    if (autoDestroy)
                    {
                        DestroyImmediate(this);
                    }
                }
            }
            else
            {
                UnityEditor.Handles.Label(transform.position, "Success Blendshape AutoSetting");
                UnityEditor.Handles.Button(transform.position, Quaternion.identity, 2f, 4f, Handles.RectangleHandleCap);
            }
        }
#endif
        public List<BlendshapeTarget> blendshapeTargets= new List<BlendshapeTarget>();
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
}
}