using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common
{
    public class Response
    {
        public const string HeartbeatEvent = null;

        public const string ReadonlyEvent = "R";

        /**
         * ok.
         */
        public const byte Ok = 20;

        /**
         * clien side timeout.
         */
        public const byte ClientTimeout = 30;

        /**
         * server side timeout.
         */
        public const byte ServerTimeout = 31;

        /**
         * request format error.
         */
        public const byte BadRequest = 40;

        /**
         * response format error.
         */
        public const byte BadResponse = 50;

        /**
         * service not found.
         */
        public const byte ServiceNotFound = 60;

        /**
         * service error.
         */
        public const byte ServiceError = 70;

        /**
         * internal server error.
         */
        public const byte ServerError = 80;

        /**
         * internal server error.
         */
        public const byte ClientError = 90;

        /**
         * server side threadpool exhausted and quick return.
         */
        public const byte ServerThreadpoolExhaustedError = 100;
        public long Mid { get; set; }
        public string Mversion { get; set; }
        public byte Mstatus { get; set; } = Ok;
        public bool Mevent { get; set; }
        public string MerrorMsg { get; set; }
        public object Mresult { get; set; }

        public Response() { }
        public Response(long id)
        {
            Mid = id;
        }
        public Response(long id, string version)
        {
            Mid = id;
            Mversion = version;
        }

        public void SetEvent(string sEvent)
        {
            Mevent = true;
            Mresult = sEvent;
        }

        public bool IsHeartBeat() => Mevent && HeartbeatEvent == (string)Mresult;

        public override string ToString()
        {
            return "Response [id=" + Mid + ", version=" + Mversion + ", status=" + Mstatus + ", event=" + Mevent
                + ", error=" + MerrorMsg + ", result=" + (Mresult == this ? "this" : Mresult) + "]";
        }
    }
}
