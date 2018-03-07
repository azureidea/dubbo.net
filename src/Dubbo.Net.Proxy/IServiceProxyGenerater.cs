using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Dubbo.Net.Proxy
{
    public interface IServiceProxyGenerater
    {
       Type GenerateProxys(Type interfaceType);
        //SyntaxTree GenerateProxyTree(Type interfaceType);
    }
}
