using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RaspaEntity
{
	public class Node
	{
		public Node()
		{
			Action = new ComponenteAction();
			follow = new Follow();
		}
		public int? ID { get; set; }
		public int Num { get; set; }
		public bool Enabled { get; set; }
		public bool Trusted { get; set; }
		public enumStato Stato { get; set; }
		public string Nome { get; set; }
		public string Descrizione { get; set; }
		public NetworkInfo Network { get; set; }

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
			Action.Reset = Enabled;
			Action.Property = true;
			Action.Remove = true;
		}
		public ComponenteAction Action { get; set; }
		public Follow follow { get; set; }

	}

	public class Nodes : Collection<Node>
	{

	}
}
