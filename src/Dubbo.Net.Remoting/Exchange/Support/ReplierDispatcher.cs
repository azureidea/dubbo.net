using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Dubbo.Net.Remoting.Exchange.Support
{
    public  class ReplierDispatcher:IReplier
    {
        private readonly IReplier _defaultReplier;
        private readonly ConcurrentDictionary<Type,IReplier> _repliers=new ConcurrentDictionary<Type, IReplier>();

        public ReplierDispatcher(IReplier defaultReplier=null, ConcurrentDictionary<Type, IReplier> repliers = null)
        {
            this._defaultReplier = defaultReplier;
            if (repliers != null && repliers.Count > 0)
            {
                foreach (var replier in repliers)
                {
                    _repliers.TryAdd(replier.Key, replier.Value);
                }
            }
        }

        public  ReplierDispatcher AddReplier(Type type, IReplier replier)
        {
            _repliers.TryAdd(type, replier);
            return this;
        }

        public  ReplierDispatcher RemoveReplier(Type type)
        {
            _repliers.TryRemove(type,out var replier);
            return this;
        }

        private IReplier GetReplier(Type type)
        {
            _repliers.TryGetValue(type, out var replier);
            if (replier != null)
                return replier;
            if (_defaultReplier != null)
            {
                return _defaultReplier;
            }
            throw new Exception("Replier not found, Unsupported message object: " + type);
        }
        public Task<object> ReplyAsync(IExchangeChannel channel, object request)
        {
            return GetReplier(request.GetType()).ReplyAsync(channel, request);
        }
    }
}
