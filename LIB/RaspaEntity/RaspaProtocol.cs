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

		public RaspaProtocol(bool esito,string message,int? idSubscription, enumComando comando, RaspaProtocol_Nodo mittente, RaspaProtocol_Nodo destinatario, RaspaProtocol_Componente componente)
		{
			Esito = esito;
			Message = message;
			IDSubcription = idSubscription;
			Comando = comando;
			Mittente = mittente;
			Destinatario = destinatario;
			Componente = componente;
		}
		public RaspaProtocol(int? idSubscription, enumComando comando, RaspaProtocol_Nodo mittente, RaspaProtocol_Nodo destinatario, RaspaProtocol_Componente componente)
		{
			Esito = true;
			Message = "";
			IDSubcription = idSubscription;
			Comando = comando;
			Mittente = mittente;
			Destinatario = destinatario;
			Componente = componente;
		}

		public RaspaProtocol(string message)
		{
			RaspaProtocol ele = JsonConvert.DeserializeObject<RaspaProtocol>(message);
			if (ele != null)
			{
				Esito = ele.Esito;
				Message = ele.Message;
				IDSubcription = ele.IDSubcription;
				Comando = ele.Comando;
				Mittente = ele.Mittente;
				Destinatario = ele.Destinatario;
				Componente = ele.Componente;
			}
		}

		public string BuildJson()
		{
			string json = JsonConvert.SerializeObject(this);
			return json;
		}

		public bool Esito { get; set; }
		public string Message { get; set; }

		public int? IDSubcription { get; set; }
		public enumComando Comando { get; set; }
		public RaspaProtocol_Nodo Mittente { get; set; }
		public RaspaProtocol_Nodo Destinatario { get; set; }
		public RaspaProtocol_Componente Componente { get; set; }

	}

	public class RaspaProtocol_Nodo
	{
		public RaspaProtocol_Nodo(int? id, bool? enabled, bool? trusted, int num, string ipv4)
		{
			ID = id;
			Enabled = enabled;
			Trusted = trusted;
			Num = num;
			IPv4 = ipv4;
		}
		public int? ID { get; set; }
		public bool? Enabled { get; set; }
		public bool? Trusted { get; set; }
		public int Num { get; set; }
		public string IPv4 { get; set; }
	}

	public class RaspaProtocol_Componente
	{

		public RaspaProtocol_Componente(int? id, enumComponente tipo, int? edge,int pin,string value,int? milliseconds)
		{
			ID = id;
			Tipo = tipo;
			if (edge == null)
				Edge = null;
			else
				Edge = (GpioPinEdge)edge;
			Pin = pin;
			Value = value;
			Milliseconds = milliseconds;
		}
		public int? ID { get; set; }
		public enumComponente Tipo { get; set; }
		public GpioPinEdge? Edge { get; set; }
		public int Pin { get; set; }
		public string Value { get; set; }
		public int? Milliseconds { get; set; }
	}
}
