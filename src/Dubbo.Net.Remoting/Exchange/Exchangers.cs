using System;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting.Exchange.Support;
using Dubbo.Net.Remoting.Transport;

namespace Dubbo.Net.Remoting.Exchange
{
    public class Exchangers
    {
        public static Task<IExchangeServer> BindAsync(string url, IReplier replier)
        {
            return BindAsync(URL.ValueOf(url), replier);
        }

        public static Task<IExchangeServer> BindAsync(URL url, IReplier replier)
        {
            return BindAsync(url, new ChannelHandlerAdapter(), replier);
        }

        public static Task<IExchangeServer> BindAsync(string url, IChannelHandler handler, IReplier replier)
        {
            return BindAsync(URL.ValueOf(url), handler, replier);
        }

        public static Task<IExchangeServer> BindAsync(URL url, IChannelHandler handler, IReplier replier)
        {
            return BindAsync(url, new ExchangeHandlerDispatcher(replier, handler));
        }

        public static Task<IExchangeServer> BindAsync(string url, IExchangeHandler handler)
        {
            return BindAsync(URL.ValueOf(url), handler);
        }

        public static Task<IExchangeServer> BindAsync(URL url, IExchangeHandler handler)
        {
            if (url == null)
            {
                throw new ArgumentException("url == null");
            }
            if (handler == null)
            {
                throw new ArgumentException("handler == null");
            }
            url = url.AddParameterIfAbsent(Constants.CodecKey, "exchange");
            return GetExchanger(url).BindAsync(url, handler);
        }

        public static Task<IExchangeClient> ConnectAsync(string url)
        {
            return ConnectAsync(URL.ValueOf(url));
        }

        public static Task<IExchangeClient> ConnectAsync(URL url)
        {
            return ConnectAsync(url, new ChannelHandlerAdapter(), null);
        }

        public static Task<IExchangeClient> ConnectAsync(string url, IReplier replier)
        {
            return ConnectAsync(URL.ValueOf(url), new ChannelHandlerAdapter(), replier);
        }

        public static Task<IExchangeClient> ConnectAsync(URL url, IReplier replier)
        {
            return ConnectAsync(url, new ChannelHandlerAdapter(), replier);
        }

        public static Task<IExchangeClient> ConnectAsync(string url, IChannelHandler handler, IReplier replier)
        {
            return ConnectAsync(URL.ValueOf(url), handler, replier);
        }

        public static Task<IExchangeClient> ConnectAsync(URL url, IChannelHandler handler, IReplier replier)
        {
            return ConnectAsync(url, new ExchangeHandlerDispatcher(replier, handler));
        }

        public static Task<IExchangeClient> ConnectAsync(string url, IExchangeHandler handler)
        {
            return ConnectAsync(URL.ValueOf(url), handler);
        }

        public static async Task<IExchangeClient> ConnectAsync(URL url, IExchangeHandler handler)
        {
            if (url == null)
            {
                throw new ArgumentException("url == null");
            }
            if (handler == null)
            {
                throw new ArgumentException("handler == null");
            }
            url = url.AddParameterIfAbsent(Constants.CodecKey, "exchange");
            return await GetExchanger(url).ConnectAsync(url, handler);
        }

        public static IExchanger GetExchanger(URL url)
        {
            string type = url.GetParameter(Constants.ExchangerKey, Constants.DefaultExchanger);
            return GetExchanger(type);
        }

        public static IExchanger GetExchanger(string type)
        {
            return ObjectFactory.GetInstance<IExchanger>(type);
        }
}
}
