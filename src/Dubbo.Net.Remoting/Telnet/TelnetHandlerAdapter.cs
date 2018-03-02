using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Remoting.Transport;

namespace Dubbo.Net.Remoting.Telnet
{
    public class TelnetHandlerAdapter : ChannelHandlerAdapter, ITelnetHandler
    {
        public virtual string Telnet(IChannel channel, string msg)
        {
            String prompt = channel.Url.GetParameterAndDecoded(Constants.PromptKey, Constants.DefaultPrompt);
            var noprompt = msg.Contains("--no-prompt");
            msg = msg.Replace("--no-prompt", "");
            StringBuilder buf = new StringBuilder();
            msg = msg.Trim();
            String command;
            if (msg.Length > 0)
            {
                int i = msg.IndexOf(' ');
                if (i > 0)
                {
                    command = msg.Substring(0, i).Trim();
                    msg = msg.Substring(i + 1).Trim();
                }
                else
                {
                    command = msg;
                    msg = "";
                }
            }
            else
            {
                command = "";
            }
            if (command.Length > 0)
            {
                try
                {

                    var handler = ObjectFactory.GetInstance<ITelnetHandler>(command);
                    var result = handler.Telnet(channel, msg);
                    if (result == null)
                    {
                        return null;
                    }
                    buf.Append(result);
                }
                catch (Exception t)
                {
                    buf.Append("Unsupported command: ");
                    buf.Append(command);
                }
            }
            if (buf.Length > 0)
            {
                buf.Append("\r\n");
            }
            if (!string.IsNullOrEmpty(prompt) && !noprompt)
            {
                buf.Append(prompt);
            }
            return buf.ToString();
        }
    }
}
