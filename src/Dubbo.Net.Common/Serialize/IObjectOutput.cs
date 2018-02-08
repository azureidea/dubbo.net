namespace Dubbo.Net.Common.Serialize
{
    public interface IObjectOutput:IDataOutput
    {
        void WriteObject(object obj);
    }
}
