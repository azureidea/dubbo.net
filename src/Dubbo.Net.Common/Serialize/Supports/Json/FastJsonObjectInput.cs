
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Dubbo.Net.Common.Serialize.Supports.Json
{
    public class FastJsonObjectInput : IObjectInput
    {
        private readonly StreamReader _reader;

        public FastJsonObjectInput(MemoryStream stream) : this(new StreamReader(stream)) { }
        public FastJsonObjectInput(StreamReader reader)
        {
            _reader = reader;
        }

        public bool ReadBool()
        {
            return ReadObject<bool>();
        }

        public byte ReadByte()
        {
            return ReadObject<byte>();
        }

        public byte[] ReadBytes()
        {
            return Encoding.UTF8.GetBytes(ReadLine());
        }

        public double ReadDouble()
        {
            return ReadObject<double>();
        }

        public float ReadFloat()
        {
            return ReadObject<float>();
        }

        public int ReadInt()
        {
            return ReadObject<int>();
        }

        public long ReadLong()
        {
            return ReadObject<long>();
        }

        public object ReadObject()
        {
            return JsonConvert.DeserializeObject(ReadLine());
        }

        public T ReadObject<T>()
        {
            var line = ReadLine();
            return JsonConvert.DeserializeObject<T>(line);
        }

        public T ReadObject<T>(Type type)
        {
            return (T)JsonConvert.DeserializeObject(ReadLine(), type);
        }

        public object ReadObject(Type type)
        {
            return JsonConvert.DeserializeObject(ReadLine(), type);
        }

        public short ReadShort()
        {
            return ReadObject<short>();
        }

        public string ReadUTF()
        {
            return ReadObject<string>();
        }

        private string ReadLine()
        {
            var line = _reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) throw new EndOfStreamException();
            return line;
        }
    }
}
