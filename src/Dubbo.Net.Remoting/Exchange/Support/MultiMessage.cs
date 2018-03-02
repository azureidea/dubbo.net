using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Dubbo.Net.Remoting.Exchange.Support
{
    public class MultiMessage
    {
        private readonly List<object> _messages=new List<object>();

        private MultiMessage() { }

        public static MultiMessage CreateFromCollection(ICollection<object> collection)
        {
            var result=new MultiMessage();
            result.AddMessages(collection);
            return result;
        }
        public static MultiMessage Create()
        {
            return new MultiMessage();
        }

        public void AddMessage(object msg)
        {
            _messages.Add(msg);
        }

        public void AddMessages(ICollection<object> collection)
        {
            _messages.AddRange(collection);
        }

        public ICollection<object> GetMessages()
        {
            var list = new List<object>();
            _messages.ForEach(list.Add);
            return list;
        }

        public int Count=> _messages.Count;
      

        public object Get(int index)
        {
            return _messages[index];
        }

        public bool IsEmpty()
        {
            return _messages.Count == 0;
        }

        public ICollection<object> RemoveMessages()
        {
            ICollection<object> result = GetMessages();
            _messages.Clear();
            return result;
        }

        public IEnumerable<object> GetEnumerator()
        {
            return _messages;
        }
    }
}
