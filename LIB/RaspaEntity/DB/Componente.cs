using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			follow = new Follow();
		}
		public Componente(int id,bool enabled, bool trusted,int num,string ipv4)
		{
			ID = id;
			Enabled = enabled;
			Trusted = trusted;
			Node_Num = num;
			IPv4 = ipv4;
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
		

		public string Descrizione { get; set; }

		public int IDComponenteTipo { get; set; }
		public enumComponente Tipo { get; set; }

		public enumStato Attivo
		{
			get
			{
				enumStato res = enumStato.off;
				switch(Tipo)
				{
					case enumComponente.centrale:
					case enumComponente.nodo:
						res = (Enabled && Trusted) ? enumStato.on : enumStato.off;
						break;
					case enumComponente.light:
						res = (Enabled && Trusted && Value=="1") ? enumStato.on : enumStato.off;
						break;
					case enumComponente.pir:
						res = (Enabled && Trusted && Value=="2") ? enumStato.on : enumStato.off;
						break;
				}
				return res;
			}
		}
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
			Action.Reset = (Tipo == enumComponente.pir && Attivo == enumStato.on);
			Action.Schema = (Tipo != enumComponente.nodo && Tipo != enumComponente.centrale);
			Action.Property = true;
			Action.Remove = true;
		}
		public ComponenteAction Action { get; set; }
		public int Node_Num { get; set; }
		public int Node_Pin { get; set; }
		public string Value { get; set; }
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
		public bool Schema { get; set; }
		public bool Reset { get; set; }
		public bool Property { get; set; }
		public bool Remove { get; set; }
	}
}
