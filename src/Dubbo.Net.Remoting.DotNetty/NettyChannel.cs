using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting.Transport;

namespace Dubbo.Net.Remoting.Netty
{
    public sealed class NettyChannel : AbstractChannel
    {
        private static readonly ConcurrentDictionary<DotNetty.Transport.Channels.IChannel, NettyChannel> ChannelMap =
            new ConcurrentDictionary<DotNetty.Transport.Channels.IChannel, NettyChannel>();
        private readonly DotNetty.Transport.Channels.IChannel _channel;
        private readonly ConcurrentDictionary<string, object> _attributes = new ConcurrentDictionary<string, object>();


        public NettyChannel(DotNetty.Transport.Channels.IChannel channel, URL url, IChannelHandler handler) : base(url, handler)
        {
            _channel = channel ?? throw new ArgumentException("netty channel==null");
        }

        public static NettyChannel GetOrAddChannel(DotNetty.Transport.Channels.IChannel ch, URL url, IChannelHandler handler)
        {
            if (ch == null)
                return null;
            if (!ChannelMap.ContainsKey(ch))
            {
                var nettyChannel = new NettyChannel(ch, url, handler);
                ChannelMap.TryAdd(ch, nettyChannel);
            }

            return ChannelMap[ch];
        }

        public static void RemoveChannelIfDisconnected(DotNetty.Transport.Channels.IChannel ch)
        {
            if (ch != null && ch.Active)
                ChannelMap.TryRemove(ch, out NettyChannel channel);
        }

        public override EndPoint RemoteAddress => _channel.LocalAddress;

        public override bool IsConnected => _channel.Active;

        public override async Task<Response> SendAsync(object message, bool sent)
        {
            try
            {
                await _channel.WriteAndFlushAsync(message);
                return null;
            }
            catch (Exception e)
            {
                throw new RemotingException(this, "Failed to send message " + message + " to " + RemoteAddress + ", cause: " + e.Message, e);
            }
        }

        public override bool HasAttribute(string key)
        {
            return _attributes.ContainsKey(key);
        }

        public override object GetAttribute(string key)
        {
            _attributes.TryGetValue(key, out var result);
            return result;
        }

        public override void SetAttribute(string key, object value)
        {
            if (value == null)
                _attributes.TryRemove(key, out var result);
            else
            {
                _attributes.TryAdd(key, value);
            }
        }

        public override void RemoveAttribute(string key)
        {
            _attributes.TryRemove(key, out var result);
        }
        public override string ToString()
        {
            return "NettyChannel [channel=" + _channel + "]";
        }
    }
}
