using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Common
{
    public interface IResetable
    {
        void Reset(URL url);
    }
}
