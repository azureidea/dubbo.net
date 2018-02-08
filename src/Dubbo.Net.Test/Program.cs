using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Handlers;
using Dubbo.Net.Common.Serialize.Supports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Rpc;
using Dubbo.Net.Rpc.Procotol;

namespace Dubbo.Net.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var bootstrap = new Bootstrap();
            //bootstrap.Channel<TcpSocketChannel>()
            //    .Option(ChannelOption.TcpNodelay, true)
            //    .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(3))
            //    .Group(new MultithreadEventLoopGroup())
            //    .Option(ChannelOption.SoKeepalive, true)
            //    .Handler(new ActionChannelInitializer<ISocketChannel>(c =>
            //    {
            //        var pipeline = c.Pipeline;
            //        var serialization = new FastJsonSerialization();
            //        var codec = new DubboCodec();
            //        pipeline.AddLast(new LoggingHandler("CLT-CONN"));
            //        pipeline.AddLast("decoder", new InternalClientDecoder());
            //        pipeline.AddLast("encoder", new InternalClientEncoder());
            //        pipeline.AddLast("handler", new NettyHandler());
            //    }));
            //var channel = bootstrap.ConnectAsync("192.168.2.90", 20881).Result;
            //var request = new Request();
            //request.IsTwoWay = true;
            //request.Mversion = "2.0.0";
            //var inv = new RpcInvocation();
            //inv.SetAttachment("path", "com.mc.userconnect.api.service.PafUserService");
            //inv.SetAttachment("interface", "com.mc.userconnect.api.service.PafUserService");
            //inv.SetAttachment("timeout", "10000");
            //inv.SetAttachment("version", "0.0.0");
            //inv.ParameterTypes=new Type[] { typeof(UserSyncNotify) };
            //var notify = new UserSyncNotify
            //{
            //    version = "1.0",
            //    merchantId = "abcd",
            //    mid = "abcd",
            //    uid = "abcd",
            //    status = "abcd",
            //    token = "abcd",
            //    signature = "abcd",
            //};
            //inv.MethodName = "userSync";
            //inv.Arguments=new object[] { notify };
            //request.Mdata = inv;
            //channel.WriteAndFlushAsync(request);
            //Console.ReadLine();
        }
    }
}
