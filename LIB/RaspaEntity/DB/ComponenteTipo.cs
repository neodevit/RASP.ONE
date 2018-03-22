using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{

	public class ComponenteTipo
	{
		public ComponenteTipo()
		{
			follow = new Follow();
		}
		public int? ID { get; set; }
		public bool Enabled { get; set; }
		public string Nome { get; set; }
		public string Descrizione { get; set; }
		public Follow follow { get; set; }
	}

	public class ComponenteTipi : Collection<ComponenteTipo>
	{

	}
}
