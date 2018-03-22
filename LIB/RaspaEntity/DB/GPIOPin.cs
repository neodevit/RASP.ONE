using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public class GPIOPin
	{
		public GPIOPin()
		{
			follow = new Follow();
		}
		public int? ID { get; set; }
		public enumTipoPIN Tipo { get; set; }
		public int GPIO { get; set; }
		public int NUM { get; set; }
		public string NomeGPIO { get; set; }
		public string NomeNUM { get; set; }
		public string Descrizione { get; set; }
		public Follow follow { get; set; }
	}

	public class GPIOPins : Collection<GPIOPin>
	{

	}
}
