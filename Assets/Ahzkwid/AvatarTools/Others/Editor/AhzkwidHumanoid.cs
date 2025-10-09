#define USE_FINGER
using System.Collections.Generic;
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
#if USE_FINGER
        public Transform LeftThumbProximal;
        public Transform LeftThumbIntermediate;
        public Transform LeftThumbDistal;
        public Transform LeftIndexProximal;
        public Transform LeftIndexIntermediate;
        public Transform LeftIndexDistal;
        public Transform LeftMiddleProximal;
        public Transform LeftMiddleIntermediate;
        public Transform LeftMiddleDistal;
        public Transform LeftRingProximal;
        public Transform LeftRingIntermediate;
        public Transform LeftRingDistal;
        public Transform LeftLittleProximal;
        public Transform LeftLittleIntermediate;
        public Transform LeftLittleDistal;

        public Transform RightThumbProximal;
        public Transform RightThumbIntermediate;
        public Transform RightThumbDistal;
        public Transform RightIndexProximal;
        public Transform RightIndexIntermediate;
        public Transform RightIndexDistal;
        public Transform RightMiddleProximal;
        public Transform RightMiddleIntermediate;
        public Transform RightMiddleDistal;
        public Transform RightRingProximal;
        public Transform RightRingIntermediate;
        public Transform RightRingDistal;
        public Transform RightLittleProximal;
        public Transform RightLittleIntermediate;
        public Transform RightLittleDistal;
#endif

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

        /*
        public string GetHumanoidProperty(string transformName)
        {
            if (transformName.Contains("head"))
            {
                return "head";
            }
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


            foreach (HumanBodyBones humanBodyBone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (humanBodyBone.ToString() == transformName)
                {
                    return transformName;
                }
            }
            return null;
        }
        */


        /*
        public HumanBodyBones GetHumanBodyBones(string transformName)
        {


            switch (transformName)
            {
                case "Spine Front-Back":
                case "Spine Left-Right":
                case "Spine Twist Left-Right":
                    return HumanBodyBones.Spine;
                case "Chest Front-Back":
                case "Chest Left-Right":
                case "Chest Twist Left-Right":
                    return HumanBodyBones.Chest;
                case "UpperChest Front-Back":
                case "UpperChest Left-Right":
                case "UpperChest Twist Left-Right":
                    return HumanBodyBones.UpperChest;
                case "Neck Nod Down-Up":
                case "Neck Tilt Left-Right":
                case "Neck Turn Left-Right":
                    return HumanBodyBones.Neck;
                case "Head Nod Down-Up":
                case "Head Tilt Left-Right":
                case "Head Turn Left-Right":
                    return HumanBodyBones.Head;
                case "Left Eye Down-Up":
                case "Left Eye In-Out":
                    return HumanBodyBones.LeftEye;
                case "Right Eye Down-Up":
                case "Right Eye In-Out":
                    return HumanBodyBones.RightEye;
                case "Jaw Close":
                case "Jaw Left-Right":
                    return HumanBodyBones.Jaw;
                case "Left Upper Leg Front-Back":
                case "Left Upper Leg In-Out":
                case "Left Upper Leg Twist In-Out":
                    return HumanBodyBones.LeftUpperLeg;
                case "Left Lower Leg Stretch":
                case "Left Lower Leg Twist In-Out":
                    return HumanBodyBones.LeftLowerLeg;
                case "Left Foot Up-Down":
                case "Left Foot Twist In-Out":
                    return HumanBodyBones.LeftFoot;
                case "Left Toes Up-Down":
                    return HumanBodyBones.LeftToes;
                case "Right Upper Leg Front-Back":
                case "Right Upper Leg In-Out":
                case "Right Upper Leg Twist In-Out":
                    return HumanBodyBones.RightUpperLeg;
                case "Right Lower Leg Stretch":
                case "Right Lower Leg Twist In-Out":
                    return HumanBodyBones.RightLowerLeg;
                case "Right Foot Up-Down":
                case "Right Foot Twist In-Out":
                    return HumanBodyBones.RightFoot;
                case "Right Toes Up-Down":
                    return HumanBodyBones.RightToes;
                case "Left Shoulder Down-Up":
                case "Left Shoulder Front-Back":
                    return HumanBodyBones.LeftShoulder;
                case "Left Arm Down-Up":
                case "Left Arm Front-Back":
                case "Left Arm Twist In-Out":
                    return HumanBodyBones.LeftUpperArm;
                case "Left Forearm Stretch":
                case "Left Forearm Twist In-Out":
                    return HumanBodyBones.LeftLowerArm;
                case "Left Hand Down-Up":
                case "Left Hand In-Out":
                    return HumanBodyBones.LeftHand;
                case "Right Shoulder Down-Up":
                case "Right Shoulder Front-Back":
                    return HumanBodyBones.RightShoulder;
                case "Right Arm Down-Up":
                case "Right Arm Front-Back":
                case "Right Arm Twist In-Out":
                    return HumanBodyBones.RightUpperArm;
                case "Right Forearm Stretch":
                case "Right Forearm Twist In-Out":
                    return HumanBodyBones.RightLowerArm;
                case "Right Hand Down-Up":
                case "Right Hand In-Out":
                    return HumanBodyBones.RightHand;
            }

            foreach (HumanBodyBones humanBodyBone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (humanBodyBone.ToString() == transformName)
                {
                    return humanBodyBone;
                }
            }


            var name = transformName;
            if (FindBone(transformName, "head"))
            {
                return HumanBodyBones.Head;
            }
            if (FindBone(transformName, "neck"))
            {
                return HumanBodyBones.Neck;
            }
            if (FindBone(transformName, "chest"))
            {
                return HumanBodyBones.Chest;
            }
            if (FindBone(transformName, spineKeywords))
            {
                return HumanBodyBones.Spine;
            }
            if (FindBone(transformName, hipsKeywords))
            {
                return HumanBodyBones.Hips;
            }

            return (HumanBodyBones)(-1);
        }
        */
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

        Transform FindBone(Transform[] targetTransforms, string[] transformNames)
        {
            Transform transform = null;
            foreach (var transformName in transformNames)
            {
                if (transform == null)
                {
                    transform = FindBone(targetTransforms, transformName);
                }
                else
                {
                    break;
                }
            }
            return transform;
        }
        Transform[] FindBones(Transform[] targetTransforms, string[] kerwords)
        {
            var bones = new Transform[] { };
            foreach (var kerword in kerwords)
            {
                if (bones.Length == 0)
                {
                    bones = System.Array.FindAll(targetTransforms, transform => transform.name.ToLower().Contains(kerword));
                }
            }
            return bones;

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
        bool FindBone(string boneName, string[] kerwords)
        {
            foreach (var kerword in kerwords)
            {
                if (FindBone(boneName, kerword))
                {
                    return true;
                }
            }
            return false;
        }
        bool FindBone(string boneName, string kerword)
        {
            var lowerName = boneName.ToLower();
            if (lowerName == kerword)
            {
                return true;
            }
            if (lowerName.Contains(kerword))
            {
                return true;
            }
            return false;
        }

        static readonly string[] spineKeywords = new [] { "spine", "ribs" };
        static readonly string[] hipsKeywords = new [] { "hips", "pelvis", "hip" };
        static readonly string[] shoulderKeywords = new [] { "shoulder", "clavicle" };
        static readonly string[] handKeywords = new [] { "hand", "wrist" };
        static readonly string[] footKeywords = new [] { "foot", "ankle" };

        static readonly string[] upperLegKeywords = new [] { "thigh", "upleg" };
        static readonly string[] lowerLegKeywords = new [] { "knee", "calf" };
        static readonly string[] upperArmKeywords = new string[] { };
        static readonly string[] lowerArmKeywords = new [] { "elbow", "forearm" };


        public static readonly string[] fingerMuscleKeywords = new [] { "Stretched", "Spread" };

        static readonly Dictionary<string, string> toHumanoidMapping = new()
        {
            { "Left ", "LeftHand." },
            { "Right ", "RightHand." },
            { "Thumb ", "Thumb." },
            { "Index ", "Index." },
            { "Middle ", "Middle." },
            { "Ring ", "Ring." },
            { "Little ", "Little." }
        };
        public static string HumanoidToMuscle(string humanoidPropertyName)
        {

            var name = humanoidPropertyName;
            if (fingerMuscleKeywords.Any(type => name.Contains(type)))
            {
                foreach (var kvp in toHumanoidMapping)
                {
                    name = name.Replace(kvp.Value, kvp.Key);
                }
                name = name.Replace(" Hand.", "");
            }
            return name;
        }
        public static string MuscleToHumanoid(string muscleName)
        {
            var name = muscleName;
            if (fingerMuscleKeywords.Any(type => name.Contains(type)))
            {
                /*
                name = name.Replace("Left ", "LeftHand.");
                name = name.Replace("Right ", "RightHand.");
                name = name.Replace("Thumb ", "Thumb.");
                name = name.Replace("Index ", "Index.");
                name = name.Replace("Middle ", "Middle.");
                name = name.Replace("Ring ", "Ring.");
                name = name.Replace("Little ", "Little.");
                */

                foreach (var kvp in toHumanoidMapping)
                {
                    name = name.Replace(kvp.Key, kvp.Value);
                }
            }
            return name;
        }
        public void NameSearch(Transform[] transforms)
        {



            head = FindBone(transforms, "head");
            neck = FindBone(transforms, "neck");
            chest = FindBone(transforms, "chest");

            /*
            spine = FindBone(transforms, "spine");
            if (spine == null)
            {
                spine = FindBone(transforms, "ribs");
            }
            */
            spine = FindBone(transforms, spineKeywords);
            hips = FindBone(transforms, hipsKeywords);


            /*
            hips = FindBone(transforms, "hips");
            if (hips == null)
            {
                hips = FindBone(transforms, "pelvis");
            }
            if (hips == null)
            {
                hips = FindBone(transforms, "hip");
            }
            */



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




            /*
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

            */
            var shoulders = FindBones(transforms,shoulderKeywords);
            var handTransforms = FindBones(transforms, handKeywords);
            var footTransforms = FindBones(transforms, footKeywords);







            leftShoulder = GetLefts(shoulders).FirstOrDefault();
            rightShoulder = GetRights(shoulders).FirstOrDefault();

            leftFoot = GetLefts(footTransforms).FirstOrDefault();
            rightFoot = GetRights(footTransforms).FirstOrDefault();




            leftHand = GetLefts(handTransforms).FirstOrDefault();
            rightHand = GetRights(handTransforms).FirstOrDefault();


#if USE_FINGER
            if (leftHand != null)
            {
                var fingerTransforms = leftHand.GetComponentsInChildren<Transform>(true);

                SetFingers(fingerTransforms, ref LeftThumbProximal, ref LeftThumbIntermediate, ref LeftThumbDistal, "thumb");
                SetFingers(fingerTransforms, ref LeftIndexProximal, ref LeftIndexIntermediate, ref LeftIndexDistal, "index");
                SetFingers(fingerTransforms, ref LeftMiddleProximal, ref LeftMiddleIntermediate, ref LeftMiddleDistal, "middle");
                SetFingers(fingerTransforms, ref LeftRingProximal, ref LeftRingIntermediate, ref LeftRingDistal, "ring");
                SetFingers(fingerTransforms, ref LeftLittleProximal, ref LeftLittleIntermediate, ref LeftLittleDistal, "little");
            }

            if (rightHand != null)
            {
                var fingerTransforms = rightHand.GetComponentsInChildren<Transform>(true);

                SetFingers(fingerTransforms, ref RightThumbProximal, ref RightThumbIntermediate, ref RightThumbDistal, "thumb");
                SetFingers(fingerTransforms, ref RightIndexProximal, ref RightIndexIntermediate, ref RightIndexDistal, "index");
                SetFingers(fingerTransforms, ref RightMiddleProximal, ref RightMiddleIntermediate, ref RightMiddleDistal, "middle");
                SetFingers(fingerTransforms, ref RightRingProximal, ref RightRingIntermediate, ref RightRingDistal, "ring");
                SetFingers(fingerTransforms, ref RightLittleProximal, ref RightLittleIntermediate, ref RightLittleDistal, "little");
            }
#endif


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

            static void SetFingers(Transform[] fingerTransfoms,ref Transform proximal, ref Transform intermediate, ref Transform distal, string fingerName)
            {
                var transforms = System.Array.FindAll(fingerTransfoms, transform => transform.name.ToLower().Contains(fingerName));
                proximal = GetProximals(transforms).FirstOrDefault();
                intermediate = GetIntermediates(transforms).FirstOrDefault();
                distal = GetDistals(transforms).FirstOrDefault();
            }

            static Transform[] GetProximals(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("proximal"));
                return matchingTransforms;
            }
            static Transform[] GetIntermediates(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("intermediate"));
                return matchingTransforms;
            }
            static Transform[] GetDistals(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("distal"));
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

        public void DrawGizmo(bool ignoreHierarchy)
        {
#if UNITY_EDITOR

            List<Transform> GetChilds(params Transform[] transforms)
            {
                var childs = new List<Transform>();
                foreach (var transform in transforms)
                {
                    if (transform==null)
                    {
                        continue;
                    }
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        childs.Add(transform.GetChild(i));
                    }
                }
                return childs;
            }
            List<Transform> childs = null;
            List<Vector3> childPositions = null;
            List<Quaternion> childRotations = null;
            List<Vector3> childScales = null;
            
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
                            if (ignoreHierarchy)
                            {
                                childs = GetChilds(transform, symmetricalTransform);
                                childPositions = childs.ConvertAll(x => x.position);

                                foreach (var child in childs)
                                {
                                    UnityEditor.Undo.RecordObject(child, "Preserve Child Transform");
                                }
                            }

                            UnityEditor.Undo.RecordObject(transform, "Move Transform");
                            transform.position = newPosition;

                            if (symmetricalTransform != null)
                            {
                                UnityEditor.Undo.RecordObject(symmetricalTransform, "Move Transform");
                                var localPosition = root.InverseTransformPoint(newPosition);
                                localPosition.x = -localPosition.x;
                                symmetricalTransform.position = root.TransformPoint(localPosition);
                            }

                            if (ignoreHierarchy)
                            {
                                for (int i = 0; i < childs.Count; i++)
                                {
                                    childs[i].position = childPositions[i];
                                }
                            }
                        }
                        break;
                    case Tool.Rotate:
                        var newRotation = Handles.RotationHandle(transform.rotation, transform.position);

                        if (newRotation != transform.rotation)
                        {
                            if (ignoreHierarchy)
                            {
                                childs = GetChilds(transform, symmetricalTransform);
                                childPositions = childs.ConvertAll(x => x.position);
                                childRotations = childs.ConvertAll(x => x.rotation);

                                foreach (var child in childs)
                                {
                                    UnityEditor.Undo.RecordObject(child, "Preserve Child Transform");
                                }
                            }



                            UnityEditor.Undo.RecordObject(transform, "Rotate Transform");
                            transform.rotation = newRotation;

                            if (symmetricalTransform != null)
                            {
                                UnityEditor.Undo.RecordObject(symmetricalTransform, "Rotate Transform");
                                var localRotation = Quaternion.Inverse(root.rotation) * newRotation;

                                //localRotation = new Quaternion(-localRotation.x, localRotation.y, localRotation.z, localRotation.w); 
                                {
                                    // ��Ī ȸ�� ó�� (AI)
                                    var R = Matrix4x4.Rotate(localRotation);
                                    var S = Matrix4x4.Scale(new Vector3(-1f, 1f, 1f));
                                    var mirroredLocal = (S * R * S).rotation;
                                    localRotation = mirroredLocal;
                                }


                                symmetricalTransform.rotation = root.rotation * localRotation;
                            }


                            if (ignoreHierarchy)
                            {
                                for (int i = 0; i < childs.Count; i++)
                                {
                                    childs[i].position = childPositions[i];
                                    childs[i].rotation = childRotations[i];
                                }
                            }




                        }
                        break;
                    case Tool.Scale:
                        var newScale = Handles.ScaleHandle(transform.localScale, transform.position, Quaternion.identity);

                        if (newScale != transform.localScale)
                        {
                            if (ignoreHierarchy)
                            {
                                childs = GetChilds(transform, symmetricalTransform);
                                childPositions = childs.ConvertAll(x => x.position);
                                childScales = childs.ConvertAll(x => x.lossyScale);

                                foreach (var child in childs)
                                {
                                    UnityEditor.Undo.RecordObject(child, "Preserve Child Transform");
                                }
                            }



                            UnityEditor.Undo.RecordObject(transform, "Scale Transform");
                            transform.localScale = newScale;

                            if (symmetricalTransform != null)
                            {
                                UnityEditor.Undo.RecordObject(symmetricalTransform, "Scale Transform");
                                var localScale = new Vector3(newScale.x, newScale.y, newScale.z);
                                symmetricalTransform.localScale = localScale;
                            }


                            if (ignoreHierarchy)
                            {
                                for (int i = 0; i < childs.Count; i++)
                                {
                                    childs[i].position = childPositions[i];
                                    
                                    var ratio = new Vector3(
                                        childScales[i].x / childs[i].lossyScale.x,
                                        childScales[i].y / childs[i].lossyScale.y,
                                        childScales[i].z / childs[i].lossyScale.z
                                    );
                                    //childs[i].localScale = Vector3.Scale(childs[i].localScale, ratio);
                                }
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
