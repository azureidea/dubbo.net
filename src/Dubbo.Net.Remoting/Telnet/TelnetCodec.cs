using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DotNetty.Buffers;
using Dubbo.Net.Common;
using Dubbo.Net.Remoting.Transport;

namespace Dubbo.Net.Remoting.Telnet
{
    public class TelnetCodec : TransportCodec
    {

        private const string HistoryListKey = "telnet.history.list";
        private const string HistoryIndexKey = "telnet.history.index";
        private static readonly byte[] Up = { 27, 91, 65 };
        private static readonly byte[] Down = { 27, 91, 66 };
        private static readonly List<byte[]> Enter = new List<byte[]> { new[] { (byte)'\r', (byte)'\n' }, new[] { (byte)'\n' } };

        private static readonly List<byte[]> Exit = new List<byte[]>
        {
            new byte[] {3},
            new byte[]
            {
                unchecked((byte) -1), unchecked((byte) -12), unchecked((byte) -1), unchecked((byte) -3), 6
            } /* Linux Ctrl+C */,
            new byte[]
            {
                unchecked((byte) -1), unchecked((byte) -19), unchecked((byte) -1), unchecked((byte) -3), 6
            } /* Linux Pause */
        };

        private static string GetCharSet(IChannel channel)
        {
            if (channel != null)
            {
                string attribute = channel.GetAttribute("charset") as string;
                if (!string.IsNullOrEmpty(attribute))
                    return attribute;
                URL url = channel.Url;
                if (url != null)
                {
                    String parameter = url.GetParameter("charset", "UTF-8");
                    if (!string.IsNullOrEmpty(parameter))
                    {
                        return parameter;
                    }
                }
            }

            return "UTF-8";
        }

        private static string ToString(byte[] message, string charset)
        {
            byte[] copy = new byte[message.Length];
            int index = 0;
            for (int i = 0; i < message.Length; i++)
            {
                byte b = message[i];
                if (b == '\b')
                { // backspace
                    if (index > 0)
                    {
                        index--;
                    }
                    if (i > 2 && (short)message[i - 2] < 0)
                    { // double byte char
                        if (index > 0)
                        {
                            index--;
                        }
                    }
                }
                else if (b == 27)
                { // escape
                    if (i < message.Length - 4 && message[i + 4] == 126)
                    {
                        i = i + 4;
                    }
                    else if (i < message.Length - 3 && message[i + 3] == 126)
                    {
                        i = i + 3;
                    }
                    else if (i < message.Length - 2)
                    {
                        i = i + 2;
                    }
                }
                else if (b == unchecked((byte)-1) && i < message.Length - 2
                                 && (message[i + 1] == unchecked((byte)-3) || message[i + 1] == unchecked((byte)-5)))
                { // handshake
                    i = i + 2;
                }
                else
                {
                    copy[index++] = message[i];
                }
            }
            if (index == 0)
            {
                return "";
            }

            var str = Encoding.GetEncoding(charset).GetString(copy, 0, index);
            return str;
        }

        private static bool IsEquals(byte[] message, byte[] command)
        {
            return message.Length == command.Length && EndsWith(message, command);
        }

        private static bool EndsWith(byte[] message, byte[] command)
        {
            if (message.Length < command.Length)
                return false;
            var off = message.Length - command.Length;
            for (var i = command.Length - 1; i >= 0; i--)
            {
                if (message[off + i] != command[i])
                    return false;
            }

            return true;
        }

        public override void Encode(IChannel channel, IByteBuffer buffer, object message)
        {
            if (message is string)
            {
                if (IsClientSide(channel))
                    message += "\r\n";
                var msgData = Encoding.GetEncoding(GetCharSet(channel)).GetBytes(message.ToString());
                buffer.WriteBytes(msgData);
            }
            base.Encode(channel, buffer, message);
        }

        public override object Decode(IChannel channel, IByteBuffer buffer)
        {
            var readable = buffer.ReadableBytes;
            var message = new byte[readable];
            buffer.ReadBytes(message);
            return Decode(channel, buffer, readable, message);
        }

        protected virtual object Decode(IChannel channel, IByteBuffer buffer, int readable, byte[] message)
        {
            if (IsClientSide(channel))
            {
                return ToString(message, GetCharSet(channel));
            }
            CheckPayload(channel, readable);
            if (message == null || message.Length == 0)
            {
                return DecodeResult.NeedMoreInput;
            }

            if (message[message.Length - 1] == '\b')
            {
                try
                {
                    var doubleChar = message.Length >= 3 && (short)message[message.Length - 3] < 0;
                    var data = doubleChar ? new byte[] { 32, 32, 8, 8 } : new byte[] { 32, 8 };
                    var msg = Encoding.GetEncoding(GetCharSet(channel)).GetString(data);
                    channel.SendAsync(msg);
                }
                catch (RemotingException e)
                {
                    throw new IOException(e.ToString());
                }

                return DecodeResult.NeedMoreInput;
            }
            foreach (var cmd in Exit)
            {
                if (IsEquals(message, cmd))
                {
                    channel.CloseAsync();
                    return null;
                }
            }

            List<string> history;
            int? index;
            var up = EndsWith(message, Up);
            var down = EndsWith(message, Down);
            if (up || down)
            {
                history = (List<string>)channel.GetAttribute(HistoryListKey);
                if (history == null || history.Count == 0)
                {
                    return DecodeResult.NeedMoreInput;
                }
                index = (int?)channel.GetAttribute(HistoryIndexKey);
                var old = index;
                if (index == 0)
                {
                    index = history.Count - 1;
                }
                else
                {
                    if (up)
                    {
                        index = index - 1;
                        if (index < 0)
                        {
                            index = history.Count - 1;
                        }
                    }
                    else
                    {
                        index = index + 1;
                        if (index > history.Count - 1)
                        {
                            index = 0;
                        }
                    }
                }
                if (old == null || !old.Equals(index))
                {
                    channel.SetAttribute(HistoryIndexKey, index);
                    var value = history[index ?? 0];
                    if (old != null && old >= 0 && old < history.Count)
                    {
                        string ov = history[old ?? 0];
                        StringBuilder buf = new StringBuilder();
                        for (int i = 0; i < ov.Length; i++)
                        {
                            buf.Append("\b");
                        }
                        for (int i = 0; i < ov.Length; i++)
                        {
                            buf.Append(" ");
                        }
                        for (int i = 0; i < ov.Length; i++)
                        {
                            buf.Append("\b");
                        }
                        value = buf.ToString() + value;
                    }
                    try
                    {
                        channel.SendAsync(value);
                    }
                    catch (RemotingException e)
                    {
                        throw new IOException(e.ToString());
                    }
                }
                return DecodeResult.NeedMoreInput;
            }
            foreach (var command in Exit)
            {
                if (IsEquals(message, command))
                {
                    channel.CloseAsync();
                    return null;
                }
            }
            byte[] enter = null;
            foreach (var cmd in Enter)
            {
                if (EndsWith(message, (byte[])cmd))
                {
                    enter = (byte[])cmd;
                    break;
                }
            }
            if (enter == null)
            {
                return DecodeResult.NeedMoreInput;
            }
            history = (List<string>)channel.GetAttribute(HistoryListKey);
            index = (int?)channel.GetAttribute(HistoryIndexKey);
            channel.RemoveAttribute(HistoryIndexKey);
            if (history != null && history.Count > 0 && index != null && index >= 0 && index < history.Count)
            {
                var value = history[(int)index];
                if (value != null)
                {
                    byte[] b1 = Encoding.GetEncoding(GetCharSet(channel)).GetBytes(value);
                    if (message.Length > 0)
                    {
                        byte[] b2 = new byte[b1.Length + message.Length];
                        System.Array.Copy(b1, 0, b2, 0, b1.Length);
                        System.Array.Copy(message, 0, b2, b1.Length, message.Length);
                        message = b2;
                    }
                    else
                    {
                        message = b1;
                    }
                }
            }
            var result = ToString(message, GetCharSet(channel));
            if (!string.IsNullOrEmpty(result))
            {
                if (history == null)
                {
                    history = new List<string>();
                    channel.SetAttribute(HistoryListKey, history);
                }
                if (history.Count == 0)
                {
                    history.Add(result);
                }
                else if (!result.Equals(history[history.Count-1]))
                {
                    history.Remove(result);
                    history.Add(result);
                    if (history.Count > 10)
                    {
                        history.RemoveAt(0);
                    }
                }
            }
            return result;
        }
    }
}
