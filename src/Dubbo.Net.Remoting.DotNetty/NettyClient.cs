using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting.Transport;
using Dubbo.Net.Rpc.Procotol.Dubbo;

namespace Dubbo.Net.Remoting.Netty
{
    public class NettyClient:AbstractClient
    {

        private Bootstrap _bootstrap;
        private DotNetty.Transport.Channels.IChannel _channel; 
        public NettyClient(URL url, IChannelHandler handler) : base(url, WrapChannelHandler(url,handler))
        {
        }

        protected override async Task DoOpenAsync()
        {
             _bootstrap = new Bootstrap();
            _bootstrap.Channel<TcpSocketChannel>()
                .Group(new MultithreadEventLoopGroup(1))
                .Option(ChannelOption.SoKeepalive, true)
                .Option(ChannelOption.TcpNodelay, true)
                .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(3))
                //.Option(ChannelOption.Allocator, PooledByteBufferAllocator.Default)
                .Handler(new ActionChannelInitializer<ISocketChannel>(c =>
                {
                    var nettyClientHandler = new NettyClientHandler(Url, this);
                    var codec = new DubboCountCodec();
                    var nettyAdapter = new NettyCodecAdapter(codec, Url, this);
                    var pipeline = c.Pipeline;
                    pipeline.AddLast("decoder", nettyAdapter.GetDecoder());
                    pipeline.AddLast("encoder", nettyAdapter.GetEncoder());
                    pipeline.AddLast("handler", nettyClientHandler);
                }));
            try
            {
                _channel =   await _bootstrap.ConnectAsync(Url.Ip, Url.Port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override Task DoCloseAsync()
        {
            return _channel.CloseAsync();
        }

        protected override Task DoConnectAsync()
        {
            return DoOpenAsync();
        }

        protected override Task DoDisConnectAsync()
        {
            return _channel.DisconnectAsync();
        }

        protected override IChannel GetChannel()
        {
            return new NettyChannel(_channel,Url,ChannelHander);
        }
    }
}
