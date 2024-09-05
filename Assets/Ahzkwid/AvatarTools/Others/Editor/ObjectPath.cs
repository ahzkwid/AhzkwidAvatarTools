
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ahzkwid
{
    public class ObjectPath
    {
        public static bool IsChild(Transform target, Transform root)
        {
            if (root==null)
            {
                return false;
            }


            var rootPath = GetPath(root);
            var hierarchyPath = GetPath(target);


            if (rootPath.Length > 0)
            {
                if (hierarchyPath.StartsWith(rootPath))
                {
                    return true;
                }

            }
            return false;
        }
        public static string GetPath(Transform target, Transform root = null)
        {
            var rootPath = "";
            var hierarchyPath = "";


            if (root != null)
            {
                rootPath = SearchUtils.GetHierarchyPath(root.gameObject, false).Replace("\\", "/");
                /*
                if (rootPath.Length>0)
                {
                    rootPath = rootPath.Substring(1);
                }
                */
            }

            //hierarchyPath = bone.GetHierarchyPath();
            if (target == null)
            {
                return null;
            }
            hierarchyPath = SearchUtils.GetHierarchyPath(target.gameObject, false).Replace("\\", "/");


            if (rootPath.Length > 0)
            {
                if (IsChild(target, root))
                {
                    var startIndex = rootPath.Length+1;
                    if (startIndex > hierarchyPath.Length)
                    {
                        return null;
                    }
                    return hierarchyPath.Substring(startIndex, hierarchyPath.Length - startIndex);
                }
                return null;
            }
            //if (rootPath.Length > 0)
            //{
            //    /*
                 
            //    /Root/Child
            //    /Root
            //    Root
            //    Root/Child

            //    */

            //    {
            //        /*
            //        if (hierarchyPath.StartsWith(rootPath))
            //        {
            //            var startIndex = rootPath.Length;
            //            return hierarchyPath.Substring(startIndex, hierarchyPath.Length - startIndex);

            //            //return System.IO.Path.GetRelativePath(rootPath, hierarchyPath).Replace("\\", "/");
            //        }
            //        */
            //        var hierarchyPathSlash = hierarchyPath;
            //        var rootPathSlash = rootPath;
            //        /*
            //        if (rootPath.StartsWith("/") != hierarchyPath.StartsWith("/"))
            //        {
            //            if (rootPathSlash.StartsWith("/") == false)
            //            {
            //                rootPathSlash = "/" + rootPath;
            //            }
            //            if (hierarchyPathSlash.StartsWith("/") == false)
            //            {
            //                hierarchyPathSlash = "/" + hierarchyPath;
            //            }
            //        }
            //        */
            //        if (hierarchyPathSlash.StartsWith(rootPathSlash))
            //        {
            //            var startIndex = rootPathSlash.Length;
            //            return hierarchyPathSlash.Substring(startIndex, hierarchyPathSlash.Length - startIndex);

            //            //return System.IO.Path.GetRelativePath(rootPath, hierarchyPath).Replace("\\", "/");
            //        }


            //        //Debug.LogError($"RelativePathFail\n{hierarchyPath}\n{rootPath}");
            //        return null;


            //    }




            //    /*

            //    try
            //    {
            //        //한국어 미지원
            //        return System.IO.Path.GetRelativePath(rootPath, hierarchyPath).Replace("\\", "/");
            //    }
            //    catch (System.Exception ex)
            //    {
            //        Debug.LogError(ex);
            //        Debug.LogError($"{hierarchyPath} - {rootPath}");
            //        throw;
            //    }
            //    */
            //}
            return hierarchyPath;

        }
        public static Transform GetArmature(GameObject gameObject)
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

        public static string Join(string parentPath, string childPath)
        {
            return System.IO.Path.Join(parentPath, childPath).Replace("\\", "/");
        }
        public static Transform Find(string path, Transform root = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return root;
            }
            if (root == null)
            {
                return GameObject.Find(path)?.transform;
            }

            //return GameObject.Find(Join(GetPath(root), path))?.transform;


            return root.Find(path)?.transform;


            /*
            var transform = root.Find(path);
            if (transform==null)
            {
                transform = root.Find(Join(GetPath(root), path));
            }
            var transform = root.Find(Join(GetPath(root), path));

            return transform;
            */
            //var childrens = root.GetComponentsInChildren<Transform>(true);
            //return System.Array.Find(childrens, x => path == GetPath(x, root));
        }
        public enum VRCRootSearchOption
        {
            IncludeVRCRoot, // VRC Root 포함
            VRCRootOnly     // VRC Root만
        }


        public static List<Transform> GetRoots(VRCRootSearchOption vrcRootSearchOption = VRCRootSearchOption.IncludeVRCRoot)
        {
            var targetTransforms = new List<Transform>();
            var activeScene = SceneManager.GetActiveScene();
            if (activeScene != null)
            {
                var gameObjects = activeScene.GetRootGameObjects().ToList();
                gameObjects = gameObjects.FindAll(x => x.activeInHierarchy);
                targetTransforms = gameObjects.ConvertAll(x => x.transform);
                targetTransforms = targetTransforms.FindAll(x => GetVRCRoot(x, vrcRootSearchOption));
                gameObjects = gameObjects.FindAll(x => x != null);
            }
            return targetTransforms;
        }

        public static Transform GetVRCRoot(Transform transform, VRCRootSearchOption vrcRootSearchOption = VRCRootSearchOption.IncludeVRCRoot)
        {
            var root = transform.root;
            if (root == null)
            {
                return null;
            }
            {
                //var avatarDescriptor = root.GetComponentInChildren<VRCAvatarDescriptor>(true);
                //var components = root.GetComponentsInChildren<Component>(true);
                var components = transform.GetComponentsInParent<Component>(true);
                var avatarDescriptor = System.Array.Find(components, x =>
                {
                    if (x == null)
                    {
                        return false;
                    }
                    var type = x.GetType();
                    if (type == null)
                    {
                        return false;
                    }
                    var name = type.Name;
                    return name.Contains("AvatarDescriptor");
                });
                if (avatarDescriptor == null)
                {
                    switch (vrcRootSearchOption)
                    {
                        case VRCRootSearchOption.IncludeVRCRoot:
                            return root;
                        case VRCRootSearchOption.VRCRootOnly:
                            return null;
                        default:
                            break;
                    }
                }
                root = avatarDescriptor.transform;
            }
            return root;
        }
        /// <summary>
        /// 이 함수는 루트도 복사해버리는 결함이 있음
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="types"></param>
         static void ComponentsCopy(Transform from, Transform to, System.Type[] types = null)
        {
            // var allPathFrom = new AllPath(from);
            //var allPathTo = new AllPath(to);


            var fromComponents = from.GetComponentsInChildren<Component>(true);
            var toComponents = to.GetComponentsInChildren<Component>(true);
            /*
            Transform EqualTransform(Transform transform)
            {

                var path = allPathFrom.GetPath(transform);
                return allPathTo.GetTransform(path);

            }
            Transform EqualTransform(Transform transform)
            {
                var path = GetPath(transform, from);
                return Find(path, to);
            }
            */
            foreach (var component in fromComponents)
            {
                if (component == null)
                {
                    continue;
                }
                if (component is Transform)
                {
                    continue;
                }

                if (types != null)
                {
                    if (System.Array.FindIndex(types, x => x == component.GetType()) < 0)
                    {
                        continue;
                    }
                }






                var relativePath = ObjectPath.GetPath(component.transform, from.transform);

                //if (relativePath == null)
                if (string.IsNullOrWhiteSpace(relativePath))
                {
                    continue;
                }


                var equalTypeComponents = System.Array.FindAll(toComponents, x => x.GetType() == component.GetType());
                if (System.Array.FindIndex(equalTypeComponents, x => {
                    var characterRelativePath = ObjectPath.GetPath(to.transform, x.transform);
                    if (characterRelativePath == null)
                    {
                        return false;
                    }
                    return relativePath == characterRelativePath;
                }) >= 0)
                {
                    continue;
                }


                var equalTransform = EqualTransform(from, to, component.transform);

                if (equalTransform == null)
                {
                    equalTransform = Object.Instantiate(component.gameObject).transform;
                    equalTransform.name = component.gameObject.name;
                    var equalTransformParent = EqualTransform(from, to, component.transform.parent);
                    if (equalTransformParent != null)
                    {
                        equalTransform.parent = equalTransformParent;
                    }
                    continue;
                }

                var newComponent = equalTransform.gameObject.AddComponent(component.GetType());


                foreach (var field in component.GetType().GetFields())
                {
                    var value = field.GetValue(component);
                    field.SetValue(newComponent, value);
                }

            }

            ObjectPath.RepathComponents(from.transform, to.transform, types);

        }

        public static void RepathComponents(Transform from, Transform to, System.Type[] whiteList = null)
        {
            //var allPathFrom = new AllPath(from.gameObject);
            //var allPathTo = new AllPath(to.gameObject);
            /*
            Transform EqualTransform(Transform transform)
            {

                var path = allPathFrom.GetPath(transform);
                return allPathTo.GetTransform(path);

            }
            Transform EqualTransform(Transform transform)
            {
                var path = GetPath(transform, from);
                return Find(path, to);
            }
            Component EqualComponent(Component component)
            {

                var equalTransform = EqualTransform(component.transform);
                if (equalTransform == null)
                {
                    return null;
                }
                return equalTransform.GetComponent(component.GetType());

            }
            */
            var components = to.GetComponentsInChildren<Component>(true);
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

                if (whiteList != null)
                {
                    if (System.Array.FindIndex(whiteList, x => x == component.GetType()) < 0)
                    {
                        continue;
                    }
                }

                Debug.Log($"RepathComponent {component}");
                ObjectPath.RepathFields(from, to, component, component.GetType());


                //Debug.Log($"{component.GetType()} {component.name}");
                /*
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
                    if (IsArray(field.FieldType))
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
                            var equalTransform = EqualTransform(transform);
                            Debug.Log($"{ilist[i]}:{transform.GetComponent(item.GetType())}");
                            ilist[i] = equalTransform.GetComponent(item.GetType());
                        }
                        field.SetValue(component, ilist);

                    }
                    else if (IsClass(field.FieldType))
                    {
                        var subFields = field.FieldType.GetFields();
                        foreach (var subField in subFields)
                        {
                            subField.GetValue(value);
                        }

                    }
                    else
                    {


                        if (field.FieldType == typeof(Component))
                        {
                            var transform = value as Transform;
                            var equalComponent= EqualComponent(transform);
                            field.SetValue(component, equalComponent);
                            Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{value}\n{transform}->{equalComponent}");
                            continue;
                        }
                        
                        //if (field.FieldType == typeof(Transform))
                        //{
                        //    var transform = value as Transform;
                        //    var equalTransform = EqualTransform(transform);
                        //    field.SetValue(component, equalTransform);
                        //    Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{value}\n{transform}->{equalTransform}");
                        //    continue;
                        //}
                        
                    }
                    //Debug.Log($"{component.name}.{field.Name}");
                }
                */
            }

        }
        public static Transform[] EqualTransforms(Transform from, Transform to, Transform[] transforms)
        {
            var bones = System.Array.ConvertAll(transforms, x => 
            {
                var equalTransform = EqualTransform(from, to, x);
                if (equalTransform==null)
                {
                    return x;
                }
                return equalTransform;
            });
            //return System.Array.FindAll(bones, x => x != null);
            return bones;
        }
        public static Transform EqualTransform(Transform from, Transform to, Transform transform)
        {
            var relativePath = GetPath(transform, from);
            if (relativePath==null)
            {
                return null;
            }
            return Find(relativePath, to);
        }
        public static Component EqualComponent(Transform from, Transform to, Component component)
        {

            var equalTransform = EqualTransform(from, to, component.transform);
            if (equalTransform == null)
            {
                return null;
            }
            return equalTransform.GetComponent(component.GetType());

        }
        public static void RepathFields(Transform from, Transform to, object target, System.Type targetType)
        {
            // FieldInfo[] fields
            bool IsArray(System.Type type)
            {
                return (type.IsArray) || ((type.IsGenericType) && type.GetGenericTypeDefinition() == typeof(List<>));
            }
            bool IsClass(System.Type type)
            {
                return type.IsClass && (type.IsArray == false) && (type.Equals(typeof(string)) == false);
            }
            var blacklist = new List<System.Type> { typeof(Material), typeof(Material[]), typeof(Mesh) };
            Transform GetTransform(object value)
            {

                var transform = value as Transform;
                if (transform == null)
                {
                    var property = value.GetType().GetProperty("transform");
                    if (property == null)
                    {
                        return null;
                    }
                    transform = property.GetValue(value) as Transform;
                }
                return transform;

            }

            var fields = targetType.GetFields();
            var properties = targetType.GetProperties();


            foreach (var property in properties)
            {
                Debug.Log($"RepathFields.property: {target}.{property.Name}({property.PropertyType})");
                if (System.Attribute.IsDefined(property, typeof(System.ObsoleteAttribute)))
                {
                    continue;
                }
                if (property.CanRead == false)
                {
                    continue;
                }
                if (property.CanWrite == false)
                {
                    continue;
                }
                if (blacklist.Find(x => x == property.PropertyType) != null)
                {
                    continue;
                }
                object value = null;
                value = property.GetValue(target);

                if (value == null)
                {
                    continue;
                }
                if (value.Equals(null))
                {
                    continue;
                }
                if (IsArray(value.GetType()))
                {
                    Debug.Log($"RepathFields.property.IsArray : {target}.{property.Name}");
                    var ilist = (System.Collections.IList)value;
                    for (int i = 0; i < ilist.Count; i++)
                    {
                        var item = ilist[i];
                        if (ilist[i] is Component component)
                        {
                            Debug.Log($"RepathFields.IsArray.IsComponent {ilist[i]}");
                            var equalComponent = EqualComponent(from, to, component);
                            if (equalComponent == null)
                            {
                                continue;
                            }
                            if (GetTransform(equalComponent) == to)
                            {
                                continue;
                            }
                            ilist[i] = equalComponent;
                            continue;
                        }
                        if (IsClass(ilist[i].GetType()))
                        {
                            Debug.Log($"RepathFields.IsArray.IsClass {ilist[i]}");
                            //var subFields = ilist[i].GetType().GetFields();
                            RepathFields(from, to, ilist[i], ilist[i].GetType());

                            continue;
                        }
                    }
                    property.SetValue(target, ilist);

                }
                else
                {
                    if (value is Component component)
                    {
                        var equalComponent = EqualComponent(from, to, component);
                        if (equalComponent == null)
                        {
                            continue;
                        }
                        if (GetTransform(equalComponent) == to)
                        {
                            continue;
                        }
                        Debug.Log($"RepathFields.property.IsNotArray : {target}.{property.Name} \n{component} -> {equalComponent}");
                        property.SetValue(target, equalComponent);
                    }
                }
                /*
                if (value is Transform[])
                {
                    Debug.Log($"RepathFields.IsTransform[] : {target}.{property.Name}");
                    var transforms = (Transform[])value;
                    property.SetValue(target, EqualTransforms(from, to, transforms));
                    continue;
                }
                */
            }

            Debug.Log($"RepathFields {target}.{fields.Length}");
            foreach (var field in fields)
            {
                if (blacklist.Find(x => x == field.FieldType) != null)
                {
                    continue;
                }

                var value = field.GetValue(target);
                if (value == null)
                {
                    continue;
                }
                if (value.Equals(null))
                {
                    continue;
                }
                Debug.Log($"{target}.{field.Name}");
                if (IsArray(value.GetType()))
                {
                    Debug.Log($"RepathFields.IsArray : {target}.{field.Name}");
                    var ilist = (System.Collections.IList)value;
                    for (int i = 0; i < ilist.Count; i++)
                    {
                        var item = ilist[i];
                        if (item == null)
                        {
                            continue;
                        }
                        if (item.Equals(null))
                        {
                            continue;
                        }
                        if (ilist[i] is Component component)
                        {
                            Debug.Log($"RepathFields.IsArray.IsComponent {ilist[i]}");
                            var equalComponent = EqualComponent(from, to, component);
                            if (equalComponent == null)
                            {
                                continue;
                            }
                            if (GetTransform(equalComponent) == to)
                            {
                                continue;
                            }
                            ilist[i] = equalComponent;
                            continue;
                        }
                        else if (IsClass(ilist[i].GetType()))
                        {
                            Debug.Log($"RepathFields.IsArray.IsClass {ilist[i]}");
                            //var subFields = ilist[i].GetType().GetFields();
                            RepathFields(from, to, ilist[i], ilist[i].GetType());

                        }
                        //else
                        //{
                        //    var item = ilist[i];

                        //    var transform = GetTransform(item);
                        //    /*
                        //    var transform = item as Transform;
                        //    if (transform == null)
                        //    {
                        //        var property = item.GetType().GetProperty("transform");
                        //        if (property == null)
                        //        {
                        //            continue;
                        //        }
                        //        transform = property.GetValue(item) as Transform;
                        //    }
                        //    */
                        //    if (transform == null)
                        //    {
                        //        continue;
                        //    }
                        //    //Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{transform}");
                        //    Debug.Log($"{target}.{field.Name}.{transform}");
                        //    //Debug.Log($"GetComponent:{transform.GetComponent(item.GetType())}");
                        //    var equalTransform = EqualTransform(from, to, transform);
                        //    //var equalComponent = EqualComponent(from, to, transform);
                        //    //Debug.Log($"{ilist[i]}:{transform.GetComponent(item.GetType())}");
                        //    if (equalTransform == null)
                        //    {
                        //        continue;
                        //    }
                        //    //ilist[i] = equalComponent;
                        //    ilist[i] = equalTransform.GetComponent(item.GetType());

                        //    /*
                        //    var getComponentMethod = typeof(Transform).GetMethod("GetComponent", new System.Type[] { });
                        //    var genericMethod = getComponentMethod.MakeGenericMethod(item.GetType());
                        //    var component = genericMethod.Invoke(equalTransform, null);
                        //    ilist[i] = component;
                        //    */
                        //    //ilist[i] = equalTransform.GetComponent(item.GetType());
                        //}
                    }
                    field.SetValue(target, ilist);

                }
                /*
                else if (IsClass(value.GetType()))
                {
                    Debug.Log($"RepathFields.IsClass : {target}.{field.Name}.{value.GetType()}");

                    var subFields = value.GetType().GetFields();
                    RepathFields(value,subFields);

                }
                */
                else
                {
                    Debug.Log($"RepathFields.Normal : {target}.{field.Name}.{value.GetType()}");
                    /*
                    {
                        if (value is Transform transform)
                        {
                            if (transform == null)
                            {
                                continue;
                            }
                            var equalTransform = EqualTransform(from, to, transform);
                            if (equalTransform == null)
                            {
                                continue;
                            }
                            field.SetValue(target, equalTransform);
                            continue;
                        }
                    }
                    */
                    if (value is Component component)
                    {
                        //var transform = GetTransform(value);
                        var equalComponent = EqualComponent(from, to, component);
                        if (equalComponent == null)
                        {
                            continue;
                        }
                        if (GetTransform(equalComponent) == to)
                        {
                            continue;
                        }
                        Debug.Log($"RepathFields.field.IsNotArray : {target}.{field.Name} \n{component} -> {equalComponent}");
                        field.SetValue(target, equalComponent);
                        //Debug.Log($"{target}.{field.Name}.{value}\n{SearchUtils.GetHierarchyPath(component?.gameObject, false)}->{SearchUtils.GetHierarchyPath(equalComponent?.gameObject, false)}");
                    }
                }

            }
        }
    }
}
#endif