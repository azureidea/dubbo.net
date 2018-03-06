using System.Net;
using System.Threading.Tasks;
using Dubbo.Net.Common;

namespace Dubbo.Net.Remoting.Transport
{
    public abstract class AbstractChannel:AbstractPeer,IChannel
    {
        public AbstractChannel(URL url, IChannelHandler handler) : base(url, handler)
        {
        }

        public override async Task<Response> SendAsync(object message, bool sent)
        {
            if (IsClosed)
            {
                throw new RemotingException(this, "Failed to send message "
                                                  + (message == null ? "" : message.GetType().Name) + ":" + message
                                                  + ", cause: Channel closed. channel: " + Address + " -> " + RemoteAddress);
            }

            return null;
        }


        public abstract EndPoint RemoteAddress { get;  }
        public abstract bool IsConnected { get; }
        public abstract bool HasAttribute(string key);
        public abstract object GetAttribute(string key);
        public abstract void SetAttribute(string key, object value);
        public abstract void RemoveAttribute(string key);
    }
}
