using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property)]
    public class JavaNameAttribute:Attribute
    {
        public string Name { get; set; }
        public JavaNameAttribute(string name)
        {
            Name = name;
        }
    }
}
