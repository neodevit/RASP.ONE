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

		public RaspaProtocol(bool esito,string message, enumComando comando, enumAzione azione, Componente mittente, Componente destinatario,List<string> value, Tempo repetiteTime, enumSubribe subcribeDestination, enumSubribe subcribeResponse)
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
		public RaspaProtocol(enumComando comando, Componente mittente, enumAzione azione, Componente destinatario, List<string> value, Tempo repetiteTime, enumSubribe subcribeDestination, enumSubribe subcribeResponse)
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

		#region VALUE
		public List<string> Value { get; set; }

		#region VALUE for DB
		public void ValueFor_readDB(string val)
		{
			Value = val.Split('§').ToList<string>();
		}
		public string ValueFor_writeDB()
		{
			return string.Join("§", Value);
		}
		#endregion
		#region VALUE TEMPERATURE UMIDITY
		public string getTemperatureValue()
		{
			Decimal? res = getTemperature();
			return (res.HasValue)?res.Value.ToString():"---";
		}
		public Decimal? getTemperature()
		{
			Decimal? res = null;
			if ((Value != null && Value.Count > 0))
				res = GetValueDecimal(0);
			return res;
		}
		public string getUmidityValue()
		{
			Decimal? res = getUmidity();
			return (res.HasValue) ? res.Value.ToString() : "---";
		}
		public Decimal? getUmidity()
		{
			Decimal? res = null;
			if ((Value != null && Value.Count > 1))
				res = GetValueDecimal(1);
			return res;
		}
		#endregion
		#region IPCAM
		public string getIPCAMAddress()
		{
			string res = "about:blank";
			if ((Value != null && Value.Count > 0))
				res = Value[0];
			return res;
		}

		#endregion


		public Decimal GetValueDecimal(int num = 1, string Lenguage = "it-IT")
		{
			Decimal res = 0;
			CultureInfo culture = new CultureInfo(Lenguage);
			res = Convert.ToDecimal(Value[num], culture);
			return res;
		}
		#endregion



		public enumSubribe SubcribeDestination { get; set; }
		public enumSubribe SubcribeResponse { get; set; }

	}


}
