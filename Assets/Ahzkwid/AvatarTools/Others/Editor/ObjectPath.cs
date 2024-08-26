
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Search;
using UnityEngine;

namespace Ahzkwid
{
    public class ObjectPath
    {
        public static string GetPath(Transform target, Transform root = null)
        {
            var rootPath = "";
            var hierarchyPath = "";


            if (root != null)
            {
                rootPath = SearchUtils.GetHierarchyPath(root.gameObject, false);
                /*
                if (rootPath.Length>0)
                {
                    rootPath = rootPath.Substring(1);
                }
                */
            }

            //hierarchyPath = bone.GetHierarchyPath();
            hierarchyPath = SearchUtils.GetHierarchyPath(target.gameObject, false);
            return System.IO.Path.GetRelativePath(rootPath,hierarchyPath).Replace("\\","/");

        }
        public static string Join(string parentPath, string childPath)
        {
            return System.IO.Path.Join(parentPath, childPath).Replace("\\", "/");
        }
        public static Transform Find(string path,Transform root=null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return root;
            }
            if (root == null)
            {
                return GameObject.Find(path)?.transform;
            }

            return root.Find(path);
            //var childrens = root.GetComponentsInChildren<Transform>(true);
            //return System.Array.Find(childrens, x => path == GetPath(x, root));
        }
        public enum VRCRootSearchOption
        {
            IncludeVRCRoot, // VRC Root Æ÷ÇÔ
            VRCRootOnly     // VRC Root¸¸
        }
        public static Transform GetVRCRoot(Transform transform, VRCRootSearchOption vrcRootSearchOption= VRCRootSearchOption.IncludeVRCRoot)
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
                    if (type==null)
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
        public static void ComponentsCopy(Transform from, Transform to, System.Type[] types = null)
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

                if (relativePath == null)
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


                var equalTransform = EqualTransform(from,to,component.transform);

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

        public static void RepathComponents(Transform from, Transform to, System.Type[] types = null)
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

                if (types != null)
                {
                    if (System.Array.FindIndex(types, x => x == component.GetType()) < 0)
                    {
                        continue;
                    }
                }

                ObjectPath.RepathFields(from, to, component, component.GetType().GetFields());


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
            var bones = System.Array.ConvertAll(transforms, x => EqualTransform(from, to, x));
            return System.Array.FindAll(bones, x => x != null);
        }
        public static Transform EqualTransform(Transform from, Transform to, Transform transform)
        {
            var path = GetPath(transform, from);
            return Find(path, to);
        }
        public static Component EqualComponent(Transform from, Transform to, Component component)
        {

            var equalTransform = EqualTransform(from,to,component.transform);
            if (equalTransform == null)
            {
                return null;
            }
            return equalTransform.GetComponent(component.GetType());

        }
        public static void RepathFields(Transform from, Transform to, object target, FieldInfo[] fields)
        {
            bool IsArray(System.Type type)
            {
                return (type.IsArray) || ((type.IsGenericType) && type.GetGenericTypeDefinition() == typeof(List<>));
            }
            bool IsClass(System.Type type)
            {
                return type.IsClass && (type.IsArray == false) && (type.Equals(typeof(string)) == false);
            }
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


            Debug.Log("RepathFields");
            foreach (var field in fields)
            {

                var value = field.GetValue(target);
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
                    Debug.Log($"RepathFields.IsArray : {target}.{field.Name}");
                    var ilist = (System.Collections.IList)value;
                    for (int i = 0; i < ilist.Count; i++)
                    {
                        if (IsClass(ilist[i].GetType()))
                        {
                            Debug.Log($"RepathFields.IsArray.IsClass {ilist[i]}");
                            var subFields = ilist[i].GetType().GetFields();
                            RepathFields(from,to,ilist[i], subFields);
                        }
                        else
                        {
                            var item = ilist[i];

                            var transform = GetTransform(item);
                            /*
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
                            */
                            if (transform == null)
                            {
                                continue;
                            }
                            //Debug.Log($"{component.transform.name}.{component.name}.{field.Name}.{transform}");
                            Debug.Log($"{target}.{field.Name}.{transform}");
                            Debug.Log($"GetComponent:{transform.GetComponent(item.GetType())}");
                            var equalTransform = EqualTransform(from,to,transform);
                            Debug.Log($"{ilist[i]}:{transform.GetComponent(item.GetType())}");
                            ilist[i] = equalTransform.GetComponent(item.GetType());
                        }
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
                    if (value is Component component)
                    {
                        //var transform = GetTransform(value);
                        var equalComponent = EqualComponent(from, to, component);
                        field.SetValue(target, equalComponent);

                        Debug.Log($"{target}.{field.Name}.{value}\n{SearchUtils.GetHierarchyPath(component?.gameObject, false)}->{SearchUtils.GetHierarchyPath(equalComponent?.gameObject, false)}");
                    }
                }

            }
        }
    }
}
#endif