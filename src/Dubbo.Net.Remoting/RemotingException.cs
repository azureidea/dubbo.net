using System;
using System.Net;

namespace Dubbo.Net.Remoting
{
    public class RemotingException:Exception
    {
        public EndPoint LocalAddress { get; }
        public EndPoint RemoteAddress { get; }

        public RemotingException(EndPoint localAddress,EndPoint remoteAddress,string message,Exception ex):base(message,ex)
        {
            LocalAddress = localAddress;
            RemoteAddress = remoteAddress;
        }

        public RemotingException(EndPoint localAddress, EndPoint remoteAddress, Exception ex) : base(ex.Message,ex)
        {
            LocalAddress = localAddress;
            RemoteAddress = remoteAddress;
        }
        public RemotingException(EndPoint localAddress, EndPoint remoteAddress, string msg) : base(msg)
        {
            LocalAddress = localAddress;
            RemoteAddress = remoteAddress;
        }
        public RemotingException(IChannel channel,Exception ex):this(channel?.Address, channel?.RemoteAddress,ex)
        {

        }
        public RemotingException(IChannel channel, string message, Exception ex)
            : this(channel?.Address, channel?.RemoteAddress,message, ex) {
        }
        public RemotingException(IChannel channel,string message):
            this(channel?.Address, channel?.RemoteAddress, message)
        {

        }
    }
}
