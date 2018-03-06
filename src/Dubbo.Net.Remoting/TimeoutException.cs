namespace Dubbo.Net.Remoting
{
    public class TimeoutException:RemotingException
    {
        public const int ClientSide = 0;
        public const int ServerSide = 1;
        private const long SerialVersionUID = 3122966731958222692L;
        public int Phase { get; }

        public TimeoutException(bool serverSide,IChannel channel,string msg) : base(channel, msg)
        {
            Phase = serverSide ? ServerSide : ClientSide;
        }
        public bool IsServerSide() { return Phase == ServerSide; }
        public bool IsClientSide() { return Phase == ClientSide; }
    }
}
