using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Dubbo.Net.Common
{
    public class Request
    {
        public const string HeartBeatEvent = null;
        public const string ReadonlyEvent = "R";
        private static long _invokeId = 0;

        public long Mid { get; set; }
        public string Mversion { get; set; }
        public bool IsTwoWay { get; set; }
        public bool IsEvent { get; set; }
        public bool IsBroken { get; set; }
        public object Mdata { get; set; }

        public Request()
        {
            Mid = NewId();
        }
        public Request(long id)
        {
            Mid = id;
        }

        public void SetEvent(string eve)
        {
            IsEvent = true;
            Mdata = eve;
        }

        public bool IsHeartbeat()
        {
            return IsEvent && HeartBeatEvent == Mdata as string;
        }
        private long NewId()
        {
            return Interlocked.Increment(ref _invokeId);
        }
    }
}
