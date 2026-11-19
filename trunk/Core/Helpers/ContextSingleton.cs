using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;

namespace Eureka.Stroke
{
    /// <summary>
    /// Context Singleton class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ContextSingletonBase<T>
    {
        public T Subject
        {
            get { return (T)CallContext.GetData(key); }
            set { CallContext.SetData(key, value); }
        }

        abstract protected string key
        {
            get;
        }
    }
}
