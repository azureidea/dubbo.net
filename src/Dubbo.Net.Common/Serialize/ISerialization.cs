using System.IO;

namespace Dubbo.Net.Common.Serialize
{
    public interface ISerialization
    {
        byte GetContentTypeId();
        string GetContentType();
        IObjectOutput Serialize(MemoryStream stream);
        IObjectInput Deserialize(MemoryStream stream);
    }
}
