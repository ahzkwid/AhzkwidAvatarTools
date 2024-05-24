
using UnityEngine;



#if UNITY_EDITOR


//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class AnchorOverrideTool : EditorWindow
{
    public GameObject[] roots;
    public Transform anchor = null;
    //public Object textureFolder;
    //public bool createBackup = true;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(AnchorOverrideTool))]
    public static void Init()
    {
        var window = GetWindow<AnchorOverrideTool>(utility: false, title: nameof(AnchorOverrideTool));
        window.minSize = new Vector2(300, 200);
        window.maxSize = window.minSize;
        window.Show();
    }
    public void SetAnchor(GameObject root, Transform anchor)
    {
        var renders=root.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var render in renders)
        {
            render.probeAnchor = anchor;
            UnityEditor.EditorUtility.SetDirty(render);
            Debug.Log($"{render.name}.probeAnchor={anchor.name}");
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(roots)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(anchor)));
            EditorGUILayout.Space();
            if (roots == null || roots.Length == 0) 
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
        if (GUILayout.Button("Run"))
        {
            foreach (var root in roots)
            {
                SetAnchor(root, anchor);
            }
        }
        GUI.enabled = true;
    }

}
#endif