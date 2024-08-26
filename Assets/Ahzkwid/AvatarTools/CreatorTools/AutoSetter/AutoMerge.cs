

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;

namespace Ahzkwid
{


    [CustomEditor(typeof(AutoMerge))]
    public class AutoMergeEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            GUI.enabled = false;
            {
                var script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
            GUI.enabled = true;


            //base.OnInspectorGUI();
            serializedObject.Update();
            {
                {
                    var category = (AutoMerge.Category)serializedObject.FindProperty(nameof(AutoMerge.category)).enumValueIndex;
                    switch (category)
                    {
                        case AutoMerge.Category.Cloth:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AutoMerge.mergeTrigger)));
                            break;
                        case AutoMerge.Category.Item:
                            break;
                        default:
                            break;
                    }
                }



                DrawPropertiesExcluding(serializedObject, "m_Script", nameof(AutoMerge.mergeTrigger));
            }
            serializedObject.ApplyModifiedProperties();

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
            var option = (AutoMerge.ClothRoot.Option)property.FindPropertyRelative(nameof(AutoMerge.ClothRoot.option)).intValue;
            switch (option)
            {
                case AutoMerge.ClothRoot.Option.GameObject:
                    break;
                case AutoMerge.ClothRoot.Option.Parent:
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
                var option = (AutoMerge.ClothRoot.Option)property.FindPropertyRelative(nameof(AutoMerge.ClothRoot.option)).intValue;
                switch (option)
                {
                    case AutoMerge.ClothRoot.Option.GameObject:
                        {
                            var path = nameof(AutoMerge.ClothRoot.gameObject);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                        }
                        break;
                    case AutoMerge.ClothRoot.Option.Parent:
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





    /*
    [InitializeOnLoad]
    public class FastFind
    {
        static FastFind()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;


            //autoMerges = FindAutoMergeFast();
            //if (autoMerges.Length>0)
            {
                EditorApplication.hierarchyChanged += OnHierarchyChanged;
                //UpdateList();
            }
        }
        static void UpdateList()
        {
            renderers = Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
            //autoMerges = Object.FindObjectsByType<AutoMerge>(FindObjectsSortMode.None);
        }


        static System.DateTime lastDateTimeFind = System.DateTime.MinValue;
        static SkinnedMeshRenderer[] renderers = new SkinnedMeshRenderer[] { };
        public static SkinnedMeshRenderer[] FindSkinnedMeshRendererFast()
        {
            //return Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
            //renderers = System.Array.FindAll(renderers, x => x != null);
            foreach (var item in renderers)
            {
                if (item==null)
                {
                    renderers = Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
                    return renderers;
                }
            }
            if (renderers.Length != 0)
            {
                return renderers;
                var dateTime = System.DateTime.Now;
                if (dateTime < lastDateTimeFind.AddSeconds(10f))
                {
                    return renderers;
                }
                lastDateTimeFind = dateTime;

            }


            renderers = Object.FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);

            return renderers;
        }


        static System.DateTime lastDateTimeFindAuro = System.DateTime.MinValue;
        static AutoMerge[] autoMerges = new AutoMerge[] { };
        public static AutoMerge[] FindAutoMergeFast()
        {
            //autoMerges = System.Array.FindAll(autoMerges, x => x != null);
            autoMerges = Object.FindObjectsByType<AutoMerge>(FindObjectsSortMode.None);

            return autoMerges;
            foreach (var item in autoMerges)
            {
                if (item == null)
                {
                    autoMerges = Object.FindObjectsByType<AutoMerge>(FindObjectsSortMode.None);

                    return autoMerges;
                }
            }
            if (autoMerges.Length != 0)
            {
                return autoMerges;
                var dateTime = System.DateTime.Now;
                if (dateTime < lastDateTimeFindAuro.AddSeconds(10f))
                {
                    return autoMerges;
                }
                lastDateTimeFindAuro = dateTime;

            }


            autoMerges = Object.FindObjectsByType<AutoMerge>(FindObjectsSortMode.None);

            return autoMerges;
        }



        private static void OnHierarchyChanged()
        {
            //Debug.Log("하이어라키 변경");
            UpdateList();

        }
    }
    */


    [ExecuteInEditMode]
    public class AutoMerge : MonoBehaviour
    {
        public enum Category
        {
            Cloth, Item
        }
        public Category category = Category.Cloth;




        public enum MergeTrigger
        {
            Always, Runtime
        }
        public MergeTrigger mergeTrigger = MergeTrigger.Runtime;

        public Object[] targetMeshs;


        [ClothRootAttribute]
        public List<ClothRoot> clothRoots = new List<ClothRoot>();
        [System.Serializable]
        public class ClothRoot
        {
            public enum Option
            {
                Parent, GameObject
            }

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


        public Transform GetClothRootTransform()
        {
            return GetClothRootTransform(clothRoots.First());
        }
        public Transform GetClothRootTransform(ClothRoot clothRoot)
        {



            Transform target = null;
            switch (clothRoot.option)
            {
                case ClothRoot.Option.GameObject:
                    if (clothRoot.gameObject == null)
                    {
                        target = transform;
                    }
                    else
                    {
                        target = clothRoot.gameObject.transform;
                    }
                    break;
                case ClothRoot.Option.Parent:
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
            var targetTransforms = new List<Transform>();
            if ((targetMeshs == null) || (targetMeshs.Length == 0))
            {
                var activeScene = SceneManager.GetActiveScene();
                if (activeScene!=null)
                {
                    var gameObjects = activeScene.GetRootGameObjects().ToList();
                    gameObjects = gameObjects.FindAll(x => x.activeInHierarchy);
                    targetTransforms = gameObjects.ConvertAll(x => x.transform);
                    targetTransforms = targetTransforms.FindAll(x=> ObjectPath.GetVRCRoot(x, ObjectPath.VRCRootSearchOption.VRCRootOnly));

                }
                //return null;
            }
            else
            {
                var renderers = FindObjectsByType<SkinnedMeshRenderer>(FindObjectsSortMode.None);
                //var renderers = FastFind.FindSkinnedMeshRendererFast();
                foreach (var targetMesh in targetMeshs)
                {
                    if (targetMesh == null)
                    {
                        continue;
                    }
                    var targets = System.Array.FindAll(renderers, target => target.sharedMesh == targetMesh).ToList();

                    if (targets.Count <= 0)
                    {
                        continue;
                    }

                    //화면상거리
                    var screenPoint = HandleUtility.WorldToGUIPoint(transform.position);
                    targets = targets.OrderBy(target => Vector2.Distance(HandleUtility.WorldToGUIPoint(target.transform.position), screenPoint)).ToList();

                    //절대거리
                    //targets = targets.OrderBy(target => Vector3.Distance(target.transform.position, transform.position)).ToArray();


                    if (targets.Count <= 0)
                    {
                        continue;
                    }
                    targetTransforms.AddRange(targets.ConvertAll(x=>x.transform));

                    /*
                    var targetTransform = targets.First().transform;
                    if (targetTransform.parent != null)
                    {
                        targetTransform = targetTransform.parent;
                    }
                    return targetTransform;
                    */
                }

            }




            if (targetTransforms.Count > 0)
            {
                var targetTransform = targetTransforms.First();
                if (targetTransform.parent != null)
                {
                    targetTransform = ObjectPath.GetVRCRoot(targetTransform);
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
            var clothRoot = GetClothRootTransform();
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
        }
        public void SetParent()
        {
            var clothRoot = GetClothRootTransform();
            var targetTransform = GetTargetTransform();

            clothRoot.parent = targetTransform;
        }




        void OnEnable()
        {
            autoMerges.Add(this);
            autoMerges = autoMerges.FindAll(x => x != null);
            autoMerges = autoMerges.GroupBy(x => x).Select(x => x.First()).ToList();

            SceneView.duringSceneGui -= OnSceneGUI;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
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
        static List<AutoMerge> autoMerges=new List<AutoMerge>();
        static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            //var autoMerges = FastFind.FindAutoMergeFast();
            //var autoMerges = FindObjectsByType<AutoMerge>(FindObjectsSortMode.None);

            //foreach (var autoMerge in FindObjectsOfType<AutoMerge>())
            autoMerges = autoMerges.FindAll(x => x != null);
            autoMerges = autoMerges.GroupBy(x => x).Select(x => x.First()).ToList();
            foreach (var autoMerge in autoMerges)
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
            if (SnapCheck())
            {
                DrawSnap();
            }
            Update();

        }

        bool SnapCheck()
        {
            var clothRoot = GetClothRootTransform();
            if (clothRoot.parent == null)
            {
                return true;
            }
            return false;
        }

        void Snap()
        {
            //Debug.Log("Snap");
            if (clothRoots.Count == 0)
            {
                return;
            }


            if (SnapCheck())
            {
                SetParent();
            }
        }


        public void Merge()
        {
            var clothRoot = GetClothRootTransform();
            /*
            var targetTransform = GetTargetTransform();
            if (targetTransform == null)
            {
                return;
            }
            if (clothRoot.parent == targetTransform)
            {
                AvatarMergeTool.Merge(targetTransform.gameObject, clothRoot.gameObject);
                DestroyImmediate(this);
            }
            */
            var targetTransform = ObjectPath.GetVRCRoot(clothRoot, ObjectPath.VRCRootSearchOption.IncludeVRCRoot);
            if (targetTransform != clothRoot)
            {
                AvatarMergeTool.Merge(targetTransform.gameObject, clothRoot.gameObject);
                DestroyImmediate(this);
            }
        }
        public void FollowBones(Transform from,Transform to)
        {

            var transforms=from.GetComponentsInChildren<Transform>(true);


            foreach (var transform in transforms)
            {
                var equalTransform = ObjectPath.EqualTransform(from, to, transform);
                if (equalTransform == null)
                {
                    continue;
                }
                transform.localScale = equalTransform.localScale;
                transform.localPosition = equalTransform.localPosition;
                transform.localRotation = equalTransform.localRotation;
            }


        }
        public void FollowBones()
        {
            switch (mergeTrigger)
            {
                case MergeTrigger.Always:
                    return;
                case MergeTrigger.Runtime:
                    if (EditorApplication.isPlaying)
                    {
                        return;
                    }
                    break;
                default:
                    break;
            }


            var clothRoot = GetClothRootTransform();

            var targetTransform = ObjectPath.GetVRCRoot(clothRoot, ObjectPath.VRCRootSearchOption.IncludeVRCRoot);
            if (targetTransform != clothRoot)
            {
                FollowBones(clothRoot,targetTransform);
            }
        }
        public void Run()
        {
            if (clothRoots.Count == 0)
            {
                return;
            }
            switch (category)
            {
                case Category.Cloth:
                    break;
                case Category.Item:
                    return;
                default:
                    break;
            }

            Merge();
            //if (clothRoot.parent == null)

        }

        //System.DateTime lastDateTime = System.DateTime.MinValue;
        // Start is called before the first frame update
        // Update is called once per frame
        void Update()
        {
            {
                if (endDrag)
                {
                    /*

                    var dateTime = System.DateTime.Now;
                    if (dateTime < lastDateTime.AddSeconds(0.1f))
                    {
                        return;
                    }
                    lastDateTime = dateTime;

                    */

                    Snap();
                    endDrag = false;
                }



            }
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
            FollowBones();
        }
    }
}
#endif