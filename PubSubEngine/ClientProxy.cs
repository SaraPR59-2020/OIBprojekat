using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace PubSubEngine
{
    public class ClientProxy : ChannelFactory<ISubForEngine>, ISubForEngine, IDisposable
    {
        ISubForEngine factory;

        public ClientProxy(NetTcpBinding binding, string address) : base(binding, address)
        {
        
            factory = this.CreateChannel();

        }

        public void Connect()
        {
            try
            {
                factory.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }

        public void SendDataToSubscriber(string alarm, byte[] sign, byte[] publisherName)
        {
            try
            {
                factory.SendDataToSubscriber(alarm, sign, publisherName); //greska
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }


    }
}
