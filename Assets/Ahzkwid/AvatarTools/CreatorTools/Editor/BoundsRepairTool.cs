
using UnityEngine;



#if UNITY_EDITOR


//텍스처 폴더 경로를 지정한 폴더 하위경로로 수정함
using UnityEditor;

[InitializeOnLoad]
class BoundsRepairTool : EditorWindow
{
    public GameObject root;
    public Bounds bounds = new Bounds(Vector3.zero, Vector3.one*2);
    //public Object textureFolder;
    //public bool createBackup = true;


    [UnityEditor.MenuItem("Ahzkwid/AvatarTools/CreatorTools/" + nameof(BoundsRepairTool))]
    public static void Init()
    {
        var window = GetWindow<BoundsRepairTool>(utility: false, title: nameof(BoundsRepairTool));
        window.minSize = new Vector2(300, 200);
        window.maxSize = window.minSize;
        window.Show();
    }
    public void BoundsRepair(GameObject root, Bounds bounds)
    {
        var renders=root.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var render in renders)
        {
            render.localBounds = bounds;
            UnityEditor.EditorUtility.SetDirty(render);
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(root)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(bounds)));
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
        if (GUILayout.Button("설정"))
        {
            BoundsRepair(root, bounds);
            /*
            if (createBackup)
            {
                var characterCopy = Instantiate(character);
                characterCopy.name = character.name+" (Backup)";
                characterCopy.SetActive(false);
            }
            ShapekeyCopy(character, shapekey);
            */
        }
        GUI.enabled = true;
    }

}
#endif