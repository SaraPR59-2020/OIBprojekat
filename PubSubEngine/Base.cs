using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubEngine
{
    static class Base
    {
        internal static ConcurrentDictionary<string, Subscriber> Subscribers = new ConcurrentDictionary<string, Subscriber>();

    }
}
