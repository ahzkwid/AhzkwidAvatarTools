using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditorInternal;







    [CustomEditor(typeof(MaterialAutoSetter))]
    public class MaterialAutoSetterEditor : Editor
    {
        Hashtable reorderableListTable = new Hashtable();



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
                DrawPropertiesExcluding(serializedObject, "m_Script");

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


    


    public class MaterialAutoSetter : MonoBehaviour
{
    [System.Serializable]
    public class MaterialTarget
    {
        public Mesh mesh;
        //[BlendshapeSettingData]
        public Material[] materials;
        //public Dictionary<string,float> keyValuePairs = new Dictionary<string,float>();
    }

        bool success = false;

        public bool autoDestroy = true;

        public List<MaterialTarget> materialTargets = new List<MaterialTarget>();
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


                    foreach (var blendshapeTarget in blendshapeAutoSetter.materialTargets)
                    {
                        var renderers = System.Array.FindAll(skinnedMeshRenderers, renderer => renderer.sharedMesh == blendshapeTarget.mesh);
                        foreach (var renderer in renderers)
                        {
                            renderer.materials = blendshapeTarget.materials;
                            success = true;
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