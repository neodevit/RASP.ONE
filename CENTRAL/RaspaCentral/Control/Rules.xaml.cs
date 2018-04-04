using RaspaDB;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace RaspaCentral
{

	public sealed partial class Rules : UserControl
	{
		public Rules()
		{
			this.InitializeComponent();
		}

		RecRule REC;
		ObservableCollection<RecRule> RECS;
		MainPage Pagina;
		public void INIT(int ID_Componente, MainPage pagina)
		{

			Pagina = pagina;


			// leggi dal DB rules precedenti
			// todo
			REC = new RecRule();


			popolateRules(ID_Componente);
			show(enumShow.rules);


			// INIT CONTROL
			INIT_CONTROLS();

		}

		private void INIT_CONTROLS()
		{
			// componenti presenti a sistema
			initComboComponente(se_componente);
			initComboComponente(allora_componente);

			// inizializzo combo valori possibili
			initComboValore(se_condizione);
			initComboValore(allora_condizione);
		}


		#region RULES
		private void popolateRules(int ID_Componente)
		{
			DBCentral DB = new DBCentral();
			RECS = DB.GetRulesByID_Componente(ID_Componente);


			grid_roles.ItemsSource = RECS;

		}


		private void grid_roles_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var physicalPoint = e.GetPosition(sender as RadDataGrid);
			var cell = (sender as RadDataGrid).HitTestService.CellInfoFromPoint(physicalPoint);

			if (cell != null)
			{
				RecRule riga = (RecRule)cell.Item;
				if (riga.ID.HasValue)
					popolateRule(riga.ID.Value);
			}
		}

		#endregion

		#region RULE
		private void popolateRule(int ID)
		{
			DBCentral DB = new DBCentral();
			REC = DB.GetRulesByID(ID);
			if (REC == null)
				return;

			// allinea a tutti IDRULE corrente
			REC.setIDRule();

			BindRule();

			show(enumShow.rule);

		}
		private void BindRule()
		{
			grid_role.ItemsSource = REC.ITEM;
		}

		private void grid_role_Tapped(object sender, TappedRoutedEventArgs e)
		{
			var physicalPoint = e.GetPosition(sender as RadDataGrid);
			var cell = (sender as RadDataGrid).HitTestService.CellInfoFromPoint(physicalPoint);

			if (cell != null)
			{
				RecRules_item riga = (RecRules_item)cell.Item;
				if (riga.ID.HasValue)
					switch (riga.Tipo)
					{
						case enumRulesType.se:
							AzzeraMaschera_SE();
							se_componente.SelectedValue = riga.NOME;
							se_condizione.SelectedValue = riga.Condizione;
							se_valore.Text = riga.Valore;
							show(enumShow.scheda_SE);
							break;
						case enumRulesType.azione:
							allora_componente.SelectedValue = riga.NOME;
							allora_condizione.SelectedValue = riga.Condizione;
							allora_valore.Text = riga.Valore;
							show(enumShow.scheda_ALLORA);
							break;
						case enumRulesType.wait:
							wait_Seconds.Value = Convert.ToInt32(riga.Valore);
							show(enumShow.scheda_WAIT);
							break;
					}
			}

		}

		private void btnAddIF_Click(object sender, RoutedEventArgs e)
		{
			show(enumShow.scheda_SE);
		}

		private void btnAddWait_Click(object sender, RoutedEventArgs e)
		{
			show(enumShow.scheda_WAIT);
		}

		private void btnAddAction_Click(object sender, RoutedEventArgs e)
		{
			show(enumShow.scheda_ALLORA);
		}
		private void btnSaveRule_Click(object sender, RoutedEventArgs e)
		{
			DBCentral DB = new DBCentral();
			RaspaResult res = DB.SetRule(REC, "Utente");
			if (!res.Esito)
				Pagina.messaggio.Text = res.Message;

		}


		#endregion

		#region POPUP
		private void btnSave_cancel_Click(object sender, RoutedEventArgs e)
		{
			show(enumShow.rule);
		}

		#region SE
		private void se_condizione_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			enumRulesValore val = (enumRulesValore)se_condizione.SelectedIndex;
			if (val == enumRulesValore.uguale ||
				val == enumRulesValore.minore ||
				val == enumRulesValore.maggiore)
				se_valore.IsEnabled = true;
			else
				se_valore.IsEnabled = false;

		}
		private void btnAddCondition_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// COMPONGO SE
			int se_id = 0;
			if (se_componente.SelectedValue != null)
				se_id = (from p in elencoCOMPONENTI
					  where p.Value.Contains(se_componente.SelectedValue.ToString())
					  select p.Key)
					.FirstOrDefault();

			string se_nome = "";
				if (se_componente.SelectedValue!= null)
					se_nome = se_componente.SelectedValue.ToString();

			enumRulesValore se_cond = (enumRulesValore)se_condizione.SelectedIndex;
			string val = se_valore.Text;

			// CREA ITEM
			RecRules_item se = new RecRules_item( enumRulesType.se,se_id, REC.ID.Value, REC.TotLinea++, se_nome, se_cond, val);

			// CONTROLLI FORMALI
			RaspaResult res = Verifica_SE(se);
			if (!res.Esito)
			{
				Pagina.messaggio.Text = res.Message;
				return;
			}

			// AGGIUNGI 
			REC.ITEM.Add(se);

			// REFRESH LISTA
			BindRule();

			// VISUALIZZA LISTA
			show(enumShow.rule);

		}
		private RaspaResult Verifica_SE(RecRules_item se)
		{
			RaspaResult res = new RaspaResult(true);
			if (!se.ID.HasValue)
				return new RaspaResult(false, "SE ID non trovato");
			if (se.Condizione == enumRulesValore.nessuna)
				return new RaspaResult(false, "Condizione è obbligatoria");
			else if (REC.exist(enumRulesType.se,se.ID.Value, se.Condizione, se.Valore))
				return new RaspaResult(false, "Rule già presente");
			else if (se.CondizioneConValue && string.IsNullOrEmpty(se.Valore))
				return new RaspaResult(false, "Valore Obbligatorio");

			return res;
		}

		private void AzzeraMaschera_SE()
		{
			// AZZERO CAMPI
			se_componente.SelectedIndex = -1;
			se_condizione.SelectedIndex = -1;
			se_valore.Text = "";
			se_valore.IsEnabled = false;
		}



		#endregion

		#region ALLORA
		private void allora_condizione_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			enumRulesValore val = (enumRulesValore)allora_condizione.SelectedIndex;
			if (val == enumRulesValore.uguale ||
				val == enumRulesValore.minore ||
				val == enumRulesValore.maggiore)
				allora_valore.IsEnabled = true;
			else
				allora_valore.IsEnabled = false;
		}
		private void btnAddallora_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// COMPONGO ALLORA
			int allora_id = 0;
			if (allora_componente.SelectedValue != null)
				allora_id = (from p in elencoCOMPONENTI
							 where p.Value.Contains(allora_componente.SelectedValue.ToString())
							 select p.Key)
						.FirstOrDefault();

			string allora_nome = "";
			if (allora_componente.SelectedValue != null)
				allora_nome = allora_componente.SelectedValue.ToString();

			enumRulesValore allora_cond = (enumRulesValore)allora_condizione.SelectedIndex;
			string val = allora_valore.Text;

			// CREA ITEM
			RecRules_item allora = new RecRules_item(enumRulesType.azione, allora_id, REC.ID.Value, REC.TotLinea++, allora_nome, allora_cond, val);

			// CONTROLLI FORMALI
			RaspaResult res = Verifica_ALLORA(allora);
			if (!res.Esito)
			{
				Pagina.messaggio.Text = res.Message;
				return;
			}

			// AGGIUNGI 
			REC.ITEM.Add(allora);

			// REFRESH LISTA
			BindRule();

			// VISUALIZZA LISTA
			show(enumShow.rule);

		}
		private RaspaResult Verifica_ALLORA(RecRules_item allora)
		{
			RaspaResult res = new RaspaResult(true);
			if (!allora.ID.HasValue)
				return new RaspaResult(false, "SE ID non trovato");
			if (allora.Condizione == enumRulesValore.nessuna)
				return new RaspaResult(false, "Condizione è obbligatoria");
			else if (REC.exist(enumRulesType.azione,allora.ID.Value, allora.Condizione, allora.Valore))
				return new RaspaResult(false, "Rule già presente");
			else if (allora.CondizioneConValue && string.IsNullOrEmpty(allora.Valore))
				return new RaspaResult(false, "Valore Obbligatorio");

			return res;
		}

		private void AzzeraMaschera_ALLORA()
		{
			// AZZERO CAMPI
			allora_componente.SelectedIndex = -1;
			allora_condizione.SelectedIndex = -1;
			allora_valore.Text = "";
			allora_valore.IsEnabled = false;
		}

		#endregion

		#region WAIT	

		private void btnSave_wait_Tapped(object sender, TappedRoutedEventArgs e)
		{
			int val = Convert.ToInt32(wait_Seconds.Value??0)*1000;

			// CONTROLLI FORMALI
			if (val==0)
			{
				Pagina.messaggio.Text = "Errore devi impostare un valore";
				return;
			}

			// CREA ITEM
			RecRules_item allora = new RecRules_item(enumRulesType.wait, 0, REC.ID.Value, REC.TotLinea++, "WAIT",  enumRulesValore.nessuna, val.ToString());

			// AGGIUNGI 
			REC.ITEM.Add(allora);

			// REFRESH LISTA
			BindRule();

			// VISUALIZZA LISTA
			show(enumShow.rule);
		}

		private void AzzeraMaschera_WAIT()
		{
			// AZZERO CAMPI
			wait_Seconds.Value = 0;
		}

		#endregion

		#endregion

		#region POPOLA COMBO
		Dictionary<int, string> elencoCOMPONENTI;
		private void initComboComponente(ComboBox combo)
		{
			try
			{
				// Dictionary
				elencoCOMPONENTI = new Dictionary<int, string>();
				combo.Items.Clear();

				// Inizializza dal DB
				DBCentral DB = new DBCentral();
				Componenti recs = DB.GetComponente_ECCETTO_NODO();
				foreach (Componente rec in recs)
					if (rec.ID.HasValue)
					{
						elencoCOMPONENTI.Add(rec.ID.Value, rec.Nome);
						combo.Items.Add(rec.Nome);
					}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void initComboValore(ComboBox combo)
		{
			try
			{
				combo.Items.Clear();
				combo.Items.Add("OFF");
				combo.Items.Add("ON");
				combo.Items.Add("Uguale di");
				combo.Items.Add("Minore di");
				combo.Items.Add("Maggiore di");
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion


		public enum enumShow
		{
			rules,
			rule,
			scheda_SE,
			scheda_ALLORA,
			scheda_WAIT,
		}
		public void show(enumShow show)
		{
			switch (show)
			{
				case enumShow.rules:
					Panel_rules.Visibility = Visibility.Visible;
					Panel_rule.Visibility = Visibility.Collapsed;
					scheda_SE.Visibility = Visibility.Collapsed;
					scheda_ALLORA.Visibility = Visibility.Collapsed;
					scheda_WAIT.Visibility = Visibility.Collapsed;
					break;
				case enumShow.rule:
					Panel_rules.Visibility = Visibility.Collapsed;
					Panel_rule.Visibility = Visibility.Visible;
					scheda_SE.Visibility = Visibility.Collapsed;
					scheda_ALLORA.Visibility = Visibility.Collapsed;
					scheda_WAIT.Visibility = Visibility.Collapsed;

					btnAddWait.Visibility = Visibility.Visible;
					btnAddAction.Visibility = Visibility.Visible;
					btnAddIF.Visibility = Visibility.Visible;
					break;
				case enumShow.scheda_SE:
					AzzeraMaschera_SE();

					Panel_rules.Visibility = Visibility.Collapsed;
					Panel_rule.Visibility = Visibility.Visible;
					scheda_SE.Visibility = Visibility.Visible;
					scheda_ALLORA.Visibility = Visibility.Collapsed;
					scheda_WAIT.Visibility = Visibility.Collapsed;

					btnAddWait.Visibility = Visibility.Collapsed;
					btnAddAction.Visibility = Visibility.Collapsed;
					btnAddIF.Visibility = Visibility.Collapsed;
					break;
				case enumShow.scheda_ALLORA:
					AzzeraMaschera_ALLORA();

					Panel_rules.Visibility = Visibility.Collapsed;
					Panel_rule.Visibility = Visibility.Visible;
					scheda_SE.Visibility = Visibility.Collapsed;
					scheda_ALLORA.Visibility = Visibility.Visible;
					scheda_WAIT.Visibility = Visibility.Collapsed;

					btnAddWait.Visibility = Visibility.Collapsed;
					btnAddAction.Visibility = Visibility.Collapsed;
					btnAddIF.Visibility = Visibility.Collapsed;
					break;
				case enumShow.scheda_WAIT:
					AzzeraMaschera_WAIT();

					Panel_rules.Visibility = Visibility.Collapsed;
					Panel_rule.Visibility = Visibility.Visible;
					scheda_SE.Visibility = Visibility.Collapsed;
					scheda_ALLORA.Visibility = Visibility.Collapsed;
					scheda_WAIT.Visibility = Visibility.Visible;

					btnAddWait.Visibility = Visibility.Collapsed;
					btnAddAction.Visibility = Visibility.Collapsed;
					btnAddIF.Visibility = Visibility.Collapsed;
					break;
			}
		}

	}
}
