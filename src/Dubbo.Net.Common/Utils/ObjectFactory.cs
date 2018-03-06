using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dubbo.Net.Common.Utils
{
    /// <summary>
    /// 对象工厂
    /// </summary>
    public class ObjectFactory
    {
        /// <summary>
        /// 实例缓存
        /// </summary>
        private static Dictionary<Type, object> _instance = new Dictionary<Type, object>();
        private static object _syncObj = new object();
        /// <summary>
        /// 类型缓存
        /// </summary>
        private static Dictionary<Type, Type> _typeMap = new Dictionary<Type, Type>();

        /// <summary>
        /// 带Key的类型缓存
        /// </summary>
        private static ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>> _keyMap =
            new ConcurrentDictionary<Type, ConcurrentDictionary<string, Type>>();
        /// <summary>
        /// 注册接口实现类型
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="impType"></param>
        public static void Register(string interfaceType, string impType)
        {
            Type key = Type.GetType(interfaceType);
            Type value = Type.GetType(impType);
            Register(key, value);
        }
        public static void Register(string interfaceType, string impType,object key)
        {
            if (key == null)
            {
                Register(interfaceType,impType);
                return;
            }
            Type iType = Type.GetType(interfaceType);
            Type impl = Type.GetType(impType);
            Register(iType, impl);
        }
        public static void Register<TInterface, TImplement>(object key) where TImplement : TInterface
        {
            Type iType = typeof(TInterface);
            Type impl = typeof(TImplement);
            if (key == null)
            {
                Register<TInterface,TImplement>();
                return;
            }

            Register(iType, impl, key);
        }
        public static void Register<TInterface, TImplement>() where TImplement:TInterface
        {
            Type key = typeof(TInterface);
            Type value = typeof(TImplement);
            if (!_typeMap.ContainsKey(key))
            {
                lock (_typeMap)
                {
                    if (!_typeMap.ContainsKey(key))
                    {
                        _typeMap.Add(key, value);
                    }
                }
            }
        }

        public static void Register(Type interfaceType, Type impType)
        {
            Type key = interfaceType;
            Type value = impType;
            if (interfaceType == null || impType == null)
                throw new ArgumentNullException();
            if (!interfaceType.IsAssignableFrom(impType))
                throw new ArgumentException("interfaceType type is not assignable from impType");
            if (!_typeMap.ContainsKey(key))
            {
                lock (_typeMap)
                {
                    if (!_typeMap.ContainsKey(key))
                    {
                        _typeMap.Add(key, value);
                    }
                }
            }
        }

        public static void Register(Type implType, object impl)
        {
            if(_instance.ContainsKey(implType))
                return;
            lock (_instance)
            {
                if(_instance.ContainsKey(implType))
                    return;
                _instance.Add(implType, impl);
            }
        }
        public static void Register(Type interfaceType, Type impType,object key)
        {
            if (key == null)
            {
                Register(interfaceType, impType);
                return;
            }
            if (interfaceType == null || impType == null)
                throw new ArgumentNullException();
            if (!interfaceType.IsAssignableFrom(impType))
                throw new ArgumentException("interfaceType type is not assignable from impType");
            var keyStr = key.ToString();
            if (!_keyMap.TryGetValue(interfaceType, out var value))
            {
                value=new ConcurrentDictionary<string, Type>();
                _keyMap.TryAdd(interfaceType, value);
            }
            if(value.ContainsKey(keyStr))
                return;
            value.TryAdd(keyStr, impType);
            //if (!_keyMap.Any(c => c.Key == keyStr&&c.InterfaceType==interfaceType))
            //{
            //    lock (_keyMap)
            //    {
            //        if (!_keyMap.Any(c => c.Key == keyStr && c.InterfaceType == interfaceType))
            //        {
            //            var info = new TypeRegisterInfo { InstanceType = impl, Key = keyStr,InterfaceType = iType};
            //            _keyMap.Add(info);
            //        }
            //    }
            //}
        }
        public static T GetInstance<T>()
        {
            Type tType = typeof(T);
            return (T)GetInstance(tType);
        }
        public static T GetInstance<T>(object key)
        {
            Type tType = typeof(T);
            return (T)GetInstance(tType,key);
        }
        public static object GetInstance(string type)
        {
            if (string.IsNullOrEmpty(type))
                return null;
            return GetInstance(Type.GetType(type));
        }
        public static object GetInstance(string type,object key)
        {
            if (string.IsNullOrEmpty(type))
                return null;
            return GetInstance(Type.GetType(type),key);
        }
        /// <summary>
        /// 获取对象实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetInstance(Type type)
        {
            if (type == null)
                return null;
            Type instanceType = GetInstanceType(type);
            if (instanceType == null)
            {
                throw new Exception(string.Format("未找到{0}的实现类", type.FullName));
            }
            if (!_instance.ContainsKey(instanceType))
            {
                lock (_syncObj)
                {
                    if (!_instance.ContainsKey(instanceType))
                    {
                        object instance = CreateInstance(instanceType);
                        _instance.Add(instanceType, instance);
                    }
                }
            }
            return _instance[instanceType];
        }
        public static object GetInstance(Type type,object key)
        {
            if (type == null)
                return null;
            Type instanceType = GetInstanceType(type,key);
            if (instanceType == null)
            {
                throw new Exception(string.Format("未找到{0}的实现类", type.FullName));
            }
            if (!_instance.ContainsKey(instanceType))
            {
                lock (_syncObj)
                {
                    if (!_instance.ContainsKey(instanceType))
                    {
                        object instance = CreateInstance(instanceType);
                        _instance.Add(instanceType, instance);
                    }
                }
            }
            return _instance[instanceType];
        }

        public static List<string> GetTypeKeys<T>()
        {
            var type = typeof(T);
            if(!_keyMap.TryGetValue(type,out var dic))
                return new List<string>();
            return dic.Keys.ToList();
        }
        private static Type GetInstanceType(Type type)
        {
            if (type.IsInterface)
            {
                if (_typeMap.ContainsKey(type))
                {
                    return _typeMap[type];
                }
                return null;
            }
            else
            {
                return type;
            }
        }
        private static Type GetInstanceType(Type type,object key)
        {
            if (key == null)
            {
                return GetInstanceType(type);
            }
            if (type.IsInterface)
            {
                var keyStr = key.ToString();
                if (!_keyMap.TryGetValue(type, out var dic))
                    return null;
                if (dic.TryGetValue(keyStr, out var value))
                    return value;
                return null;
            }
            else
            {
                return type;
            }
        }
        private static object CreateInstance(Type type)
        {
            Type singletonType = Type.GetType("Dubbo.Net.Common.Utils.SingleInstance`1,Dubbo.Net.Common");
            Type instanceSingletonType = singletonType.MakeGenericType(type);
            return instanceSingletonType.GetProperty("Instance").GetValue(null);
        }
        /// <summary>
        /// 带Key的类型注册信息
        /// </summary>
        class TypeRegisterInfo
        {
            public Type InterfaceType { get; set; }
            /// <summary>
            /// Key
            /// </summary>
            public string Key { get; set; }
            /// <summary>
            /// 实例类型
            /// </summary>
            public Type InstanceType { get; set; }
        }
    }
}
