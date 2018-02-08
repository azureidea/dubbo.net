using Dubbo.Net.Common.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        private static readonly ConcurrentDictionary<string, Type> TypeCache = new ConcurrentDictionary<string, Type>();
        private static readonly ConcurrentDictionary<Type, string> TypNameCache = new ConcurrentDictionary<Type, string>();
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
            StringBuilder ret = new StringBuilder();

            while (c.IsArray)
            {
                ret.Append('[');
                c = c.GetElementType();
            }

            if (c.IsPrimitive)
            {
                string t = c.Name;
                if ("void".Equals(t)) ret.Append(JvmVoid);
                else if ("bool".Equals(t)) ret.Append(JvmBoolean);
                else if ("byte".Equals(t)) ret.Append(JvmByte);
                else if ("char".Equals(t)) ret.Append(JvmChar);
                else if ("double".Equals(t)) ret.Append(JvmDouble);
                else if ("float".Equals(t)) ret.Append(JvmFloat);
                else if ("int".Equals(t)) ret.Append(JvmInt);
                else if ("long".Equals(t)) ret.Append(JvmLong);
                else if ("short".Equals(t)) ret.Append(JvmShort);
            }
            else
            {
                ret.Append('L');
                var javaAttr = c.GetCustomAttributes(typeof(JavaNameAttribute), false).FirstOrDefault();
                var name = javaAttr == null ? c.FullName : ((JavaNameAttribute)javaAttr).Name;
                ret.Append(name.Replace('.', '/'));
                ret.Append(';');
            }
            return ret.ToString();
        }

        public static Type ForName(string name)
        {
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
                if ("void".Equals(name)) return typeof(void);
                else if ("boolean".Equals(name)) return typeof(bool);
                else if ("byte".Equals(name)) return typeof(byte);
                else if ("char".Equals(name)) return typeof(char);
                else if ("double".Equals(name)) return typeof(double);
                else if ("float".Equals(name)) return typeof(float);
                else if ("int".Equals(name)) return typeof(int);
                else if ("long".Equals(name)) return typeof(long);
                else if ("short".Equals(name)) return typeof(short);
            }


            return GetTypeFromCache(name);
        }

        private static Type GetTypeFromCache(string key)
        {
            return TypeCache[key];
        }

        public static void RegisteType(string key, Type type)
        {
            TypeCache[key] = type;
            TypNameCache[type] = key;
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
