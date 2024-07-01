

using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{


#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.Search;
    using VRC.SDK3.Avatars.Components;

    [CustomEditor(typeof(ObjectAutoSetter))]
    public class ObjectAutoSetterEditor : Editor
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


    public class ObjectAutoSetter : MonoBehaviour
    {
        [System.Serializable]
        public class ObjectActiveData
        {
            public string path;
            public GameObject gameObject;
            public Enabled objEnabled;
            public Enabled rendererEnabled;
            public Enabled physboneEnabled;
            public Tag tag;
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

        string GetPath(GameObject target, GameObject root = null)
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

        public bool autoDestroy = true;
        // List<BlendshapeTarget> blendshapeTargets = new List<BlendshapeTarget>();
        public List<ObjectActiveData> objectActiveDatas = new List<ObjectActiveData>();
        void OnDrawGizmos()
        {
            {
                for (int i = 0; i < objectActiveDatas.Count; i++)
                {
                    var objectActiveData = objectActiveDatas[i];
                    if (objectActiveData.gameObject != null)
                    {
                        var objectActiveDataRoot = GetRoot(objectActiveData.gameObject.transform);
                        if (objectActiveDataRoot != null)
                        {
                            objectActiveDatas[i].path = GetPath(objectActiveData.gameObject, objectActiveDataRoot.gameObject);
                            objectActiveDatas[i].gameObject = null;
                        }
                    }
                }

            }
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



                    var childrens = root.GetComponentsInChildren<Transform>(true);
                    foreach (var objectActiveData in objectActiveDatas)
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
                        switch (objectActiveData.tag)
                        {
                            case Tag.None:
                                break;
                            default:
                                var tagString = objectActiveData.tag.ToString();
                                if (System.Array.Find(UnityEditorInternal.InternalEditorUtility.tags,x=> x == tagString) != null)
                                {

                                    target.tag = tagString;
                                }
                                break;
                        }
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