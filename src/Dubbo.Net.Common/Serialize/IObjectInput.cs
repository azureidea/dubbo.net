using System;

namespace Dubbo.Net.Common.Serialize
{
    public interface IObjectInput:IDataInput
    {
        object ReadObject();
        T ReadObject<T>();
        T ReadObject<T>(Type type);
        object ReadObject(Type type);
    }
}
