using System.IO;

namespace Dubbo.Net.Common.Serialize.Supports.Json
{
    public class FastJsonSerialization : ISerialization
    {
        public IObjectInput Deserialize( MemoryStream stream)
        {
            return new FastJsonObjectInput(stream);
        }

        public string GetContentType()
        {
            return "text/json";
        }

        public byte GetContentTypeId()
        {
            return 6;
        }

        public IObjectOutput Serialize( MemoryStream stream)
        {
            return new FastJsonObjectOutput(stream);
        }
    }
}
