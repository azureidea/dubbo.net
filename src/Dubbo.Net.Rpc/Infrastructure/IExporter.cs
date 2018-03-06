namespace Dubbo.Net.Rpc.Infrastructure
{
    public interface IExporter
    {
        IInvoker GetInvoker();
        void UnExport();
    }
}
