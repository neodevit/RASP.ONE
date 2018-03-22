using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaEntity
{
	public class Rule
	{
		public Rule()
		{
			follow = new Follow();
		}
		public int? ID { get; set; }
		public int IDSubscription { get; set; }
		public string ValueSubscription { get; set; }
		
		public int NODE { get; set; }
		public string IPv4 { get; set; }
		public int PIN { get; set; }
		public enumComando COMANDO { get; set; }
		public enumComponente COMPONENTE { get; set; }
		public GpioPinEdge Edge { get; set; }
		public string VALUE { get; set; }

		public Follow follow { get; set; }

	}

	public class Rules : Collection<Rule>
	{

	}
}
