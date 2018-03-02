using System;

namespace Dubbo.Net.Common.Utils
{
    public class SingleInstance<T>
    {
        private static readonly Lazy<T> Lazy = new Lazy<T>();

        public static T Instance => Lazy.Value;
    }
}
