using System;
using System.Net;

namespace Dubbo.Net.Remoting
{
    public class ExecutionException: RemotingException
    {
        public object Request { get; }
        public ExecutionException(object request, IChannel channel, string message, Exception cause):base(channel, message, cause)
        {
            Request = request;
        }

        public ExecutionException(object request, IChannel channel, string msg):base(channel, msg)
        {
            Request = request;
        }

        public ExecutionException(object request, IChannel channel, Exception cause):base(channel, cause)
        {
            Request = request;
        }

        public ExecutionException(object request, EndPoint localAddress, EndPoint remoteAddress, string message,
                                  Exception cause): base(localAddress, remoteAddress, message, cause)
        {
            
            Request = request;
        }

        public ExecutionException(object request, EndPoint localAddress, EndPoint remoteAddress, string message): base(localAddress, remoteAddress, message)
        {
            
            Request = request;
        }

        public ExecutionException(object request, EndPoint localAddress, EndPoint remoteAddress, Exception cause):base(localAddress, remoteAddress, cause)
        {
            Request = request;
        }


        
    }
}
