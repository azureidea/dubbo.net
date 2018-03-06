using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Serialize;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Remoting.Transport
{
    public static class CodecSupport
    {
        private static readonly ConcurrentDictionary<byte, ISerialization> IdSerializationMap = new ConcurrentDictionary<byte, ISerialization>();





        public static ISerialization GetSerializationByKey(string id)
        {
            //Console.WriteLine("begin get serialization4:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            var serialization = ObjectFactory.GetInstance<ISerialization>(id);
            if (!IdSerializationMap.ContainsKey(serialization.GetContentTypeId()))
                IdSerializationMap.TryAdd(serialization.GetContentTypeId(), serialization);
            // Console.WriteLine("end get serialization5:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return serialization;
        }

        public static ISerialization GetSerialization(URL url)
        {
            //Console.WriteLine("begin get serialization2:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            //todo 根据序列化名称字符串取id
            var key = url.GetParameter(Constants.SerializationKey, "fastjson");
            //Console.WriteLine("begin get serialization3:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            return GetSerializationByKey(key);
        }

        public static ISerialization GetSerialization(URL url, byte id)
        {
            ISerialization result = GetSerializationById(id) ?? GetSerialization(url);
            return result;
        }

        static ISerialization GetSerializationById(byte id)
        {
            if (IdSerializationMap.TryGetValue(id, out var serialization))
                return serialization;
            return null;
        }
    }
}
