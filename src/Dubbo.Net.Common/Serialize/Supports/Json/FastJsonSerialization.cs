using System.IO;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Common.Serialize.Supports.Json
{
    [DependencyIoc(typeof(ISerialization),6)]
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
