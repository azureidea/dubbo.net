using System;
using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Netty
{
    public class InternalDecoder: ByteToMessageDecoder
    {
        private readonly URL _url;
        private readonly IChannelHandler _handler;
        private readonly ICodec _codec;

        public InternalDecoder(URL url, IChannelHandler handler, ICodec codec)
        {
            _url = url;
            _handler = handler;
            _codec = codec;
        }
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            NettyChannel channel = NettyChannel.GetOrAddChannel(context.Channel, _url, _handler);

            Object msg;

            int saveReaderIndex;

            try
            {
                // decode object.
                do
                {
                    saveReaderIndex = input.ReaderIndex;
                    try
                    {
                        //Console.WriteLine("decode:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        msg = _codec.Decode(channel, input);
                        //Console.WriteLine("decoded:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                    catch (IOException e)
                    {
                        throw e;
                    }
                    if (msg is DecodeResult result && result  == DecodeResult.NeedMoreInput)
                    {
                        input.SetReaderIndex(saveReaderIndex);
                        break;
                    }
                    else
                    {
                        //is it possible to go here ?
                        if (saveReaderIndex == input.ReaderIndex)
                        {
                            throw new IOException("Decode without read data.");
                        }
                        if (msg != null)
                        {
                            output.Add(msg);
                        }
                    }
                } while (input.IsReadable());
            }
            finally
            {
                NettyChannel.RemoveChannelIfDisconnected(context.Channel);
            }
        }
    }
}
