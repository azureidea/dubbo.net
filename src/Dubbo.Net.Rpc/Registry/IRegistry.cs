using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Rpc.Infrastructure;

namespace Dubbo.Net.Rpc.Registry
{
    public interface IRegistry:INode,IRegistryService
    {
        void Start();
    }
}
