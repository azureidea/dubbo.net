using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting.Transport;
using IChannel= DotNetty.Transport.Channels.IChannel;

namespace Dubbo.Net.Remoting.Netty
{
    public sealed class NettyChannel: AbstractChannel
    {
        private static readonly ILogger _logger;
        private  static readonly  ConcurrentDictionary<DotNetty.Transport.Channels.IChannel,NettyChannel> ChannelMap=new ConcurrentDictionary<DotNetty.Transport.Channels.IChannel, NettyChannel>();
        private readonly DotNetty.Transport.Channels.IChannel _channel;
        private readonly ConcurrentDictionary<string,object> _attributes=new ConcurrentDictionary<string, object>();
        public NettyChannel(DotNetty.Transport.Channels.IChannel channel,URL url, IChannelHandler handler) : base(url, handler)
        {
            _channel = channel ?? throw new ArgumentException("netty channel==null");
        }

        static NettyChannel GetOrAddChannel(DotNetty.Transport.Channels.IChannel ch, URL url, IChannelHandler handler)
        {
            if (ch == null)
                return null;
            if (!ChannelMap.ContainsKey(ch))
            {
                var nettyChannel=new NettyChannel(ch,url,handler);
                ChannelMap.TryAdd(ch, nettyChannel);
            }

            return ChannelMap[ch];
        }

        static void RemoveChannelIfDisconnected(DotNetty.Transport.Channels.IChannel ch)
        {
            if (ch != null && ch.Active)
                ChannelMap.TryRemove(ch, out NettyChannel channel);
        }

        public override EndPoint RemoteAddress => _channel.LocalAddress;

        public override bool IsConnected => _channel.Active;

        public override async Task SendAsync(object message, bool sent)
        {
            await base.SendAsync(message, sent);
            var success = true;
            var timeout = 0;
            try
            {
                var task =  _channel.WriteAndFlushAsync(message);
                if (sent)
                {
                    timeout = 0;//todo get timeout config from url
                    success=task.Wait(timeout);
                }

                var exc = task.Exception;
                if (exc != null)
                    throw exc;
            }
            catch (Exception e)
            {
                throw new RemotingException(this, "Failed to send message " + message + " to " + RemoteAddress + ", cause: " + e.Message, e);
            }
            if(!success)
                throw new RemotingException(this, "Failed to send message " + message + " to " + RemoteAddress
                                                  + "in timeout(" + timeout + "ms) limit");
        }
        public override bool HasAttribute(string key)
        {
            throw new NotImplementedException();
        }

        public override object GetAttribute(string key)
        {
            throw new NotImplementedException();
        }

        public override void SetAttribute(string key, object value)
        {
            throw new NotImplementedException();
        }

        public override void RmoveAttribute(string key)
        {
            throw new NotImplementedException();
        }
    }
}
