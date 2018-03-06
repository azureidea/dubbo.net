using System;
using System.IO;
using DotNetty.Buffers;
using Dubbo.Net.Common.Buffer;
using Dubbo.Net.Common.Serialize;

namespace Dubbo.Net.Remoting.Transport
{
    public class TransportCodec : AbstractCodec
    {
        public override void Encode(IChannel channel, IByteBuffer buffer, object message)
        {
            var output = new ChannelBufferStream(buffer);
            var objectOutput = GetSerialization(channel).Serialize(output);
            EncodeData(channel, objectOutput, message);
            objectOutput.FlushBuffer();
            if (objectOutput is ICleanable)
            {
                ((ICleanable)objectOutput).cleanup();
            }
        }

        public override object Decode(IChannel channel, IByteBuffer buffer)
        {
            var input = new ChannelBufferStream(buffer);
            var objectInput = GetSerialization(channel).Deserialize(input);
            var obj = DecodeData(channel, objectInput);
            return obj;
        }

        protected virtual void EncodeData(IChannel channel, IObjectOutput output, object message)
        {
            EncodeData(output, message);
        }

        protected virtual object DecodeData(IChannel channel, IObjectInput input)
        {
            return DecodeData(input);
        }

        protected virtual void EncodeData(IObjectOutput output, object message)
        {
            output.WriteObject(message);
        }

        protected virtual object DecodeData(IObjectInput input)
        {
            try
            {
                return input.ReadObject();
            }
            catch (Exception e)
            {
                throw new IOException("ClassNotFoundException: " + e);
            }
        }
    }
}
