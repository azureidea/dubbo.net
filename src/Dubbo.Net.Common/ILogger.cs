using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common
{
    public interface ILogger
    {
        void Error(Exception ex);
    }
}
