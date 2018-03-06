using System;

namespace Dubbo.Net.Common
{
    public interface ILogger
    {
        void Error(Exception ex);
        void Error(string msg, Exception ex);
        bool InfoEnabled { get; }
        bool DebugEnabled { get; }
        bool WarnEnabled { get; }
        void Debug(string msg);
        void Warn(string msg);
        void Warn(string msg, Exception ex);
        void Warn(Exception ex);
        void Info(string msg);
    }
}
