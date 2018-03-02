using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common.Serialize.Supports
{
    public abstract class SerializableClassRegistry
    {
        private static readonly List<Type> Registrations=new List<Type>();
        public static void RegisterClass(Type clazz) { Registrations.Add(clazz);}

        public static List<Type> GetRegisteredClasses()
        {
            return Registrations;
        }
    }
}
