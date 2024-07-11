

using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{


#if UNITY_EDITOR
    using UnityEditor;
    using System.Linq;

    [CustomEditor(typeof(AutoMerge))]
    public class AutoMergeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            {
                var autoMerge = target as AutoMerge;
                var targetMeshs = autoMerge.targetMeshs;
                var valueChanged = false;


                targetMeshs = System.Array.ConvertAll(targetMeshs, value =>
                {
                    if (value is GameObject)
                    {
                        var gameObject = value as GameObject;
                        var renderer = gameObject.GetComponent<SkinnedMeshRenderer>();
                        if (renderer != null)
                        {
                            value = renderer.sharedMesh;
                            valueChanged = true;
                        }
                    }
                    if ((value is Mesh) == false)
                    {
                        value = null;
                        valueChanged = true;
                    }
                    return value;
                });
                if (valueChanged)
                {
                    autoMerge.targetMeshs= targetMeshs;
                    UnityEditor.EditorUtility.SetDirty(target);
                }
            }

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










    [CustomPropertyDrawer(typeof(ClothRootAttribute))]
    public class ClothRootDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var option = (AutoMerge.Option)property.FindPropertyRelative(nameof(AutoMerge.ClothRoot.option)).intValue;
            switch (option)
            {
                case AutoMerge.Option.GameObject:
                    break;
                case AutoMerge.Option.Parent:
                    return EditorGUIUtility.singleLineHeight * 3;
                default:
                    break;
            }
            return EditorGUIUtility.singleLineHeight * 2;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var fieldRect = rect;

            fieldRect.height = EditorGUIUtility.singleLineHeight;
            {
                var path = nameof(AutoMerge.ClothRoot.option);
                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
            }

            fieldRect.y += EditorGUIUtility.singleLineHeight;
            //fieldRect.width /= 2;
            {
                var option = (AutoMerge.Option)property.FindPropertyRelative(nameof(AutoMerge.ClothRoot.option)).intValue;
                switch (option)
                {
                    case AutoMerge.Option.GameObject:
                        {
                            var path = nameof(AutoMerge.ClothRoot.gameObject);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        break;
                    case AutoMerge.Option.Parent:
                        var index = 0;
                        {
                            var path = nameof(AutoMerge.ClothRoot.parentIndex);
                            index = property.FindPropertyRelative(path).intValue;
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                        {
                            var target = property.serializedObject.targetObject as AutoMerge;
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


        }
    }
    public class ClothRootAttribute : PropertyAttribute
    {

    }









    [ExecuteInEditMode]
    public class AutoMerge : MonoBehaviour
    {
        public enum Option
        {
            Parent, GameObject
        }


        public Object[] targetMeshs;


        [ClothRootAttribute]
        public List<ClothRoot> clothRoots = new List<ClothRoot>();
        [System.Serializable]
        public class ClothRoot
        {
            public Option option;
            public int parentIndex = 1;
            public GameObject gameObject;
        }





        //public Transform clothRoot;
        //public GameObject clothRoot;

        public int parentIndex = 1;


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



        public Transform GetClothRootTransform(ClothRoot clothRoot)
        {

            Transform target = null;
            switch (clothRoot.option)
            {
                case Option.GameObject:
                    if (clothRoot.gameObject == null)
                    {
                        target = transform;
                    }
                    else
                    {
                        target = clothRoot.gameObject.transform;
                    }
                    break;
                case Option.Parent:
                    target = GetParent(parentIndex);
                    break;
                default:
                    break;
            }

            /*
            var rootTransform = clothRoot;
            if (rootTransform == null)
            {
                rootTransform = transform;
            }
            */
            return target;
        }
        public Transform GetTargetTransform()
        {
            if ((targetMeshs == null)|| (targetMeshs.Length==0))
            {
                return null;
            }

            foreach (var targetMesh in targetMeshs)
            {
                if (targetMesh == null)
                {
                    continue;
                }
                var targets = FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
                targets = System.Array.FindAll(targets, target => target.sharedMesh == targetMesh);


                //화면상거리
                var screenPoint = HandleUtility.WorldToGUIPoint(transform.position);
                targets = targets.OrderBy(target => Vector2.Distance(HandleUtility.WorldToGUIPoint(target.transform.position), screenPoint)).ToArray();

                //절대거리
                //targets = targets.OrderBy(target => Vector3.Distance(target.transform.position, transform.position)).ToArray();


                if (targets.Length <= 0)
                {
                    continue;
                }
                var targetTransform = targets.First().transform;
                if (targetTransform.parent != null)
                {
                    targetTransform = targetTransform.parent;
                }
                return targetTransform;
            }


            return null;
        }


        public void DrawSnap()
        {

            //Gizmos.DrawWireSphere(transform.position, 1);

            if (clothRoots.Count == 0)
            {
                return;
            }
            var clothRoot = GetClothRootTransform(clothRoots.First());
            if (clothRoot == null)
            {
                return;
            }
            var targetTransform = GetTargetTransform();
            if (targetTransform == null)
            {
                return;
            }
            var targetPosition = targetTransform.position + targetTransform.up * 0.5f;


            Gizmos.DrawLine(clothRoot.position, targetPosition);
            //DrawWireCub(targetPosition, Vector3.one, Quaternion.identity);
            //UnityEditor.Handles.DrawLine(clothRoot.position, targetPosition);
            //Gizmos.DrawWireSphere(transform.position, 1);
            Gizmos.DrawWireSphere(targetPosition, 0.5f);
        }
        static void DrawWireCub(Vector3 center, Vector3 size, Quaternion rotation)
        {
#if UNITY_EDITOR
            {
                var size1 = size;
                var size2 = size1;
                for (int x = -1; x <= 1; x += 2)
                {
                    for (int y = -1; y <= 1; y += 2)
                    {
                        size1.x = size.x * x;
                        size1.y = size.y * y;
                        size2 = size1;

                        size1.z = size.z;
                        size2.z = -size1.z;
                        DrawLine(size1, size2);
                    }
                }
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        size1.y = size.y * y;
                        size1.z = size.z * z;
                        size2 = size1;

                        size1.x = size.x;
                        size2.x = -size1.x;
                        DrawLine(size1, size2);
                    }
                }
                for (int x = -1; x <= 1; x += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        size1.x = size.x * x;
                        size1.z = size.z * z;
                        size2 = size1;

                        size1.y = size.y;
                        size2.y = -size1.y;
                        DrawLine(size1, size2);
                    }
                }
            }
            void DrawLine(Vector3 size1, Vector3 size2)
            {
                Debug.DrawLine(center + rotation * (size1 / 2), center + rotation * (size2 / 2));
            }
#endif
        }
        public void SetParent()
        {
            var clothRoot = GetClothRootTransform(clothRoots.First());
            var targetTransform = GetTargetTransform();

            clothRoot.parent = targetTransform;
        }




         void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

         void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
        }

        void OnSceneGUI(SceneView obj)
        {
            var autoMerge = this;
            autoMerge.HandleDragAndDropEvents();
            HandleDragAndDropEvents();
        }
        static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            foreach (var autoMerge in FindObjectsOfType<AutoMerge>())
            {
                autoMerge.HandleDragAndDropEvents();
            }
        }
        public void HandleDragAndDropEvents()
        {
            if (Event.current.type == EventType.DragUpdated)
            {
                OnDragUpdated();
            }
            if (Event.current.type == EventType.DragPerform)
            {
                OnDragPerform();
            }
        }
        void OnDragUpdated()
        {
            //Debug.Log("OnDragUpdated()");
            //drag = true;
        }
        void OnDragPerform()
        {
            //Debug.Log("OnDragPerform()");
            endDrag = true;
        }
        bool drag = false;
        bool endDrag = false;

        //public bool autoDestroy = true;
        void OnDrawGizmos()
        {
            DrawSnap();

        }



        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {




            if (endDrag == false)
            {
                return;
            }
            endDrag = true;

            if (clothRoots.Count == 0)
            {
                return;
            }
            var clothRoot = GetClothRootTransform(clothRoots.First());
            var targetTransform = GetTargetTransform();



            if (clothRoot.parent == null)
            {
                SetParent();
            }
            if (targetTransform == null)
            {
                return;
            }
            if (clothRoot.parent == targetTransform)
            {
                AvatarMergeTool.Merge(targetTransform.gameObject, clothRoot.gameObject);
                DestroyImmediate(this);
            }













        }
    }
#endif
}