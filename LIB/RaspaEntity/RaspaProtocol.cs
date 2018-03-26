using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

		public RaspaProtocol(bool esito,string message, enumComando comando, Componente mittente, Componente destinatario,string value)
		{
			Esito = esito;
			Message = message;
			Comando = comando;
			Mittente = mittente;
			Destinatario = destinatario;
			Value = value;
		}
		public RaspaProtocol(enumComando comando, Componente mittente, Componente destinatario,string value)
		{
			Esito = true;
			Message = "";
			Comando = comando;
			Mittente = mittente;
			Destinatario = destinatario;
			Value = value;
		}

		public RaspaProtocol(string message)
		{
			RaspaProtocol ele = JsonConvert.DeserializeObject<RaspaProtocol>(message);
			if (ele != null)
			{
				Esito = ele.Esito;
				Message = ele.Message;
				Comando = ele.Comando;
				Mittente = ele.Mittente;
				Destinatario = ele.Destinatario;
				Value = ele.Value;
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
		public Componente Mittente { get; set; }
		public Componente Destinatario { get; set; }

		public string Value { get; set; }

		public void swapMittDest()
		{
			Componente temp1 = Mittente;
			Componente temp2 = Destinatario;
			Mittente = temp2;
			Destinatario = temp1;
		}
	}


}
