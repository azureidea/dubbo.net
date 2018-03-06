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
        public const string GroupKey = "group";
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
        public const string SendReconnectKey = "send.reconnect";
        public const int DefaultReconnectPeriod = 2000;

        public const string ShutdownTimeoutKey = "shutdown.timeout";

        public const int DefaultShutdownTimeout = 1000 * 60 * 15;
        public const string CheckKey = "check";
        public const string HeartbeatKey = "heartbeat";
        public const string ThreadpoolKey = "threadpool";
        public const string DefaultClientThreadpool = "cached";
        public const string ReconnectKey = "reconnect";
        public const int DefaultServerShutdownTimeout = 10000;
        public const string ShutdownWaitKey = "dubbo.service.shutdown.wait";
        public const string ShutdownWaitSecondsKey = "dubbo.service.shutdown.wait.seconds";
        public const string LazyConnectInitialStateKey = "connect.lazy.initial.state";
        public const bool  DefaultLazyConnectInitialState = true;

        public const string PromptKey = "prompt";

        public const string DefaultPrompt = "dubbo>";
        public const string StubEventKey = "dubbo.stub.event";
        public const string  CallbackServiceKey = "callback.service.instid";
        public const string InterfaceKey = "interface";
        public const string OnConnectKey = "onconnect";
        public const  string OnDisconnectKey = "ondisconnect";
        public const bool  DefaultStubEvent = false;
        public const string IsCallbackService = "is_callback_service";
        public const string StubEventMethodsKey = "dubbo.stub.event.methods";
        public const string IsServerKey = "isserver";
        public const string ChannelReadonlyeventSentKey = "channel.readonly.sent";
        public const int  DefaultHeartbeat = 60 * 1000;
        public const string ServerKey = "server";
        public const string DefaultRemotingServer = "netty";
        public const string DefaultRemotingClient = "netty";
        public const string ClientKey = "client";
        public const string OptimizerKey = "optimizer";
        public const string ExchangerKey = "exchanger";
        public const string DefaultExchanger = "header";
        public const string ChannelAttributeReadonlyKey = "channel.readonly";
        public const string TokenKey = "token";
        public const string ConnectionsKey = "connections";
        public const string  LazyConnectKey = "lazy";
        public const string HeartbeatTimeoutKey = "heartbeat.timeout";
        public const string SideKey = "side";
        public const string ProviderSide = "provider";

        public const string ConsumerSide = "consumer";
        public const string TimestampKey = "timestamp";
        public const string PidKey = "pid";
    }
}
