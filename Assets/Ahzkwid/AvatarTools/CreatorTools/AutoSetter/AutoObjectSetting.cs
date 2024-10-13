

using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{


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
                for (int i = 0; i < autoObjectSetting.objectSettingDatas.Count; i++)
                {
                    var objectActiveData = autoObjectSetting.objectSettingDatas[i];
                    if (objectActiveData.gameObject != null)
                    {
                        var objectActiveDataRoot = AutoObjectSetting.GetRoot(objectActiveData.gameObject.transform);
                        if (objectActiveDataRoot != null)
                        {
                            autoObjectSetting.objectSettingDatas[i].path = AutoObjectSetting.GetPath(objectActiveData.gameObject, objectActiveDataRoot.gameObject);
                            autoObjectSetting.objectSettingDatas[i].gameObject = null;
                        }
                    }
                }

            }






            serializedObject.ApplyModifiedProperties();
        }
    }



    [ExecuteInEditMode]
    public class AutoObjectSetting : MonoBehaviour
    {
        public enum MergeTrigger
        {
            Always, Runtime
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
            public GameObject gameObject;
            public string path;

            //public Enabled objEnabled;
            //public Enabled rendererEnabled;
            //public Enabled physboneEnabled;
            public Tag tag;
            //public bool changeScale=false;
            //public Vector3 localScale=Vector3.one;
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
        public List<ObjectSettingDatas> objectSettingDatas = new List<ObjectSettingDatas>();
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
                        if (string.IsNullOrEmpty(objectActiveData.path))
                        {
                            continue;
                        }
                        var target = System.Array.Find(childrens, x => objectActiveData.path == GetPath(x.gameObject, root.gameObject));
                        if (target == null)
                        {
                            continue;
                        }
                        /*
                        switch (objectActiveData.objEnabled)
                        {
                            case Enabled.Enable:
                                target.gameObject.SetActive(true);
                                break;
                            case Enabled.Disable:
                                target.gameObject.SetActive(false);
                                break;
                        }
                        switch (objectActiveData.rendererEnabled)
                        {
                            case Enabled.Enable:
                            case Enabled.Disable:
                                var renderer = target.GetComponent<Renderer>();
                                if (renderer == null)
                                {
                                    break;
                                }
                                switch (objectActiveData.rendererEnabled)
                                {
                                    case Enabled.Enable:
                                        renderer.enabled = true;
                                        break;
                                    case Enabled.Disable:
                                        renderer.enabled = false;
                                        break;
                                }
                                break;
                        }
                        switch (objectActiveData.physboneEnabled)
                        {
                            case Enabled.Enable:
                            case Enabled.Disable:
                                var components = target.GetComponents<Component>();
                                foreach (var component in components)
                                {
                                    if (component == null)
                                    {
                                        continue;
                                    }
                                    if (component.GetType().Name.ToLower().Contains("physbone") == false)
                                    {
                                        continue;
                                    }
                                    switch (objectActiveData.physboneEnabled)
                                    {
                                        case Enabled.Enable:
                                            ((Behaviour)component).enabled = true;
                                            break;
                                        case Enabled.Disable:
                                            ((Behaviour)component).enabled = false;
                                            break;
                                    }
                                }
                                break;
                        }
                        if (objectActiveData.changeScale)
                        {
                            target.transform.localScale = objectActiveData.localScale;
                        }
                        */
                        switch (objectActiveData.tag)
                        {
                            case Tag.None:
                                break;
                            default:
                                var tagString = objectActiveData.tag.ToString();
                                if (System.Array.Find(UnityEditorInternal.InternalEditorUtility.tags, x => x == tagString) != null)
                                {

                                    target.tag = tagString;
                                }
                                break;
                        }
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
                        DestroyImmediate(autoObjectSetting);
                    }
                }
            }
        }

        // Start is called before the first frame update

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
#endif
}