namespace Dubbo.Net.Common.Serialize
{
    public interface IDataInput
    {
        bool ReadBool();
        byte ReadByte();
        short ReadShort();
        int ReadInt();
        long ReadLong();
        float ReadFloat();
        double ReadDouble();
        string ReadUTF();
        byte[] ReadBytes();
    }
}
