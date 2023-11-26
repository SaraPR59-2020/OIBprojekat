using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Security.Policy;
using Common;

namespace Contracts
{
	[ServiceContract]
	public interface IWCFContract
	{
		[OperationContract]
		void TestCommunication();
       [OperationContract]
        string AddPublisher(Common.Publisher pub);
        [OperationContract]
        string AddSubscriber(Subscriber sub);
    }
}
