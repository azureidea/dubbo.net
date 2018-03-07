using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.CoreTest
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
