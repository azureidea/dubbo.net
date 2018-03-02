using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting.Transport;

namespace Dubbo.Net.Remoting.Exchange.Support.Header
{
    public class HeartbeatHandler:AbstractChannelHandlerDelegate
    {
        private const string KeyReadTimstamp = "READ_TIMESTAMP";
        private const string KeyWriteTimestamp = "WRITE_TIMESTAMP";
        private static ILogger _logger;
        public HeartbeatHandler(IChannelHandler handler) : base(handler)
        {
        }

        public override Task ConnectAsync(IChannel channel)
        {
            SetReadTimestamp(channel);
            SetWriteTimestamp(channel);
            return _handler.ConnectAsync(channel);
        }

        public override Task DisconnectAsync(IChannel channel)
        {
            ClearReadTimestamp(channel);
            ClearWriteTimestamp(channel);
            return _handler.DisconnectAsync(channel);
        }

        public override Task SentAsync(IChannel channel, object message)
        {
            SetReadTimestamp(channel);
            return _handler.SentAsync(channel, message);
        }

        public override async Task RecivedAsync(IChannel channel, object message)
        {
            SetReadTimestamp(channel);
            if (IsHeartbeatRequest(message))
            {
                Request req = (Request) message;
                if (req.IsTwoWay)
                {
                    Response res=new Response(req.Mid,req.Mversion);
                    res.SetEvent(Response.HeartbeatEvent);
                    await channel.SendAsync(res);
                    if (_logger.InfoEnabled)
                    {
                        int heartbeat = channel.Url.GetParameter(Constants.HeartbeatKey, 0);
                        if (_logger.DebugEnabled)
                        {
                            _logger.Debug("Received heartbeat from remote channel " + channel.RemoteAddress
                                                                                   + ", cause: The channel has no data-transmission exceeds a heartbeat period"
                                                                                   + (heartbeat > 0 ? ": " + heartbeat + "ms" : ""));
                        }
                    }
                }
                return;
            }
            await _handler.RecivedAsync(channel, message);
        }

      



        private void SetReadTimestamp(IChannel channel)
        {
            channel.SetAttribute(KeyReadTimstamp, DateTime.Now.ToTimestamp());
        }

        private void SetWriteTimestamp(IChannel channel)
        {
            channel.SetAttribute(KeyWriteTimestamp, DateTime.Now.ToTimestamp());
        }

        private void ClearReadTimestamp(IChannel channel)
        {
            channel.RemoveAttribute(KeyReadTimstamp);
        }

        private void ClearWriteTimestamp(IChannel channel)
        {
            channel.RemoveAttribute(KeyWriteTimestamp);
        }

        private bool IsHeartbeatRequest(object message)
        {
            return message is Request request && request.IsHeartbeat();
        }

        private bool IsHeartbeatResponse(object message)
        {
            return message is Response response && response.IsHeartBeat();
        }
    }
}
