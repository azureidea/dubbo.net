using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Remoting.Exchange.Support;

namespace Dubbo.Net.Remoting.Transport
{
    public class MultiMessageHandler:AbstractChannelHandlerDelegate
    {
        public MultiMessageHandler(IChannelHandler handler) : base(handler)
        {
        }

        public override async Task RecivedAsync(IChannel channel, object message)
        {
            if (message is MultiMessage list) {
                foreach (var obj in list.GetEnumerator())
                {
                   await _handler.RecivedAsync(channel, obj);
                }
            } else {
               await _handler.RecivedAsync(channel, message);
            }
        }
    }
}
