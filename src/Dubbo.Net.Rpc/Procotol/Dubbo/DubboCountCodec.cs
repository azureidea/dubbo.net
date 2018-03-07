using System;
using System.Collections.Generic;
using System.Linq;
using DotNetty.Buffers;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
{
    public class DubboCountCodec : ICodec
    {
        private DubboCodec codec = new DubboCodec();

        public void Encode(IChannel channel, IByteBuffer buffer, Object msg)
        {
            codec.Encode(channel, buffer, msg);
        }

        public object Decode(IChannel channel, IByteBuffer buffer)
        {
            int save = buffer.ReaderIndex;
            List<object> result=new List<object>();
            do
            {
                //Console.WriteLine("begin decode:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                object obj = codec.Decode(channel, buffer);
                if (obj is DecodeResult decodeResult && decodeResult==DecodeResult.NeedMoreInput)
                {
                    //Console.WriteLine("need more input:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    buffer.SetReaderIndex(save);
                    break;
                }
                else
                {
                    result.Add(obj);
                    LogMessageLength(obj, buffer.ReaderIndex - save);
                    save = buffer.ReaderIndex;
                }
            } while (true);
            //Console.WriteLine("decode completed:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            if (result.Count==0)
            {
                return DecodeResult.NeedMoreInput;
            }
            if (result.Count == 1)
            {
                return result.First();
            }
            return result;
        }

        private void LogMessageLength(object result, int bytes)
        {
            if (bytes <= 0)
            {
                return;
            }
            if (result is Request request) {
                try
                {
                    ((RpcInvocation)request.Mdata).SetAttachment(Constants.InputKey, bytes.ToString());
                }
                catch (Exception )
                {
                    /* ignore */
                }
            } else if (result is Response res) {
                try
                {
                    if (!string.IsNullOrEmpty(res.MerrorMsg))
                    {
                        res.Mresult = res.Mresult ?? new RpcResult(new Exception(res.MerrorMsg));
                    }
                    ((RpcResult)(res).Mresult).SetAttachment(Constants.OutputKey, bytes.ToString());
                }
                catch (Exception )
                {
                    /* ignore */
                }
            }
        }
    }
}
