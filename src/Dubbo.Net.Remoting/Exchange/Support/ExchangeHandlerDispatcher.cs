using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting.Telnet;
using Dubbo.Net.Remoting.Transport;

namespace Dubbo.Net.Remoting.Exchange.Support
{
    public class ExchangeHandlerDispatcher:IExchangeHandler
    {
        private readonly ITelnetHandler _telnetHandler;
        private readonly ReplierDispatcher _replierDispatcher;
        private readonly ChannelHandlerDispatcher _handlerDispatcher;
        public ExchangeHandlerDispatcher()
        {
            _replierDispatcher = new ReplierDispatcher();
            _handlerDispatcher = new ChannelHandlerDispatcher();
            _telnetHandler = new TelnetHandlerAdapter();
        }

        public ExchangeHandlerDispatcher(IReplier replier)
        {
            _replierDispatcher = new ReplierDispatcher(replier);
            _handlerDispatcher = new ChannelHandlerDispatcher();
            _telnetHandler = new TelnetHandlerAdapter();
        }

        public ExchangeHandlerDispatcher(params IChannelHandler[] handlers)
        {
            _replierDispatcher = new ReplierDispatcher();
            _handlerDispatcher = new ChannelHandlerDispatcher(handlers);
            _telnetHandler = new TelnetHandlerAdapter();
        }

        public ExchangeHandlerDispatcher(IReplier replier, params IChannelHandler[] handlers)
        {
            _replierDispatcher = new ReplierDispatcher(replier);
            _handlerDispatcher = new ChannelHandlerDispatcher(handlers);
            _telnetHandler = new TelnetHandlerAdapter();
        }

        public ExchangeHandlerDispatcher addChannelHandler(IChannelHandler handler)
        {
            _handlerDispatcher.AddChannelHandler(handler);
            return this;
        }

        public ExchangeHandlerDispatcher removeChannelHandler(IChannelHandler handler)
        {
            _handlerDispatcher.RemoveChannelHandler(handler);
            return this;
        }

        public  ExchangeHandlerDispatcher AddReplier(Type type, IReplier replier)
        {
            _replierDispatcher.AddReplier(type, replier);
            return this;
        }

        public  ExchangeHandlerDispatcher RemoveReplier(Type type)
        {
            _replierDispatcher.RemoveReplier(type);
            return this;
        }

        public Task ConnectAsync(IChannel channel)
        {
            return _handlerDispatcher.ConnectAsync(channel);
        }

        public Task DisconnectAsync(IChannel channel)
        {
            return _handlerDispatcher.DisconnectAsync(channel);
        }

        public Task SentAsync(IChannel channel, object message)
        {
            return _handlerDispatcher.SentAsync(channel, message);
        }

        public Task RecivedAsync(IChannel channel, object message)
        {
            return _handlerDispatcher.RecivedAsync(channel, message);
        }

        public Task CaughtAsync(IChannel channel, Exception exception)
        {
            return _handlerDispatcher.CaughtAsync(channel, exception);
        }

        public string Telnet(IChannel channel, string msg)
        {
            return _telnetHandler.Telnet(channel, msg);
        }

        public  Task<object> Reply(IExchangeChannel channel, object request)
        {
            return _replierDispatcher.ReplyAsync(channel, request);
        }
    }
}
