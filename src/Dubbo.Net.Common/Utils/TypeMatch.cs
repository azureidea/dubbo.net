using System;
using System.Collections.Concurrent;
using System.Reflection;
using Dubbo.Net.Common.Attributes;

namespace Dubbo.Net.Common.Utils
{
    public static class TypeMatch
    {
        static readonly ConcurrentDictionary<string,Type> _cache=new ConcurrentDictionary<string, Type>();

        public static void RegisterType(Type type)
        {
            var attr = type.GetCustomAttribute<ReferAttribute>();
            if(attr==null||string.IsNullOrEmpty(attr.Name))
                return;
            _cache.TryAdd(attr.Name, type);
        }

        public static Type MatchType(string key)
        {
            if (_cache.TryGetValue(key, out var type))
                return type;
            return null;
        }
    }
}

