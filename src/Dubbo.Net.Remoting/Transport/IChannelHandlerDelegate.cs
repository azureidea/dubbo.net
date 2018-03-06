namespace Dubbo.Net.Remoting.Transport
{
    public interface IChannelHandlerDelegate:IChannelHandler
    {
        IChannelHandler GetHandler();
    }
}
