using System;
using System.IO;
using DotNetty.Buffers;

namespace Dubbo.Net.Remoting.Buffer
{
    public abstract class AbstractChannelBuffer : IChannelBuffer
    {
        private int _readerIndex;
        private int _writerIndex;
        private int _markedReaderIndex;
        private int _markedWriterIndex;



        public int ReaderIndex => _readerIndex;

        public void SetReaderIndex(int readerIndex)
        {
            if (readerIndex < 0 || readerIndex > _writerIndex)
            {
                throw new IndexOutOfRangeException();
            }
            _readerIndex = readerIndex;
        }


        public int WriterIndex => _writerIndex;

        public void SetWriterIndex(int writerIndex)
        {
            if (writerIndex < _readerIndex || writerIndex > Capacity)
            {
                throw new IndexOutOfRangeException();
            }
            _writerIndex = writerIndex;
        }

        public abstract byte[] Array { get; }
        public abstract bool HasArray { get; }
        public abstract int ArrayOffset { get; }

        public abstract int SetBytes(int index, MemoryStream src, int length);

        public void SetIndex(int readerIndex, int writerIndex)
        {
            if (readerIndex < 0 || readerIndex > writerIndex || writerIndex > Capacity)
            {
                throw new IndexOutOfRangeException();
            }
            _readerIndex = readerIndex;
            _writerIndex = writerIndex;
        }

        public abstract int Capacity { get; }

        public void Clear()
        {
            _readerIndex = _writerIndex = 0;
        }

        public bool Readable => ReadableBytes > 0;

        public abstract IByteBuffer ToByteBuffer(int index, int length);

        public bool Writeable=> WriteableBytes > 0;

        public int ReadableBytes=>_writerIndex - _readerIndex;

        public int WriteableBytes=> Capacity - _writerIndex;
        
        public abstract void WiteBytes(byte[] src);

        public abstract bool IsDirect { get; }

        public void MarkReaderIndex()
        {
            _markedReaderIndex = _readerIndex;
        }

        public void ResetReaderIndex()
        {
            SetReaderIndex(_markedReaderIndex);
        }

        public void MarkWriterIndex()
        {
            _markedWriterIndex = _writerIndex;
        }

        public void ResetWriterIndex()
        {
            _writerIndex = _markedWriterIndex;
        }

        public abstract IChannelBuffer Copy(int index, int len);

        public void DiscardReadBytes()
        {
            if (_readerIndex == 0)
            {
                return;
            }

            SetBytes(0, this, _readerIndex, _writerIndex - _readerIndex);
            _writerIndex -= _readerIndex;
            _markedReaderIndex = Math.Max(_markedReaderIndex - _readerIndex, 0);
            _markedWriterIndex = Math.Max(_markedWriterIndex - _readerIndex, 0);
            _readerIndex = 0;
        }

        public void EnsureWriteableBytes(int writeableBytes)
        {
            if (writeableBytes > WriteableBytes)
            {
                throw new IndexOutOfRangeException();
            }
        }

        public abstract byte GetByte(int index);

        public void GetBytes(int index, byte[] dst)
        {
            GetBytes(index, dst, 0, dst.Length);
        }

        public abstract void GetBytes(int index, byte[] dst, int dstIndex, int length);
        public abstract void GetBytes(int index, IByteBuffer dst);

        public void GetBytes(int index, IChannelBuffer dst)
        {
            GetBytes(index, dst, dst.WriteableBytes);
        }

        public void GetBytes(int index, IChannelBuffer dst, int length)
        {
            if (length > dst.WriteableBytes)
                throw new IndexOutOfRangeException();
            GetBytes(index, dst, dst.WriterIndex, length);
        }

        public abstract void GetBytes(int index, IChannelBuffer dst, int dstIndex, int length);
        public abstract void GetBytes(int index, MemoryStream dst, int length);

        public abstract void SetByte(int index, int value);

        public void SetBytes(int index, byte[] src)
        {
            SetBytes(index, src, 0, src.Length);
        }

        public abstract void SetBytes(int index, byte[] src, int srcIndex, int length);
        public abstract void SetBytes(int index, IByteBuffer src);

        public void SetBytes(int index, IChannelBuffer src)
        {
            SetBytes(index, src, src.ReadableBytes);
        }

        public void SetBytes(int index, IChannelBuffer src, int length)
        {
            if (length > src.ReadableBytes)
                throw new IndexOutOfRangeException();
            SetBytes(index, src, src.ReaderIndex, length);
        }

        public abstract void SetBytes(int index, IChannelBuffer src, int srcIndex, int length);

        public byte ReadByte()
        {
            if (_readerIndex == _writerIndex)
                throw new IndexOutOfRangeException();
            return GetByte(_readerIndex++);
        }

        public IChannelBuffer ReadBytes(int length)
        {
            CheckReadableBytes(length);
            if (length == 0)
                return ChannelBuffers.EmptyBuffer;
            IChannelBuffer buf = Factory.GetBuffer(length);
            buf.WriteBytes(this, _readerIndex, length);
            _readerIndex += length;
            return buf;
        }

        public void ReadBytes(byte[] dst, int dstIndex, int length)
        {
            CheckReadableBytes(length);
            GetBytes(_readerIndex, dst, dstIndex, length);
            _readerIndex += length;
        }

        public void ReadBytes(byte[] dst)
        {
            ReadBytes(dst, 0, dst.Length);
        }

        public void ReadBytes(IChannelBuffer dst)
        {
            ReadBytes(dst, dst.WriteableBytes);
        }

        public void ReadBytes(IChannelBuffer dst, int length)
        {
            if (length > dst.WriteableBytes)
                throw new IndexOutOfRangeException();
            ReadBytes(dst, dst.WriterIndex, length);
            dst.SetWriterIndex(dst.WriterIndex + length);
        }

        public void ReadBytes(IChannelBuffer dst, int dstIndex, int length)
        {
            CheckReadableBytes(length);
            GetBytes(_readerIndex, dst, dstIndex, length);
            _readerIndex += length;
        }

        public void ReadBytes(IByteBuffer dst)
        {
            int length = dst.WritableBytes;
            CheckReadableBytes(length);
            GetBytes(_readerIndex, dst);
            _readerIndex += length;
        }

        public void ReadBytes(MemoryStream output, int length)
        {
            CheckReadableBytes(length);
            GetBytes(_readerIndex, output, length);
            _readerIndex += length;
        }

        public void SkipBytes(int length)
        {
            int newReaderIndex = _readerIndex + length;
            if (newReaderIndex > _writerIndex)
                throw new IndexOutOfRangeException();
            _readerIndex = newReaderIndex;
        }

        public void WriteByte(int value)
        {
            SetByte(_writerIndex++, value);
        }

        public void WriteBytes(byte[] src, int srcIndex, int length)
        {
            SetBytes(_writerIndex, src, srcIndex, length);
            _writerIndex += length;
        }

        public void WriteBytes(byte[] src)
        {
            WriteBytes(src, 0, src.Length);
        }

        public void WriteBytes(IChannelBuffer src)
        {
            WriteBytes(src, src.ReadableBytes);
        }

        public void WriteBytes(IChannelBuffer src, int length)
        {
            if (length > src.ReadableBytes)
                throw new IndexOutOfRangeException();
            WriteBytes(src, src.ReaderIndex, length);
            src.SetReaderIndex(src.ReaderIndex + length);
        }

        public void WriteBytes(IChannelBuffer src, int srcIndex, int length)
        {
            SetBytes(_writerIndex, src, srcIndex, length);
            _writerIndex += length;
        }

        public void WriteBytes(IByteBuffer src)
        {
            int length = src.ReadableBytes;
            SetBytes(_writerIndex, src);
            _writerIndex += length;
        }
        public int WriteBytes(MemoryStream input, int length)
        {
            int writtenBytes = SetBytes(_writerIndex, input, length);
            if (writtenBytes > 0)
            {
                _writerIndex += writtenBytes;
            }
            return writtenBytes;
        }

        public IChannelBuffer Copy()
        {
            return Copy(_readerIndex, ReadableBytes);
        }

        public IByteBuffer ToByteBuffer()
        {
            return ToByteBuffer(_readerIndex, ReadableBytes);
        }

        public override bool Equals(Object o)
        {
            return o is IChannelBuffer&& ChannelBuffers.Equals((IChannelBuffer) this, (IChannelBuffer)o);
        }

        public abstract IChannelBufferFactory Factory { get; }

        public int CompareTo(IChannelBuffer that)
        {
            return ChannelBuffers.Compare(this, that);
        }

        public override string ToString()
        {
            return GetType().Name + '(' +
            "ridx=" + _readerIndex + ", " +
            "widx=" + _writerIndex + ", " +
            "cap=" + Capacity +
            ')';
        }

        protected void CheckReadableBytes(int minimumReadableBytes)
        {
            if (ReadableBytes < minimumReadableBytes)
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}
