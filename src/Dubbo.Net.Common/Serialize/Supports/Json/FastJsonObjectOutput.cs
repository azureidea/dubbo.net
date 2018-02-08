using System.IO;
using Newtonsoft.Json;

namespace Dubbo.Net.Common.Serialize.Supports.Json
{
    public class FastJsonObjectOutput:IObjectOutput
    {
        private StreamWriter _writer;

        public FastJsonObjectOutput(MemoryStream stream):this(new StreamWriter(stream)) { }
        public FastJsonObjectOutput(StreamWriter writter) { _writer = writter; }


        public void WriteObject(object obj)
        {
            var buffer = JsonConvert.SerializeObject(obj);
            _writer.Write(buffer);
            _writer.WriteLine();
            _writer.Flush();
        }
        public void FlushBuffer()
        {
            _writer.Flush();
        }

        public void WriteBool(bool v)
        {
            WriteObject(v);
        }

        public void WriteByte(byte v)
        {
            WriteObject(v);
        }

        public void WriteShort(short v)
        {
            WriteObject(v);
        }

        public void WriteInt(int v)
        {
            WriteObject(v);
        }

        public void WriteLong(long v)
        {
            WriteObject(v);
        }

        public void WriteFloat(float v)
        {
            WriteObject(v);
        }

        public void WriteDouble(double v)
        {
            WriteObject(v);
        }

        public void WriteUTF(string v)
        {
            WriteObject(v);
        }

        public void WriteBytes(byte[] v)
        {
            WriteObject(v);
        }

        public void WriteBytes(byte[] v, int off, int len)
        {
            WriteObject(v);
        }
    }
}
