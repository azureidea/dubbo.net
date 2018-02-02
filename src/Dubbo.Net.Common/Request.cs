using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dubbo.Net.Common
{
    public class Request
    {
        public const string HeartBeat_Event = null;
        public const string Readonly_Event = "R";
        private long invokeId = 0;

        public long Mid { get; set; }
        public string Mversion { get; set; }
        public bool IsTwoWay { get; set; }
        public bool IsEvent { get; set; }
        public bool IsBroken { get; set; }
        public RpcInvocation Mdata { get; set; }

        public Request()
        {
            Mid = NewId();
        }
        public Request(long id)
        {
            Mid = id;
        }
        private long NewId()
        {
            return Interlocked.Increment(ref invokeId);
        }
    }
}
