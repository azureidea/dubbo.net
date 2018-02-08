using System;
using System.Collections.Generic;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Dubbo.Net.Remoting;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
{
    //public class InternalClientDecoder : ByteToMessageDecoder
    //{
    //    private readonly ICodec _codec;
    //    public InternalClientDecoder(ICodec codec) { _codec = codec; }
    //    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    //    {
    //        var channel = context.Channel;
    //        var readable = input.ReadableBytes;
    //        if (readable <= 0)
    //            return;
    //        int saveReaderIndex;
    //        object msg;
    //        try
    //        {
    //            do
    //            {
    //                saveReaderIndex = input.ReaderIndex;
    //                msg = _codec.Decode(channel, input);
    //                if ((msg is DecodeResult)&&(DecodeResult)msg==DecodeResult.NeedMoreInput )
    //                {
    //                    input.SetReaderIndex(saveReaderIndex);
    //                    break;
    //                }
    //                else
    //                {
    //                    if (saveReaderIndex == input.ReaderIndex)
    //                        throw new Exception("Decode without read data.");
    //                    if (msg != null)
    //                    {
    //                        output.Add(msg);
    //                    }
    //                }
    //            } while (input.IsReadable());
    //        }
    //        catch (Exception ex)
    //        {
    //        }
    //    }
    //}
    //public class InternalServerDecoder : ByteToMessageDecoder
    //{
    //    private readonly ICodec _codec;
    //    public InternalServerDecoder(ICodec codec) { _codec = codec; }
    //    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    //    {
    //        var channel = context.Channel;
    //        var result = _codec.Decode(channel, input);
    //        output.Add(result);
    //    }
    //}
}
