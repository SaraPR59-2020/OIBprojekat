using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [DataContract]
    public class Topic
    {
        private string id;
        private string name;
        private DateTime created;

        public Topic(string name)
        {
            this.Id = "TOPIC"+ Guid.NewGuid().ToString();
            this.Name = name;
            this.Created = DateTime.Now;
        }
        [DataMember]
        public string Id { get => id; set => id = value; }
        [DataMember]
        public string Name { get => name; set => name = value; }
        [DataMember]
        public DateTime Created { get => created; set => created = value; }

        public override string ToString()
        {
            return "Topic: id " + Id + " name : " + Name;
        }
    }
}
