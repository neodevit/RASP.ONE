using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{

	public class Comando
	{
		public Comando()
		{
			follow = new Follow();
		}
		public int? ID { get; set; }
		public int IDSubscription { get; set; }
		public string Valore { get; set; }
		public Follow follow { get; set; }

	}

	public class Comandi : Collection<Comando>
	{

	}
}
