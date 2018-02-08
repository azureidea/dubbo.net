namespace Dubbo.Net.Common.Serialize
{
    public interface IDataOutput
    {
        void WriteBool(bool v);
        void WriteByte(byte v);
        void WriteShort(short v);
        void WriteInt(int v);
        void WriteLong(long v);
        void WriteFloat(float v);
        void WriteDouble(double v);
        void WriteUTF(string v);
        void WriteBytes(byte[] v);
        void WriteBytes(byte[] v, int off, int len);
        void FlushBuffer();
    }
}
