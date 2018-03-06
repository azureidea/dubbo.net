using Dubbo.Net.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Transport
{
    public class ChannelHandlerDispatcher:IChannelHandler
    {

        static ILogger _log;
        public List<IChannelHandler> ChannelHandlers { get; } = new List<IChannelHandler>();

        public ChannelHandlerDispatcher() { }

        public ChannelHandlerDispatcher(params IChannelHandler[] handlers):this(handlers?.ToList()) { }

        public ChannelHandlerDispatcher(ICollection<IChannelHandler> handlers)
        {
            if (handlers != null && handlers.Count > 0)
            {
                ChannelHandlers.AddRange(handlers);
            }
        }
        public ChannelHandlerDispatcher AddChannelHandler(IChannelHandler handler)
        {
            ChannelHandlers.Add(handler);
            return this;
        }
        public ChannelHandlerDispatcher RemoveChannelHandler(IChannelHandler handler)
        {
            ChannelHandlers.Remove(handler);
            return this;
        }

        public async Task ConnectAsync(IChannel channel)
        {
            foreach(var listener in ChannelHandlers)
            {
                try
                {
                   await listener.ConnectAsync(channel);
                }catch(Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }

        public async Task DisconnectAsync(IChannel channel)
        {
            foreach(var listener in ChannelHandlers)
            {
                try
                {
                    await listener.DisconnectAsync(channel);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }

        public async Task SentAsync(IChannel channel, object message)
        {
            foreach (var listener in ChannelHandlers)
            {
                try
                {
                    await listener.SentAsync(channel,message);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }

        public async Task RecivedAsync(IChannel channel, object message)
        {
            foreach (var listener in ChannelHandlers)
            {
                try
                {
                   await listener.RecivedAsync(channel,message);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }

        public async Task CaughtAsync(IChannel channel, Exception exception)
        {
            foreach (var listener in ChannelHandlers)
            {
                try
                {
                   await listener.CaughtAsync(channel,exception);
                }
                catch (Exception ex)
                {
                    _log.Error(ex);
                }
            }
        }
    }
}
