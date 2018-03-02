using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Serialize;
using Dubbo.Net.Remoting;
using Dubbo.Net.Remoting.Exchange.codec;
using Dubbo.Net.Rpc.Procotol.Dubbo;
using Dubbo.Net.Rpc.Support;

namespace Dubbo.Net.Rpc.Procotol
{
    public class DubboCodec : ExchangeCodec
    {
        public static readonly String Name = "dubbo";
        public static readonly String DubboVersion = Common.Version.GetVersion(typeof(DubboCodec), Common.Version.GetVersion());
        public const byte ResponseWithException = 0;
        public const byte ResponseValue = 1;
        public const byte ResponseNullValue = 2;
        public static readonly object[] EmptyObjectArray = new object[0];
        public static readonly Type[] EmptyClassArray = new Type[0];

        protected override object DecodeBody(IChannel channel, MemoryStream input, byte[] header)
        {
            byte flag = header[2], proto = (byte)(flag & SerializationMask);
            ISerialization s = GetSerialization(channel);
            // get request id.
            var id = ByteUtil.Bytes2Long(header, 4);
            if ((flag & FlagRequest) == 0)
            {
                // decode response.
                Response res = new Response(id);
                if ((flag & FlagRequest) != 0)
                {
                    res.SetEvent(Response.HeartbeatEvent);
                }
                // get status.
                byte status = header[3];
                res.Mstatus = status;
                if (status == Response.Ok)
                {
                    try
                    {
                        object data;
                        if (res.IsHeartBeat())
                        {
                            data = DecodeEventData(channel, Deserialize(s, input));
                        }
                        else if (res.Mevent)
                        {
                            data = DecodeEventData(channel, Deserialize(s, input));
                        }
                        else
                        {
                            DecodeableRpcResult result;
                            if (channel.Url.GetParameter(Constants.DecodeInIoThreadKey,Constants.DefaultDecodeInIoThread))
                            {
                                result = new DecodeableRpcResult(channel, res, input,
                                    (IInvocation)InvocationUtils.GetInvocation(id), proto);
                                result.Decode();
                            }
                            else
                            {
                                result = new DecodeableRpcResult(channel, res,input,
                                    (IInvocation)InvocationUtils.GetInvocation(id), proto);
                            }
                            data = result;
                        }
                        res.Mresult = data;
                    }
                    catch (Exception t)
                    {
                        res.Mstatus = Response.ClientError;
                        res.MerrorMsg = t.ToString();
                    }
                }
                else
                {
                    res.MerrorMsg = Deserialize(s, input).ReadUTF();
                }
                return res;
            }

            // decode request.
            Request req = new Request(id);
            req.Mversion = "2.0.0";
            req.IsTwoWay = (flag & FlagRequest) != 0;
            if ((flag & FlagEvent) != 0)
            {
                req.SetEvent(Request.HeartBeatEvent);
            }
            try
            {
                object data;
                if (req.IsHeartbeat())
                {
                    data = DecodeEventData(channel, Deserialize(s, input));
                }
                else if (req.IsEvent)
                {
                    data = DecodeEventData(channel, Deserialize(s, input));
                }
                else
                {
                    DecodeableRpcInvocation inv;
                    if (channel.Url.GetParameter(Constants.DecodeInIoThreadKey,Constants.DefaultDecodeInIoThread))
                    {
                        inv = new DecodeableRpcInvocation(channel, req, input, proto);
                        inv.Decode();
                    }
                    else
                    {
                        inv = new DecodeableRpcInvocation(channel, req,input, proto);
                    }
                    data = inv;
                }
                req.Mdata = (data);
            }
            catch (Exception t)
            {
                // bad request
                req.IsBroken = (true);
                req.Mdata = (t);
            }
            return req;
        }

        private IObjectInput Deserialize(ISerialization serialization, MemoryStream input)
        {
            return serialization.Deserialize(input);
        }

        private byte[] ReadMessageData(MemoryStream input)
        {
            if (input.Length > 0)
            {
                byte[] result = new byte[input.Length];
                input.Read(result, 0, result.Length);
                return result;
            }
            return new byte[] { };
        }

        protected override void EncodeRequestData(IChannel channel, IObjectOutput output, object data)
        {
            RpcInvocation inv = (RpcInvocation)data;

            output.WriteUTF(inv.GetAttachment(Constants.DubboVersionKey, DubboVersion));
            output.WriteUTF(inv.GetAttachment(Constants.PathKey));
            output.WriteUTF(inv.GetAttachment(Constants.VersionKey));

            output.WriteUTF(inv.MethodName);
            output.WriteUTF(ReflectUtil.GetDesc(inv.ParameterTypes));
            object[]
            args = inv.Arguments;
            if (args != null)
            {
                foreach (var arg in args)
                {
                    output.WriteObject(arg);
                }
            }
            output.WriteObject(inv.Attachments);
        }

        protected override void EncodeResponseData(IChannel channel, IObjectOutput output, object data)
        {
            IResult result = (IResult)data;

            Exception th = result.Exception;
            if (th == null)
            {
                object ret = result.Value;
                if (ret == null)
                {
                    output.WriteByte(ResponseNullValue);
                }
                else
                {
                    output.WriteByte(ResponseValue);
                    output.WriteObject(ret);
                }
            }
            else
            {
                output.WriteByte(ResponseWithException);
                output.WriteObject(th);
            }
        }
    }
}
