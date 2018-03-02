using System;

namespace Dubbo.Net.Common.Utils
{
    public class DependencyIocAttribute:Attribute
    {
        public Type InterFaceType { get; set; }
        public object[] Keys { get; set; }

        public DependencyIocAttribute(Type type,params object[] keys)
        {
            InterFaceType = type;
            Keys = keys;
        }
    }
}
