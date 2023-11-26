using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace PubSubEngine
{
    public class PubSubService : IWCFContract
    {
        public static Dictionary<string, Topic> topics = new Dictionary<string, Topic>();
        public static Dictionary<string, Publisher> publishers = new Dictionary<string, Publisher>();
        public static Dictionary<string, Subscriber> subscriber = new Dictionary<string, Subscriber>();
        public string AddPublisher(Publisher pub)
        {
         
            if (publishers.ContainsKey(pub.Id))
            {
                return "Publisher sa ovim id-em vec postoji";
            }
            else
            {
                publishers.Add(pub.Id, pub);
                topics.Add(pub.TopicS.Id, pub.TopicS);
                Console.WriteLine("Korisnik " + pub.Id + " je postavio temu: " + pub.TopicS.Name);
                return "Uspesno ste poslali temu:" + pub.TopicS.Name;
            } 
        }

        public string AddSubscriber(Subscriber sub)
        {
           
            if (topics.Count == 0)
            {
                Console.WriteLine("Korisnik " + sub.Id + " je usao na servis ali nema tema za pretplatu...");
                return "Trenutno nije dostupna ni jedna tema na koju se mozete pretplatiti. Pokusajte kasnije!";
            }
            else
            {
                
                Console.WriteLine("Korisnik " + sub.Id + " bira temu za pretplatu...");
                string returnTopics = "";
                foreach (Topic topic in topics.Values)
                {
                    returnTopics += topic.Name + " ";
                }
                return returnTopics;
            }
        }

        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }
    }
}
