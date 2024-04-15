using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;
    /*

[CustomPropertyDrawer(typeof(BlendshapeSettingDataAttribute))]
public class BlendshapeSettingDataDrawer : PropertyDrawer
{
public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
{
    var fieldRect = rect;
    fieldRect.width /= 2;
    //{
    //    fieldRect.width /= 2f/3f;
    //    var path = nameof(BlendshapeAutoSetter.BlendshapeSettingData.mesh);
    //    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
    //}
    //fieldRect.x += fieldRect.width;

    var mesh = property.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeTarget.mesh)).objectReferenceValue as Mesh;

        {
            var path = nameof(BlendshapeAutoSetter.BlendshapeSettingData.key);
            if (mesh == null)
            {
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }
            else
            {
                var propertyRelative = property.FindPropertyRelative(path);
                var key = propertyRelative.stringValue;
                var blendshapeTargetsIndex = property.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeSettingData.blendshapeTargetsIndex)).intValue;
                var blendshapeValuesIndex = property.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeSettingData.blendshapeValuesIndex)).intValue;
                //if (EditorGUILayout.DropdownButton(new GUIContent($"{mesh.name}.blendshapeNames"), FocusType.Passive))
                if (EditorGUI.DropdownButton(fieldRect, new GUIContent($"{key}"), FocusType.Passive))
                {
                    var menu = new GenericMenu();

                    for (int i = 0; i < mesh.blendShapeCount; i++)
                    {
                        menu.AddItem(new GUIContent($"{i}: {mesh.GetBlendShapeName(i)}"), false, Callback, new object[] { mesh.GetBlendShapeName(i), blendshapeTargetsIndex, blendshapeValuesIndex });
                        //menu.AddItem(new GUIContent($"{i}: {mesh.GetBlendShapeName(i)}"), false, Callback, new object[] { mesh.GetBlendShapeName(i), propertyRelative });
                    }
                    menu.ShowAsContext();


                    void Callback(object obj)
                    {
                        var objs= obj as object[];
                        var key = objs[0] as string;
                        var blendshapeTargetsIndex = (int)objs[1];
                        var blendshapeValuesIndex = (int)objs[2];
                        //SerializedProperty
                    }
                }
            }
        }
        {
            fieldRect.x += fieldRect.width;
            var propertyRelative = property.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeSettingData.value));
            propertyRelative.floatValue = EditorGUI.Slider(fieldRect, propertyRelative.floatValue, 0, 100);
        }
        //EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeSettingData.value)), label, true);
    }
}
public class BlendshapeSettingDataAttribute : PropertyAttribute
{

}

    */








    [CustomEditor(typeof(BlendshapeAutoSetter))]
    public class AutoBlendshapeSetterEditor : Editor
    {
        Hashtable reorderableListTable = new Hashtable();




        public void DrawBlendshapeValues(Rect offset,SerializedProperty property, Mesh mesh, List<BlendshapeAutoSetter.BlendshapeSettingData> blendshapeValues)
        {
            var propertyPath = property.propertyPath;
            var reorderableListProperty = property;


            //var reorderableListProperty = serializedObject.FindProperty(propertyPath);

            if (reorderableListTable[propertyPath] == null)
            {
                reorderableListTable[propertyPath] = new ReorderableList(serializedObject, reorderableListProperty);
            }
            var reorderableList = (ReorderableList)reorderableListTable[propertyPath];

                //헤더명
                reorderableList.drawHeaderCallback = (rect) => {
                    EditorGUI.LabelField(rect, $"{property.name} ({reorderableListProperty.arraySize})");
                    };
                

                reorderableList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var elementProperty = reorderableListProperty.GetArrayElementAtIndex(index);

                    var fieldRect = rect;
                    fieldRect.height = EditorGUIUtility.singleLineHeight;
                    fieldRect.width /= 2;


                    var blendshapeValue = blendshapeValues[index];
                    {
                        var path = nameof(BlendshapeAutoSetter.BlendshapeSettingData.key);
                        if (mesh == null)
                        {
                            //EditorGUI.PropertyField(fieldRect, elementProperty.FindPropertyRelative(path), new GUIContent(path), true);
                            elementProperty.FindPropertyRelative(path).stringValue = EditorGUI.TextField(fieldRect, elementProperty.FindPropertyRelative(path).stringValue);
                        }
                        else
                        {
                            var propertyRelative = elementProperty.FindPropertyRelative(path);
                            var key = propertyRelative.stringValue;
                            //if (EditorGUILayout.DropdownButton(new GUIContent($"{mesh.name}.blendshapeNames"), FocusType.Passive))
                            if (EditorGUI.DropdownButton(fieldRect, new GUIContent($"{key}"), FocusType.Passive))
                            {
                                var menu = new GenericMenu();

                                for (int i = 0; i < mesh.blendShapeCount; i++)
                                {
                                    menu.AddItem(new GUIContent($"{i}: {mesh.GetBlendShapeName(i)}"), false, Callback, new object[] { mesh.GetBlendShapeName(i), blendshapeValue });
                                    //menu.AddItem(new GUIContent($"{i}: {mesh.GetBlendShapeName(i)}"), false, Callback, new object[] { mesh.GetBlendShapeName(i), propertyRelative });
                                }
                                menu.ShowAsContext();


                                void Callback(object obj)
                                {
                                    var objs = obj as object[];
                                    var key = objs[0] as string;
                                    var blendshapeValue = objs[1] as BlendshapeAutoSetter.BlendshapeSettingData;
                                    propertyRelative.stringValue = key;
                                    blendshapeValue.key = key;
                                    //SerializedProperty
                                }
                            }
                        }
                    }
                    //if (EditorGUILayout.DropdownButton(new GUIContent($"{mesh.name}.blendshapeNames"), FocusType.Passive))
                    //var key = elementProperty.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeSettingData.key)).stringValue;
                    //if (EditorGUI.DropdownButton(fieldRect, new GUIContent($"{key}"), FocusType.Passive))
                    //{
                    //    var menu = new GenericMenu();

                    //    for (int i = 0; i < mesh.blendShapeCount; i++)
                    //    {
                    //        menu.AddItem(new GUIContent($"{i}: {mesh.GetBlendShapeName(i)}"), false, Callback, i);
                    //    }
                    //    menu.ShowAsContext();


                    //    void Callback(object obj)
                    //    {
                    //        //sampleClass.gameObject.layer = (int)obj;
                    //    }
                    //}
                    {
                        fieldRect.x += fieldRect.width;

                        //fieldRect.y += EditorGUIUtility.singleLineHeight;
                        var propertyRelative = elementProperty.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeSettingData.value));
                        propertyRelative.floatValue = EditorGUI.Slider(fieldRect, propertyRelative.floatValue, 0, 100);
                    }
                };

                offset.y += EditorGUIUtility.singleLineHeight;
                reorderableList.DoList(offset);


                //요소별크기
                reorderableList.elementHeightCallback = (index) => {

                    var height = EditorGUIUtility.singleLineHeight;
                    var elementProperty = reorderableListProperty.GetArrayElementAtIndex(index);

                    foreach (var field in typeof(BlendshapeAutoSetter.BlendshapeTarget).GetFields())
                    {
                        if ((field.IsStatic) || (field.IsNotSerialized))
                        {
                        }
                        else
                        {
                            //height = EditorGUI.GetPropertyHeight(elementProperty, true);

                            //height -= EditorGUIUtility.singleLineHeight * 2;
                            height = EditorGUIUtility.singleLineHeight;
                        }
                    }
                    return height;
                };

        }

        public void DrawBlendshapeTargets(string propertyPath)
        {
            /*
{
    //메쉬값과 인덱스들을 항상 통일
    var blendshapeAutoSetter = target as BlendshapeAutoSetter;
    //var flag = false; //임시값들이라 더티플래그 필요 없음

    for (int i = 0; i < blendshapeAutoSetter.blendshapeTargets.Count; i++)
    {
        var blendshapeTarget = blendshapeAutoSetter.blendshapeTargets[i];

        if (blendshapeTarget.mesh == null)
        {
            return;
        }


        for (int j = 0; j < blendshapeTarget.blendshapeValues.Count; j++)
        {
            var blendshapeValue = blendshapeTarget.blendshapeValues[j];

            //if (blendshapeValue.mesh != blendshapeTarget.mesh)
            //{
            //    blendshapeValue.mesh = blendshapeTarget.mesh;
            //    flag = true;
            //}
            blendshapeValue.mesh = blendshapeTarget.mesh;
                        blendshapeValue.blendshapeTargetsIndex = i;
                        blendshapeValue.blendshapeValuesIndex = j;
                    }
                }
            //if (flag)
            //{
            //    UnityEditor.EditorUtility.SetDirty(target);
            //}
        }
            
            */

            var blendshapeAutoSetter = target as BlendshapeAutoSetter;

            var reorderableListProperty = serializedObject.FindProperty(propertyPath);

            if (reorderableListTable[propertyPath] == null)
            {
                reorderableListTable[propertyPath] = new ReorderableList(serializedObject, reorderableListProperty);
            }
            var reorderableList = (ReorderableList)reorderableListTable[propertyPath];

            {

                //헤더명
                reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, $"{propertyPath} ({reorderableListProperty.arraySize})");

                //요소크기
                //reorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 5;

                reorderableList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    var elementProperty = reorderableListProperty.GetArrayElementAtIndex(index);

                        var fieldRect = rect;
                        fieldRect.height = EditorGUIUtility.singleLineHeight;
                        EditorGUI.PropertyField(fieldRect, elementProperty.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeTarget.mesh)));

                    var mesh = elementProperty.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeTarget.mesh)).objectReferenceValue as Mesh;
                    var blendshapeValues = blendshapeAutoSetter.blendshapeTargets[index].blendshapeValues;
                    /*
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        //if (EditorGUILayout.DropdownButton(new GUIContent($"{mesh.name}.blendshapeNames"), FocusType.Passive))
                        if (EditorGUI.DropdownButton(fieldRect,new GUIContent($"{mesh.name}.blendshapeNames"), FocusType.Passive))
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
                    */

                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                    DrawBlendshapeValues(rect, elementProperty.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeTarget.blendshapeValues)),mesh, blendshapeValues);
                    {
                        //var property = elementProperty.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeTarget.blendshapeValues));
                        //EditorGUI.PropertyField(fieldRect, property);
                    }
                    //EditorGUI.PropertyField(fieldRect, elementProperty.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeTarget.blendshapeValues)));

                };
                reorderableList.DoLayoutList();


                //요소별크기
                reorderableList.elementHeightCallback = (index) => {

                    var elementProperty = reorderableListProperty.GetArrayElementAtIndex(index);
                    var height = 0f;
                    // height = EditorGUI.GetPropertyHeight(elementProperty, true);

                    height += EditorGUIUtility.singleLineHeight*3;


                    foreach (var field in typeof(BlendshapeAutoSetter.BlendshapeTarget).GetFields())
                    {
                        if ((field.IsStatic) || (field.IsNotSerialized))
                        {
                        }
                        else
                        {


                            var fieldProperty = elementProperty.FindPropertyRelative(field.Name);
                            if (fieldProperty.isArray)
                            {
                                height += EditorGUIUtility.singleLineHeight*1.11f * Mathf.Max(1,fieldProperty.arraySize);
                            }
                            else
                            {
                               height += EditorGUIUtility.singleLineHeight;
                            }
                        }
                    }
                    return height;
                };


                /*
                //선택된 필드
                if (reorderableList.index >= 0)
                {
                    var index = reorderableList.index;
                    if ((reorderableListProperty != null) && (reorderableListProperty.arraySize > 0) && (index < reorderableListProperty.arraySize))
                    {
                        var elementAtIndex = reorderableListProperty.GetArrayElementAtIndex(index);

                        var mesh = elementAtIndex.FindPropertyRelative(nameof(BlendshapeAutoSetter.BlendshapeTarget.mesh)).objectReferenceValue;

                        //기본표시
                        foreach (var field in typeof(BlendshapeAutoSetter.BlendshapeTarget).GetFields())
                        {
                            if ((field.IsStatic) || (field.IsNotSerialized))
                            {
                            }
                            else
                            {
                                //if (field.Name != elementName)
                                {
                                    EditorGUILayout.PropertyField(elementAtIndex.FindPropertyRelative(field.Name));
                                }
                            }
                        }
                    }
                }
                */

                /*
                //요소
                var elementName = nameof(AutoBlendshapeSetter.BlendshapeTarget.mesh);
                reorderableList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    EditorGUI.PropertyField(rect, reorderableListProperty.GetArrayElementAtIndex(index).FindPropertyRelative(elementName));
                    //기본표시
                    foreach (var field in typeof(AutoBlendshapeSetter.BlendshapeTarget).GetFields())
                    {
                        if (field.Name != elementName)
                        {
                            EditorGUILayout.PropertyField(reorderableListProperty.GetArrayElementAtIndex(index).FindPropertyRelative(field.Name));
                        }
                    }
                };
                reorderableList.DoLayoutList();
                */

                /*
                //선택된 필드
                if (reorderableList.index >= 0)
                {
                    var index = reorderableList.index;
                    if ((reorderableListProperty != null) && (reorderableListProperty.arraySize > 0) && (index < reorderableListProperty.arraySize))
                    {
                        var elementAtIndex = reorderableListProperty.GetArrayElementAtIndex(index);

                        //기본표시
                        foreach (var field in typeof(AutoBlendshapeSetter.BlendshapeTarget).GetFields())
                        {
                            if ((field.IsStatic) || (field.IsNotSerialized))
                            {
                            }
                            else
                            {
                                if (field.Name != elementName)
                                {
                                    EditorGUILayout.PropertyField(elementAtIndex.FindPropertyRelative(field.Name));
                                }
                            }
                        }
                    }
                }
                */

            }
        }

        public override void OnInspectorGUI()
    {
            //base.OnInspectorGUI();

            GUI.enabled = false;
            {
                var script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
            GUI.enabled = true;






            serializedObject.Update();
            {
                DrawPropertiesExcluding(serializedObject, "m_Script", nameof(BlendshapeAutoSetter.blendshapeTargets));


                DrawBlendshapeTargets(nameof(BlendshapeAutoSetter.blendshapeTargets));

                var blendshapeAutoSetter = target as BlendshapeAutoSetter;
                foreach (var blendshapeTarget in blendshapeAutoSetter.blendshapeTargets)
                {
                    if (blendshapeTarget == null)
                    {
                        continue;
                    }
                    var mesh = blendshapeTarget.mesh;
                    if (mesh == null)
                    {
                        continue;
                    }
                }
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
#endif


    


    public class BlendshapeAutoSetter : MonoBehaviour
{
    [System.Serializable]
    public class BlendshapeSettingData
    {
        public string key;
        public float value = 100;
    }
    [System.Serializable]
    public class BlendshapeTarget
    {
        public Mesh mesh;
        //[BlendshapeSettingData]
        public List<BlendshapeSettingData> blendshapeValues;
        //public Dictionary<string,float> keyValuePairs = new Dictionary<string,float>();
    }

        bool success = false;

        public bool autoDestroy = true;

        public List<BlendshapeTarget> blendshapeTargets = new List<BlendshapeTarget>();
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
                                var index = blendshapeTarget.mesh.GetBlendShapeIndex(blendshapeValue.key);
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
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        
    }
}
}
