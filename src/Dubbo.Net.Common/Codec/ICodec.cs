using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common.Codec
{
    public interface ICodec
    {
        void EncodeRequest(IChannel channel, IByteBuffer buffer, Request message);
        void EncodeResponse(IChannel channel, IByteBuffer buffer, Response message);
        Request DecodeRequest(IChannel channel, IByteBuffer buffer);
        Response DecodeResponse(IChannel channel, IByteBuffer buffer);
    }
}
