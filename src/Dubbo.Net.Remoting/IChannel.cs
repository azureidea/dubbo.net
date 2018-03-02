using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Dubbo.Net.Common;

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
