using Dubbo.Net.Common;
using Dubbo.Net.Remoting.Transport;
using System;

namespace Dubbo.Net.Remoting
{
    public class Transporters
    {
        static Transporters()
        {
            //todo version check
        }

        private Transporters()
        {

        }

        public static IServer Bind(string url, params IChannelHandler[] handlers)
        {
            return Bind(URL.ValueOf(url), handlers);
        }
        public static IServer Bind(URL url, params IChannelHandler[] handlers)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (handlers == null || handlers.Length == 0)
                throw new ArgumentNullException("handlers");
            IChannelHandler handler;
            if (handlers.Length == 1)
            {
                handler = handlers[0];
            }
            else
            {
                handler = new ChannelHandlerDispatcher(handlers);
            }
            return GetTransporters().Bind(url, handler);
        }

        public static IClient Connect(string url, params IChannelHandler[] handlers)
        {
            return Connect(URL.ValueOf(url), handlers);
        }
        public static IClient Connect(URL url, params IChannelHandler[] handlers)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url == null");
            }
            IChannelHandler handler;
            if (handlers == null || handlers.Length == 0)
            {
                handler = new ChannelHandlerAdapter();
            }
            else if (handlers.Length == 1)
            {
                handler = handlers[0];
            }
            else
            {
                handler = new ChannelHandlerDispatcher(handlers);
            }
            return GetTransporters().Connected(url, handler);
        }

        public static ITransporter GetTransporters()
        {
            //todo get transporters from container
            throw new NotImplementedException();
        }
    }
}
