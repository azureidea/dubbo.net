using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common.Utils
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
