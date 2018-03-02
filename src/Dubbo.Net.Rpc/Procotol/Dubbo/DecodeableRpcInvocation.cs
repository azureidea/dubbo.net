using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Serialize;
using Dubbo.Net.Remoting;
using Dubbo.Net.Remoting.Transport;

namespace Dubbo.Net.Rpc.Procotol.Dubbo
{
    public class DecodeableRpcInvocation : RpcInvocation, IDecodeable
    {
        private readonly IChannel _channel;

        private readonly byte _serializationType;

        private readonly MemoryStream _inputStream;

        private readonly Request _request;

        private volatile bool _hasDecoded;

        public DecodeableRpcInvocation(IChannel channel, Request request, MemoryStream input, byte id)
        {
            this._channel = channel;
            this._request = request;
            this._inputStream = input;
            this._serializationType = id;
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
                    _request.IsBroken = true;
                    _request.Mdata = e;
                }
                finally
                {
                    _hasDecoded = true;
                }
            }
        }


        public object Decode(IChannel channel, MemoryStream input)
        {
            IObjectInput inputo = CodecSupport.GetSerialization(channel.Url, _serializationType)
                        .Deserialize( input);

            SetAttachment(Constants.DubboVersionKey, inputo.ReadUTF());
            SetAttachment(Constants.PathKey, inputo.ReadUTF());
            SetAttachment(Constants.VersionKey, inputo.ReadUTF());

            MethodName = (inputo.ReadUTF());
            try
            {
                object[] args;
                Type[] pts;
                string desc = inputo.ReadUTF();
                if (desc.Length == 0)
                {
                    pts = DubboCodec.EmptyClassArray;
                    args = DubboCodec.EmptyObjectArray;
                }
                else
                {
                    pts = ReflectUtil.Desc2ClassArray(desc);
                    args = new object[pts.Length];
                    for (int i = 0; i < args.Length; i++)
                    {
                        try
                        {
                            args[i] = inputo.ReadObject(pts[i]);
                        }
                        catch (Exception e)
                        {
                            //
                        }
                    }
                }
                ParameterTypes=(pts);

                Dictionary<string, string> map =  inputo.ReadObject<Dictionary<string, string>>();
            if (map != null && map.Count > 0) {
                Dictionary<string, string> attachment = Attachments;
                if (attachment == null) {
                    attachment = new Dictionary<string, string>();
                }

                foreach (var kv in map)
                {
                    attachment.Add(kv.Key,kv.Value);
                }

                Attachments = attachment;
            }
            //decode argument ,may be callback
            for (int i = 0; i<args.Length; i++)
            {
                args[i] = inputo.ReadObject();
            }

                Arguments = args;

            } catch (Exception e) {
            throw new IOException("Read invocation data failed."+e);
        } finally {
        }
        return this;
    }
    }
}
