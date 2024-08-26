
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ahzkwid
{
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
                    var name = field.Name.Replace("right", "left");
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
        public static Transform GetBoneTransform(GameObject gameObject,HumanBodyBones humanBodyBone)
        {
            return new AhzkwidHumanoid(gameObject).GetBoneTransform(humanBodyBone);
        }
        public Transform GetBoneTransform(HumanBodyBones humanBodyBone)
        {
            var fields = GetType().GetFields();
            var boneName = humanBodyBone.ToString();

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
                var humanBodyBone = System.Array.Find(humanBodyBones, x => lowerFieldName == x.ToString().ToLower());
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



        readonly string[] upperLegKeywords = new string[] { "thigh", "upleg" };
        readonly string[] lowerLegKeywords = new string[] { "knee", "calf" };
        readonly string[] upperArmKeywords = new string[] { };
        readonly string[] lowerArmKeywords = new string[] { "elbow", "forearm" };
        public void NameSearch(Transform[] transforms)
        {



            head = FindBone(transforms, "head");
            neck = FindBone(transforms, "neck");
            chest = FindBone(transforms, "chest");
            spine = FindBone(transforms, "spine");
            if (spine == null)
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
            var legTransforms = FindWithKeywords(transforms, (new string[] { "leg" }).Concat(upperLegKeywords).Concat(lowerLegKeywords).ToArray());
            var armTransforms = FindWithKeywords(transforms, new string[] { "arm" }.Concat(lowerArmKeywords).ToArray(), new string[] { "armature" });

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



            //�ҰŹ�
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
                        if (arms.Sum(x => (x != null) ? 1 : 0) == 3) //4���� �Ѱ��� ������� �����õ�
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

            //�����ġ�� ����
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
                var filteredTransform = System.Array.Find(targetTransforms, transform => transform.name.ToLower() == transformName);
                if (filteredTransform != null)
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
                if (field.Name == "root")
                {
                    continue;
                }
                UnityEditor.Handles.Label(transform.position, transform.name);

                var symmetricalTransform = GetSymmetricalTransform(transform);
                switch (UnityEditor.Tools.current)
                {
                    case Tool.View:
                        break;
                    case Tool.Move:
                        var newPosition = Handles.PositionHandle(transform.position, Quaternion.identity);

                        if (newPosition != transform.position)
                        {
                            UnityEditor.Undo.RecordObject(transform, "Move Transform");
                            transform.position = newPosition;

                            if (symmetricalTransform != null)
                            {
                                UnityEditor.Undo.RecordObject(symmetricalTransform, "Move Transform");
                                var localPosition = root.InverseTransformPoint(newPosition);
                                localPosition.x = -localPosition.x;
                                symmetricalTransform.position = root.TransformPoint(localPosition);
                            }
                        }
                        break;
                    case Tool.Rotate:
                        var newRotation = Handles.RotationHandle(transform.rotation, transform.position);

                        if (newRotation != transform.rotation)
                        {
                            UnityEditor.Undo.RecordObject(transform, "Rotate Transform");
                            transform.rotation = newRotation;

                            if (symmetricalTransform != null)
                            {
                                UnityEditor.Undo.RecordObject(symmetricalTransform, "Rotate Transform");
                                var localRotation = Quaternion.Inverse(root.rotation) * newRotation;
                                localRotation = new Quaternion(-localRotation.x, localRotation.y, localRotation.z, localRotation.w); // ��Ī ȸ�� ó��
                                symmetricalTransform.rotation = root.rotation * localRotation;
                            }
                        }
                        break;
                    case Tool.Scale:
                        var newScale = Handles.ScaleHandle(transform.localScale, transform.position, Quaternion.identity);

                        if (newScale != transform.localScale)
                        {
                            UnityEditor.Undo.RecordObject(transform, "Scale Transform");
                            transform.localScale = newScale;

                            if (symmetricalTransform != null)
                            {
                                UnityEditor.Undo.RecordObject(symmetricalTransform, "Scale Transform");
                                var localScale = new Vector3(newScale.x, newScale.y, newScale.z);
                                symmetricalTransform.localScale = localScale;
                            }
                        }

                        break;
                    case Tool.Rect:
                        break;
                    case Tool.Transform:
                        break;
                    case Tool.Custom:
                        break;
                    case Tool.None:
                        break;
                    default:
                        break;
                }

            }
#endif
        }
        public void Update(GameObject root)
        {
            Clear();
            if (root == null)
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

}
