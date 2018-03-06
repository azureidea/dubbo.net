using System;
using System.Collections.Generic;

namespace Dubbo.Net.Rpc
{
    public interface IResult
    {
        object Value { get; set; }
        Exception Exception { get; set; }
        bool HasException { get;  }
        object Recreate();
        Dictionary<string, string> Attachments { get; set; }
        string GetAttachment(string key, string defaultValue="");
    }
}
