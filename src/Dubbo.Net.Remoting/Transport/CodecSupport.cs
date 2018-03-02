using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Serialize;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Remoting.Transport
{
    public static class CodecSupport
    {
        private static readonly Dictionary<byte, ISerialization> IdSerializationMap = new Dictionary<byte, ISerialization>();

        public static void RegisterSerializer(ISerialization serialization)
        {
            IdSerializationMap[serialization.GetContentTypeId()]=serialization;
        }

      

        public static ISerialization GetSerializationById(byte id)
        {
            return ObjectFactory.GetInstance<ISerialization>(id);
            //if (!IdSerializationMap.ContainsKey(id))
            //    return null;
            //return IdSerializationMap[id];
        }

        public static ISerialization GetSerialization(URL url)
        {
            var key = url.GetParameter(Constants.SerializationKey, Constants.DefaultRemotingSerialization);
            return GetSerializationById(key);
        }

        public static ISerialization GetSerialization(URL url, byte id)
        {
            ISerialization result = GetSerializationById(id) ?? GetSerialization(url);
            return result;
        }
    }
}
