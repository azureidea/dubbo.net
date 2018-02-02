using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using Dubbo.Net.Common.Infrastructure;

namespace Dubbo.Net.Common.Codec
{
    public class DubboCodec : ICodec
    {
        private const string DefaultDubboVersion = "2.6.0";
        private const string EnterSplitter = "\r\n";
        /// <summary>
        /// 头长度16字节
        /// </summary>
        private const int HeaderLength = 16;
        /// <summary>
        /// 魔术值
        /// </summary>
        private const short Magic = unchecked((short)0xdabb);
        /// <summary>
        /// 魔术值第一位
        /// </summary>
        private readonly byte _magicHigh = ByteUtil.Short2Bytes(Magic)[0];
        /// <summary>
        /// 魔术值第二位
        /// </summary>
        private readonly byte _magicLow = ByteUtil.Short2Bytes(Magic)[1];
        /// <summary>
        /// 是否请求标记
        /// </summary>
        private const byte FlagRequest = 0x80;
        /// <summary>
        /// 是否双工传输
        /// </summary>
        private const byte FlagTwoWay = 0x40;
        /// <summary>
        /// 是否事件
        /// </summary>
        private const byte FlagEvent = 0x20;
        /// <summary>
        /// 序列化协议
        /// </summary>
        private const int SerializationMask = 0x1f;

        readonly ISerialization _serialization;
        public DubboCodec(ISerialization serialization)
        {
            _serialization = serialization;
        }
        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public object Decode(IChannel channel, IByteBuffer buffer)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 请求编码
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="buffer"></param>
        /// <param name="message"></param>
        public void EncodeRequest(IChannel channel, IByteBuffer buffer, Request message)
        {
            var header = new byte[HeaderLength];
            ByteUtil.Short2Bytes(Magic, header);                                                        //设置头的前两字节，魔术值
            header[2] = (byte)(FlagRequest | _serialization.GetContentTypeId());      //头第三字节，标记位，是否请求标记|事件标记|双工标记
            if (message.IsTwoWay) header[2] |= FlagTwoWay;
            if (message.IsEvent) header[2] |= FlagEvent;

            ByteUtil.Long2Bytes(message.Mid, header, 4);                                          //5到12字节为请求ID  long类型8字节 64位
            var inv = message.Mdata;
            List<byte[]> bytes = new List<byte[]>();
            //body相关写入
            var version = inv.GetAttachment(Constants.DubboVersionKey, DefaultDubboVersion);
            bytes.Add(_serialization.Serialize(version));
            //添加行
            bytes.Add(_serialization.Serialize(EnterSplitter));
            bytes.Add(_serialization.Serialize(inv.GetAttachment(Constants.PathKey)));
            bytes.Add(_serialization.Serialize(EnterSplitter));
        }

        public void EncodeResponse(IChannel channel, IByteBuffer buffer, Response message)
        {
            throw new NotImplementedException();
        }

        public Request DecodeRequest(IChannel channel, IByteBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public Response DecodeResponse(IChannel channel, IByteBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
