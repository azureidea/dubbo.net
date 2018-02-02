using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common.Infrastructure
{
    public interface ISerialization
    {
        byte GetContentTypeId();
        string GetContentType();
        byte[] Serialize<T>(T obj);
        T Deserialize<T>(byte[] input);
    }
}
