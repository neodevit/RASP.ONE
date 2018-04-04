using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaEntity
{
	public class RecRule
	{
		public RecRule()
		{
			ITEM = new ObservableCollection<RecRules_item>();
			follow = new Follow();
		}
		public int? ID { get; set; }
		public void setIDRule()
		{
			foreach (RecRules_item item in ITEM)
				item.ID_RULE = ID;
		}
		public string NOME { get; set; }
		public string DESCRIZIONE { get; set; }

		public int TotLinea { get { return ITEM.Max(m => m.POS); } set { TotLinea = value; } }

		public ObservableCollection<RecRules_item> ITEM { get; set; }
		public bool exist(enumRulesType tipo,int id)
		{
			return ITEM.Any(prop => prop.Tipo == tipo && prop.ID == id);
		}
		public bool exist(enumRulesType tipo, int id, enumRulesValore Condizione)
		{
			return ITEM.Any(prop => prop.Tipo == tipo && prop.ID == id && prop.Condizione == Condizione);
		}
		public bool exist(enumRulesType tipo, int id, enumRulesValore Condizione,string Valore)
		{
			return ITEM.Any(prop => prop.Tipo == tipo && prop.ID == id && prop.Condizione == Condizione && prop.Valore == Valore);
		}


		public Follow follow { get; set; }

	}


	public enum enumRulesValore
	{
		nessuna=-1,
		off = 0,
		on = 1,
		uguale = 2,
		minore = 3,
		maggiore = 4
	}
	public enum enumRulesType
	{
		nessuna = -1,
		se = 0,
		azione = 1,
		wait = 2,
	}
	public class RecRules_item
	{
		public RecRules_item()
		{

		}
		public RecRules_item(enumRulesType tipo,int idrule,int id_componente,int pos, string nome, enumRulesValore cond, string val)
		{
			Tipo = tipo;
			ID_RULE = idrule;
			ID_Componente = id_componente;
			NOME = nome;
			Condizione = cond;
			Valore = val;
			POS = pos;
		}

		public int? ID { get; set; }
		public enumRulesType Tipo { get; set; }
		public string TipoDescr { get { return Tipo.ToString(); } }
		public int? ID_RULE { get; set; }
		public int ID_Componente { get; set; }
		public int POS { get; set; }
		public string NOME { get; set; }
		public enumRulesValore Condizione { get; set; }
		public bool CondizioneConValue { get { return (Condizione==enumRulesValore.maggiore || Condizione==enumRulesValore.minore || Condizione==enumRulesValore.uguale); } }
		public string Valore { get; set; }
	}




}
