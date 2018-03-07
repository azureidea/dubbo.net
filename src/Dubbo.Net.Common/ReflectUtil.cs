using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dubbo.Net.Common.Attributes;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Common
{
    public static class ReflectUtil
    {
        /**
    * void(V).
    */
        public const char JvmVoid = 'V';

        /**
         * boolean(Z).
         */
        public const char JvmBoolean = 'Z';

        /**
         * byte(B).
         */
        public const char JvmByte = 'B';

        /**
         * char(C).
         */
        public const char JvmChar = 'C';

        /**
         * double(D).
         */
        public const char JvmDouble = 'D';

        /**
         * float(F).
         */
        public const char JvmFloat = 'F';

        /**
         * int(I).
         */
        public const char JvmInt = 'I';

        /**
         * long(J).
         */
        public const char JvmLong = 'J';

        /**
         * short(S).
         */
        public const char JvmShort = 'S';
        public static readonly Type[] EmptyClassArray = new Type[0];

        private static readonly ConcurrentDictionary<string, Type> DescType = new ConcurrentDictionary<string, Type>();
        private static readonly ConcurrentDictionary<Type,string> TypeDesc=new ConcurrentDictionary<Type, string>();
        private static readonly ConcurrentDictionary<string, string> JavaNameCache = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> CsNameCache = new ConcurrentDictionary<string, string>();


        /// <summary>
        /// 获取java所需要的方法名
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static string GetMethodName(MethodInfo method)
        {
            var name = method.DeclaringType.FullName + "." + method.Name;
            if (JavaNameCache.TryGetValue(name, out var value))
                return value;
            var attr = method.GetCustomAttributes<ReferAttribute>().FirstOrDefault();
            var javaName = "";
            javaName = attr == null ? method.Name : attr.Name;
            JavaNameCache.TryAdd(name, javaName);
            CsNameCache.TryAdd(javaName, method.Name);
            return javaName;
        }
        /// <summary>
        /// 获取C#方法名
        /// </summary>
        /// <param name="javaName"></param>
        /// <returns></returns>
        public static string GetCsMethodName(string javaName)
        {
            if (CsNameCache.TryGetValue(javaName, out var csName))
                return csName;
            return "";
        }
        /// <summary>
        /// 获取java所需要的方法名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetJavaTypeName(Type type)
        {
            var name = type.FullName ;
            if (JavaNameCache.TryGetValue(name, out var value))
                return value;
            var attr = type.GetCustomAttributes<ReferAttribute>().FirstOrDefault();
            var javaName = "";
            javaName = attr == null ? type.Name : attr.Name;
            JavaNameCache.TryAdd(name, javaName);
            CsNameCache.TryAdd(javaName, type.Name);
            return javaName;
        }
        public static string GetDesc(Type[] cs)
        {
            if (cs.Length == 0)
                return "";
            StringBuilder sb = new StringBuilder(64);
            foreach (var type in cs)
            {
                sb.Append(GetDesc(type));
            }
            return sb.ToString();
        }
        public static string GetDesc(Type c)
        {
            if (TypeDesc.TryGetValue(c, out var value))
                return value;
            StringBuilder ret = new StringBuilder();

            while (c.IsArray)
            {
                ret.Append('[');
                c = c.GetElementType();
            }

            if (c.IsPrimitive)
            {
                string t = c.Name;
                if ("void".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmVoid);
                else if ("bool".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmBoolean);
                else if ("byte".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmByte);
                else if ("char".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmChar);
                else if ("double".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmDouble);
                else if ("float".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmFloat);
                else if ("Int32".Equals(t,StringComparison.OrdinalIgnoreCase)) ret.Append(JvmInt);
                else if ("Int64".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmLong);
                else if ("short".Equals(t, StringComparison.OrdinalIgnoreCase)) ret.Append(JvmShort);
            }
            else
            {
                ret.Append('L');
                var javaAttr = c.GetCustomAttributes(typeof(ReferAttribute), false).FirstOrDefault();
                var name = javaAttr == null ? c.FullName : ((ReferAttribute)javaAttr).Name;
                ret.Append(name.Replace('.', '/'));
                ret.Append(';');
            }
            value= ret.ToString();
            TypeDesc.TryAdd(c, value);
            return value;
        }

        public static Type ForName(string name)
        {
            if (DescType.TryGetValue(name, out Type type))
                return type;
            int c = 0, index = name.IndexOf('[');
            if (index > 0)
            {
                c = (name.Length - index) / 2;
                name = name.Substring(0, index);
            }
            if (c > 0)
            {
                StringBuilder sb = new StringBuilder();
                while (c-- > 0)
                    sb.Append("[");

                if ("void".Equals(name)) sb.Append(JvmVoid);
                else if ("boolean".Equals(name)) sb.Append(JvmBoolean);
                else if ("byte".Equals(name)) sb.Append(JvmByte);
                else if ("char".Equals(name)) sb.Append(JvmChar);
                else if ("double".Equals(name)) sb.Append(JvmDouble);
                else if ("float".Equals(name)) sb.Append(JvmFloat);
                else if ("int".Equals(name)) sb.Append(JvmInt);
                else if ("long".Equals(name)) sb.Append(JvmLong);
                else if ("short".Equals(name)) sb.Append(JvmShort);
                else sb.Append('L').Append(name).Append(';'); // "java.lang.Object" ==> "Ljava.lang.Object;"
                name = sb.ToString();
            }
            else
            {
                if ("void".Equals(name)) type= typeof(void);
                else if ("boolean".Equals(name)) type = typeof(bool);
                else if ("byte".Equals(name)) type = typeof(byte);
                else if ("char".Equals(name)) type = typeof(char);
                else if ("double".Equals(name)) type = typeof(double);
                else if ("float".Equals(name)) type = typeof(float);
                else if ("int32".Equals(name)) type = typeof(int);
                else if ("int64".Equals(name)) type = typeof(long);
                else if ("short".Equals(name)) type = typeof(short);
            }

            DescType.TryAdd(name, type);

            return type;
        }

       

      

        public static Type[] Desc2ClassArray(string desc)
        {
            if (string.IsNullOrEmpty(desc))
                return EmptyClassArray;
            List<Type> cs=new List<Type>();

            return cs.ToArray();
        }
    }
}

