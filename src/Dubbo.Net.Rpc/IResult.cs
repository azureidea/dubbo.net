using System;
using System.Collections.Generic;
using System.Text;

namespace Dubbo.Net.Rpc
{
    public interface IResult
    {
        object Value { get; set; }
        Exception Exception { get; set; }
        bool HasException { get; set; }
        object Recreate();
        Dictionary<string, string> Attachments { get; set; }
        string GetAttachment(string key, string defaultValue="");
    }
}
