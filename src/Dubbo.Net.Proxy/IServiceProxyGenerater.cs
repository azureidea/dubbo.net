using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Dubbo.Net.Proxy
{
    public interface IServiceProxyGenerater
    {
       IEnumerable<Type> GenerateProxys(IEnumerable<Type> interfaceTypes);
        SyntaxTree GenerateProxyTree(Type interfaceType);
    }
}
