







#if UNITY_EDITOR


using UnityEngine;
using System.Collections.Generic;



using UnityEditor;
using UnityEditor.Search;
using UnityEditorInternal;
using System.Linq;
using System.Collections;

[InitializeOnLoad]
class AvatarMergeTool : EditorWindow
{

    Hashtable reorderableListTable = new Hashtable();
    public void DrawArray(string propertyPath)
    {

        var reorderableListProperty = serializedObject.FindProperty(propertyPath);

        if (reorderableListTable[propertyPath] == null)
        {
            reorderableListTable[propertyPath] = new ReorderableList(serializedObject, reorderableListProperty);
        }
        var reorderableList = (ReorderableList)reorderableListTable[propertyPath];

        //serializedObject.Update();
        {
            //헤더명
            //reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, $"{propertyPath} ({reorderableListProperty.arraySize})");
            reorderableList.drawHeaderCallback = (rect) => EditorGUI.PropertyField(rect, serializedObject.FindProperty(propertyPath));

            //요소크기
            reorderableList.elementHeight = EditorGUIUtility.singleLineHeight;

            reorderableList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var elementProperty = reorderableListProperty.GetArrayElementAtIndex(index);

                var fieldRect = rect;
                fieldRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(fieldRect, elementProperty);
            };
            reorderableList.DoLayoutList();

        }
        //serializedObject.ApplyModifiedProperties();
    }








    public enum MergeType
    {
        Default
        , ForceMerge
    }


    public GameObject[] characters;
    //public GameObject cloth;
    public GameObject[] cloths = new GameObject[] { null };
    public List<Transform> boneTransforms = new List<Transform>();
    public bool createBackup = true;
    public bool nameMerge = false;
    public bool editMode = false;
    public MergeType mergeType = MergeType.Default;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/" + nameof(AvatarMergeTool))]
    public static void Init()
    {
        var window = GetWindow<AvatarMergeTool>(false, nameof(AvatarMergeTool));
        window.minSize = new Vector2(400, 200);
        window.Show();
    }
    SerializedObject serializedObject;




    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    public AhzkwidHumanoid humanoid = null;
    private void OnSceneGUI(SceneView sceneView)
    {
        if ((cloths == null) ||(cloths.Length == 0))
        {
            humanoid = null;
            return;
        }
        if (editMode)
        {
            humanoid.DrawGizmo();
        }
        /*

        foreach (var boneTransform in boneTransforms)
        {
            if (boneTransform==null)
            {
                continue;
            }
            var newPosition = Handles.PositionHandle(boneTransform.position, Quaternion.identity);

            if (newPosition != boneTransform.position)
            {
                UnityEditor.Undo.RecordObject(boneTransform, "Move Transform");
                boneTransform.position = newPosition;
            }
            boneTransforms.Add(boneTransform);
        }

        */
        sceneView.Repaint();
    }

    /*
    GameObject InstantiatePrefab(GameObject gameObject)
    {
        GameObject copy = null;
        if (PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab)
        {
            copy = PrefabUtility.InstantiatePrefab(gameObject, gameObject.transform.parent) as GameObject;
            if (copy == null)
            {

                var target = gameObject;
                //var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target);
                var source = PrefabUtility.GetCorrespondingObjectFromSource(target);
                copy = PrefabUtility.InstantiatePrefab(source, target.transform.parent) as GameObject;
                var modifications = PrefabUtility.GetPropertyModifications(target);
                PrefabUtility.SetPropertyModifications(copy, modifications);


            }
        }
        if (copy == null)
        {
            copy = Instantiate(gameObject);
        }
        return copy;
    }
    */
    static GameObject InstantiatePrefabTemporary(GameObject gameObject, Transform parent = null)
    {
        //임시
        if (parent == null)
        {
            parent = gameObject.transform.parent;
        }
        GameObject copy = null;

        if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
        {
            copy = Instantiate(gameObject, parent);
            CreateChildBones(copy.transform, gameObject.transform);
        }
        if (copy == null)
        {
            copy = Instantiate(gameObject, parent);
        }
        return copy;
    }
    GameObject InstantiatePrefab(GameObject gameObject, Transform parent = null)
    {
        if (parent == null)
        {
            parent = gameObject.transform.parent;
        }
        GameObject copy = null;

        //if (PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab)
        if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
        {

            //if (PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.Variant)
            {

                //copy = PrefabUtility.InstantiatePrefab(gameObject, parent) as GameObject;
                if (copy == null)
                {
                    var target = gameObject;
                    //var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target); //하위값을 복사 안 해버림
                    var source = PrefabUtility.GetCorrespondingObjectFromSource(target); //부모를 복사해버림
                    //copy = GameObjectUtility.DuplicateGameObject(gameObject); //2023.1.0부터
                    copy = PrefabUtility.InstantiatePrefab(source, parent) as GameObject;
                    var modifications = PrefabUtility.GetPropertyModifications(target);
                    PrefabUtility.SetPropertyModifications(copy, modifications);

                    CreateChildBones(copy.transform, gameObject.transform);


                    //var addedGameObjects = PrefabUtility.GetAddedGameObjects(gameObject);
                    //EditorUtility.CopySerialized(copy.transform, gameObject.transform);
                }
            }
        }
        if (copy == null)
        {
            copy = Instantiate(gameObject, parent);
        }
        return copy;
    }
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
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(mergeType)));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(characters)));
            DrawArray(nameof(characters));

            EditorGUILayout.Space();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(cloth)));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(cloths)));


            if (mergeType == MergeType.ForceMerge)
            {
                GameObject fieldReturn = null;
                EditorGUI.BeginChangeCheck();
                {
                    fieldReturn = (GameObject)EditorGUILayout.ObjectField("Cloth", cloths.FirstOrDefault(), typeof(GameObject), true);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    cloths = new GameObject[] { fieldReturn };

                    if (humanoid == null)
                    {
                        humanoid = new AhzkwidHumanoid();
                    }
                    if ((cloths != null) && (cloths.Length > 0))
                    {
                        humanoid.Update(cloths.First());
                    }
                    else
                    {
                        humanoid.Clear();
                    }

                }
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(humanoid)));
                GUI.enabled = true;
            }
            else
            {
                DrawArray(nameof(cloths));
            }



            EditorGUILayout.Space();

            if (mergeType != MergeType.ForceMerge)
            {
                createBackup = false;
                //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
            }
            else
            {
                createBackup = true;

                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
                GUI.enabled = true;

                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(editMode)));
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(nameMerge)));
        }
        serializedObject.ApplyModifiedProperties();
        if (characters==null)
        {
            allReady = false;
        }
        else if (System.Array.FindAll(characters, x => x != null).Length == 0)
        {
            allReady = false;
        }
        if (cloths == null)
        {
            allReady = false;
        }
        else if (System.Array.FindAll(cloths, x => x != null).Length == 0)
        {
            allReady = false;
        }

        /*
        if (GUILayout.Button("Test"))
        {
            var cloth = cloths.FirstOrDefault();

            bool IsArrayCustom(System.Type type)
            {
                return (type.IsArray) || ((type.IsGenericType) && type.GetGenericTypeDefinition() == typeof(List<>));
            }
            var components = cloth.GetComponentsInChildren<Component>(true);
            foreach (var component in components)
            {
                if (component == null)
                {
                    continue;
                }
                if (component is Transform)
                {
                    continue;
                }
                //Debug.Log($"{component.GetType()} {component.name}");
                foreach (var field in component.GetType().GetFields())
                {

                    var value = field.GetValue(component);
                    if (value == null)
                    {
                        continue;
                    }
                    if (value.Equals(null))
                    {
                        continue;
                    }
                    if (IsArrayCustom(field.FieldType))
                    {
                        var ilist = (System.Collections.IList)value;
                        for (int i = 0; i < ilist.Count; i++)
                        {
                            var item = ilist[i];
                            var transform = item as Transform;
                            if (transform == null)
                            {
                                var property = item.GetType().GetProperty("transform");
                                if (property == null)
                                {
                                    continue;
                                }
                                transform = property.GetValue(item) as Transform;
                            }
                            if (transform == null)
                            {
                                continue;
                            }
                            Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{transform}");
                            Debug.Log($"GetComponent:{transform.GetComponent(item.GetType())}");
                            var equalTransform = EqualTransform(transform, transform, transform);
                            Debug.Log($"{ilist[i]}:{transform.GetComponent(item.GetType())}");
                            ilist[i] = equalTransform.GetComponent(item.GetType());
                        }
                        field.SetValue(component, ilist);

                    }
                    else
                    {
                        if (field.FieldType == typeof(Transform))
                        {
                            var transform = value as Transform;
                            var equalTransform = EqualTransform(transform, transform, transform);
                            field.SetValue(component, equalTransform);
                            Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{value}\n{transform}->{equalTransform}");
                            continue;
                        }
                    }
                    //Debug.Log($"{component.name}.{field.Name}");
                }
            }
            
        }
    
        */


        GUI.enabled = allReady;
        {
            if (mergeType == MergeType.ForceMerge)
            {
                if (GUILayout.Button("Fit"))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Fit(characters.FirstOrDefault(), cloths.FirstOrDefault());
                    }
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (GUILayout.Button("Merge"))
            {
                boneTransforms.Clear();
                for (int i = 0; i < cloths.Length; i++)
                {
                    var cloth = cloths[i];
                    var character = characters[i % characters.Length];
                    if (nameMerge)
                    {
                        character.transform.name += $" {cloth.transform.name}";
                    }
                }
                for (int i = 0; i < cloths.Length; i++)
                {
                    var cloth = cloths[i];
                    var character = characters[i% characters.Length];

                    if (createBackup&&(mergeType==MergeType.Default))
                    {
                        /*
                        var characterCopy = PrefabUtility.InstantiatePrefab(character, character.transform.parent) as GameObject;
                        if (characterCopy == null)
                        {
                            var target = character;
                            //var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target);
                            var source = PrefabUtility.GetCorrespondingObjectFromSource(target);
                            characterCopy = PrefabUtility.InstantiatePrefab(source, target.transform.parent) as GameObject;
                        }
                        if (characterCopy == null)
                        {
                            characterCopy = Instantiate(character);
                        }
                        var clothCopy = PrefabUtility.InstantiatePrefab(cloth, cloth.transform.parent) as GameObject;
                        if (clothCopy == null)
                        {
                            var target = cloth;
                            //var source = PrefabUtility.GetCorrespondingObjectFromOriginalSource(target);
                            var source = PrefabUtility.GetCorrespondingObjectFromSource(target);
                            characterCopy = PrefabUtility.InstantiatePrefab(source, target.transform.parent) as GameObject;
                        }
                        if (clothCopy == null)
                        {
                            clothCopy = Instantiate(cloth);
                        }
                        */
                        var characterCopy = InstantiatePrefab(character);
                        characterCopy.transform.name += " (Clone)";
                        //character.SetActive(false);
                        characterCopy.SetActive(true);
                        //foreach (var cloth in cloths)
                        {
                            var clothCopy = InstantiatePrefab(cloth);
                            cloth.SetActive(false);
                            clothCopy.SetActive(true);
                            //Merge(characterCopy, clothCopy, mergeType);
                            Merge(clothCopy, clothCopy, mergeType);

                            if (mergeType == MergeType.ForceMerge)
                            {
                                if (boneTransforms.Count <= 0)
                                {
                                    //GetBoneTransforms(clothCopy);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Merge(character, cloth);
                        //foreach (var cloth in cloths)
                        {
                            Merge(character, cloth, mergeType);
                        }
                        if (mergeType == MergeType.ForceMerge)
                        {
                            if (boneTransforms.Count <= 0)
                            {
                                //GetBoneTransforms(cloth);
                            }
                        }
                    }
                }
            }
        }
        GUI.enabled = true;
        /*
        if (GUILayout.Button("Test()"))
        {
            Test();
        }
        */
    }

    static Transform EqualTransform(Transform target, Transform from, Transform to)
    {
        var fromPath = RelativePath(target, from);
        var transforms = to.GetComponentsInChildren<Transform>(true);
        return System.Array.Find(transforms, x => RelativePath(x, to) == fromPath);
    }
    static string RelativePath(Transform target, Transform root = null)
    {
        var rootName = "";
        var hierarchyPath = "";


        if (root != null)
        {
            try
            {
                rootName = SearchUtils.GetHierarchyPath(root.gameObject, false);

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError(root.ToString() + "nothing root");
                //Debug.LogError(bone.name + "는 root가 없음");
                throw;
            }
        }
        try
        {
            //hierarchyPath = bone.GetHierarchyPath();
            hierarchyPath = SearchUtils.GetHierarchyPath(target.gameObject, false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError(target.ToString() + "nothing GetHierarchyPath");
            throw;
        }

        var startIndex = rootName.Length;

        if (rootName.Length > hierarchyPath.Length)
        {
            Debug.LogWarning("rootName.Length > hierarchyPath.Length");
            Debug.LogWarning($"{rootName} > {hierarchyPath}");
            return null;
        }

        if (hierarchyPath.Substring(0, rootName.Length) != rootName)
        {
            Debug.LogWarning("hierarchyPath.Substring(0, rootName.Length) != rootName");
            Debug.LogWarning($"{rootName} != {hierarchyPath}.Substring(0, rootName.Length)");
            return null;
        }

        try
        {
            return hierarchyPath.Substring(startIndex, hierarchyPath.Length - startIndex);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError($"{hierarchyPath} - {rootName}");
            throw;
        }
    }
    static string ArmaturePath(Transform bone, Transform rootBone = null)
    {
        var rootName = "";
        var hierarchyPath = "";


        if (rootBone != null)
        {
            try
            {
                //rootName = bone.root.name;
                //rootName = rootBone.GetHierarchyPath();
                rootName = SearchUtils.GetHierarchyPath(rootBone.gameObject, false);

            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError(bone.ToString() + "nothing root");
                //Debug.LogError(bone.name + "는 root가 없음");
                throw;
            }
        }
        try
        {
            //hierarchyPath = bone.GetHierarchyPath();
            hierarchyPath = SearchUtils.GetHierarchyPath(bone.gameObject, false);
        }
        catch (System.Exception ex)
        {
            Debug.LogError(ex);
            Debug.LogError(bone.ToString() + "nothing GetHierarchyPath");
            throw;
        }

        var startIndex = -1;

        if (rootBone != null)
        {
            startIndex = rootName.Length;
        }
        else
        {
            startIndex = hierarchyPath.IndexOf("armature");
            if (startIndex < 0)
            {
                startIndex = hierarchyPath.IndexOf("Armature");
            }
        }
        if (startIndex >= 0)
        {
            return hierarchyPath.Substring(startIndex, hierarchyPath.Length - startIndex);
        }
        else
        {
            Debug.LogWarning("Cant Find Armature");
            return hierarchyPath;
        }
    }
    void Test()
    {

        //Debug.Log(cloth.transform.root.name);
        //Debug.Log(SearchUtils.GetHierarchyPath(cloth, false));
        ////Debug.Log(cloth.transform.GetHierarchyPath());
        ////Debug.Log(cloth.transform.GetShortHierarchyPath());
        //Debug.Log(ArmaturePath(cloth.transform, cloth.transform));
    }
    static void AddChildBones(ref List<Transform> list, Transform rootBone)
    {
        list.Add(rootBone);
        for (int i = 0; i < rootBone.childCount; i++)
        {
            var child = rootBone.GetChild(i);
            AddChildBones(ref list, child);
        }
    }


    static void CreateChildBones(Transform rootBoneCharacter, Transform rootBoneCloth)
    {


        var characterChilds = new List<Transform>();
        for (int i = 0; i < rootBoneCharacter.childCount; i++)
        {
            characterChilds.Add(rootBoneCharacter.GetChild(i));
        }


        var clothChilds = new List<Transform>();
        for (int i = 0; i < rootBoneCharacter.childCount; i++)
        {
            characterChilds.Add(rootBoneCharacter.GetChild(i));
        }








        var addedGameObjects = PrefabUtility.GetAddedGameObjects(rootBoneCloth.gameObject);



        var childCount = characterChilds.Count;

        for (int i = 0; i < rootBoneCloth.childCount; i++)
        {
            var childCloth = rootBoneCloth.GetChild(i);
            var childCharacter = characterChilds.Find(x => x.name == childCloth.name);
            if (childCharacter == null)
            {
                Debug.Log($"Create : {childCloth.name}");

                /*
                var gameObject = PrefabUtility.InstantiatePrefab(childCloth, rootBoneCharacter) as GameObject;
                if (gameObject == null)
                {
                    gameObject = Instantiate(childCloth, rootBoneCharacter).gameObject;
                }
                */
                //var gameObject = Instantiate(childCloth.gameObject, rootBoneCharacter);

                //gameObject = InstantiatePrefab(childCloth.gameObject, rootBoneCharacter);
                GameObject gameObject = null;
                //if (EditorUtility.IsPersistent(childCloth))
                //gameObject = InstantiatePrefab(childCloth.gameObject, rootBoneCharacter);
                gameObject = InstantiatePrefabTemporary(childCloth.gameObject, rootBoneCharacter);
                
                //gameObject = Instantiate(childCloth.gameObject, rootBoneCharacter);
                //var gameObject=Instantiate(childCloth, rootBoneCharacter);
                //gameObject.transform.parent = rootBoneCharacter;
                gameObject.name = childCloth.name;
                gameObject.transform.localPosition = childCloth.transform.localPosition;
                gameObject.transform.localRotation = childCloth.transform.localRotation;
                gameObject.transform.localScale = childCloth.transform.localScale;

                /*
                gameObject = childCloth.gameObject;



                var localPosition = gameObject.transform.localPosition;
                var localRotation = gameObject.transform.localRotation;
                var localScale = gameObject.transform.localScale;

                gameObject.transform.parent = rootBoneCharacter;


                gameObject.transform.localPosition = localPosition;
                gameObject.transform.localRotation = localRotation;
                gameObject.transform.localScale = localScale;
                */
                characterChilds.Add(gameObject.transform);
            }
            else
            {
                CreateChildBones(childCharacter, childCloth);
            }
        }
    }
    /*
    void AddChildBones(ref List<Transform> list, Transform rootBoneCharacter, Transform rootBoneCloth)
    {
        list.Add(rootBoneCharacter);

        var characterChilds=new List<Transform>();

        for (int i = 0; i < rootBoneCharacter.childCount; i++)
        {
            characterChilds.Add(rootBoneCharacter.GetChild(i));
        }

        var childCount=characterChilds.Count;

        for (int i = 0; i < rootBoneCloth.childCount; i++)
        {
            var childCloth = rootBoneCloth.GetChild(i);
            //if (childCloth.name== "DynamicLowerArm.L")
            //{
            //    Debug.Log($"{childCloth.name}");
            //}
            if (characterChilds.FindIndex(x=>x.name == childCloth.name)<0)
            {
                Debug.Log($"{childCloth.name}생성");

                //var gameObject = PrefabUtility.InstantiatePrefab(childCloth, rootBoneCharacter) as GameObject;
                //if (gameObject == null)
                //{
                //    gameObject = Instantiate(childCloth, rootBoneCharacter).gameObject;
                //}


                ////var gameObject=Instantiate(childCloth, rootBoneCharacter);
                ////gameObject.transform.parent = rootBoneCharacter;
                //gameObject.name = childCloth.name;
                //gameObject.transform.localPosition = childCloth.transform.localPosition;
                //gameObject.transform.localRotation = childCloth.transform.localRotation;
                //gameObject.transform.localScale = childCloth.transform.localScale;

                var gameObject = childCloth.gameObject;
                childCloth.parent = rootBoneCharacter;

                characterChilds.Add(gameObject.transform);
            }
        }

        foreach (var characterChild in characterChilds)
        {
            AddChildBones(ref list, characterChild);
        }

    }
        */
    /*
    void MergeBones(Transform character, Transform cloth)
    {


        cloth.Find
        for (int i = 0; i < cloth.childCount; i++)
        {
            var childCloth = cloth.GetChild(i);
            if (childCloth.name == "DynamicLowerArm.L")
            {
                Debug.Log($"{childCloth.name}");
            }
            if (characterChilds.FindIndex(x => x.name == childCloth.name) < 0)
            {
                Debug.Log($"{childCloth.name}생성");
                var gameObject = Instantiate(childCloth, character);
                //gameObject.transform.parent = rootBoneCharacter;
                gameObject.name = childCloth.name;
                gameObject.transform.localPosition = childCloth.transform.localPosition;
                gameObject.transform.localRotation = childCloth.transform.localRotation;
                gameObject.transform.localScale = childCloth.transform.localScale;

                characterChilds.Add(gameObject.transform);
            }
        }


    }
    */
    public void GetBoneTransforms(GameObject character)
    {
        //foreach (var cloth in cloths)
        {
            if (character == null)
            {
                return;
            }
            var animator = character.GetComponentInChildren<Animator>(true);
            if (animator == null)
            {
                return;
            }
            foreach (var humanBodyBone in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
            {

                if (humanBodyBone < 0)
                {
                    continue;
                }
                if (humanBodyBone >= HumanBodyBones.LastBone)
                {
                    continue;
                }
                if (humanBodyBone >= HumanBodyBones.LeftToes)
                {
                    continue;
                }
                if (humanBodyBone >= HumanBodyBones.RightToes)
                {
                    continue;
                }
                var boneTransform = animator.GetBoneTransform(humanBodyBone);
                if (boneTransform == null)
                {
                    continue;
                }
                boneTransforms.Add(boneTransform);
            }
        }

    }
    public static void Merge(GameObject character, GameObject cloth, MergeType mergeType=MergeType.Default)
    {

        switch (mergeType)
        {
            case MergeType.ForceMerge:
                var characterCopy = Instantiate(character);
                characterCopy.SetActive(true);
                character.SetActive(false);
                //cloth.SetActive(true);
                var clothCopy = Instantiate(cloth);
                clothCopy.SetActive(true);
                cloth.SetActive(false);


                ForceMerge(characterCopy, clothCopy);
                break;
            case MergeType.Default:
            default:
                MergeDefault(character, cloth);
                break;
        }
    }

    public static void Fit(GameObject character, GameObject cloth)
    {
        //var characterAnimator = character.GetComponentInChildren<Animator>(true);
        //var clothAnimator = character.GetComponentInChildren<Animator>(true);
        var characterAnimator = character.GetComponent<Animator>();




        var humanoid = new AhzkwidHumanoid(cloth);



        {
            var characterHeghit = 1f;
            var clothHeghit = 1f;
            var characterPos = Vector3.zero;
            var clothPos = Vector3.zero;
            {
                var animator = characterAnimator;
                var leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                var rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                var leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                var rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

                var upper = (leftUpperArm.transform.position + rightUpperArm.transform.position) / 2;
                var lower = (leftFoot.transform.position + rightFoot.transform.position) / 2;

                characterPos = lower;
                characterHeghit = upper.y - lower.y;
            }

            {


                var upper = (humanoid.leftUpperArm.transform.position + humanoid.rightUpperArm.transform.position) / 2;
                var lower = (humanoid.leftFoot.transform.position + humanoid.rightFoot.transform.position) / 2;

                clothPos = lower;
                clothHeghit = upper.y - lower.y;
            }

            if (clothHeghit <= 0)
            {
                return;
            }


            var ratio = characterHeghit / clothHeghit;
            //Debug.Log($"characterHeghit: {characterHeghit}, clothHeghit: {clothHeghit}, ratio: {ratio}");
            cloth.transform.localScale *= ratio;
            cloth.transform.position += characterPos - clothPos;
            {
                var leftFootCharacter = characterAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
                var rightFootCharacter = characterAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
                humanoid.leftFoot.position = leftFootCharacter.position;
                humanoid.rightFoot.position = rightFootCharacter.position;

            }
        }
    }
    public static void ForceMerge(GameObject character, GameObject cloth)
    {
        //var characterAnimator = character.GetComponentInChildren<Animator>(true);
        //var clothAnimator = character.GetComponentInChildren<Animator>(true);
        var characterAnimator = character.GetComponent<Animator>();
        //var clothAnimator = cloth.GetComponent<Animator>();




        var humanoid = new AhzkwidHumanoid(cloth);

        foreach (var humanBodyBone in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
        {

            if (humanBodyBone < 0)
            {
                continue;
            }
            if (humanBodyBone >= HumanBodyBones.LastBone)
            {
                continue;
            }
            Transform characterTransform;
            try
            {
                characterTransform = characterAnimator.GetBoneTransform(humanBodyBone);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError(humanBodyBone);
                throw;
            }
            //var clothTransform = clothAnimator.GetBoneTransform(humanBodyBone);
            var clothTransform = humanoid.GetBoneTransform(humanBodyBone);
            //characterTransform.localPosition = poseTransform.localPosition;
            if (characterTransform == null)
            {
                continue;
            }
            if (clothTransform == null)
            {
                continue;
            }
            //characterTransform.localRotation = clothTransform.localRotation;
            clothTransform.parent = characterTransform;
        }


        cloth.transform.parent = character.transform;
    }

    [System.Serializable]
    public class AhzkwidHumanoid
    {
        
        public Transform root;

        public Transform head;
        public Transform neck;
        public Transform chest;
        public Transform spine;
        public Transform hips;


        public Transform leftShoulder;
        public Transform rightShoulder;

        public Transform leftUpperLeg;
        public Transform leftLowerLeg;
        public Transform leftFoot;
        public Transform rightUpperLeg;
        public Transform rightLowerLeg;
        public Transform rightFoot;

        public Transform leftUpperArm;
        public Transform leftLowerArm;
        public Transform leftHand;
        public Transform rightUpperArm;
        public Transform rightLowerArm;
        public Transform rightHand;

        public Transform GetSymmetricalTransform(Transform input)
        {
            var fields = GetType().GetFields();
            fields = System.Array.FindAll(fields, field => field.FieldType == typeof(Transform));


            foreach (var field in fields)
            {
                if (field.GetValue(this) as Transform != input)
                {
                    continue;
                }
                if (field.Name.Contains("right"))
                {
                    var name = field.Name.Replace("right","left");
                    return GetType().GetField(name).GetValue(this) as Transform;
                }
                if (field.Name.Contains("left"))
                {
                    var name = field.Name.Replace("left", "right");
                    return GetType().GetField(name).GetValue(this) as Transform;
                }
            }
            return null;
        }

        public void Clear()
        {
            var fields = GetType().GetFields();

            foreach (var field in fields)
            {
                if (field.FieldType == typeof(Transform))
                {
                    field.SetValue(this, null);
                }
            }
        }

        public HumanBodyBones GetHumanBodyBones(Transform transform)
        {
            foreach (HumanBodyBones humanBodyBone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (GetBoneTransform(humanBodyBone) == transform)
                {
                    return humanBodyBone;
                }
            }
            return (HumanBodyBones)(-1);
        }
        public Transform GetBoneTransform(HumanBodyBones HumanBodyBone)
        {
            var fields = GetType().GetFields();
            var boneName = HumanBodyBone.ToString();

            foreach (var field in fields)
            {
                if (field.FieldType != typeof(Transform))
                {
                    continue;
                }
                if (field.Name.ToLower() != boneName.ToLower())
                {
                    continue;
                }
                return (Transform)field.GetValue(this);
            }
            return null;
        }
        public void HumanoidSearch(Animator animator)
        {
            var fields = GetType().GetFields();
            var humanBodyBones = (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones));

            foreach (var field in fields)
            {
                if (field.FieldType != typeof(Transform))
                {
                    continue;
                }
                var lowerFieldName = field.Name.ToLower();
                var humanBodyBone = System.Array.Find(humanBodyBones, x => lowerFieldName==x.ToString().ToLower());
                if (humanBodyBone < 0)
                {
                    continue;
                }
                field.SetValue(this, animator.GetBoneTransform(humanBodyBone));
                /*
                if (Enum.TryParse(field.Name, out HumanBodyBones humanBodyBone))
                {
                    field.SetValue(this, animator.GetBoneTransform(humanBodyBone));
                }
                */
            }
        }



        readonly string[] upperLegKeywords = new string[] { "thigh","upleg" };
        readonly string[] lowerLegKeywords = new string[] { "knee", "calf" };
        readonly string[] upperArmKeywords = new string[] { };
        readonly string[] lowerArmKeywords = new string[] { "elbow", "forearm" };
        public void NameSearch(Transform[] transforms)
        {



            head = FindBone(transforms, "head");
            neck = FindBone(transforms, "neck");
            chest = FindBone(transforms, "chest");
            spine = FindBone(transforms, "spine");
            if (spine==null)
            {
                spine = FindBone(transforms, "ribs");
            }
            
            hips = FindBone(transforms, "hips");
            if (hips == null)
            {
                hips = FindBone(transforms, "pelvis");
            }
            if (hips == null)
            {
                hips = FindBone(transforms, "hip");
            }
            /*

            Transform[] headTransforms;
            (headTransforms, head) = FindLast(transforms, "head");


            Transform[] neckTransforms;
            (neckTransforms, neck) = FindLast(transforms, "neck");

            Transform[] chestTransforms;
            (chestTransforms, chest) = FindLast(transforms, "chest");

            Transform[] spineTransforms;
            (spineTransforms, spine) = FindLast(transforms, "spine");
            if (spine == null)
            {
                (spineTransforms, spine) = FindLast(transforms, "ribs");
            }



            Transform[] hipsTransforms;
            (hipsTransforms, hips) = FindLast(transforms, "hips");

            */



            /*
            Transform[] legTransforms;
            {
                var keywords = new string[] { "leg", "knee" };
                legTransforms = System.Array.FindAll(transforms, transform =>
                {
                    var lowerName = transform.name.ToLower();
                    return System.Array.Find(keywords, keyword => lowerName.Contains(keyword)) != null;
                });
            }

            Transform[] armTransforms;
            {
                var keywords = new string[] { "arm", "elbow" };
                armTransforms = System.Array.FindAll(transforms, transform =>
                {
                    var lowerName = transform.name.ToLower();
                    if (lowerName=="armature")
                    {
                        return false;
                    }
                    return System.Array.Find(keywords, keyword => lowerName.Contains(keyword)) != null;
                });
            }
            */
            var legTransforms = FindWithKeywords(transforms, (new string[] { "leg"}).Concat(upperLegKeywords).Concat(lowerLegKeywords).ToArray());
            var armTransforms = FindWithKeywords(transforms, new string[] { "arm"}.Concat(lowerArmKeywords).ToArray(), new string[] { "armature" });

            //Debug.Log(armTransforms.Length);

            var leftLegTransforms = GetLefts(legTransforms);
            leftUpperLeg = GetUppers(leftLegTransforms).FirstOrDefault();
            leftLowerLeg = GetLowers(leftLegTransforms).FirstOrDefault();

            var rightLegTransforms = GetRights(legTransforms);
            rightUpperLeg = GetUppers(rightLegTransforms).FirstOrDefault();
            rightLowerLeg = GetLowers(rightLegTransforms).FirstOrDefault();



            var leftArmTransforms = GetLefts(armTransforms);
            leftUpperArm = GetUppers(leftArmTransforms).FirstOrDefault();
            leftLowerArm = GetLowers(leftArmTransforms).FirstOrDefault();

            var rightArmTransforms = GetRights(armTransforms);
            rightUpperArm = GetUppers(rightArmTransforms).FirstOrDefault();
            rightLowerArm = GetLowers(rightArmTransforms).FirstOrDefault();




            var shoulders = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("shoulder"));
            if (shoulders.Length == 0)
            {
                shoulders = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("clavicle"));
            }




            var handTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("hand"));
            if (handTransforms.Length == 0)
            {
                handTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("wrist"));
            }

            var footTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("foot"));
            if (footTransforms.Length == 0)
            {
                footTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("ankle"));
            }


            leftShoulder = GetLefts(shoulders).FirstOrDefault();
            rightShoulder = GetRights(shoulders).FirstOrDefault();

            leftFoot = GetLefts(footTransforms).FirstOrDefault();
            rightFoot = GetRights(footTransforms).FirstOrDefault();
            



            leftHand = GetLefts(handTransforms).FirstOrDefault();
            rightHand = GetRights(handTransforms).FirstOrDefault();



            //소거법
            {
                if ((leftUpperArm == null) || (rightUpperArm == null))
                {
                    if ((leftLowerArm != null) && (rightLowerArm != null))
                    {
                        leftUpperArm = System.Array.Find(leftArmTransforms, transform => transform != leftLowerArm);
                        rightUpperArm = System.Array.Find(rightArmTransforms, transform => transform != rightLowerArm);
                    }
                    {
                        var arms = new Transform[] { leftLowerArm, rightLowerArm, leftUpperArm, rightUpperArm };
                        if (arms.Sum(x => (x != null) ? 1 : 0) == 3) //4개중 한개만 비었을때 복원시도
                        {
                            var index = System.Array.FindIndex(arms, x => x == null);
                            arms[index] = System.Array.Find(armTransforms, transform => System.Array.Find(arms, arm => arm == transform) == null);


                            leftLowerArm = arms[0];
                            rightLowerArm = arms[1];
                            leftUpperArm = arms[2];
                            rightUpperArm = arms[3];
                        }
                    }
                }



                if ((leftUpperLeg == null) || (rightUpperLeg == null))
                {
                    if ((leftLowerLeg != null) && (rightLowerLeg != null))
                    {
                        leftUpperLeg = System.Array.Find(leftLegTransforms, transform => transform != leftLowerLeg);
                        rightUpperLeg = System.Array.Find(rightLegTransforms, transform => transform != rightLowerLeg);
                    }
                }

                if ((leftLowerLeg == null) || (rightLowerLeg == null))
                {
                    if ((leftUpperLeg != null) && (rightUpperLeg != null))
                    {
                        leftLowerLeg = System.Array.Find(leftLegTransforms, transform => transform != leftUpperLeg);
                        rightLowerLeg = System.Array.Find(rightLegTransforms, transform => transform != rightUpperLeg);
                    }
                }
            }

            //노드위치로 산출
            {
                if (chest == null)
                {

                    if ((leftUpperArm != null) && (rightUpperArm != null))
                    {

                        var commonParents = GetCommonParents(leftUpperArm, rightUpperArm);
                        var commonParent = commonParents.FirstOrDefault();
                        if (commonParent != null)
                        {
                            if ((spine != commonParent) && (hips != commonParent))
                            {
                                chest = commonParent;
                            }
                        }
                    }
                    else if ((leftLowerArm != null) && (rightLowerArm != null))
                    {

                        var commonParents = GetCommonParents(leftLowerArm, rightLowerArm);
                        var commonParent = commonParents.FirstOrDefault();
                        if (commonParent != null)
                        {
                            if ((spine != commonParent) && (hips != commonParent))
                            {
                                chest = commonParent;
                            }
                        }
                    }
                    else if ((leftHand != null) && (rightHand != null))
                    {

                        var commonParents = GetCommonParents(leftHand, rightHand);
                        var commonParent = commonParents.FirstOrDefault();
                        if (commonParent != null)
                        {
                            if ((spine != commonParent) && (hips != commonParent))
                            {
                                chest = commonParent;
                            }
                        }
                    }
                    else if ((leftShoulder != null) && (rightShoulder != null))
                    {

                        var commonParents = GetCommonParents(leftShoulder, rightShoulder);
                        var commonParent = commonParents.FirstOrDefault();
                        if (commonParent != null)
                        {
                            if ((spine != commonParent) && (hips != commonParent))
                            {
                                chest = commonParent;
                            }
                        }
                    }
                    else if (neck != null)
                    {
                        var commonParents = GetCommonParents(neck);
                        var commonParent = commonParents.FirstOrDefault();

                        if (commonParent != null)
                        {
                            var nodes = GetNodes(commonParent, neck);
                            //Debug.Log($"nodes.Length:{nodes.Length}");
                            if ((nodes != null) && (nodes.Length > 0))
                            {
                                chest = nodes.FirstOrDefault();
                            }
                        }
                    }
                    else if (head != null)
                    {
                        var commonParents = GetCommonParents(head);
                        var commonParent = commonParents.FirstOrDefault();

                        if (commonParent != null)
                        {
                            var nodes = GetNodes(commonParent, head);
                            //Debug.Log($"nodes.Length:{nodes.Length}");
                            if ((nodes != null) && (nodes.Length > 0))
                            {
                                chest = nodes.FirstOrDefault();
                            }
                        }
                    }
                }


                if ((leftUpperLeg == null) || (rightUpperLeg == null))
                {

                    if ((leftFoot != null) && (rightFoot != null))
                    {
                        var commonParents = GetCommonParents(leftFoot, rightFoot);
                        var commonParent = commonParents.FirstOrDefault();
                        //Debug.Log($"commonParent:{commonParent.name}");
                        if (hips == null)
                        {
                            if ((spine != commonParent) && (chest != commonParent))
                            {
                                hips = commonParent;
                            }
                        }
                        if (commonParent != null)
                        {
                            {
                                var nodes = GetNodes(commonParent, leftFoot);
                                //Debug.Log($"nodes.Length:{nodes.Length}");
                                if ((nodes != null) && (nodes.Length > 0))
                                {
                                    leftUpperLeg = nodes[0];
                                    if (nodes.Length >= 2)
                                    {
                                        leftLowerLeg = nodes[nodes.Length / 2];
                                    }
                                }
                            }
                            {
                                var nodes = GetNodes(commonParent, rightFoot);
                                //Debug.Log($"nodes.Length:{nodes.Length}");
                                if ((nodes != null) && (nodes.Length > 0))
                                {
                                    rightUpperLeg = nodes[0];
                                    if (nodes.Length >= 2)
                                    {
                                        rightLowerLeg = nodes[nodes.Length / 2];
                                    }
                                }
                            }
                        }
                    }
                    /*
                    if (leftLegTransforms.Length > 0)
                    {
                        leftUpperLeg = leftLegTransforms[0];
                    }
                    if (rightLegTransforms.Length > 0)
                    {
                        rightUpperLeg = rightLegTransforms[0];
                    }
                    */
                }


                if ((leftUpperArm == null) || (rightUpperArm == null))
                {

                    if ((leftHand != null) && (rightHand != null))
                    {
                        var commonParents = GetCommonParents(leftHand, rightHand);
                        var commonParent = commonParents.FirstOrDefault();
                        /*
                        if (chest == null)
                        {
                            if ((spine != commonParent) && (hips != commonParent))
                            {
                                chest = commonParent;
                            }
                        }
                        */
                        if (commonParent != null)
                        {
                            var index = 0;

                            if ((leftShoulder != null) && (rightShoulder != null))
                            {
                                index = 1;
                            }
                            {
                                var nodes = GetNodes(commonParent, leftHand);
                                if ((nodes != null) && (nodes.Length > 0))
                                {
                                    leftUpperArm = nodes[index];
                                    if (nodes.Length >= 2)
                                    {
                                        leftLowerArm = nodes[nodes.Length / 2];
                                    }
                                }
                            }
                            {
                                var nodes = GetNodes(commonParent, rightHand);
                                if ((nodes != null) && (nodes.Length > 0))
                                {
                                    rightUpperArm = nodes[index];
                                    if (nodes.Length >= 2)
                                    {
                                        rightLowerArm = nodes[nodes.Length / 2];
                                    }
                                }
                            }
                        }
                    }
                    /*
                    if (leftLegTransforms.Length > 0)
                    {
                        leftUpperLeg = leftLegTransforms[0];
                    }
                    if (rightLegTransforms.Length > 0)
                    {
                        rightUpperLeg = rightLegTransforms[0];
                    }
                    */
                }
            }






            /*
            if (chest == null)
            {
                if (spineTransforms.Length >= 2)
                {
                    spine = spineTransforms[0];
                    chest = spineTransforms[spineTransforms.Length - 1];
                }
            }
            */
            Transform[] FindWithKeywords(Transform[] transforms, string[] keywords, string[] excludedKeywords = null)
            {
                if (excludedKeywords != null)
                {
                    transforms = System.Array.FindAll(transforms, transform =>
                    {
                        var lowerName = transform.name.ToLower();
                        return System.Array.Find(excludedKeywords, keyword => lowerName.Contains(keyword)) == null;
                    });
                }
                return System.Array.FindAll(transforms, transform =>
                {
                    var lowerName = transform.name.ToLower();
                    return System.Array.Find(keywords, keyword => lowerName.Contains(keyword)) != null;
                });
            }
            Transform[] GetUppers(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("upper"));
                foreach (var keyword in upperArmKeywords)
                {
                    if (matchingTransforms.Length != 0)
                    {
                        break;
                    }
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains(keyword));
                }
                foreach (var keyword in upperLegKeywords)
                {
                    if (matchingTransforms.Length != 0)
                    {
                        break;
                    }
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains(keyword));
                }
                return matchingTransforms;
            }
            Transform[] GetLowers(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("lower"));
                foreach (var keyword in lowerArmKeywords)
                {
                    if (matchingTransforms.Length != 0)
                    {
                        break;
                    }
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains(keyword));
                }
                foreach (var keyword in lowerLegKeywords)
                {
                    if (matchingTransforms.Length != 0)
                    {
                        break;
                    }
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains(keyword));
                }
                return matchingTransforms;
            }
            

            Transform[] GetLefts(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("left"));
                if (matchingTransforms.Length == 0)
                {
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToUpper().Contains(" L "));
                    //Debug.Log($"matchingTransforms.Length:{matchingTransforms[0].Length}");
                }
                if (matchingTransforms.Length == 0)
                {
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToUpper().Contains(".L"));
                }
                if (matchingTransforms.Length == 0)
                {
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToUpper().Contains("_L"));
                }
                return matchingTransforms;
            }
            Transform[] GetRights(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("right"));
                if (matchingTransforms.Length == 0)
                {
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToUpper().Contains(" R "));
                }
                if (matchingTransforms.Length == 0)
                {
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToUpper().Contains(".R"));
                }
                if (matchingTransforms.Length == 0)
                {
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToUpper().Contains("_R"));
                }
                return matchingTransforms;
            }

            Transform FindBone(Transform[] targetTransforms, string transformName)
            {
                var filteredTransform = System.Array.Find(targetTransforms, transform => transform.name.ToLower()== transformName);
                if (filteredTransform!=null)
                {
                    return filteredTransform;
                }
                var filteredTransforms = System.Array.FindAll(targetTransforms, transform => transform.name.ToLower().Contains(transformName));
                return filteredTransforms.FirstOrDefault();
            }
            (Transform[], Transform) FindLast(Transform[] targetTransforms, string transformName)
            {
                var filteredTransforms = System.Array.FindAll(targetTransforms, transform => transform.name.ToLower().Contains(transformName));
                if (filteredTransforms.Length > 0)
                {
                    return (filteredTransforms, filteredTransforms.Last());
                }
                return (filteredTransforms, null);
            }
            (Transform[], Transform) FindFirst(Transform[] targetTransforms, string transformName)
            {
                var filteredTransforms = System.Array.FindAll(targetTransforms, transform => transform.name.ToLower().Contains(transformName));
                if (filteredTransforms.Length > 0)
                {
                    return (filteredTransforms, filteredTransforms[0]);
                }
                return (filteredTransforms, null);
            }
            Transform[] GetCommonParents(params Transform[] targetTransforms)
            {
                if ((targetTransforms == null) || (targetTransforms.Length == 0))
                {
                    return null;
                }
                var parentsArray = System.Array.ConvertAll(targetTransforms, x => x.GetComponentsInParent<Transform>());
                var commonParents = parentsArray[0];
                for (int i = 1; i < parentsArray.Length; i++)
                {
                    commonParents = System.Array.FindAll(commonParents, x => parentsArray[i].Contains(x));
                }
                return commonParents;
            }
            Transform[] GetNodes(Transform parent, Transform child)
            {
                if ((parent == null) || (child == null))
                {
                    return null;
                }
                var parents = child.GetComponentsInParent<Transform>();
                var parentIndex = System.Array.FindIndex(parents, x => x == parent);
                if (parentIndex < 0)
                {
                    return null;
                }
                if (parentIndex == 0)
                {
                    return new Transform[] { };
                }
                return parents.Take(parentIndex).Reverse().ToArray();
            }
        }

        public void DrawGizmo()
        {
#if UNITY_EDITOR
            var fields = GetType().GetFields();

            foreach (var field in fields)
            {
                if (field.FieldType != typeof(Transform))
                {
                    continue;
                }
                var transform = (Transform)field.GetValue(this);
                if (transform == null)
                {
                    continue;
                }
                UnityEditor.Handles.Label(transform.position, transform.name);


                var newPosition = Handles.PositionHandle(transform.position, Quaternion.identity);

                if (newPosition != transform.position)
                {
                    UnityEditor.Undo.RecordObject(transform, "Move Transform");
                    transform.position = newPosition;

                    var symmetricalTransform = GetSymmetricalTransform(transform);
                    if (symmetricalTransform != null)
                    {
                        var localPosition = root.InverseTransformPoint(newPosition);
                        localPosition.x = -localPosition.x;
                        symmetricalTransform.position = root.TransformPoint(localPosition);
                        UnityEditor.Undo.RecordObject(symmetricalTransform, "Move Transform");
                    }
                }
            }
#endif
        }
        public void Update(GameObject root)
        {
            Clear();
            if (root==null)
            {
                return;
            }
            this.root = root.transform;


            var animator = root.GetComponent<Animator>();
            if ((animator != null) && (animator.isHuman))
            {
                HumanoidSearch(animator);
            }
            else
            {
                NameSearch(root.GetComponentsInChildren<Transform>(true));
            }

            /*
            var transforms = root.GetComponentsInChildren<Transform>();
            Transform[] headTransforms;
            (headTransforms, head) = FindLast(transforms, "head");


            Transform[] neckTransforms;
            (neckTransforms, neck) = FindLast(transforms, "neck");

            Transform[] chestTransforms;
            (chestTransforms, chest) = FindLast(transforms, "chest");

            Transform[] spineTransforms;
            (spineTransforms, spine) = FindLast(transforms, "spine");
            if (spine == null)
            {
                (spineTransforms, spine) = FindLast(transforms, "ribs");
            }

            hip = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("hip")).First();
            spine = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("spine")).First();
            chest = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("chest")).First();
            neck = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("neck")).First();
            head = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("head")).First();

            var legs = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("leg")|| x.name.ToLower().Contains("knee"));
            var shoulders = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("shoulder"));
            var arms = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("arm"));
            var hands = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("hand"));
            var foots = System.Array.FindAll(transforms, x => x.name.ToLower().Contains("foot"));

            leftHand = GetLeft(hands);
            rightHand = GetRight(hands);
            leftFoot = GetLeft(foots);
            rightFoot = GetRight(foots);
            */
        }

        public AhzkwidHumanoid()
        {
        }

        public AhzkwidHumanoid(GameObject root)
        {
            Update(root);
        }

    }

    public static void MergeDefault(GameObject character, GameObject cloth)
    {

        Transform GetRootBone(GameObject gameObject)
        {
            /*
            var armatureNames = new string[] { "Armature", "armature" };
            {
                foreach (var armatureName in armatureNames)
                {
                    return gameObject.transform.Find(armatureName);
                }
            }
            */


            var characterRenderers = character.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            var rootBones = System.Array.ConvertAll(characterRenderers, x => x.rootBone);
            var shortPathIndex = -1;
            var shortPathMin = int.MaxValue;

            for (int i = 0; i < rootBones.Length; i++)
            {
                var parents = rootBones[i].GetComponentsInParent<Transform>();
                if (parents.Length < shortPathMin)
                {
                    shortPathMin = parents.Length;
                    shortPathIndex = i;
                }
            }
            if (shortPathIndex > -1)
            {
                return rootBones[shortPathIndex];
            }
            else
            {
                Debug.LogError("nothing skinedmesh");
            }

            return null;
        }
        Transform GetArmature(GameObject gameObject)
        {
            var armatureNames = new string[] { "Armature", "armature" };
            {
                foreach (var armatureName in armatureNames)
                {
                    var armature = gameObject.transform.Find(armatureName);
                    if (armature != null)
                    {
                        return armature;
                    }
                }
            }
            return null;
        }











        if (cloth == null)
        {
            Debug.LogError("cloth==null");
        }














        {
            //var armatureNames = new string[] { "Armature", "armature" };
            //{
            //    foreach (var armatureName in armatureNames)
            //    {
            //        var armature = cloth.transform.Find(armatureName);
            //        if (armature != null)
            //        {
            //            armature.gameObject.SetActive(false);
            //        }
            //    }
            //}
            var armature = GetArmature(cloth);
            if (armature != null)
            {
                armature.gameObject.SetActive(false);
            }
        }


        cloth.transform.parent = character.transform;

        var characterRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>(true);
        var clothRenderers = cloth.GetComponentsInChildren<SkinnedMeshRenderer>(true);


        //var bones = (Transform[])characterRenderer.bones.Clone();

        {
            Debug.Log($"characterRenderer.bones.Length: {characterRenderer.bones.Length}");
            var list = new List<Transform>();


            /*
            foreach (var clothRenderer in clothRenderers)
            {
                //var rootBone = characterRenderer.rootBone;
                //var rootBone = character.transform;
                var rootBone = GetRootBone(character);
                //AddChildBones(ref list, rootBone, clothRenderer.rootBone);
                AddChildBones(ref list, GetArmature(character), GetArmature(cloth));
                Debug.Log($"list.Count: {list.Count}");

                
            }
            */

            {
                var characterArmature = GetArmature(character);
                var clothArmature = GetArmature(cloth);


                if (characterArmature == null)
                {
                    Debug.LogError("characterArmature == null");
                }
                if (characterArmature == null)
                {
                    Debug.LogError("characterArmature == null");
                }
                CreateChildBones(characterArmature, clothArmature);
            }

            //AddChildBones(ref list, GetArmature(character), GetArmature(cloth));
            AddChildBones(ref list, GetArmature(character));
            Debug.Log($"list.Count: {list.Count}");

            /*
            foreach (var clothRenderer in clothRenderers)
            {
                var rootBone = characterRenderer.rootBone;
                //var rootBone = character.transform;
                //var rootBone = GetRootBone(character);
                AddChildBones(ref list, rootBone, clothRenderer.rootBone);
                Debug.Log($"list.Count: {list.Count}");


            }
            */


            //var bones = characterRenderer.bones;
            var bones = list.ToArray();
            Debug.Log($"bones.Length: {bones.Length}");
            //Debug.Log($"characterRenderer.bones: {string.Join(",", System.Array.ConvertAll(characterRenderer.bones, x => x.name))}"); //BlendshapeAutoSetter와 충돌
            Debug.Log($"bones: {string.Join(",", System.Array.ConvertAll(bones, x => x.name))}");

            //return;
            //targetRenderer.sharedMesh = originalRenderer.sharedMesh;





            /*
            var originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(cloth);
            var originalPrefabRenderers = originalPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var clothRenderer in clothRenderers)
            {
                var originalPrefabRender = System.Array.Find(originalPrefabRenderers, x => x.sharedMesh == clothRenderer.sharedMesh);
                clothRenderer.bones = originalPrefabRender.bones;
            }
            Debug.LogWarning("Repair");
            */


            foreach (var clothRenderer in clothRenderers)
            {
                if (PrefabUtility.IsPartOfPrefabInstance(clothRenderer))
                {
                    //PrefabUtility.RevertObjectOverride(clothRenderer, InteractionMode.UserAction);
                    var originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(clothRenderer);
                    clothRenderer.bones = originalPrefab.bones;
                }


                Debug.Log($"clothRenderer.bones: {string.Join(",", System.Array.ConvertAll(clothRenderer.bones, x => x.name))}");


                /*
                //var bonesFilters= System.Array.FindAll(bones, x => System.Array.FindIndex(clothRenderer.bones, y => y.name == x.name) >= 0);
                var boneList = new List<Transform>();
                for (int i = 0; i < clothRenderer.bones.Length; i++)
                {
                    //var boneName = clothRenderer.bones[i].name;
                    //var boneNameParent = clothRenderer.bones[i].parent.name;
                    //boneList.Add(System.Array.Find(bones, x => (x.name == boneName) && (x.parent.name == boneNameParent)));
                    boneList.Add(GetEqualBone(bones, clothRenderer.bones[i]));
                }
                */
                //clothRenderer.bones = boneList.ToArray();
                Debug.Log($"{clothRenderer}.bones.Length (Pre): {clothRenderer.bones.Length}");
                var equalBones = GetEqualBones(bones, clothRenderer.bones);
                if (equalBones.Length > 0)
                {
                    clothRenderer.bones = equalBones;
                }
                //clothRenderer.bones = System.Array.FindAll(characterRenderer.bones,x=> System.Array.FindIndex(clothRenderer.bones, y => y.name == x.name) >= 0);
                Debug.Log($"{clothRenderer}.bones.Length (After): {clothRenderer.bones.Length}");
                clothRenderer.rootBone = GetEqualBone(bones, clothRenderer.rootBone);
                {
                    Transform probeAnchor=null;
                    //probeAnchor = GetEqualBone(bones, clothRenderer.probeAnchor);
                    if (clothRenderer.probeAnchor != null)
                    {
                        //본이 아니면 일반 오브젝트경로
                        if (probeAnchor == null)
                        {
                            var transforms = character.GetComponentsInChildren<Transform>();
                            var relativePath=RelativePath(clothRenderer.probeAnchor, cloth.transform);
                            if (relativePath == null)
                            {
                                continue;
                            }
                            var equalBone = System.Array.Find(transforms, x => {
                                var characterRelativePath = RelativePath(x, character.transform);
                                if (characterRelativePath==null)
                                {
                                    return false;
                                }
                                return characterRelativePath == relativePath;
                                });
                            probeAnchor = equalBone;
                        }
                    }
                    clothRenderer.probeAnchor = probeAnchor;
                }


                UnityEditor.EditorUtility.SetDirty(clothRenderer);
                //clothRenderer.probeAnchor = characterRenderer.probeAnchor;
            }


            {
                var characterComponents = character.GetComponentsInChildren<Component>(true);
                var clothComponents = cloth.GetComponentsInChildren<Component>(true);
                //var clothRelativePaths = System.Array.ConvertAll(clothComponents, component => RelativePath(component.transform,cloth.transform));
                //var characterRelativePaths = System.Array.ConvertAll(characterComponents, component => RelativePath(component.transform, character.transform));
                //characterComponents = System.Array.FindAll(characterComponents, x => System.Array.FindIndex(clothRelativePaths, relativePath => RelativePath(x.transform, character.transform) == relativePath) >= 0);
                //clothComponents = System.Array.FindAll(clothComponents, x => System.Array.FindIndex(clothRelativePaths, relativePath => RelativePath(x.transform, cloth.transform) == relativePath) >= 0);

                foreach (var component in clothComponents)
                {
                    if (component == null)
                    {
                        continue;
                    }
                    if (component is Transform)
                    {
                        continue;
                    }
                    var relativePath = RelativePath(component.transform, cloth.transform);

                    if (relativePath==null)
                    {
                        continue;
                    }


                    var equalTypeComponents = System.Array.FindAll(characterComponents, x => x.GetType() == component.GetType());
                    if (System.Array.FindIndex(equalTypeComponents, x => {
                        var characterRelativePath = RelativePath(x.transform, character.transform);
                        if (characterRelativePath==null)
                        {
                            return false;
                        }
                        return relativePath == characterRelativePath;
                        }) >= 0)
                    {
                        continue;
                    }

                    var equalTransform = EqualTransform(component.transform, cloth.transform, character.transform);

                    if (equalTransform == null)
                    {
                        continue;
                    }
                    var newComponent=equalTransform.gameObject.AddComponent(component.GetType());


                    foreach (var field in component.GetType().GetFields())
                    {
                        var value = field.GetValue(component);
                        field.SetValue(newComponent, value);
                    }

                }




            }
















            {
                bool IsArrayCustom(System.Type type)
                {
                    return (type.IsArray) || ((type.IsGenericType) && type.GetGenericTypeDefinition() == typeof(List<>));
                }
                var components = cloth.GetComponentsInChildren<Component>(true);
                foreach (var component in components)
                {
                    if (component == null)
                    {
                        continue;
                    }
                    if (component is Transform)
                    {
                        continue;
                    }
                    //Debug.Log($"{component.GetType()} {component.name}");
                    foreach (var field in component.GetType().GetFields())
                    {

                        var value = field.GetValue(component);
                        if (value == null)
                        {
                            continue;
                        }
                        if (value.Equals(null))
                        {
                            continue;
                        }
                        if (IsArrayCustom(field.FieldType))
                        {
                            var ilist = (System.Collections.IList)value;
                            for (int i = 0; i < ilist.Count; i++)
                            {
                                var item = ilist[i];
                                var transform = item as Transform;
                                if (transform == null)
                                {
                                    var property = item.GetType().GetProperty("transform");
                                    if (property == null)
                                    {
                                        continue;
                                    }
                                    transform = property.GetValue(item) as Transform;
                                }
                                if (transform == null)
                                {
                                    continue;
                                }
                                Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{transform}");
                                Debug.Log($"GetComponent:{transform.GetComponent(item.GetType())}");
                                var equalTransform = EqualTransform(transform, cloth.transform, character.transform);
                                Debug.Log($"{ilist[i]}:{transform.GetComponent(item.GetType())}");
                                ilist[i] = equalTransform.GetComponent(item.GetType());
                            }
                            field.SetValue(component, ilist);

                        }
                        else
                        {
                            if (field.FieldType == typeof(Transform))
                            {
                                var transform = value as Transform;
                                var equalTransform = EqualTransform(transform, cloth.transform, character.transform);
                                field.SetValue(component, equalTransform);
                                Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{value}\n{transform}->{equalTransform}");
                                continue;
                            }
                        }
                        //Debug.Log($"{component.name}.{field.Name}");
                    }
                }




            }
            /*
            {
                //Debug.Log($"physBones convert");

                var physBones = cloth.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
                var physBoneColliders = cloth.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>(true);
                foreach (var physBone in physBones)
                {
                    if (physBone.rootTransform == null)
                    {
                        continue;
                    }
                    physBone.rootTransform = GetEqualBone(bones, physBone.rootTransform);
                    physBone.ignoreTransforms = physBone.ignoreTransforms.ConvertAll(x=>GetEqualBone(bones, x));
                }
                foreach (var physBoneCollider in physBoneColliders)
                {
                    if (physBoneCollider.rootTransform == null)
                    {
                        continue;
                    }
                    physBoneCollider.rootTransform = GetEqualBone(bones, physBoneCollider.rootTransform);
                }
                foreach (var physBone in physBones)
                {
                    var bone = GetEqualBone(bones, physBone.transform);
                    if (bone==null)
                    {
                        continue;
                    }
                    var physBoneTarget = bone.GetComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>();

                    if (physBoneTarget==null)
                    {
                        continue;
                    }
                    {
                        physBoneTarget.colliders = physBone.colliders.ConvertAll(collider => GetEqualBone(bones, collider.transform).GetComponent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBoneCollider>() as VRC.Dynamics.VRCPhysBoneColliderBase);

                    }
                    //Debug.Log($"{physBoneTarget.name}.colliders: {physBoneTarget.colliders.Count}");
                }
            }
            */
        }




        Transform[] GetEqualBones(Transform[] targetBones, Transform[] sourceBones)
        {
            var bones = System.Array.ConvertAll(sourceBones, x => GetEqualBone(targetBones, x));
            return System.Array.FindAll(bones, x => x != null);
        }
        Transform GetEqualBone(Transform[] bones, Transform bone)
        {
            if (bone == null)
            {
                return null;
            }
            //var equalBone = System.Array.Find(bones, x => ArmaturePath(x, character.transform) == ArmaturePath(bone, cloth.transform));
            var equalBone = System.Array.Find(bones, x => ArmaturePath(x) == ArmaturePath(bone));
            if (equalBone == null)
            {
                Debug.Log($"{SearchUtils.GetHierarchyPath(bone.gameObject, false)} - {SearchUtils.GetHierarchyPath(cloth.gameObject, false)}");
                Debug.Log($"{ArmaturePath(bone, cloth.transform)}");
            }
            return equalBone;
            var boneName = bone.name;
            var boneNameParent = bone.parent.name;
            return System.Array.Find(bones, x => (x.name == boneName) && (x.parent.name == boneNameParent));

        }

    }
}
#endif