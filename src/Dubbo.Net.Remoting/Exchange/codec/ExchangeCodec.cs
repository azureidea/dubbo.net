using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNetty.Buffers;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting.Telnet;
using Dubbo.Net.Common.Buffer;
using Dubbo.Net.Common.Infrastructure;
using Dubbo.Net.Common.Serialize;

namespace Dubbo.Net.Remoting.Exchange.codec
{
    public class ExchangeCodec : TelnetCodec
    {

        // header length.
        protected static readonly int HeaderLength = 16;
        // magic header.
        protected static readonly short Magic = unchecked((short)0xdabb);
        protected static readonly byte MagicHigh = ByteUtil.Short2Bytes(Magic)[0];
        protected static readonly byte MagicLow = ByteUtil.Short2Bytes(Magic)[1];
        // message flag.
        protected static readonly byte FlagRequest = (byte)0x80;
        protected static readonly byte FlagTwoway = (byte)0x40;
        protected static readonly byte FlagEvent = (byte)0x20;
        protected static readonly int SerializationMask = 0x1f;

        public short GetMagicCode()
        {
            return Magic;
        }

        public override void Encode(IChannel channel, IByteBuffer buffer, object msg)
        {
            if (msg is Request)
            {
                EncodeRequest(channel, buffer, (Request)msg);
            }
            else if (msg is Response)
            {
                EncodeResponse(channel, buffer, (Response)msg);
            }
            else
            {
                base.Encode(channel, buffer, msg);
            }
        }

        public override object Decode(IChannel channel, IByteBuffer buffer)
        {
            int readable = buffer.ReadableBytes;
            byte[] header = new byte[Math.Min(readable, HeaderLength)];
            buffer.ReadBytes(header);
            return Decode(channel, buffer, readable, header);
        }

        protected override object Decode(IChannel channel, IByteBuffer buffer, int readable, byte[] header)
        {
            // check magic number.
            if (readable > 0 && header[0] != MagicHigh
                    || readable > 1 && header[1] != MagicLow)
            {
                int length = header.Length;
                if (header.Length < readable)
                {
                    header = ByteUtil.CopyOf(header, readable);
                    buffer.ReadBytes(header, length, readable - length);
                }
                for (int i = 1; i < header.Length - 1; i++)
                {
                    if (header[i] == MagicHigh && header[i + 1] == MagicLow)
                    {
                        buffer.SetReaderIndex(buffer.ReaderIndex - header.Length + i);
                        header = ByteUtil.CopyOf(header, i);
                        break;
                    }
                }
                return base.Decode(channel, buffer, readable, header);
            }
            // check length.
            if (readable < HeaderLength)
            {
                return DecodeResult.NeedMoreInput;
            }

            // get data length.
            int len = ByteUtil.Bytes2Int(header, 12);
            CheckPayload(channel, len);

            int tt = len + HeaderLength;
            if (readable < tt)
            {
                return DecodeResult.NeedMoreInput;
            }

            // limit input stream.
            ChannelBufferStream input = new ChannelBufferStream(buffer, len);

            try
            {
                return DecodeBody(channel, input, header);
            }
            finally
            {
                if (input.Available() > 0)
                {
                    try
                    {
                        if (input.Available() > 0)
                        {
                            input.Position = input.Available();
                        }
                    }
                    catch (IOException e)
                    {
                    }
                }
            }
        }

        protected virtual object DecodeBody(IChannel channel, MemoryStream input, byte[] header)
        {
            byte flag = header[2], proto = (byte)(flag & SerializationMask);
            ISerialization s = GetSerialization(channel);
            IObjectInput inStream = s.Deserialize(input);
            // get request id.
            long id = ByteUtil.Bytes2Long(header, 4);
            if ((flag & FlagRequest) == 0)
            {
                // decode response.
                Response res = new Response(id);
                if ((flag & FlagEvent) != 0)
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
                            data = DecodeEventData(channel, inStream);
                        }
                        else if (res.Mevent)
                        {
                            data = DecodeEventData(channel, inStream);
                        }
                        else
                        {
                            data = DecodeResponseData(channel, inStream);
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
                    res.MerrorMsg = inStream.ReadUTF();
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
                    data = DecodeEventData(channel, inStream);
                }
                else if (req.IsEvent)
                {
                    data = DecodeEventData(channel, inStream);
                }
                else
                {
                    data = DecodeRequestData(channel, inStream);
                }

                req.Mdata = data;
            }
            catch (Exception t)
            {
                // bad request
                req.IsBroken = true;
                req.Mdata = t;
            }
            return req;
        }

     

        protected void EncodeRequest(IChannel channel, IByteBuffer buffer, Request req)
        {
            ISerialization serialization = GetSerialization(channel);
            // header.
            byte[] header = new byte[HeaderLength];
            // set magic number.
            ByteUtil.Short2Bytes(Magic, header);

            // set request and serialization flag.
            header[2] = (byte)(FlagRequest | serialization.GetContentTypeId());

            if (req.IsTwoWay) header[2] |= FlagTwoway;
            if (req.IsEvent) header[2] |= FlagEvent;

            // set request id.
            ByteUtil.Long2Bytes(req.Mid, header, 4);

            // encode request data.
            int savedWriteIndex = buffer.WriterIndex;
            buffer.SetWriterIndex(savedWriteIndex + HeaderLength);
            ChannelBufferStream bos = new ChannelBufferStream(buffer);
            IObjectOutput output = serialization.Serialize(bos);
            if (req.IsEvent)
            {
                EncodeEventData(channel, output, req.Mdata);
            }
            else
            {
                EncodeRequestData(channel, output, req.Mdata);
            }
            output.FlushBuffer();
            bos.Flush();
            bos.Close();
            int len = bos.WritenBytes();
            CheckPayload(channel, len);
            ByteUtil.Int2Bytes(len, header, 12);

            // write
            buffer.SetWriterIndex(savedWriteIndex);
            buffer.WriteBytes(header); // write header.
            buffer.SetWriterIndex(savedWriteIndex + HeaderLength + len);
        }

        protected void EncodeResponse(IChannel channel, IByteBuffer buffer, Response res)
        {
            int savedWriteIndex = buffer.WriterIndex;
            try
            {
                ISerialization serialization = GetSerialization(channel);
                // header.
                byte[] header = new byte[HeaderLength];
                // set magic number.
                ByteUtil.Short2Bytes(Magic, header);
                // set request and serialization flag.
                header[2] = serialization.GetContentTypeId();
                if (res.IsHeartBeat()) header[2] |= FlagEvent;
                // set response status.
                byte status = res.Mstatus;
                header[3] = status;
                // set request id.
                ByteUtil.Long2Bytes(res.Mid, header, 4);
                buffer.SetWriterIndex(savedWriteIndex + HeaderLength);
                ChannelBufferStream bos = new ChannelBufferStream(buffer);
                IObjectOutput output = serialization.Serialize(bos);
                // encode response data or error message.
                if (status == Response.Ok)
                {
                    if (res.IsHeartBeat())
                    {
                        EncodeEventData(channel, output, res.Mresult);
                    }
                    else
                    {
                        EncodeResponseData(channel, output, res.Mresult);
                    }
                }
                else output.WriteUTF(res.MerrorMsg);
                output.FlushBuffer();
                bos.Flush();
                bos.Close();

                int len = bos.WritenBytes();
                CheckPayload(channel, len);
                ByteUtil.Int2Bytes(len, header, 12);
                // write
                buffer.SetWriterIndex(savedWriteIndex);
                buffer.WriteBytes(header); // write header.
                buffer.SetWriterIndex(savedWriteIndex + HeaderLength + len);
            }
            catch (Exception t)
            {
                // clear buffer
                buffer.SetWriterIndex(savedWriteIndex);
                // send error message to Consumer, otherwise, Consumer will wait till timeout.
                if (!res.Mevent && res.Mstatus != Response.BadResponse)
                {
                    Response r = new Response(res.Mid, res.Mversion);
                    r.Mstatus = Response.BadResponse;
                    r.MerrorMsg = t.ToString();
                }
            }
        }


        protected override object DecodeData(IObjectInput input)
        {
            return DecodeRequestData(input);
        }



        protected virtual object DecodeRequestData(IObjectInput input)
        {
            return input.ReadObject();
        }

        protected virtual object DecodeResponseData(IObjectInput input)
        {
            return input.ReadObject();
        }


        protected override void EncodeData(IObjectOutput output, object data)
        {
            EncodeRequestData(output, data);
        }

        private void EncodeEventData(IObjectOutput output, object data)
        {
            output.WriteObject(data);
        }


        protected virtual void EncodeRequestData(IObjectOutput output, object data)
        {
            output.WriteObject(data);
        }

        protected virtual void EncodeResponseData(IObjectOutput output, object data)
        {
            output.WriteObject(data);
        }


        protected override object DecodeData(IChannel channel, IObjectInput input)
        {
            return DecodeRequestData(channel, input);
        }

        protected virtual object DecodeEventData(IChannel channel, IObjectInput input)
        {
                return input.ReadObject();
        }


        protected virtual object DecodeRequestData(IChannel channel, IObjectInput input)
        {
            return DecodeRequestData(input);
        }

        protected virtual object DecodeResponseData(IChannel channel, IObjectInput input)
        {
            return DecodeResponseData(input);
        }

        protected virtual object DecodeResponseData(IChannel channel, IObjectInput input, object requestData)
        {
            return DecodeResponseData(channel, input);
        }


        protected override void EncodeData(IChannel channel, IObjectOutput output, object data)
        {
            EncodeRequestData(channel, output, data);
        }

        private void EncodeEventData(IChannel channel, IObjectOutput output, object data)
        {
            EncodeEventData(output, data);
        }

   

        protected virtual void EncodeRequestData(IChannel channel, IObjectOutput output, object data)
        {
            EncodeRequestData(output, data);
        }

        protected virtual void EncodeResponseData(IChannel channel, IObjectOutput output, object data)
        {
            EncodeResponseData(output, data);
        }
    }
}
