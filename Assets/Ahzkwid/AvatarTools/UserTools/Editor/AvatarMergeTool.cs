







#if UNITY_EDITOR

namespace Ahzkwid
{

    using UnityEngine;
    using System.Collections.Generic;



    using UnityEditor;
    using System.Linq;
    using System.Collections;
    using static Ahzkwid.AvatarMergeTool;
    using static AnimationRepairTool;
    using UnityEngine.TextCore.Text;

    [InitializeOnLoad]
    public class AvatarMergeTool : EditorWindow
    {

        Hashtable reorderableListTable = new Hashtable();
        public void DrawArray(string propertyPath)
        {

            var reorderableListProperty = serializedObject.FindProperty(propertyPath);

            if (reorderableListTable[propertyPath] == null)
            {
                reorderableListTable[propertyPath] = new UnityEditorInternal.ReorderableList(serializedObject, reorderableListProperty);
            }
            var reorderableList = (UnityEditorInternal.ReorderableList)reorderableListTable[propertyPath];

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
        public enum ForceMergeType
        {
            HumanBodyBones
            , Path
        }


        public GameObject[] characters;
        //public GameObject cloth;
        public GameObject[] cloths = new GameObject[] { null };
        //public List<Transform> boneTransforms = new List<Transform>();
        public bool createBackup = true;
        public bool nameMerge = false;
        public bool editMode = false;
        public bool ignoreHierarchy = false;
        
        public MergeType mergeType = MergeType.Default;

        public ForceMergeType forceMergeType = ForceMergeType.HumanBodyBones;
        public AutoMerge.MergeTrigger mergeTrigger = AutoMerge.MergeTrigger.Runtime;


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
            if ((cloths == null) || (cloths.Length == 0))
            {
                humanoid = null;
                return;
            }
            if (editMode)
            {
                humanoid.DrawGizmo(ignoreHierarchy);
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
            void UpdateHumanoid(GameObject cloth)
            {
                cloths = new GameObject[] { cloth };

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
            if (serializedObject == null)
            {
                serializedObject = new SerializedObject(this);
            }
            var allReady = true;
            serializedObject.Update();
            {

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(mergeTrigger)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(mergeType)));
                }
            }
            serializedObject.ApplyModifiedProperties();


            if (EditorGUI.EndChangeCheck())
            {
                UpdateHumanoid(cloths.FirstOrDefault());
            }
            serializedObject.Update();
            {


                if (mergeType == MergeType.ForceMerge)
                {
                    //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(forceMergeType)));
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(characters)));
                //DrawArray(nameof(characters));

                EditorGUILayout.Space();
                //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(cloth)));
                //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(cloths)));


                if (mergeType == MergeType.ForceMerge)
                {
                    GameObject fieldReturn = null;
                    EditorGUI.BeginChangeCheck();
                    {
                        fieldReturn = EditorGUILayout.ObjectField("Cloth", cloths.FirstOrDefault(), typeof(GameObject), true) as GameObject;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        UpdateHumanoid(fieldReturn);
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
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(ignoreHierarchy)));
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(nameMerge)));
            }
            serializedObject.ApplyModifiedProperties();
            if (characters == null)
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
                    //boneTransforms.Clear();
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
                        var character = characters[i % characters.Length];

                        if (createBackup && (mergeTrigger == AutoMerge.MergeTrigger.Always))
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
                                Merge(clothCopy, clothCopy, mergeType, forceMergeType);

                                //if (mergeType == MergeType.ForceMerge)
                                {
                                    //if (boneTransforms.Count <= 0)
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
                            if (cloth.transform.parent == character.transform)
                            {
                                cloth.transform.parent = null;
                            }
                            var components = cloth.GetComponentsInChildren<Component>(true);
                            if (System.Array.FindIndex(components,x=>x?.name=="AutoMerge")>=0)
                            {
                                cloth.transform.parent = character.transform;
                            }
                            else
                            {
                                switch (mergeTrigger)
                                {
                                    case AutoMerge.MergeTrigger.Always:
                                        Merge(character, cloth, mergeType, forceMergeType);
                                        break;
                                    case AutoMerge.MergeTrigger.Runtime:
                                        AddAutoMerge(character, cloth, mergeType, forceMergeType);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            //if (mergeType == MergeType.ForceMerge)
                            {
                                //if (boneTransforms.Count <= 0)
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

        /*
        static Transform EqualTransform(Transform target, Transform from, Transform to)
        {
            var fromPath = ObjectPath.GetPath(target, from);
            var transforms = to.GetComponentsInChildren<Transform>(true);
            return System.Array.Find(transforms, x => ObjectPath.GetPath(x, to) == fromPath);
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
            //return System.IO.Path.GetRelativePath(hierarchyPath, rootName);

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
        */
        void Test()
        {

            //Debug.Log(cloth.transform.root.name);
            //Debug.Log(SearchUtils.GetHierarchyPath(cloth, false));
            ////Debug.Log(cloth.transform.GetHierarchyPath());
            ////Debug.Log(cloth.transform.GetShortHierarchyPath());
            //Debug.Log(ArmaturePath(cloth.transform, cloth.transform));
        }


        /// <summary>
        /// 리스트에 ChildBone을 재귀적으로 추가합니다.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="rootBone"></param>
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


            if (rootBoneCloth == null)
            {
                Debug.LogWarning("rootBoneCloth==null");
                return;
            }



            var characterChilds = new List<Transform>();
            for (int i = 0; i < rootBoneCharacter.childCount; i++)
            {
                characterChilds.Add(rootBoneCharacter.GetChild(i));
            }
            /*

            var clothChilds = new List<Transform>();
            for (int i = 0; i < rootBoneCharacter.childCount; i++)
            {
                clothChilds.Add(rootBoneCloth.GetChild(i));
            }
            */








            //var addedGameObjects = PrefabUtility.GetAddedGameObjects(rootBoneCloth.gameObject);



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



                    RepairPhysBones(gameObject.transform);
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
                    if (childCharacter.transform.localPosition.x != 0)
                    {
                        if (childCharacter.transform.localPosition.x > childCloth.transform.localPosition.x * 99f)
                        {
                            childCharacter.transform.localPosition = childCloth.transform.localPosition;
                        }
                        if (childCharacter.transform.localPosition.x * 99f < childCloth.transform.localPosition.x)
                        {
                            childCharacter.transform.localPosition = childCloth.transform.localPosition;
                        }
                    }
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
        /*
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
        */
        public static void Merge(GameObject character, GameObject cloth, MergeType mergeType = MergeType.Default,ForceMergeType forceMergeType = ForceMergeType.HumanBodyBones)
        {

            switch (mergeType)
            {
                case MergeType.ForceMerge:

                    /*
                    var characterCopy = Instantiate(character);
                    characterCopy.SetActive(true);
                    character.SetActive(false);
                    //cloth.SetActive(true);
                    var clothCopy = Instantiate(cloth);
                    clothCopy.SetActive(true);
                    cloth.SetActive(false);


                    ForceMerge(characterCopy, clothCopy);
                    */
                    ForceMerge(character, cloth, forceMergeType);
                    break;
                case MergeType.Default:
                default:
                    MergeDefault(character, cloth);
                    break;
            }
        }

        public static void AddAutoMerge(GameObject character, GameObject cloth, MergeType mergeType = MergeType.Default, ForceMergeType forceMergeType=ForceMergeType.HumanBodyBones)
        {

            cloth.transform.parent = character.transform;
            var gameObject = new GameObject("AutoMerge");
            gameObject.tag = "EditorOnly";
            gameObject.transform.parent = cloth.transform;
            var autoMerge= gameObject.AddComponent<AutoMerge>();
            autoMerge.mergeType = mergeType;
            autoMerge.forceMergeType = forceMergeType;

            var clothRoot = new AutoMerge.ClothRoot();
            clothRoot.option = AutoMerge.ClothRoot.Option.GameObject;
            clothRoot.gameObject = cloth;
            autoMerge.clothRoots.Add(clothRoot);
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
        public static void ForceMerge(GameObject character, GameObject cloth, ForceMergeType forceMergeType)
        {
            //var characterAnimator = character.GetComponentInChildren<Animator>(true);
            //var clothAnimator = character.GetComponentInChildren<Animator>(true);
            var characterAnimator = character.GetComponent<Animator>();
            //var clothAnimator = cloth.GetComponent<Animator>();



            switch (forceMergeType)
            {
                case ForceMergeType.HumanBodyBones:

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

                        RepairPhysBones(clothTransform);
                    }
                    break;
                case ForceMergeType.Path:

                        var clothArmature = ObjectPath.GetArmature(cloth);

                        var characterArmature = ObjectPath.GetArmature(character);

                        if (clothArmature == null)
                        {
                            Debug.LogError("clothArmature == null");
                        }
                        if (characterArmature == null)
                        {
                            Debug.LogError("characterArmature == null");
                        }

                        var characterBones = new Dictionary<string,Transform>();
                        {
                            var transforms = characterArmature.GetComponentsInChildren<Transform>(true);
                            foreach (var transform in transforms)
                            {
                                var key = ObjectPath.GetPath(transform, characterArmature);
                                if (key==null)
                                {
                                    Debug.LogError($"key == null\n{transform.name}");
                                }
                                characterBones.Add(key, transform);
                            }
                        }


                        var clothBones = new Dictionary<string,Transform>();
                        {
                            var transforms= clothArmature.GetComponentsInChildren<Transform>(true);
                            foreach (var transform in transforms)
                            {
                                var key = ObjectPath.GetPath(transform, clothArmature);
                                if (key == null)
                                {
                                    Debug.LogError($"key == null\n{transform.name}");
                                }
                                clothBones.Add(key, transform);
                            }
                        }
                        foreach (var clothBone in clothBones)
                        {
                            if (characterBones.TryGetValue(clothBone.Key, out var value))
                            {
                                clothBone.Value.parent = value;
                                RepairPhysBones(clothBone.Value);
                            }
                        }





                    break;
                default:
                    break;
            }


            cloth.transform.parent = character.transform;


            RepairPhysBones(character.transform);




        }
        public static void RepairPhysBones(Transform transform)
        {
            var physBones = transform.GetComponentsInParent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
            var parents = transform.GetComponentsInParent<Transform>(true);
            foreach (var physBone in physBones)
            {
                var physBoneTarget = physBone.rootTransform;
                if (physBoneTarget == null)
                {
                    physBoneTarget = physBone.transform;
                }


                var contains = false;
                foreach (var parent in parents)
                {
                    if (physBone.ignoreTransforms.Contains(parent))
                    {
                        contains = true;
                        break;
                    }
                }
                if (contains)
                {
                    continue;
                }
                physBone.ignoreTransforms.Add(transform);
            }
        }
        public static void RepairPhysBones(Transform from,Transform to)
        {
            //Repair PhysBone
            var physBones = from.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
            foreach (var physBone in physBones)
            {
                var physBoneTarget = physBone.rootTransform;
                if (physBoneTarget == null)
                {
                    if (from==to)
                    {
                        physBoneTarget = physBone.transform;
                    }
                    else
                    {
                        physBoneTarget = ObjectPath.EqualTransform(from.transform, to.transform, physBone.transform);
                    }
                }
                if (physBoneTarget == null)
                {
                    continue;
                }

                var parents = physBoneTarget.GetComponentsInParent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
                foreach (var parent in parents)
                {
                    if (physBoneTarget == parent.transform)
                    {
                        continue;
                    }
                    if (physBone.transform == parent.transform)
                    {
                        continue;
                    }
                    if (parent.ignoreTransforms.Contains(physBoneTarget))
                    {
                        continue;
                    }
                    parent.ignoreTransforms.Add(physBoneTarget);
                }
            }
        }
        public static void MergeDefault(GameObject character, GameObject cloth)
        {
            /*

            Transform GetRootBone(GameObject gameObject)
            {
                //var armatureNames = new string[] { "Armature", "armature" };
                //{
                //    foreach (var armatureName in armatureNames)
                //    {
                //        return gameObject.transform.Find(armatureName);
                //    }
                //}


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


            
        */








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
                var armature = ObjectPath.GetArmature(cloth);
                if (armature != null)
                {
                    armature.gameObject.SetActive(false);
                }
            }

            {
                var parents= cloth.transform.GetComponentsInParent<Transform>();
                if (System.Array.FindIndex(parents,x=>x== character.transform) < 0)
                {
                    cloth.transform.parent = character.transform;
                }
            }

            var characterRenderer = character.GetComponentInChildren<SkinnedMeshRenderer>(true);
            var clothRenderers = cloth.GetComponentsInChildren<SkinnedMeshRenderer>(true);


            var bones = (Transform[])characterRenderer.bones.Clone();

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
                    var characterArmature = ObjectPath.GetArmature(character);
                    var clothArmature = ObjectPath.GetArmature(cloth);


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
                AddChildBones(ref list, ObjectPath.GetArmature(character));
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


                ////var bones = characterRenderer.bones;
                //var bones = list.ToArray();
                //Debug.Log($"bones.Length: {bones.Length}");
                ////Debug.Log($"characterRenderer.bones: {string.Join(",", System.Array.ConvertAll(characterRenderer.bones, x => x.name))}"); //BlendshapeAutoSetter와 충돌
                //Debug.Log($"bones: {string.Join(",", System.Array.ConvertAll(bones, x => x.name))}");

                ////return;
                ////targetRenderer.sharedMesh = originalRenderer.sharedMesh;





                ///*
                //var originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(cloth);
                //var originalPrefabRenderers = originalPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();

                //foreach (var clothRenderer in clothRenderers)
                //{
                //    var originalPrefabRender = System.Array.Find(originalPrefabRenderers, x => x.sharedMesh == clothRenderer.sharedMesh);
                //    clothRenderer.bones = originalPrefabRender.bones;
                //}
                //Debug.LogWarning("Repair");
                //*/

                //foreach (var clothRenderer in clothRenderers)
                //{
                //    if (PrefabUtility.IsPartOfPrefabInstance(clothRenderer))
                //    {
                //        //PrefabUtility.RevertObjectOverride(clothRenderer, InteractionMode.UserAction);
                //        var originalPrefab = PrefabUtility.GetCorrespondingObjectFromSource(clothRenderer);
                //        if ((clothRenderer.bones != null) && (originalPrefab.bones != null))
                //        {
                //            Debug.Log($"clothRenderer.bones: {string.Join(",", System.Array.ConvertAll(clothRenderer.bones, x => x?.name))}" +
                //                $"->originalPrefab.bones: {string.Join(",", System.Array.ConvertAll(originalPrefab.bones, x => x?.name))}");
                //        }
                //        clothRenderer.bones = originalPrefab.bones;
                //    }


            //        Debug.Log($"clothRenderer.bones: {string.Join(",", System.Array.ConvertAll(clothRenderer.bones, x => x?.name))}");


            //        ////var bonesFilters= System.Array.FindAll(bones, x => System.Array.FindIndex(clothRenderer.bones, y => y.name == x.name) >= 0);
            //        //var boneList = new List<Transform>();
            //        //for (int i = 0; i < clothRenderer.bones.Length; i++)
            //        //{
            //        //    //var boneName = clothRenderer.bones[i].name;
            //        //    //var boneNameParent = clothRenderer.bones[i].parent.name;
            //        //    //boneList.Add(System.Array.Find(bones, x => (x.name == boneName) && (x.parent.name == boneNameParent)));
            //        //    boneList.Add(GetEqualBone(bones, clothRenderer.bones[i]));
            //        //}
            //        //clothRenderer.bones = boneList.ToArray();
            //        //ObjectPath.RepathFields(cloth.transform, character.transform, clothRenderer,clothRenderer.GetType().GetFields());

            //        Debug.Log($"{clothRenderer}.bones.Length (Pre): {clothRenderer.bones.Length}");
            //        //var equalBones = GetEqualBones(bones, clothRenderer.bones, character.transform, cloth.transform);
            //        var equalBones = ObjectPath.EqualTransforms(cloth.transform, character.transform, clothRenderer.bones);
            //        if (equalBones.Length > 0)
            //        {
            //            clothRenderer.bones = equalBones;
            //        }
            //        //clothRenderer.bones = System.Array.FindAll(characterRenderer.bones,x=> System.Array.FindIndex(clothRenderer.bones, y => y.name == x.name) >= 0);
            //        Debug.Log($"{clothRenderer}.bones.Length (After): {clothRenderer.bones.Length}");
            //        //clothRenderer.rootBone = GetEqualBone(bones, clothRenderer.rootBone, character.transform, cloth.transform);
            //        clothRenderer.rootBone = ObjectPath.EqualTransform(cloth.transform, character.transform, clothRenderer.rootBone);
            //        {
            //            Transform probeAnchor = null;
            //            //probeAnchor = GetEqualBone(bones, clothRenderer.probeAnchor);
            //            if (clothRenderer.probeAnchor != null)
            //            {
            //                //본이 아니면 일반 오브젝트경로
            //                if (probeAnchor == null)
            //                {
            //                    var transforms = character.GetComponentsInChildren<Transform>();
            //                    var relativePath = ObjectPath.GetPath(clothRenderer.probeAnchor, cloth.transform);
            //                    if (relativePath == null)
            //                    {
            //                        //이미 할당됨
            //                        continue;
            //                    }
            //                    var equalBone = System.Array.Find(transforms, x =>
            //                    {
            //                        var characterRelativePath = ObjectPath.GetPath(x, character.transform);
            //                        if (characterRelativePath == null)
            //                        {
            //                            return false;
            //                        }
            //                        return characterRelativePath == relativePath;
            //                    });
            //                    probeAnchor = equalBone;
            //                }
            //            }
            //            clothRenderer.probeAnchor = probeAnchor;
            //        }

            //        UnityEditor.EditorUtility.SetDirty(clothRenderer);
            //        //clothRenderer.probeAnchor = characterRenderer.probeAnchor;
            //    }

                /*
                {
                    var characterComponents = character.GetComponentsInChildren<Component>(true);
                    var clothComponents = cloth.GetComponentsInChildren<Component>(true);
                    //var clothRelativePaths = System.Array.ConvertAll(clothComponents, component => RelativePath(component.transform,cloth.transform));
                    //var characterRelativePaths = System.Array.ConvertAll(characterComponents, component => RelativePath(component.transform, character.transform));
                    //characterComponents = System.Array.FindAll(characterComponents, x => System.Array.FindIndex(clothRelativePaths, relativePath => RelativePath(x.transform, character.transform) == relativePath) >= 0);
                    //clothComponents = System.Array.FindAll(clothComponents, x => System.Array.FindIndex(clothRelativePaths, relativePath => RelativePath(x.transform, cloth.transform) == relativePath) >= 0);

                    foreach (var component in characterComponents)
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

                        if (relativePath!=null)
                        {
                            //의상의 아마추어 하위는 진행하지 않음
                            //continue;
                            if (relativePath.Length > "Armature".Length)
                            {
                                if (relativePath.Substring(0, "Armature".Length).ToLower() == "armature")
                                {
                                    continue;
                                }
                            }
                        }

                        //if (SearchUtils.GetHierarchyPath(component.gameObject, false).Contains(SearchUtils.GetHierarchyPath(cloth.gameObject, false)))
                        //{
                        //    continue;
                        //}

                        //var equalTypeComponents = System.Array.FindAll(characterComponents, x => x.GetType() == component.GetType());
                        //if (System.Array.FindIndex(equalTypeComponents, x => {
                        //    var characterRelativePath = RelativePath(x.transform, character.transform);
                        //    if (characterRelativePath==null)
                        //    {
                        //        return false;
                        //    }
                        //    return relativePath == characterRelativePath;
                        //    }) < 0)
                        //{
                        //    //동일한 컴포넌트중 캐릭터상에 존재하지 않으면 스킵
                        //    continue;
                        //}

                        var equalTransform = EqualTransform(component.transform, cloth.transform, character.transform);

                        if (equalTransform == null)
                        {
                            Debug.LogWarning($"equalTransform==null");
                            Debug.LogWarning($"{SearchUtils.GetHierarchyPath(component.transform.gameObject, false)}" +
                                $"\n{SearchUtils.GetHierarchyPath(cloth.gameObject, false)}" +
                                $"\n{SearchUtils.GetHierarchyPath(character.gameObject, false)}");
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
                */






                ObjectPath.RepathComponents(cloth.transform, character.transform);


                //RepairPhysBones(cloth.transform, character.transform);
                /*
                {
                    //Repair PhysBone
                    var physBones = cloth.GetComponentsInChildren<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
                    foreach (var physBone in physBones)
                    {
                        var physBoneTarget = physBone.rootTransform;
                        if (physBoneTarget == null)
                        {
                            physBoneTarget = ObjectPath.EqualTransform(cloth.transform, character.transform, physBone.transform);
                        }
                        if (physBoneTarget == null)
                        {
                            continue;
                        }

                        var parents= physBoneTarget.GetComponentsInParent<VRC.SDK3.Dynamics.PhysBone.Components.VRCPhysBone>(true);
                        foreach (var parent in parents)
                        {
                            if (physBoneTarget == parent.transform)
                            {
                                continue;
                            }
                            if (parent.ignoreTransforms.Contains(physBoneTarget))
                            {
                                continue;
                            }
                            parent.ignoreTransforms.Add(physBoneTarget);
                        }
                    }
                }
                */






                //ObjectPath.ComponentsCopy(cloth.transform, character.transform);
                //var components = character.GetComponentsInChildren<Component>(true);

                /*
                var components = character.GetComponentsInChildren<Component>(true);
                foreach (var component in components)
                {
                    ObjectPath.RepathFields(cloth.transform, character.transform, component, component.GetType().GetFields());
                }
                */
                //{
                //    //기타 컴포넌트 병합

                //    bool IsArrayCustom(System.Type type)
                //    {
                //        return (type.IsArray) || ((type.IsGenericType) && type.GetGenericTypeDefinition() == typeof(List<>));
                //    }
                //    var components = character.GetComponentsInChildren<Component>(true);
                //    foreach (var component in components)
                //    {
                //        if (component == null)
                //        {
                //            continue;
                //        }
                //        if (component is Transform)
                //        {
                //            continue;
                //        }
                //        //var compPath = SearchUtils.GetHierarchyPath(component.gameObject, false);
                //        //var relativePath = RelativePath(component.transform, cloth.transform);
                //        var relativePath = ObjectPath.GetPath(component.transform, cloth.transform);
                //        if (relativePath != null)
                //        {
                //            //의상의 아마추어 하위는 진행하지 않음
                //            //continue;

                //            if (relativePath.ToLower().StartsWith("armature"))
                //            {
                //                continue;
                //            }
                //            /*
                //            if (relativePath.Length > "Armature".Length)
                //            {
                //                if (relativePath.Substring(0, "Armature".Length).ToLower() == "armature")
                //                {
                //                    continue;
                //                }
                //            }
                //            */
                //        }
                //        //Debug.Log($"{component.GetType()} {component.name}");

                //        //ObjectPath.RepathFields(cloth.transform, character.transform, component, component.GetType().GetFields());

                //        foreach (var field in component.GetType().GetFields())
                //        {

                //            var value = field.GetValue(component);
                //            if (value == null)
                //            {
                //                continue;
                //            }
                //            if (value.Equals(null))
                //            {
                //                continue;
                //            }
                //            if (IsArrayCustom(field.FieldType))
                //            {
                //                var ilist = (System.Collections.IList)value;
                //                for (int i = 0; i < ilist.Count; i++)
                //                {
                //                    var item = ilist[i];
                //                    var transform = item as Transform;
                //                    if (transform == null)
                //                    {
                //                        if (item == null)
                //                        {
                //                            continue;
                //                        }
                //                        var property = item.GetType().GetProperty("transform");
                //                        if (property == null)
                //                        {
                //                            continue;
                //                        }
                //                        transform = property.GetValue(item) as Transform;
                //                    }
                //                    if (transform == null)
                //                    {
                //                        continue;
                //                    }
                //                    Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{transform}");
                //                    Debug.Log($"GetComponent:{transform.GetComponent(item.GetType())}");
                //                    //var equalTransform = EqualTransform(transform, cloth.transform, character.transform);
                //                    var equalTransform = ObjectPath.EqualTransform(cloth.transform, character.transform, transform);
                //                    Debug.Log($"{ilist[i]}:{transform.GetComponent(item.GetType())}");
                //                    if (equalTransform == null)
                //                    {
                //                        Debug.LogWarning($"equalTransform==null");
                //                        Debug.LogWarning($"{SearchUtils.GetHierarchyPath(transform.gameObject, false)}" +
                //                            $"\n{SearchUtils.GetHierarchyPath(cloth.gameObject, false)}" +
                //                            $"\n{SearchUtils.GetHierarchyPath(character.gameObject, false)}");
                //                        ilist[i] = item;
                //                    }
                //                    else
                //                    {
                //                        ilist[i] = equalTransform.GetComponent(item.GetType());
                //                    }
                //                }
                //                field.SetValue(component, ilist);

                //            }
                //            else
                //            {
                //                if (field.FieldType == typeof(Transform))
                //                {
                //                    var transform = value as Transform;
                //                    //var equalTransform = EqualTransform(transform, cloth.transform, character.transform);
                //                    var equalTransform = ObjectPath.EqualTransform(cloth.transform, character.transform, transform);
                //                    if (equalTransform == null)
                //                    {
                //                        continue;
                //                    }
                //                    field.SetValue(component, equalTransform);
                //                    Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{value}\n{transform}->{equalTransform}");
                //                    continue;
                //                }
                //            }
                //        //Debug.Log($"{component.name}.{field.Name}");
                //        }

                //    }




                //}
                /*
                {
                    var components = character.GetComponentsInChildren<Component>(true);
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
                            if (field.FieldType == typeof(RuntimeAnimatorController))
                            {
                                var runtimeAnimator = value as RuntimeAnimatorController;
                                var animator = runtimeAnimator as AnimatorController;
                                var layers = animator.layers;
                                foreach (var item in animator.layers)
                                {

                                }
                                var equalTransform = EqualTransform(transform, cloth.transform, character.transform);
                                field.SetValue(component, equalTransform);
                                Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{value}\n{transform}->{equalTransform}");
                                continue;
                            }
                            //Debug.Log($"{component.name}.{field.Name}");
                        }
                    }
                }
                */
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



            /*
            Transform[] GetEqualBones(Transform[] targetBones, Transform[] sourceBones, Transform targetBonesRoot = null, Transform sourceBonesRoot = null)
            {
                var bones = System.Array.ConvertAll(sourceBones, x => GetEqualBone(targetBones, x, targetBonesRoot, sourceBonesRoot));
                return System.Array.FindAll(bones, x => x != null);
            }
            Transform GetEqualBone(Transform[] bones, Transform bone, Transform bonesRoot = null, Transform boneRoot = null)
            {
                if (bone == null)
                {
                    return null;
                }
                //var equalBone = System.Array.Find(bones, x => ArmaturePath(x, character.transform) == ArmaturePath(bone, cloth.transform));
                var bonePath = ArmaturePath(bone);
                var equalBone = System.Array.Find(bones, x => ArmaturePath(x) == bonePath);
                //var bonePath = RelativePath(bone, boneRoot);
                //var equalBone = System.Array.Find(bones, x => RelativePath(x, bonesRoot) == bonePath);
                if (equalBone == null)
                {
                    Debug.Log($"{SearchUtils.GetHierarchyPath(bone.gameObject, false)} - {SearchUtils.GetHierarchyPath(cloth.gameObject, false)}");
                    //Debug.Log($"{ArmaturePath(bone, cloth.transform)}");
                    //Debug.Log($"{RelativePath(bone, boneRoot)}");
                    Debug.Log($"{bonePath}");
                }
                return equalBone;
                var boneName = bone.name;
                var boneNameParent = bone.parent.name;
                return System.Array.Find(bones, x => (x.name == boneName) && (x.parent.name == boneNameParent));

            }
            */

        }
    }
}
#endif