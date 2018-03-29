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
		public RaspaTag(int id,enumComponente tipo)
		{
			ID = id;
			Tipo = tipo;
		}
		public RaspaTag(string tag)
		{
			string[] ANodo = tag.Split('_');
			if (ANodo.Length < 3)
				return;
			ID = Convert.ToInt32(ANodo[1]);
			Tipo = (enumComponente)Convert.ToInt32(ANodo[2]);
		}

		public bool CompareDestinatario(RaspaProtocol message)
		{
			bool res = false;
			if (ID == message.Destinatario.ID &&
				Tipo == message.Destinatario.Tipo)
				res = true;

			return res;
		}
		public bool CompareMittente(RaspaProtocol message)
		{
			bool res = false;
			if (ID == message.Mittente.ID &&
				Tipo == message.Mittente.Tipo)
				res = true;

			return res;
		}

		public string BuildTag()
		{
			return "RASP.ONE_" + ID + "_" + (int)Tipo;
		}
		public string BuildTag(RaspaProtocol message)
		{
			return "RASP.ONE_" + message.Destinatario.ID + "_" + (int)message.Destinatario.Tipo;
		}

		public int ID { get; set; }
		public enumComponente Tipo { get; set; }
	}
}
