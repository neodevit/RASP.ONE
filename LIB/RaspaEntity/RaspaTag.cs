using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
    public class RaspaTag
    {


		public RaspaTag()
		{
		}

		public RaspaTag(enumComponente componente, int node, int pin, string value,string type)
		{
			Node_componente = componente;
			Node_num = node;
			Node_pin = pin;
			Node_value = value;
			Node_type = type;
		}
		public RaspaTag(string tag)
		{
			string[] ANodo = tag.Split('_');
			if (ANodo.Length < 5)
				return;
			Node_num = Convert.ToInt32(ANodo[0]);
			Node_pin = Convert.ToInt32(ANodo[1]);
			Node_value = ANodo[2];
			Node_componente = (enumComponente)Convert.ToInt32(ANodo[3]);
			Node_type = ANodo[4];
		}

		public bool CompareDestinatario(RaspaProtocol message)
		{
			bool res = false;
			if (Node_num == message.Destinatario.Num &&
				Node_pin == message.Componente.Pin &&
				Node_componente == message.Componente.Tipo)
				res = true;

			return res;
		}
		public bool CompareMittente(RaspaProtocol message)
		{
			bool res = false;
			if (Node_num == message.Mittente.Num &&
				Node_pin == message.Componente.Pin &&
				Node_componente == message.Componente.Tipo)
				res = true;

			return res;
		}

		public string BuildTag()
		{
			return Node_num + "_" + Node_pin + "_" + Node_value + "_" + (int)Node_componente + "_" + Node_type;
		}
		public string BuildTag(RaspaProtocol message)
		{
			return message.Destinatario.Num + "_" + message.Componente.Pin + "_" + message.Componente.Value + "_" + ((int)message.Componente.Edge).ToString();
		}

		public enumComponente Node_componente { get; set; }
		public int Node_num { get; set; }
		public int Node_pin { get; set; }
		public string Node_value { get; set; }
		public string Node_type { get; set; }
	}
}
