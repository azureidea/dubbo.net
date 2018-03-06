using System.Net;

namespace Dubbo.Net.Remoting
{
    public interface IChannel:IEndpoint
    {
        EndPoint RemoteAddress {get;}
        bool IsConnected {get;}
        bool HasAttribute(string key);
        object GetAttribute(string key);
        void SetAttribute(string key, object value);
        void RemoveAttribute(string key);
    }
}
