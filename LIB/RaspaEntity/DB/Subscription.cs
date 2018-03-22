using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public class Subscription
	{
		public Subscription()
		{
			follow = new Follow();
		}
		public int? ID { get; set; }
		public int NODE_subscriber { get; set; }
		public int NODO_Subscription { get; set; }
		public string NODE_IPv4 { get; set; }
		public int PIN_Subscription { get; set; }



		public Follow follow { get; set; }

	}

	public class Subscriptions : Collection<Subscription>
	{

	}
}
