using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Test
{
    [DependencyIoc(typeof(IConfigUtils))]
    public class CacheConfigUtil:IConfigUtils
    {
        public string GetProperty(string key)
        {
            return "";
        }
    }
}
