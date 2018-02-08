using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using DotNetty.Buffers;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Infrastructure;
using Dubbo.Net.Common.Serialize;
using Dubbo.Net.Remoting.Buffer;

namespace Dubbo.Net.Remoting.Transport
{
    public abstract class AbstractCodec:ICodec
    {
        private const int DefaultPayload = 8 * 1024 * 1024;
        private const string PayloadKey = "payload";
        private const string SideKey = "side";
        protected static void CheckPayload(IChannel channel, long size)
        {
            int payload = DefaultPayload;
            if (channel?.Url != null)
            {
                payload = channel.Url.GetParameter(PayloadKey, DefaultPayload);
            }

            if (payload > 0 && size > payload)
            {
                throw new Exception("Data length too large: " + size + ", max payload: " + payload + ", channel: " + channel);
            }
        }

        protected ISerialization GetSerialization(IChannel channel)
        {
            //todo get serialization from container
            return null;
            //return CodecSupport.GetSerialization(channel.GetUrl());
        }

        protected bool IsClientSide(IChannel channel)
        {
            var side = (string) channel.GetAttribute(SideKey);
            if ("client" == side)
                return true;
            if ("server" == side)
                return false;
            var address = channel.RemoteAddress;
            var url = channel.Url;
            var client = false;//url.Port == address. && address.Ip == url.Ip;
            channel.SetAttribute(SideKey,client?"client":"server");
            return client;
        }

        protected bool IsServerSide(IChannel channel)
        {
            return !IsClientSide(channel);
        }

        public abstract void Encode(IChannel channel, IByteBuffer buffer, object message);
        public abstract object Decode(IChannel channel, IByteBuffer buffer);
    }
}
