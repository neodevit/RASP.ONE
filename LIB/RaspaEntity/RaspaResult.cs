using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public class RaspaResult
	{
		public RaspaResult()
		{
		}
		public RaspaResult(int chiave)
		{
			ID = chiave;
			Esito = true;
			Level = enumLevel.nessuno;
			Message = "";
		}

		public RaspaResult(bool passed)
		{
			Esito = passed;
			Level = enumLevel.nessuno;
			Message = "";
			Value = "";
		}
		public RaspaResult(bool passed, string messaggio)
		{
			Esito = passed;
			if (Esito)
				Level = enumLevel.warning;
			else
				Level = enumLevel.error;

			Message = messaggio;
			Value = "";
		}

		public RaspaResult(bool passed, string messaggio, string value)
		{
			Esito = passed;
			if (Esito)
				Level = enumLevel.warning;
			else
				Level = enumLevel.error;

			Message = messaggio;
			Value = value;
		}
		public RaspaResult(bool passed, bool warning, string messaggio)
		{
			Esito = passed;
			if (Esito)
				Level = enumLevel.warning;
			else
				if (warning)
				Level = enumLevel.warning;
			else
				Level = enumLevel.error;

			Message = messaggio;
		}

		public RaspaResult(bool passed, string messaggio, int id)
		{
			ID = id;

			Esito = passed;
			if (Esito)
				Level = enumLevel.warning;
			else
				Level = enumLevel.error;

			Message = messaggio;
		}


		public RaspaResult(bool passed, enumLevel livello, string messaggio)
		{
			Esito = passed;
			Level = livello;
			Message = messaggio;
		}

		[DataMember]
		public bool Esito { get; set; }
		[DataMember]
		public bool Warning { get; set; }
		public int GetEsito()
		{
			return (Esito) ? 1 : 0;
		}
		[DataMember]
		public int? ID { get; set; }
		[DataMember]
		public string Value { get; set; }
		[DataMember]
		public enumLevel Level { get; set; }
		[DataMember]
		public string Message { get; set; }

	}
}
