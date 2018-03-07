using System;

namespace Dubbo.Net.Common.Attributes
{
    public class ReferAttribute:Attribute
    {
        public string Name { get; set; }

        public ReferAttribute(string name)
        {
            Name = name;
        }
    }
}
