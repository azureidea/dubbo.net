using DotNetty.Transport.Channels;

namespace Dubbo.Net.Common.Handlers
{
    public class NettyHandler:ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            var msg = message as Response;
            var result = msg.Mresult;
        }
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }
    }
}
