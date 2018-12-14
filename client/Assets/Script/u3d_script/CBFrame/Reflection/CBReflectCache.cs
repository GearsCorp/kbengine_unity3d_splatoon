using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace CBFrame.Reflection
{
    public class CBReflectionCache
    {
        private static Dictionary<Type, List<FieldInfo>> _fieldCache =
                                new Dictionary<Type, List<FieldInfo>>();

        private static Dictionary<Type, List<PropertyInfo>> _propertyCache =
                        new Dictionary<Type, List<PropertyInfo>>();

        private static Dictionary<string, Type> _types = new Dictionary<string, Type>();

        public static Type GetTypeByName(string name)
        {
            return Type.GetType(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">属性类型</typeparam>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static List<FieldInfo> GetFieldsByType(Type type)
        {
            if (!_fieldCache.ContainsKey(type))
            {
                Cache(type);
            }

            return _fieldCache[type];
        }

        public static List<PropertyInfo> GetPropertiesByType(Type type)
        {
            if (!_propertyCache.ContainsKey(type))
            {
                Cache(type);
            }

            return _propertyCache[type];
        }

        public static FieldInfo GetFieldByName(Type type, string name)
        {
            var fields = GetFieldsByType(type);

            FieldInfo field = fields.Find((item) =>
            {
                return item.Name == name;
            });

            return field;
        }

        public static PropertyInfo GetPropertyByName(Type type, string name)
        {
            var Properties = GetPropertiesByType(type);

            PropertyInfo property = Properties.Find((item) =>
            {
                return item.Name == name;
            });

            return property;
        }

        public static MethodInfo GetMethodByName(Type type, string name)
        {
            return type.GetMethod(name);
        }

        private static void Cache(Type type)
        {
            var fieldCache = new List<FieldInfo>();
            var propertyCache = new List<PropertyInfo>();
            var fields = type.GetFields(BindingFlags.Instance |
                                        BindingFlags.IgnoreCase |
                                        BindingFlags.GetField |
                                        BindingFlags.SetField |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public);

            var properties = type.GetProperties(BindingFlags.Instance |
                                        BindingFlags.IgnoreCase |
                                        BindingFlags.GetField |
                                        BindingFlags.SetField |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public);

            for (int i = 0; i < fields.Length; i++)
            {
                fieldCache.Add(fields[i]);
            }

            for (int i = 0; i < properties.Length; i++)
            {
                propertyCache.Add(properties[i]);
            }

            _fieldCache.Add(type, fieldCache);
            _propertyCache.Add(type, propertyCache);
        }

        public static object CreateInstance(string assemblyName, string name)
        {
            string fullName = assemblyName + name;
            object instance = null;

            if (_types.ContainsKey(fullName))
            {
                instance = Activator.CreateInstance(_types[fullName]);
            }
            else
            {
                var wrapper = Activator.CreateInstance(assemblyName, name);
                instance = wrapper.Unwrap();
            }

            return instance;
        }

        public static T CreateInstance<T>()
        {
            return Activator.CreateInstance<T>();
        }
    }
}