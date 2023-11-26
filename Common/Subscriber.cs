using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Subscriber
    {
        private string id;
        public List<Topic> topics = new List<Topic>();

        public Subscriber()
        {
            this.id = "SUB" + Guid.NewGuid().ToString();
        }

        [DataMember]
        public string Id { get => id; set => id = value; }

    }
}
