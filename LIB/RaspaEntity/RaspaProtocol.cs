using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaEntity
{
    public class RaspaProtocol
    {


		public RaspaProtocol()
		{
		}

		public RaspaProtocol(bool esito,string message, enumComando comando, enumAzione azione, Componente mittente, Componente destinatario,string value, Tempo repetiteTime, enumSubribe subcribeDestination, enumSubribe subcribeResponse)
		{
			Esito = esito;
			Message = message;
			Comando = comando;
			Azione = azione;
			Mittente = mittente;
			Destinatario = destinatario;
			Value = value;
			RepetiteTime = repetiteTime;
			SubcribeDestination = subcribeDestination;
			SubcribeResponse = subcribeResponse;
		}
		public RaspaProtocol(enumComando comando, Componente mittente, enumAzione azione, Componente destinatario,string value, Tempo repetiteTime, enumSubribe subcribeDestination, enumSubribe subcribeResponse)
		{
			Esito = true;
			Message = "";
			Comando = comando;
			Azione = azione;
			Mittente = mittente;
			Destinatario = destinatario;
			Value = value;
			RepetiteTime = repetiteTime;
			SubcribeDestination = subcribeDestination;
			SubcribeResponse = subcribeResponse;
		}
		public RaspaProtocol(string message)
		{
			RaspaProtocol ele = JsonConvert.DeserializeObject<RaspaProtocol>(message);
			if (ele != null)
			{
				Esito = ele.Esito;
				Message = ele.Message;
				Comando = ele.Comando;
				Azione = ele.Azione;
				Mittente = ele.Mittente;
				Destinatario = ele.Destinatario;
				Value = ele.Value;
				RepetiteTime = ele.RepetiteTime;
				SubcribeDestination = ele.SubcribeDestination;
				SubcribeResponse = ele.SubcribeResponse;
			}
		}

		public string BuildJson()
		{
			string json = JsonConvert.SerializeObject(this);
			return json;
		}

		public bool Esito { get; set; }
		public string Message { get; set; }

		public enumComando Comando { get; set; }
		public enumAzione Azione { get; set; }
		public Componente Mittente { get; set; }
		public Componente Destinatario { get; set; }

		public Tempo RepetiteTime { get; set; }
		public string Value { get; set; }
		public Decimal GetValueDecimal()
		{
			CultureInfo culture = new CultureInfo("it-IT");
			return Convert.ToDecimal(Value, culture);
		}

		public enumSubribe SubcribeDestination { get; set; }
		public enumSubribe SubcribeResponse { get; set; }

		public void swapMittDest()
		{
			Componente temp1 = Mittente;
			Componente temp2 = Destinatario;
			Mittente = temp2;
			Destinatario = temp1;

		}
	}


}
