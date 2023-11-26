using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Publisher
    {
        private string id;
        private Topic topicS;

        public Publisher( Topic topic)
        {
            this.id = "PUB" + Guid.NewGuid().ToString();
            this.topicS = topic;
        }
        [DataMember]
        public string Id { get => id; set => id = value; }
        [DataMember]
        public Topic TopicS { get => topicS; set => topicS = value; }
    }
}
