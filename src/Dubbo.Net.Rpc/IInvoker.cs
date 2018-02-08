using System;
using System.Collections.Generic;
using System.Text;
using Dubbo.Net.Common;

namespace Dubbo.Net.Rpc
{
    public interface IInvoker
    {
        URL Url { get; set; }
    }
}
