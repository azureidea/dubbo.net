using DotNetty.Buffers;
using System;
using System.IO;

namespace Dubbo.Net.Common.Buffer
{
    public class ChannelBufferStream:MemoryStream
    {
        private readonly IByteBuffer _buffer;
        private readonly int startIndex;
        private readonly int endIndex;

        public ChannelBufferStream(IByteBuffer buffer)
        {
            _buffer = buffer ?? throw new NullReferenceException("buffer");
            startIndex = _buffer.WriterIndex;
        }
        public ChannelBufferStream(IByteBuffer buffer, int length)
        {
            _buffer = buffer ?? throw new NullReferenceException("buffer");
            if (length < 0)
                throw new ArgumentException("length:" + length);
            if (length > buffer.ReadableBytes)
                throw new IndexOutOfRangeException();
            this._buffer = buffer;
            startIndex = buffer.ReaderIndex;
            endIndex = startIndex + length;
            buffer.MarkReaderIndex();
        }

        public int WritenBytes() { return _buffer.WriterIndex - startIndex; }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count == 0)
                return;
            _buffer.WriteBytes(buffer, offset, count);
        }
        public IByteBuffer Buffer() { return _buffer; }


        public int ReadBytes()
        {
            return _buffer.ReaderIndex - startIndex;
        }
        public int Available()
        {
            return endIndex - _buffer.ReaderIndex;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var available = Available();
            if (available <= 0) return -1;
            count = Math.Min(available, count);
            _buffer.ReadBytes(buffer, offset, count);
            return count;
        }
        

    }
}
