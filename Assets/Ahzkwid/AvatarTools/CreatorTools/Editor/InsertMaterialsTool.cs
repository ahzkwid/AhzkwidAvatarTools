



#if UNITY_EDITOR

using UnityEngine;

//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class InsertMaterialsTool : EditorWindow
{
    public Material[] materials;
    public GameObject root;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(InsertMaterialTool))]
    public static void Init()
    {
        var window = GetWindow<InsertMaterialsTool>(utility: false, title: nameof(InsertMaterialsTool));
        window.minSize = new Vector2(300, 200);
        //window.maxSize = window.minSize;
        window.Show();
    }
    public void InsertMaterials(GameObject root, Material[] materials)
    {
        var renders=root.GetComponentsInChildren<Renderer>();
        foreach (var render in renders)
        {
            var sharedMaterials = render.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                var material = System.Array.FindLast(materials, x => x.name == sharedMaterials[i].name);
                if (material != null)
                {
                    sharedMaterials[i] = material;
                }
            }
            render.sharedMaterials = sharedMaterials;
        }
    }
    SerializedObject serializedObject;
    void OnGUI()
    {
        if (serializedObject == null)
        {
            serializedObject = new SerializedObject(this);
        }



        var allReady = true;




        serializedObject.Update();
        {
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(materials)));
            EditorGUILayout.Space();
            if (materials == null)
            {
                allReady = false;
            }
            GUI.enabled = allReady;
            if (GUILayout.Button("ResetMaterials"))
            {
                materials = null;
            }
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(root)));
            EditorGUILayout.Space();
            if (root == null)
            {
                allReady = false;
            }
            //if (textureFolder == null)
            {
                //allReady = false;
            }
            if (allReady)
            {
                //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
            }
        }
        serializedObject.ApplyModifiedProperties();


        GUI.enabled = allReady;
        if (GUILayout.Button("Insert"))
        {
            InsertMaterials(root, materials);
        }
        GUI.enabled = true;
    }

}
#endif