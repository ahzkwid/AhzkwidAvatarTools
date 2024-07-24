
using System.Collections.Generic;
using UnityEngine;

namespace Ahzkwid
{
    using System.Linq;
#if UNITY_EDITOR
    using UnityEditor;






    [CustomEditor(typeof(MaterialAutoSetter))]
    public class MaterialAutoSetterEditor : Editor
    {
        //Hashtable reorderableListTable = new Hashtable();



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




                {

                    var materialAutoSetter = target as MaterialAutoSetter;
                    var materialTargets = materialAutoSetter.materialTargets;
                    var valueChanged = false;


                    materialTargets = materialTargets.ConvertAll(materialTarget =>
                    {
                        var value = materialTarget.mesh;
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
                        materialTarget.mesh = value;
                        return materialTarget;
                    });
                    if (valueChanged)
                    {
                        materialAutoSetter.materialTargets = materialTargets;
                        UnityEditor.EditorUtility.SetDirty(target);
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





    [ExecuteInEditMode]
    public class MaterialAutoSetter : MonoBehaviour
    {
        [System.Serializable]
        public class MaterialData
        {
            public Material material;
            public int index;
        }
        [System.Serializable]
        public class MaterialTarget
        {
            public Object mesh;
            //[BlendshapeSettingData]
            public MaterialData[] materialDatas;
            //public Dictionary<string,float> keyValuePairs = new Dictionary<string,float>();
        }

        bool success = false;

        public bool autoDestroy = true;

        public List<MaterialTarget> materialTargets = new List<MaterialTarget>();
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var materialAutoSetter = this;
            var transform = materialAutoSetter.transform;
            if (success == false)
            {
                UnityEditor.Handles.Label(transform.position, "Finding Character");
            }
            else
            {
                UnityEditor.Handles.Label(transform.position, "Success Blendshape AutoSetting");
            }
            Update();
        }
#endif
        // Start is called before the first frame update

        // Update is called once per frame
        void Update()
        {
            //foreach (var materialAutoSetter in FindObjectsOfType<MaterialAutoSetter>())
            {
                var materialAutoSetter = this;
                var transform = materialAutoSetter.transform;
                if (materialAutoSetter.success)
                {
                    return;
                }
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


                    foreach (var materialTarget in materialAutoSetter.materialTargets)
                    {
                        var renderers = System.Array.FindAll(skinnedMeshRenderers, renderer => renderer.sharedMesh == materialTarget.mesh);
                        foreach (var renderer in renderers)
                        {
                            var materialDatas = materialTarget.materialDatas;
                            var materialsLength = materialDatas.Length;
                            materialsLength = Mathf.Max(materialsLength, materialDatas.Max(x => x.index) + 1);
                            materialsLength = Mathf.Max(materialsLength, renderer.sharedMaterials.Length);
                            var materials = new Material[materialsLength];
                            for (int i = 0; i < materials.Length; i++)
                            {
                                var materialData = System.Array.Find(materialDatas, x => x.index == i);
                                if (materialData != null)
                                {
                                    materials[i] = materialData.material;
                                    continue;
                                }
                                if (i < renderer.sharedMaterials.Length)
                                {
                                    materials[i] = renderer.sharedMaterials[i];
                                }
                            }
                            renderer.materials = materials;
                            materialAutoSetter.success = true;
                        }
                    }
                }
                if (materialAutoSetter.success)
                {
                    if (materialAutoSetter.autoDestroy)
                    {
                        DestroyImmediate(materialAutoSetter);
                    }
                }
            }
        }
    }
}