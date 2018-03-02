using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;

namespace Dubbo.Net.Test
{
    [DependencyIoc(typeof(ILogger))]
    public class ConsoleLog:ILogger
    {
        public void Error(Exception ex)
        {
            Console.WriteLine(ex);
        }

        public void Error(string msg, Exception ex)
        {
            Console.WriteLine(msg+":"+ex);
        }

        public bool InfoEnabled =>true;
        public bool DebugEnabled => true;
        public bool WarnEnabled =>true;

        public void Debug(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Warn(string msg)
        {
            Console.WriteLine(msg);
        }

        public void Warn(string msg, Exception ex)
        {
            Console.WriteLine(msg+":"+ex);
        }

        public void Warn(Exception ex)
        {
            Console.WriteLine(ex);
        }

        public void Info(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
