using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Netty
{
    public class NettyClientHandler : ChannelDuplexHandler
    {
        private readonly URL _url;

        private readonly IChannelHandler _handler;

        public NettyClientHandler(URL url, IChannelHandler handler)
        {
            this._url = url ?? throw new ArgumentException("url == null");
            this._handler = handler ?? throw new ArgumentException("handler == null");
        }


        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            ctx.FireChannelActive();
        }


        public override void ChannelInactive(IChannelHandlerContext ctx)
        {
            ctx.FireChannelInactive();
        }


        public override async Task DisconnectAsync(IChannelHandlerContext ctx)
        {
            NettyChannel channel = NettyChannel.GetOrAddChannel(ctx.Channel, _url, _handler);
            try
            {
               await _handler.DisconnectAsync(channel);
            }
            finally
            {
                NettyChannel.RemoveChannelIfDisconnected(ctx.Channel);
            }
        }


        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            NettyChannel channel = NettyChannel.GetOrAddChannel(ctx.Channel, _url, _handler);
            try
            {
                //Console.WriteLine("channel read:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                _handler.RecivedAsync(channel, msg).Wait();
               // Console.WriteLine("channel read completed:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            finally
            {
                NettyChannel.RemoveChannelIfDisconnected(ctx.Channel);
            }
        }



        public override async Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            await base.WriteAsync(ctx, msg);
            NettyChannel channel = NettyChannel.GetOrAddChannel(ctx.Channel, _url, _handler);
            try
            {
               await _handler.SentAsync(channel, msg);
            }
            finally
            {
                NettyChannel.RemoveChannelIfDisconnected(ctx.Channel);
            }
        }


        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception cause)
        {
            NettyChannel channel = NettyChannel.GetOrAddChannel(ctx.Channel, _url, _handler);
            try
            {
                _handler.CaughtAsync(channel, cause);
            }
            finally
            {
                NettyChannel.RemoveChannelIfDisconnected(ctx.Channel);
            }
        }
    }
}
