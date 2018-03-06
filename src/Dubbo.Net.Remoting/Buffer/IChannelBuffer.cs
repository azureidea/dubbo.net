using DotNetty.Buffers;
using System.IO;

namespace Dubbo.Net.Remoting.Buffer
{
    public interface IChannelBuffer
    {

        int Capacity { get; }
        void Clear();
        IChannelBuffer Copy();
        IChannelBuffer Copy(int index, int len);
        void DiscardReadBytes();
        void EnsureWriteableBytes(int writeableBytes);
        bool Equals(object o);
        IChannelBufferFactory Factory { get; }
        byte GetByte(int index);
        void GetBytes(int index, byte[] dst);
        void GetBytes(int index, byte[] dst, int dstIndex, int length);
        void GetBytes(int index, IByteBuffer dst);
        void GetBytes(int index, IChannelBuffer dst);
        void GetBytes(int index, IChannelBuffer dst, int len);
        void GetBytes(int index, IChannelBuffer dst, int dstIndex, int length);
        void GetBytes(int index, MemoryStream dst, int length);
        bool IsDirect { get; }
        void MarkReaderIndex();
        void MarkWriterIndex();
        bool Readable { get; }
        int ReadableBytes { get; }
        byte ReadByte();
        void ReadBytes(byte[] dst);
        void ReadBytes(byte[] dst, int dstIndex, int length);
        void ReadBytes(IByteBuffer dst);
        void ReadBytes(IChannelBuffer dst);
        void ReadBytes(IChannelBuffer dst, int len);
        void ReadBytes(IChannelBuffer dst, int dstIndex, int len);
        IChannelBuffer ReadBytes(int length);
        void ResetReaderIndex();
        void ResetWriterIndex();
        int ReaderIndex { get; }
        void SetReaderIndex(int readerIndex);
        void ReadBytes(MemoryStream dst, int length);
        void SetByte(int index, int value);
        void SetBytes(int index, byte[] src);
        void SetBytes(int index, byte[] src, int srcIndex, int length);
        void SetBytes(int index, IByteBuffer src);
        void SetBytes(int index, IChannelBuffer src);
        void SetBytes(int index, IChannelBuffer src, int length);
        void SetBytes(int index, IChannelBuffer src, int srcIndex, int length);
        int SetBytes(int index, MemoryStream src, int length);
        void SetIndex(int readerIndex, int writerIndex);
        void SkipBytes(int length);
        IByteBuffer ToByteBuffer();
        IByteBuffer ToByteBuffer(int index, int length);
        bool Writeable { get; }
        int WriteableBytes { get; }
        void WriteByte(int value);
        void WiteBytes(byte[] src);
        void WriteBytes(byte[] src, int index, int length);
        void WriteBytes(IByteBuffer src);
        void WriteBytes(IChannelBuffer src);
        void WriteBytes(IChannelBuffer src, int length);
        void WriteBytes(IChannelBuffer src, int srcIndex, int length);
        int WriteBytes(MemoryStream src, int length);
        int WriterIndex { get; }
        void SetWriterIndex(int writerIndex);
        byte[] Array { get; }
        bool HasArray { get; }
        int ArrayOffset { get; }
    }
}
