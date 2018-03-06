using System;
using System.Collections.Generic;

namespace Dubbo.Net.Common.Serialize.Supports
{
    public interface ISerializationOptimizer
    {
        ICollection<Type> GetSerializableClasses();
    }
}
