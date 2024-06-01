
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using static VRC.Dynamics.PhysBoneManager;








#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Search;
using UnityEditorInternal;

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


    public GameObject character;
    //public GameObject cloth;
    public GameObject[] cloths = new GameObject[] { null };
    public bool createBackup = true;
    public MergeType mergeType = MergeType.Default;


    //[UnityEditor.MenuItem("Ahzkwid/AvatarTools/" + nameof(AvatarMergeTool))]
    public static void Init()
    {
        var window = GetWindow<AvatarMergeTool>(false, nameof(AvatarMergeTool));
        window.minSize = new Vector2(400, 200);
        window.Show();
    }
    SerializedObject serializedObject;

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
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(character)));
            EditorGUILayout.Space();
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(cloth)));
            //EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(cloths)));
            DrawArray(nameof(cloths));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(mergeType)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(createBackup)));
        }
        serializedObject.ApplyModifiedProperties();
        if (character == null)
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
        GUI.enabled = allReady;
        {
            if (GUILayout.Button("Merge"))
            {
                if (createBackup)
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
                    foreach (var cloth in cloths)
                    {
                        var clothCopy = InstantiatePrefab(cloth);
                        cloth.SetActive(false);
                        clothCopy.SetActive(true);
                        //Merge(characterCopy, clothCopy, mergeType);
                        Merge(character, cloth, mergeType);
                    }
                }
                else
                {
                    //Merge(character, cloth);
                    foreach (var cloth in cloths)
                    {
                        Merge(character, cloth, mergeType);
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

        return hierarchyPath.Substring(startIndex, hierarchyPath.Length - startIndex);
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
    public static void ForceMerge(GameObject character, GameObject cloth)
    {
        //var characterAnimator = character.GetComponentInChildren<Animator>(true);
        //var clothAnimator = character.GetComponentInChildren<Animator>(true);
        var characterAnimator = character.GetComponent<Animator>();
        var clothAnimator = cloth.GetComponent<Animator>();


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
                var animator = clothAnimator;
                var leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
                var rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot);
                var leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
                var rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);


                var upper = (leftUpperArm.transform.position + rightUpperArm.transform.position) / 2;
                var lower = (leftFoot.transform.position + rightFoot.transform.position) / 2;

                clothPos = lower;
                clothHeghit = upper.y - lower.y;
            }

            if (clothHeghit<=0)
            {
                return;
            }


            var ratio = characterHeghit / clothHeghit;
            Debug.Log($"characterHeghit: {characterHeghit}, clothHeghit: {clothHeghit}, ratio: {ratio}");
            cloth.transform.localScale *= ratio;
            cloth.transform.position += characterPos - clothPos;
            {
                var leftFootCloth = clothAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
                var rightFootCloth = clothAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
                var leftFootCharacter = characterAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
                var rightFootCharacter = characterAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
                leftFootCloth.position = leftFootCharacter.position;
                rightFootCloth.position = rightFootCharacter.position;

            }
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
            var clothTransform = clothAnimator.GetBoneTransform(humanBodyBone);
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
            clothTransform.parent= characterTransform;
        }


        cloth.transform.parent = character.transform;
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
                            var equalBone = System.Array.Find(transforms, x => RelativePath(x, character.transform) == RelativePath(clothRenderer.probeAnchor,cloth.transform));
                            probeAnchor = equalBone;
                        }
                    }
                    clothRenderer.probeAnchor = probeAnchor;
                }


                UnityEditor.EditorUtility.SetDirty(clothRenderer);
                //clothRenderer.probeAnchor = characterRenderer.probeAnchor;
            }

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