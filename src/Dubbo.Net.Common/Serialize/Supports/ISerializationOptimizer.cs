using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common.Serialize.Supports
{
    public interface ISerializationOptimizer
    {
        ICollection<Type> GetSerializableClasses();
    }
}
