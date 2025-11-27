#define USE_FINGER
#define USE_BREAST
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
        public Transform leftThumbProximal;
        public Transform leftThumbIntermediate;
        public Transform leftThumbDistal;
        public Transform leftIndexProximal;
        public Transform leftIndexIntermediate;
        public Transform leftIndexDistal;
        public Transform leftMiddleProximal;
        public Transform leftMiddleIntermediate;
        public Transform leftMiddleDistal;
        public Transform leftRingProximal;
        public Transform leftRingIntermediate;
        public Transform leftRingDistal;
        public Transform leftLittleProximal;
        public Transform leftLittleIntermediate;
        public Transform leftLittleDistal;

        public Transform rightThumbProximal;
        public Transform rightThumbIntermediate;
        public Transform rightThumbDistal;
        public Transform rightIndexProximal;
        public Transform rightIndexIntermediate;
        public Transform rightIndexDistal;
        public Transform rightMiddleProximal;
        public Transform rightMiddleIntermediate;
        public Transform rightMiddleDistal;
        public Transform rightRingProximal;
        public Transform rightRingIntermediate;
        public Transform rightRingDistal;
        public Transform rightLittleProximal;
        public Transform rightLittleIntermediate;
        public Transform rightLittleDistal;
#endif
#if USE_BREAST
        public Transform leftBreast;
        public Transform rightBreast;
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




        public System.Reflection.FieldInfo[] GetBonesField()
        {
            var bones = new List<System.Reflection.FieldInfo>();
            var fields = GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(Transform))
                {
                    continue;
                }
                bones.Add(field);
            }
            return bones.ToArray();
        }
        public Transform[] GetBoneTransforms()
        {
            var bones = new List<Transform>();
            var fields = GetBonesField();
            foreach (var field in fields)
            {
                var transform = (Transform)field.GetValue(this);
                if (transform == null)
                {
                    continue;
                }
                bones.Add(transform);
            }
            return bones.ToArray();
        }
        public Transform[] GetBoneTransforms(HumanBodyBones[] humanBodyBone)
        {
            var bones = System.Array.ConvertAll(humanBodyBone, x => GetBoneTransform(x));
            bones=System.Array.FindAll(bones, x => x != null);
            return bones;
        }
        public Transform GetBoneTransform(HumanBodyBones humanBodyBone)
        {
            var fields = GetBonesField();
            var boneName = humanBodyBone.ToString();

            foreach (var field in fields)
            {
                if (field.Name.ToLower() != boneName.ToLower())
                {
                    continue;
                }
                return (Transform)field.GetValue(this);
            }
            return null;
        }

        public Transform[] GetBoneTransformChilds(AvatarMask avatarMask)
        {
            var humanBodyBones = AvatarMaskToHumanBodyBones(avatarMask);
            var childs = GetBoneTransformChilds(humanBodyBones);
            return childs.ToArray();
        }
        public Transform[] GetBoneTransformChilds(params HumanBodyBones[] humanBodyBone)
        {
            var bones = GetBoneTransforms(humanBodyBone);
            var childs = GetBoneTransformChilds(bones);
            return childs.ToArray();
        }
        public Transform[] GetBoneTransformChilds(params Transform[] bones)
        {
            Transform[] GetChilds(Transform bone, Transform[] ignored)
            {
                var list = new List<Transform>();
                for (int i = 0; i < bone.childCount; i++)
                {
                    var child = bone.GetChild(i);
                    if (ignored.Contains(child))
                    {
                        continue;
                    }
                    list.Add(child);

                    var childs = GetChilds(child, ignored);
                    list.AddRange(childs);
                }
                return list.ToArray();
            }
            var childs = new List<Transform>();
            var ignored = bones;
            foreach (var bone in bones)
            {
                childs.AddRange(GetChilds(bone, ignored));
            }
            return childs.ToArray();
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

        static readonly string[] headKeywords = new[] { "head" };
        static readonly string[] neckKeywords = new[] { "neck" };
        static readonly string[] chestKeywords = new[] { "chest" };

        static readonly string[] spineKeywords = new[] { "spine", "ribs" };
        static readonly string[] hipsKeywords = new [] { "hips", "pelvis", "hip" };
        static readonly string[] shoulderKeywords = new[] { "shoulder", "clavicle" };
        static readonly string[] breastKeywords = new[] { "breast", "oppai" };
        static readonly string[] handKeywords = new [] { "hand", "wrist" };
        static readonly string[] footKeywords = new [] { "foot", "ankle" };

        static readonly string[] upperLegKeywords = new [] { "thigh", "upleg" };
        static readonly string[] lowerLegKeywords = new [] { "knee", "calf" };
        static readonly string[] upperArmKeywords = new string[] { };
        static readonly string[] lowerArmKeywords = new [] { "elbow", "forearm" };


        static readonly string[] fingerKeywords = new[] { "thumb", "index", "middle" , "ring", "little" };


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
        /*
        public Transform GetBoneTransform(string name)
        {
            var humanBodyBones = GetHumanBodyBones(muscleName);
            var transforms = GetBoneTransforms();
            head = FindBone(transforms, "head");
        }
        public HumanBodyBones GetHumanBodyBones(string name)
        {
            var lowerName= name.ToLower();
            if (headKeywords.Any(x => lowerName.Contains(x)))
            {
                return HumanBodyBones.Head;
            }
            if (neckKeywords.Any(x => lowerName.Contains(x)))
            {
                return HumanBodyBones.Neck;
            }
            if (chestKeywords.Any(x => lowerName.Contains(x)))
            {
                return HumanBodyBones.Chest;
            }
            if (spineKeywords.Any(x => lowerName.Contains(x)))
            {
                return HumanBodyBones.Spine;
            }
            if (hipsKeywords.Any(x => lowerName.Contains(x)))
            {
                return HumanBodyBones.Hips;
            }




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



















            return (HumanBodyBones)(-1);
        }
        */
        public static HumanBodyBones[] GetFingers()
        {
            var list = new List<HumanBodyBones>();
            foreach (var humanBodyBones in (HumanBodyBones[])System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                var boneName = humanBodyBones.ToString().ToLower();
                if (fingerKeywords.Any(x => boneName.Contains(x)))
                {
                    list.Add(humanBodyBones);
                }
            }
            return list.ToArray();
        }
        public static AvatarMaskBodyPart HumanBodyBonesToAvatarMaskBodyPart(HumanBodyBones humanBodyBones)
        {
            foreach (var avatarMaskBodyPart in (AvatarMaskBodyPart[])System.Enum.GetValues(typeof(AvatarMaskBodyPart)))
            {
                var bones = AvatarMaskBodyPartToHumanBodyBones(avatarMaskBodyPart);
                if (bones == null)
                {
                    continue;
                }
                if (bones.Contains(humanBodyBones)==false)
                {
                    continue;
                }
                return avatarMaskBodyPart;
            }
            return (AvatarMaskBodyPart)(-1);
        }
        public static HumanBodyBones[] AvatarMaskBodyPartToHumanBodyBones(AvatarMaskBodyPart avatarMaskBodyPart)
        {
            var name = avatarMaskBodyPart.ToString().ToLower();


            switch (avatarMaskBodyPart)
            {
                case AvatarMaskBodyPart.Root:
                    break;
                case AvatarMaskBodyPart.Body:
                    return new HumanBodyBones[]
                    {
                    HumanBodyBones.Hips,
                    HumanBodyBones.Spine,
                    HumanBodyBones.Chest,
                    HumanBodyBones.UpperChest,
                    };
                case AvatarMaskBodyPart.Head:
                    return new HumanBodyBones[]
                    {
                    HumanBodyBones.Neck,
                    HumanBodyBones.Head,
                    };
                case AvatarMaskBodyPart.LeftLeg:
                    return new HumanBodyBones[]
                    {
                    HumanBodyBones.LeftUpperLeg,
                    HumanBodyBones.LeftLowerLeg,
                    HumanBodyBones.LeftFoot,
                    HumanBodyBones.LeftToes,
                    };
                case AvatarMaskBodyPart.RightLeg:
                    return new HumanBodyBones[]
                    {
                    HumanBodyBones.RightUpperLeg,
                    HumanBodyBones.RightLowerLeg,
                    HumanBodyBones.RightFoot,
                    HumanBodyBones.RightToes,
                    };
                case AvatarMaskBodyPart.LeftArm:
                    return new HumanBodyBones[]
                    {
                    HumanBodyBones.LeftShoulder,
                    HumanBodyBones.LeftUpperArm,
                    HumanBodyBones.LeftLowerArm,
                    };
                case AvatarMaskBodyPart.RightArm:
                    return new HumanBodyBones[]
                    {
                    HumanBodyBones.RightShoulder,
                    HumanBodyBones.RightUpperArm,
                    HumanBodyBones.RightLowerArm,
                    };
                case AvatarMaskBodyPart.LeftFingers:
                    {
                        var fingers = GetFingers();
                        fingers = GetLefts(fingers);
                        return fingers;
                    }
                case AvatarMaskBodyPart.RightFingers:
                    {
                        var fingers = GetFingers();
                        fingers = GetRights(fingers);
                        return fingers;
                    }
                case AvatarMaskBodyPart.LeftFootIK:
                    break;
                case AvatarMaskBodyPart.RightFootIK:
                    break;
                case AvatarMaskBodyPart.LeftHandIK:
                    break;
                case AvatarMaskBodyPart.RightHandIK:
                    break;
                case AvatarMaskBodyPart.LastBodyPart:
                    break;
                default:
                    break;
            }


            return null;
        }
        public static AvatarMaskBodyPart[] AvatarMaskToAvatarMaskBodyParts(AvatarMask avatarMask)
        {
            var avatarMaskBodyParts = (AvatarMaskBodyPart[])System.Enum.GetValues(typeof(AvatarMaskBodyPart));
            avatarMaskBodyParts=System.Array.FindAll(avatarMaskBodyParts, x =>
            {
                if (x==AvatarMaskBodyPart.LastBodyPart)
                {
                    return false;
                }
                return avatarMask.GetHumanoidBodyPartActive(x);
            });
            return avatarMaskBodyParts.ToArray();
        }
        public static HumanBodyBones[] AvatarMaskToHumanBodyBones(AvatarMask avatarMask)
        {
            var avatarMaskBodyParts = AvatarMaskToAvatarMaskBodyParts(avatarMask);
            var humanBodyBones = new List<HumanBodyBones>();
            foreach (var avatarMaskBodyPart in avatarMaskBodyParts)
            {
                var bones = AvatarMaskBodyPartToHumanBodyBones(avatarMaskBodyPart);
                if (bones == null)
                {
                    continue;
                }
                humanBodyBones.AddRange(bones);
            }
            return humanBodyBones.ToArray();
        }
        public void NameSearch(Transform[] transforms)
        {



            head = FindBone(transforms, headKeywords);
            neck = FindBone(transforms, neckKeywords);
            chest = FindBone(transforms, chestKeywords);

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

                SetFingers(fingerTransforms, ref leftThumbProximal, ref leftThumbIntermediate, ref leftThumbDistal, "thumb");
                SetFingers(fingerTransforms, ref leftIndexProximal, ref leftIndexIntermediate, ref leftIndexDistal, "index");
                SetFingers(fingerTransforms, ref leftMiddleProximal, ref leftMiddleIntermediate, ref leftMiddleDistal, "middle");
                SetFingers(fingerTransforms, ref leftRingProximal, ref leftRingIntermediate, ref leftRingDistal, "ring");
                SetFingers(fingerTransforms, ref leftLittleProximal, ref leftLittleIntermediate, ref leftLittleDistal, "little");
            }

            if (rightHand != null)
            {
                var fingerTransforms = rightHand.GetComponentsInChildren<Transform>(true);

                SetFingers(fingerTransforms, ref rightThumbProximal, ref rightThumbIntermediate, ref rightThumbDistal, "thumb");
                SetFingers(fingerTransforms, ref rightIndexProximal, ref rightIndexIntermediate, ref rightIndexDistal, "index");
                SetFingers(fingerTransforms, ref rightMiddleProximal, ref rightMiddleIntermediate, ref rightMiddleDistal, "middle");
                SetFingers(fingerTransforms, ref rightRingProximal, ref rightRingIntermediate, ref rightRingDistal, "ring");
                SetFingers(fingerTransforms, ref rightLittleProximal, ref rightLittleIntermediate, ref rightLittleDistal, "little");
            }
#endif





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
                if (matchingTransforms.Length==0)
                {
                    matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("intemediate"));
                }
                return matchingTransforms;
            }
            static Transform[] GetDistals(Transform[] transforms)
            {
                var matchingTransforms = System.Array.FindAll(transforms, transform => transform.name.ToLower().Contains("distal"));
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

        static string[] GetLefts(string[] names)
        {

            var matchingNames = System.Array.FindAll(names, name => name.ToLower().Contains("left"));

            var upperKeywords = new string[] { " L ", ".L", "_L" };
            foreach (var keyword in upperKeywords)
            {
                if (matchingNames.Length != 0)
                {
                    break;
                }
                matchingNames = System.Array.FindAll(names, name => name.ToUpper().Contains(keyword));
            }
            return matchingNames;
        }
        static string[] GetRights(string[] names)
        {

            var matchingNames = System.Array.FindAll(names, name => name.ToLower().Contains("right"));

            var upperKeywords = new string[] { " R ", ".R", "_R" };
            foreach (var keyword in upperKeywords)
            {
                if (matchingNames.Length != 0)
                {
                    break;
                }
                matchingNames = System.Array.FindAll(names, name => name.ToUpper().Contains(keyword));
            }
            return matchingNames;
        }
        static HumanBodyBones[] GetLefts(HumanBodyBones[] humanBodyBones)
        {

            var names = System.Array.ConvertAll(humanBodyBones, t => t.ToString());
            var matchingNames = GetLefts(names);
            var matchingBones = System.Array.FindAll(humanBodyBones, x => names.Contains(x.ToString()));
            return matchingBones;
        }
        static HumanBodyBones[] GetRights(HumanBodyBones[] humanBodyBones)
        {

            var names = System.Array.ConvertAll(humanBodyBones, t => t.ToString());
            var matchingNames = GetRights(names);
            var matchingBones = System.Array.FindAll(humanBodyBones, x => names.Contains(x.ToString()));
            return matchingBones;
        }
        Transform[] GetLefts(Transform[] transforms)
        {
            var names= System.Array.ConvertAll(transforms, t => t.name);
            var matchingNames = GetLefts(names);
            var matchingTransforms = System.Array.FindAll(transforms,x=> names.Contains(x.name));
            /*
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
            */
            return matchingTransforms;
        }
        Transform[] GetRights(Transform[] transforms)
        {
            var names = System.Array.ConvertAll(transforms, t => t.name);
            var matchingNames = GetRights(names);
            var matchingTransforms = System.Array.FindAll(transforms, x => names.Contains(x.name));
            /*
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
            */
            return matchingTransforms;
        }
        Transform[] GetBones()
        {
            return GetBoneTransforms();
        }
        public void MirrorX()
        {
            int CountAllChildren(Transform t)
            {
                int count = 0;
                foreach (Transform c in t)
                {
                    count++;
                    count += CountAllChildren(c);
                }
                return count;
            }
            var fields = GetBonesField();

            var dictionary = new Dictionary<Transform, Quaternion>();
            foreach (var field in fields)
            {
                var transform = (Transform)field.GetValue(this);
                if (transform == null)
                {
                    continue;
                }
                if (field.Name == "root")
                {
                    continue;
                }
                dictionary.Add(transform, transform.rotation);
            }
            {
                //부모우선 정렬
                dictionary = dictionary
    .OrderByDescending(t => CountAllChildren(t.Key))
    .ToDictionary(x => x.Key, x => x.Value);
            }
            foreach (var item in dictionary)
            {
                var transform = item.Key;
                var symmetricalTransform = GetSymmetricalTransform(transform);

                var rotation = item.Value;

                if (symmetricalTransform == null)
                {
                    symmetricalTransform=transform;
                }
                var localRotation = Quaternion.Inverse(root.rotation) * rotation;

                localRotation = new Quaternion(
                    // 대칭 회전 처리 (AI)
                    -localRotation.x,
                     localRotation.y,
                     localRotation.z,
                    -localRotation.w
                );
                var newRotation= root.rotation * localRotation;
                Debug.Log($"{symmetricalTransform.name}.rotation:{symmetricalTransform.rotation.eulerAngles}->{newRotation.eulerAngles}");


#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(symmetricalTransform, "X Mirror");
#endif
                symmetricalTransform.rotation = newRotation;


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
                    if (transform == null)
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
                                    // 대칭 회전 처리 (AI)
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
            if (root == null)
            {
                return;
            }
            Update(root.transform);
        }
        public void Update(Transform root)
        {
            Clear();
            if (root == null)
            {
                return;
            }
            this.root = root;


            var animator = root.GetComponent<Animator>();
            if ((animator != null) && (animator.isHuman))
            {
                HumanoidSearch(animator);
            }
            else
            {
                NameSearch(root.GetComponentsInChildren<Transform>(true));
            }

#if USE_BREAST
            if (chest != null)
            {
                var chestChilds = chest.GetComponentsInChildren<Transform>(true);
                var breasts = FindBones(chestChilds, breastKeywords);
                breasts = System.Array.FindAll(breasts, x => x.name.ToLower().Contains("root") == false);//루트는 전부 제거
                leftBreast = GetLefts(breasts).FirstOrDefault();
                rightBreast = GetRights(breasts).FirstOrDefault();
            }


#endif

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

        public AhzkwidHumanoid(Transform root)
        {
            Update(root);
        }

    }

}
