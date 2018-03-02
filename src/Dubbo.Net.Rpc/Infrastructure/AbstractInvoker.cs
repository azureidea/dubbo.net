using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dubbo.Net.Common;
using Dubbo.Net.Common.Utils;
using Dubbo.Net.Rpc.Support;

namespace Dubbo.Net.Rpc.Infrastructure
{
    public abstract class AbstractInvoker:IInvoker
    {
        private readonly Type _type;
        private readonly URL _url;
        private readonly Dictionary<string, string> _attachment;
        private volatile bool _available = true;
        private bool _destroyed = false;

        protected AbstractInvoker(Type type, URL url, String[] keys): this(type, url, ConvertAttachment(url, keys))
        {
            
        }

        protected AbstractInvoker(Type type, URL url, Dictionary<string,string> attachment = null)
        {
            this._type = type?? throw new ArgumentException("service type == null");
            this._url = url ?? throw new ArgumentException("service url == null");
            this._attachment = attachment;
        }
        private static Dictionary<string, string> ConvertAttachment(URL url, string[] keys)
        {
            if (keys == null || keys.Length == 0)
            {
                return null;
            }
            var attachment = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                var value = url.GetParameter(key,"");
                if (!string.IsNullOrEmpty(value))
                {
                    attachment.Add(key, value);
                }
            }
            return attachment;
        }
        public URL GetUrl()
        {
            return _url;
        }

        public virtual bool IsAvailable()
        {
            return _available;
        }

        protected void SetAvailable(bool available)
        {
            this._available = available;
        }

        public virtual void Destroy()
        {
            _destroyed = true;
            SetAvailable(false);
        }
        public bool IsDestroyed()
        {
            return _destroyed;
        }
        public Type GetInterface()
        {
            return _type;
        }

        public async Task<IResult> Invoke(IInvocation inv)
        {
            if (_destroyed)
            {
                throw new Exception("Rpc invoker for service " + this + " on consumer " + NetUtils.GetLocalAddress()
                                       + " use dubbo version " + Common.Version.GetVersion()
                                       + " is DESTROYED, can not be invoked any more!");
            }
            RpcInvocation invocation = (RpcInvocation)inv;
            invocation.Invoker=this;
            if (_attachment != null && _attachment.Count > 0)
            {
                invocation.AddAttachmentsIfAbsent(_attachment);
            }
            //todo RpcContext
            //var context = RpcContext.getContext().getAttachments();
            //if (context != null)
            //{
            //    invocation.AddAttachmentsIfAbsent(context);
            //}
            if (GetUrl().GetMethodParameter(invocation.MethodName, Constants.AsyncKey, false))
            {
                invocation.SetAttachment(Constants.AsyncKey, "true");
            }
            RpcUtils.AttachInvocationIdIfAsync(GetUrl(), invocation);


            try
            {
                return await DoInvoke(invocation);
            }
            catch (Exception e)
            {
                return new RpcResult(e);
            }
        }

        protected abstract Task<IResult> DoInvoke(IInvocation invocation);
        public override string ToString()
        {
            return GetInterface() + " -> " + (GetUrl() == null ? "" : GetUrl().ToString());
        }
    }
}
