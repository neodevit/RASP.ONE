using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RaspaEntity
{
	public class Componente
	{
		public Componente()
		{
			Action = new ComponenteAction();
			repeatTime = new Tempo();
			follow = new Follow();
		}
		public Componente(int id, bool enabled, bool trusted, int num, string ipv4)
		{
			ID = id;
			Enabled = enabled;
			Trusted = trusted;
			Node_Num = num;
			IPv4 = ipv4;
			Action = new ComponenteAction();
			repeatTime = new Tempo();
			follow = new Follow();
		}

		public int? ID { get; set; }
		public bool Enabled { get; set; }
		public bool Trusted { get; set; }

		public bool Error { get; set; }
		public string ErrorMessage { get; set; }

		public string Nome { get; set; }
		public string HostName { get; set; }
		public string OSVersion { get; set; }
		public string NodeSWVersion { get; set; }
		public string SystemProductName { get; set; }
		public string SystemID { get; set; }

		public enumStato Stato { get; set; }

		public string Descrizione { get; set; }

		public int IDComponenteTipo { get; set; }
		public enumComponente Tipo { get; set; }


		public bool repeat { get; set; }
		public Tempo repeatTime { get; set; }


		public Double PositionTop { get; set; }
		public Double PositionLeft { get; set; }
		public Double PositionBottom { get; set; }
		public Double PositionRight { get; set; }
		public Thickness Margin
		{
			get
			{
				return new Thickness(PositionLeft, PositionTop, PositionRight, PositionBottom);
			}
			set
			{
				Thickness valore = value;
				PositionLeft = valore.Left;
				PositionTop = valore.Top;
				PositionRight = valore.Right;
				PositionBottom = valore.Bottom;
			}
		}
		public void calcAction()
		{
			Action.Enabled = !Enabled;
			Action.Disabled = Enabled;
			Action.Reset = (Tipo == enumComponente.pir && Stato == enumStato.on);
			Action.Schema = (Tipo != enumComponente.nodo && Tipo != enumComponente.centrale);
			Action.Regole = (Tipo != enumComponente.nodo && Tipo != enumComponente.centrale);
			Action.Property = true;
			Action.Remove = true;
		}
		public ComponenteAction Action { get; set; }
		public int Node_Num { get; set; }
		public int Node_Pin { get; set; }

		#region VALUE
		public List<string> Value { get; set; }

		#region VALUE for DB
		public void ValueFor_readDB(string val)
		{
			if (string.IsNullOrEmpty(val))
				Value = val.Split('§').ToList<string>(); 
		}
		public string ValueFor_writeDB()
		{
			string res = "";
			if (Value!=null)
				res = string.Join("§", Value);
			return res;
		}
		#endregion

		#region VALUE TEMPERATURE UMIDITY
		public string getTemperatureValue()
		{
			Decimal? res = getTemperature();
			return (res.HasValue) ? res.Value.ToString() : "---";
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

		public Decimal GetValueDecimal(int num = 1,string Lenguage = "it-IT")
		{
			Decimal res = 0;
			CultureInfo culture = new CultureInfo(Lenguage);
			res = Convert.ToDecimal(Value[num], culture);
			return res;
		}
		#endregion

		public string IPv4 { get; set; }
		public string IPv6 { get; set; }
		public string BlueTooth { get; set; }

		public string Options { get; set; }
		public string HWAddress { get; set; }
		public Follow follow { get; set; }

	}

	public class Componenti : Collection<Componente>
	{

	}

	public class ComponenteAction
	{
		public bool Enabled { get; set; }
		public bool Disabled { get; set; }
		public bool Regole { get; set; }
		public bool Schema { get; set; }
		public bool Reset { get; set; }
		public bool Property { get; set; }
		public bool Remove { get; set; }
	}
}
