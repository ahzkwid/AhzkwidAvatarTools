

using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{
    using System;


#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.Search;

    [CustomEditor(typeof(AutoObjectSetting))]
    public class AutoObjectSettingEditor : Editor
    {
        public override void OnInspectorGUI()
        {



            GUI.enabled = false;
            {
                var script = MonoScript.FromMonoBehaviour((MonoBehaviour)target);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
            GUI.enabled = true;


            serializedObject.Update();
            {
                //base.OnInspectorGUI();


                DrawPropertiesExcluding(serializedObject, "m_Script", nameof(AutoObjectSetting.clips), nameof(AutoObjectSetting.objectSettingDatas));


                /*
                {
                    var dataType = (AutoObjectSetting.DataType)serializedObject.FindProperty(nameof(AutoObjectSetting.dataType)).enumValueIndex;
                    switch (dataType)
                    {
                        case AutoObjectSetting.DataType.Animation:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AutoObjectSetting.clips)));
                            break;
                        case AutoObjectSetting.DataType.ObjectSettingDatas:
                            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AutoObjectSetting.objectSettingDatas)));
                            break;
                        default:
                            break;
                    }
                }
                */
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AutoObjectSetting.clips)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(AutoObjectSetting.objectSettingDatas)));


            }

            if (GUILayout.Button("Apply"))
            {
                var autoObjectSetting = target as AutoObjectSetting;
                autoObjectSetting.Apply();
            }


            {
                GUI.enabled = false;
                {
                    EditorGUILayout.LabelField("you want download this tool ?");
                    EditorGUILayout.LabelField("https://ahzkwid.booth.pm/items/5463945");
                }
                GUI.enabled = true;
            }
            {
                var autoObjectSetting = target as AutoObjectSetting;
                autoObjectSetting.UpdateLegacy();
                //for (int i = 0; i < autoObjectSetting.objectSettingDatas.Count; i++)
                //{
                //    var objectActiveData = autoObjectSetting.objectSettingDatas[i];
                //    /*
                //    if (objectActiveData.gameObject != null)
                //    {
                //        var objectActiveDataRoot = AutoObjectSetting.GetRoot(objectActiveData.gameObject.transform);
                //        if (objectActiveDataRoot != null)
                //        {
                //            autoObjectSetting.objectSettingDatas[i].path = AutoObjectSetting.GetPath(objectActiveData.gameObject, objectActiveDataRoot.gameObject);
                //            autoObjectSetting.objectSettingDatas[i].gameObject = null;
                //        }
                //    }
                //    */

                //    if (objectActiveData.gameObject != null)
                //    {
                //        objectActiveData.targetObject.gameObject = objectActiveData.gameObject;
                //        objectActiveData.gameObject = null;
                //    }
                //    if (string.IsNullOrWhiteSpace(objectActiveData.path) == false)
                //    {
                //        objectActiveData.targetObject.path = objectActiveData.path;
                //        objectActiveData.path = null;
                //    }
                //    {
                //        var targetObject = objectActiveData.targetObject;
                //        if (targetObject != null)
                //        {
                //            targetObject.ConvertPath();
                //        }
                //    }
                //    {
                //        var targetObject = objectActiveData.fromObject;
                //        if (targetObject != null)
                //        {
                //            targetObject.ConvertPath();
                //        }
                //    }
                //}

            }





            serializedObject.ApplyModifiedProperties();
        }
    }





    [CustomPropertyDrawer(typeof(AutoObjectSettingTargetAttribute))]
    public class AutoObjectSettingTargetDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            float propertyCount = 3f;
            {
                var targetType = (AutoObjectSetting.TargetObject.TargetType)property.FindPropertyRelative(nameof(AutoObjectSetting.TargetObject.targetType)).intValue;
                switch (targetType)
                {
                    case AutoObjectSetting.TargetObject.TargetType.Path:
                        propertyCount += 1;
                        break;
                    case AutoObjectSetting.TargetObject.TargetType.GameObject:
                        break;
                    default:
                        break;
                }
            }
            return EditorGUIUtility.singleLineHeight * propertyCount;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var fieldRect = rect;

            fieldRect.height = EditorGUIUtility.singleLineHeight;


            {


                {
                    var path = nameof(AutoObjectSetting.TargetObject.targetType);
                    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                }

                {
                    var path = nameof(AutoObjectSetting.TargetObject.gameObject);
                    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                }




                var targetType = (AutoObjectSetting.TargetObject.TargetType)property.FindPropertyRelative(nameof(AutoObjectSetting.TargetObject.targetType)).intValue;
                switch (targetType)
                {
                    case AutoObjectSetting.TargetObject.TargetType.Path:
                        {
                            var path = nameof(AutoObjectSetting.TargetObject.path);
                            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                            fieldRect.y += EditorGUIUtility.singleLineHeight;
                        }
                        break;
                    case AutoObjectSetting.TargetObject.TargetType.GameObject:
                        break;
                    default:
                        break;
                }

            }
        }
    }
    public class AutoObjectSettingTargetAttribute : PropertyAttribute
    {

    }









    [CustomPropertyDrawer(typeof(AutoObjectSettingObjectSettingDatasAttribute))]
    public class AutoObjectSettingObjectSettingDatasDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            float propertyCount = 3f;
            float height = 0f;
            {
                var action = (AutoObjectSetting.ObjectSettingDatas.Action)property.FindPropertyRelative(nameof(AutoObjectSetting.ObjectSettingDatas.action)).intValue;
                switch (action)
                {
                    case AutoObjectSetting.ObjectSettingDatas.Action.SetTag:
                        propertyCount += 1;
                        break;
                    case AutoObjectSetting.ObjectSettingDatas.Action.MaterialsCopy:
                        break;
                    default:
                        break;
                }


                height = EditorGUIUtility.singleLineHeight * propertyCount;


                {
                    var path = nameof(AutoObjectSetting.ObjectSettingDatas.targetObject);
                    var pathProperty = property.FindPropertyRelative(path);
                    height += EditorGUI.GetPropertyHeight(pathProperty, new GUIContent(path), true);
                    height += EditorGUIUtility.singleLineHeight;
                }
                switch (action)
                {
                    case AutoObjectSetting.ObjectSettingDatas.Action.SetTag:
                        break;
                    case AutoObjectSetting.ObjectSettingDatas.Action.MaterialsCopy:
                        {
                            var path = nameof(AutoObjectSetting.ObjectSettingDatas.fromObject);
                            var pathProperty = property.FindPropertyRelative(path);
                            height += EditorGUI.GetPropertyHeight(pathProperty, new GUIContent(path), true);
                            height += EditorGUIUtility.singleLineHeight;
                        }
                        break;
                    default:
                        break;
                }
            }
            return height;
        }
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var fieldRect = rect;

            fieldRect.height = EditorGUIUtility.singleLineHeight;




            void DrawTarget(string path,string header)
            {
                var border = 2f;
                var pathProperty = property.FindPropertyRelative(path);
                var height = EditorGUI.GetPropertyHeight(pathProperty, new GUIContent(path), true);



                var helpBoxRect = new Rect(fieldRect.x - border, fieldRect.y, fieldRect.width + border * 2, height + EditorGUIUtility.singleLineHeight);
                EditorGUI.HelpBox(helpBoxRect, "", MessageType.None);

                fieldRect.y += border;
                //EditorGUI.indentLevel += 1;
                {
                    //var path = nameof(AutoObjectSetting.ObjectSettingDatas.targetObject);


                    {

                        //Header

                        var labelRect = new Rect(fieldRect.x, fieldRect.y, fieldRect.width, EditorGUIUtility.singleLineHeight);
                        //EditorGUI.HelpBox(labelRect, "", MessageType.None);
                        EditorGUI.LabelField(labelRect, header, EditorStyles.boldLabel);
                        fieldRect.y += EditorGUIUtility.singleLineHeight;
                    }
                    //fieldRect.y += EditorGUIUtility.singleLineHeight * 0.5f;





                    EditorGUI.PropertyField(fieldRect, pathProperty, new GUIContent(path), true);

                    fieldRect.y += height;
                }
                //EditorGUI.indentLevel -= 1;

                fieldRect.y += EditorGUIUtility.singleLineHeight * 0.5f;
            }
            {


                {
                    var path = nameof(AutoObjectSetting.ObjectSettingDatas.action);
                    EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                    fieldRect.y += EditorGUIUtility.singleLineHeight;
                }


                {
                    var action = (AutoObjectSetting.ObjectSettingDatas.Action)property.FindPropertyRelative(nameof(AutoObjectSetting.ObjectSettingDatas.action)).intValue;
                    switch (action)
                    {
                        case AutoObjectSetting.ObjectSettingDatas.Action.SetTag:

                            {
                                var path = nameof(AutoObjectSetting.ObjectSettingDatas.targetObject);
                                DrawTarget(path, "Target");
                            }
                            {
                                //var border = 2f;
                                //{

                                //    //Header
                                //    var labelRect = new Rect(fieldRect.x + border, fieldRect.y, fieldRect.width, EditorGUIUtility.singleLineHeight);
                                //    EditorGUI.LabelField(labelRect, "-Target", EditorStyles.boldLabel);
                                //    fieldRect.y += EditorGUIUtility.singleLineHeight;
                                //}
                                //EditorGUI.indentLevel += 1;
                                //{
                                //    var path = nameof(AutoObjectSetting.ObjectSettingDatas.targetObject);
                                //    var pathProperty = property.FindPropertyRelative(path);



                                //    var height = EditorGUI.GetPropertyHeight(pathProperty, new GUIContent(path), true);

                                //    var helpBoxRect = new Rect(fieldRect.x - border, fieldRect.y - border, fieldRect.width + border * 2, height + border * 2);
                                //    EditorGUI.HelpBox(helpBoxRect, "", MessageType.None);




                                //    EditorGUI.PropertyField(fieldRect, pathProperty, new GUIContent(path), true);

                                //    fieldRect.y += height;
                                //}
                                //EditorGUI.indentLevel -= 1;

                            }

                            {
                                var path = nameof(AutoObjectSetting.ObjectSettingDatas.tag);
                                EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                                fieldRect.y += EditorGUIUtility.singleLineHeight;
                            }
                            break;
                        case AutoObjectSetting.ObjectSettingDatas.Action.MaterialsCopy:
                            {
                                var path = nameof(AutoObjectSetting.ObjectSettingDatas.fromObject);
                                DrawTarget(path, "From");

                                //var path = nameof(AutoObjectSetting.ObjectSettingDatas.fromObject);
                                //EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(path), new GUIContent(path), true);
                                //fieldRect.y += EditorGUIUtility.singleLineHeight;
                            }
                            {
                                var path = nameof(AutoObjectSetting.ObjectSettingDatas.targetObject);
                                DrawTarget(path, "To");
                            }
                            break;
                        default:
                            break;
                    }
                }

            }
        }
    }
    public class AutoObjectSettingObjectSettingDatasAttribute : PropertyAttribute
    {

    }




















    [ExecuteInEditMode]
    public class AutoObjectSetting : MonoBehaviour
    {
        public enum MergeTrigger
        {
            OneShot, Runtime, Always
        }
        public MergeTrigger mergeTrigger = MergeTrigger.Runtime;



        public enum DataType
        {
            Animation, ObjectSettingDatas
        }
        //public DataType dataType= DataType.Animation;

        public AnimationClip[] clips;

        [System.Serializable]
        public class ObjectSettingDatas
        {

            public enum Action
            {
                SetTag,
                MaterialsCopy,
            }
            public Action action;
            [AutoObjectSettingTargetAttribute]
            public TargetObject targetObject = new();
            [AutoObjectSettingTargetAttribute]
            public TargetObject fromObject = new();

            [HideInInspector]
            [System.Obsolete]
            public GameObject gameObject;
            [HideInInspector]
            [System.Obsolete]
            public string path;

            //public Enabled objEnabled;
            //public Enabled rendererEnabled;
            //public Enabled physboneEnabled;
            public Tag tag = Tag.None;
            //public bool changeScale=false;
            //public Vector3 localScale=Vector3.one;


            public void Apply(Transform root, Transform[] rootChildrens)
            {
                //var target = System.Array.Find(childrens, x => objectActiveData.path == GetPath(x.gameObject, root.gameObject));
                var target = targetObject.GetGameObject(root, rootChildrens);
                if (target == null)
                {
                    return;
                }
                switch (action)
                {
                    case Action.SetTag:
                        switch (tag)
                        {
                            case Tag.None:
                                break;
                            default:
                                var tagString = tag.ToString();
                                if (System.Array.Find(UnityEditorInternal.InternalEditorUtility.tags, x => x == tagString) != null)
                                {
                                    target.tag = tagString;
                                }
                                break;
                        }
                        break;
                    case Action.MaterialsCopy:
                        var from = fromObject.GetGameObject(root, rootChildrens);
                        if (from == null)
                        {
                            return;
                        }
                        var fromRenderer = from.GetComponent<Renderer>();
                        if (fromRenderer == null)
                        {
                            return;
                        }
                        var toRenderer = target.GetComponent<Renderer>();
                        if (toRenderer == null)
                        {
                            return;
                        }
                        toRenderer.sharedMaterials = fromRenderer.sharedMaterials;
                        break;
                    default:
                        break;
                }
            }
        }
        [System.Serializable]
        public class TargetObject
        {

            public enum TargetType
            {
                Path,
                GameObject,
            }
            public TargetType targetType;
            public GameObject gameObject;
            public string path;

            public void ConvertPath()
            {
                if (targetType != TargetType.Path)
                {
                    return;
                }
                if (gameObject == null)
                {
                    return;
                }
                var objectActiveDataRoot = GetRoot(gameObject.transform);
                if (objectActiveDataRoot == null)
                {
                    return;
                }
                path = ObjectPath.GetPath(gameObject.transform, objectActiveDataRoot.gameObject.transform);
                gameObject = null;
            }
            public GameObject GetGameObject(Transform root, Transform[] rootChildrens)
            {
                switch (targetType)
                {
                    case TargetType.Path:
                        if (string.IsNullOrEmpty(path))
                        {
                            return null;
                        }

                        var target = System.Array.Find(rootChildrens, x => {
                            var targetPath = ObjectPath.GetPath(x.transform, root);
                            if (path == targetPath)
                            {
                                return true;
                            }
                            if (path.StartsWith("/"))
                            {
                                if (path == "/" + targetPath)
                                {
                                    return true;
                                }
                            }
                            return false;
                        });

                        return target.gameObject;
                    case TargetType.GameObject:
                        return gameObject;
                    default:
                        break;
                }
                return null;
            }
        }

        public enum Enabled
        {
            None,
            Enable,
            Disable
        }
        public enum Tag
        {
            None,
            Untagged,
            Respawn,
            Finish,
            EditorOnly,
            MainCamera,
            Player,
            GameController
        }
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

        public static string GetPath(GameObject target, GameObject root = null)
        {
            var rootPath = "";



            if (root != null)
            {
                rootPath = SearchUtils.GetHierarchyPath(root, false);
            }
            var hierarchyPath = SearchUtils.GetHierarchyPath(target, false);



            var startIndex = -1;

            if (rootPath != null)
            {
                startIndex = rootPath.Length;
            }
            if (startIndex >= 0)
            {
                return hierarchyPath.Substring(startIndex, hierarchyPath.Length - startIndex);
            }
            else
            {
                return hierarchyPath;
            }
        }
        bool success = false;

        //public bool autoDestroy = true;
        // List<BlendshapeTarget> blendshapeTargets = new List<BlendshapeTarget>();
        [AutoObjectSettingObjectSettingDatasAttribute]
        public List<ObjectSettingDatas> objectSettingDatas = new List<ObjectSettingDatas>();


        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            //EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            //EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGUI;
        }
        void OnSceneGUI(SceneView obj)
        {
            UpdateCustom(); 
            //Update();
            //HandleDragAndDropEvents();
        }
        /*
        void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            HandleDragAndDropEvents();
        }
        void HandleDragAndDropEvents()
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
        }
        void OnDragPerform()
        {
            //Debug.Log("OnDragPerform()");
        }
        */

        void OnDrawGizmos()
        {
            if (mergeTrigger != MergeTrigger.Runtime)
            {
                if (success == false)
                {
                    UnityEditor.Handles.Label(transform.position, "Finding Character");
                }
                else
                {
                    UnityEditor.Handles.Label(transform.position, "Success AutoSetting");
                }
            }
            Update();
        }
        public bool Apply()
        {




            var autoObjectSetting = this;

            //Animation
            {
                var root = ObjectPath.GetVRCRoot(autoObjectSetting.transform, ObjectPath.VRCRootSearchOption.VRCRootOnly);
                /*
                var root = GetRoot(autoObjectSetting.transform);
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
                */

                if (root == null)
                {
                    return false;
                }

                //AnimationMode.SampleAnimationClip(root.gameObject, clip, currentTime);

                //Undo.RegisterCompleteObjectUndo(root.gameObject, "Apply Animation");


                {
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
                    foreach (var clip in clips)
                    {
                        clip.SampleAnimation(root.gameObject, clip.length);
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








                //ObjectSettingDatas
                {
                    var childrens = root.GetComponentsInChildren<Transform>(true);
                    foreach (var objectActiveData in autoObjectSetting.objectSettingDatas)
                    {
                        objectActiveData.Apply(root,childrens);



                        //if (string.IsNullOrEmpty(objectActiveData.path))
                        //{
                        //    continue;
                        //}


                        ////var target = System.Array.Find(childrens, x => objectActiveData.path == GetPath(x.gameObject, root.gameObject));
                        //var target = System.Array.Find(childrens, x => {
                        //    var path = ObjectPath.GetPath(x.transform, root.transform);
                        //    if (objectActiveData.path == path)
                        //    {
                        //        return true;
                        //    }
                        //    if (objectActiveData.path == "/"+path)
                        //    {
                        //        return true;
                        //    }
                        //    return false;
                        //    });
                        //if (target == null)
                        //{
                        //    continue;
                        //}
                        ///*
                        //switch (objectActiveData.objEnabled)
                        //{
                        //    case Enabled.Enable:
                        //        target.gameObject.SetActive(true);
                        //        break;
                        //    case Enabled.Disable:
                        //        target.gameObject.SetActive(false);
                        //        break;
                        //}
                        //switch (objectActiveData.rendererEnabled)
                        //{
                        //    case Enabled.Enable:
                        //    case Enabled.Disable:
                        //        var renderer = target.GetComponent<Renderer>();
                        //        if (renderer == null)
                        //        {
                        //            break;
                        //        }
                        //        switch (objectActiveData.rendererEnabled)
                        //        {
                        //            case Enabled.Enable:
                        //                renderer.enabled = true;
                        //                break;
                        //            case Enabled.Disable:
                        //                renderer.enabled = false;
                        //                break;
                        //        }
                        //        break;
                        //}
                        //switch (objectActiveData.physboneEnabled)
                        //{
                        //    case Enabled.Enable:
                        //    case Enabled.Disable:
                        //        var components = target.GetComponents<Component>();
                        //        foreach (var component in components)
                        //        {
                        //            if (component == null)
                        //            {
                        //                continue;
                        //            }
                        //            if (component.GetType().Name.ToLower().Contains("physbone") == false)
                        //            {
                        //                continue;
                        //            }
                        //            switch (objectActiveData.physboneEnabled)
                        //            {
                        //                case Enabled.Enable:
                        //                    ((Behaviour)component).enabled = true;
                        //                    break;
                        //                case Enabled.Disable:
                        //                    ((Behaviour)component).enabled = false;
                        //                    break;
                        //            }
                        //        }
                        //        break;
                        //}
                        //if (objectActiveData.changeScale)
                        //{
                        //    target.transform.localScale = objectActiveData.localScale;
                        //}
                        //*/
                        //switch (objectActiveData.action)
                        //{
                        //    case ObjectSettingDatas.Action.SetTag:
                        //        switch (objectActiveData.tag)
                        //        {
                        //            case Tag.None:
                        //                break;
                        //            default:
                        //                var tagString = objectActiveData.tag.ToString();
                        //                if (System.Array.Find(UnityEditorInternal.InternalEditorUtility.tags, x => x == tagString) != null)
                        //                {

                        //                    target.tag = tagString;
                        //                }
                        //                break;
                        //        }
                        //        break;
                        //    case ObjectSettingDatas.Action.MaterialsCopy:
                        //        break;
                        //    default:
                        //        break;
                        //}
                    }
                }




            }
            return true;

        }

        public void Run()
        {
            //foreach (var autoObjectSetting in FindObjectsOfType<AutoObjectSetting>())
            {

                if (PrefabStageUtility.GetCurrentPrefabStage() != null) //프리팹 수정모드라면 중단
                {
                    return;
                }











                var autoObjectSetting = this;
                if (autoObjectSetting.success)
                {
                    return;
                }



                if (Apply()==false)
                {
                    return;
                }
                /*

                switch (dataType)
                {
                    case DataType.Animation:

                        break;
                    case DataType.ObjectSettingDatas:
                        break;
                    default:
                        break;
                }
                */

                autoObjectSetting.success = true;

                //if (success)
                {
                    //if (autoObjectSetting.autoDestroy)
                    {
                        switch (mergeTrigger)
                        {
                            case MergeTrigger.Always:
                                break;
                            case MergeTrigger.OneShot:
                            case MergeTrigger.Runtime:
                                DestroyImmediate(autoObjectSetting);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        public void UpdateLegacy()
        {
            foreach (var objectActiveData in objectSettingDatas)
            {
                if (objectActiveData.gameObject != null)
                {
                    objectActiveData.targetObject.gameObject = objectActiveData.gameObject;
                    objectActiveData.gameObject = null;
                }
                if (string.IsNullOrWhiteSpace(objectActiveData.path) == false)
                {
                    objectActiveData.targetObject.path = objectActiveData.path;
                    objectActiveData.path = null;
                }
                {
                    var targetObject = objectActiveData.targetObject;
                    if (targetObject != null)
                    {
                        targetObject.ConvertPath();
                    }
                }
                {
                    var targetObject = objectActiveData.fromObject;
                    if (targetObject != null)
                    {
                        targetObject.ConvertPath();
                    }
                }
            }
        }

        void UpdateCustom()
        {
            UpdateLegacy();

            switch (mergeTrigger)
            {
                case MergeTrigger.Always:
                case MergeTrigger.OneShot:
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
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            //UpdateCustom();
        }
    }
#endif
}