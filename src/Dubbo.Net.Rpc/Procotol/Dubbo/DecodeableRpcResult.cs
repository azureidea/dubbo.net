using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dubbo.Net.Remoting;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Serialize;
using Dubbo.Net.Remoting.Transport;
using Dubbo.Net.Rpc.Support;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
{
    public class DecodeableRpcResult : RpcResult, IDecodeable
    {
        private IChannel _channel;

        private byte _serializationType;

        private MemoryStream _inputStream;

        private Response _response;

        private IInvocation _invocation;

        private volatile bool _hasDecoded;

        public DecodeableRpcResult(IChannel channel, Response response, MemoryStream input, IInvocation invocation, byte id)
        {
            this._channel = channel;
            this._response = response;
            this._inputStream = input;
            this._invocation = invocation;
            this._serializationType = id;
        }



        public object Decode(IChannel channel, MemoryStream input)
        {
            IObjectInput inputo = CodecSupport.GetSerialization(channel.Url, _serializationType)
                    .Deserialize( input);

            byte flag = inputo.ReadByte();
            switch (flag)
            {
                case DubboCodec.ResponseNullValue:
                    break;
                case DubboCodec.ResponseValue:
                    try
                    {
                        Type[] returnType = RpcUtils.GetReturnTypes(_invocation);
                        Value=(returnType == null || returnType.Length == 0 ? inputo.ReadObject() :

                                (returnType.Length == 1 ? inputo.ReadObject(returnType[0])
                                            : inputo.ReadObject(returnType[0])));
                    }
                    catch (Exception e)
                    {
                        throw new IOException("Read response data failed." + e);
                    }
                    break;
                case DubboCodec.ResponseWithException:
                    try
                    {
                        object obj = inputo.ReadObject();
                        if (obj is Exception == false)
                            throw new IOException("Response data error, expect Throwable, but get " + obj);
                        Exception=((Exception)obj);
                    }
                    catch (Exception e)
                    {
                        throw new IOException("Read response data failed." + e);
                    }
                    break;
                default:
                    throw new IOException("Unknown result flag, expect '0' '1' '2', get " + flag);
            }
            return this;
        }

        public void Decode()
        {
            if (!_hasDecoded && _channel != null && _inputStream != null)
            {
                try
                {
                    Decode(_channel, _inputStream);
                }
                catch (Exception e)
                {
                    _response.Mstatus = (Response.ClientError);
                    _response.MerrorMsg = e.ToString();
                }
                finally
                {
                    _hasDecoded = true;
                }
            }
        }
    }
}
