using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common
{
    /// <summary>
    /// dubbo相关常量
    /// </summary>
    public struct Constants
    {
        /// <summary>
        /// dubbo版本号key
        /// </summary>
        public const string DubboVersionKey = "dubbo";
        /// <summary>
        /// 路径key
        /// </summary>
        public const string PathKey = "path";
        /// <summary>
        /// 版本key
        /// </summary>
        public const string VersionKey = "version";

        public const string DecodeInIoThreadKey = "decode.in.io";
        public const bool DefaultDecodeInIoThread = true;
        public const string SerializationKey = "serialization";
        public const byte DefaultRemotingSerialization = 6;
        public const string IdKey = "id";
        public const string AutoAttachInvocationIdKey = "invocationid.autoattach";
        public const string Invoke = "$invoke";
        public const string AsyncKey = "async";
        public const string ReturnKey = "return";
        public const string InputKey = "input";
        public const string OutputKey = "output";
        public const string SentKey = "sent";
        public const string CodecKey = "codec";
        public const string TimeoutKey = "timeout";
        public const int DefaultTimeout = 1000;
        public const int DefaultConnectTimeout = 3000;
        public const string ConnectTimeoutKey = "connect.timeout";
    }
}
