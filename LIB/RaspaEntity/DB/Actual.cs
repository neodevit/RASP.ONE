using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public class Actual
	{
		public Actual()
		{
			follow = new Follow();
		}
		public int? ID { get; set; }
		
		public int node_num { get; set; }
		public enumComponente Componente { get; set; }
		public int node_pin { get; set; }
		public enumStato node_stato { get; set; }
		public Follow follow { get; set; }

	}

	public class Actuals : Collection<Actual>
	{

	}
}
