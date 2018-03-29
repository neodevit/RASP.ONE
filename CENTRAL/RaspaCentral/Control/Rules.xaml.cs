using RaspaDB;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
		public void INIT(int id)
		{
			AzzeraMaschera();

			DBCentral DB = new DBCentral();
			Componente componente = DB.GetComponenteByID(id);
			if (componente == null)
				return;

			// componenti presenti a sistema
			initComboComponente(se_componente);
			initComboComponente(allora_componente);


			se_componente.SelectedValue = componente.Nome;

			// inizializzo combo valori possibili
			initComboValore(se_condizione);
			initComboValore(allora_condizione);
			initComboValore(allora_condizione_2);

			show(enumShow.scheda);


		}
		private void condizione_componente_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{


		}
		private void se_condizione_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RecRules.enumValore val = (RecRules.enumValore)se_condizione.SelectedIndex;
			if (val == RecRules.enumValore.uguale ||
				val == RecRules.enumValore.minore ||
				val == RecRules.enumValore.maggiore)
				se_valore.IsEnabled = true;
			else
				se_valore.IsEnabled = false;

		}

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


		private void allora_componente_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

		}
		private void allora_condizione_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RecRules.enumValore val = (RecRules.enumValore)allora_condizione.SelectedIndex;
			if (val == RecRules.enumValore.uguale ||
				val == RecRules.enumValore.minore ||
				val == RecRules.enumValore.maggiore)
				allora_valore.IsEnabled = true;
			else
				allora_valore.IsEnabled = false;

		}
		private void allora_condizione_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			RecRules.enumValore val = (RecRules.enumValore)allora_condizione_2.SelectedIndex;
			if (val == RecRules.enumValore.uguale ||
				val == RecRules.enumValore.minore ||
				val == RecRules.enumValore.maggiore)
				allora_valore_2.IsEnabled = true;
			else
				allora_valore_2.IsEnabled = false;

		}

		private void AzzeraMaschera()
		{
			// AZZERO CAMPI
			se_componente.SelectedIndex = -1;
			se_condizione.SelectedIndex = -1;
			se_valore.Text = "";
			se_valore.IsEnabled = false;
			allora_componente.SelectedIndex = -1;
			allora_condizione.SelectedIndex = -1;
			allora_valore.Text = "";
			allora_condizione_2.SelectedIndex = -1;
			allora_valore_2.Text = "";
			allora_attendi.Text = "0";
			allora_attendi_2.Text = "0";
			allora_valore.IsEnabled = false;
		}


		private void btnSave_Tapped(object sender, TappedRoutedEventArgs e)
		{
			RecRules rec = new RecRules();

			// SE
			rec.SE = (from p in elencoCOMPONENTI
								where p.Value.Contains(se_componente.SelectedValue.ToString())
								select p.Key)
								.FirstOrDefault();
			rec.SE_NOME = se_componente.SelectedValue.ToString();
			rec.SE_condizione = (RecRules.enumValore)se_condizione.SelectedIndex;
			rec.SE_valore = se_valore.Text;

			// ALLORA
			rec.ALLORA = (from p in elencoCOMPONENTI
					  where p.Value.Contains(allora_componente.SelectedValue.ToString())
					  select p.Key)
					.FirstOrDefault();
			rec.ALLORA_NOME = allora_componente.SelectedValue.ToString();

			rec.ALLORA_condizione = (RecRules.enumValore)allora_condizione.SelectedIndex;
			rec.ALLORA_valore = allora_valore.Text;

			rec.ALLORA_condizione_2 = (RecRules.enumValore)allora_condizione_2.SelectedIndex;
			rec.ALLORA_valore_2 = allora_valore_2.Text;

			rec.ALLORA_attendi = Convert.ToInt32(allora_attendi.Text);
			rec.ALLORA_attendi_2 = Convert.ToInt32(allora_attendi_2.Text);

			// INSERISCE IN LISTA
			ListViewItem regola_item = new ListViewItem();
			regola_item.Tag = 0;
			regola_item.Content = rec.TESTO;
			regole.Items.Insert(0,regola_item);
			//regole.Items.Insert(0, rec.TESTO);

			// AZZERA MASCHERA
			AzzeraMaschera();

			show(enumShow.lista);

		}


		#region ELENCO
		private void regole_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
		{

		}
		#endregion











		public void show(enumShow show)
		{
			lista.Visibility = Visibility.Collapsed;
			scheda.Visibility = Visibility.Collapsed;
			switch(show)
			{
				case enumShow.scheda:
					scheda.Visibility = Visibility.Visible;
					break;
				case enumShow.lista:
					lista.Visibility = Visibility.Visible;
					break;
			}
		}
		public enum enumShow
		{
			lista,
			scheda,
		}
		public class RecRules
		{
			public RecRules()
			{

			}
			public int SE { get; set; }
			public string SE_NOME { get; set; }
			public enumValore SE_condizione{ get; set; }
			public string SE_valore { get; set; }

			public int ALLORA { get; set; }
			public string ALLORA_NOME { get; set; }
			public enumValore ALLORA_condizione { get; set; }
			public string ALLORA_valore { get; set; }

			public enumValore ALLORA_condizione_2 { get; set; }
			public string ALLORA_valore_2 { get; set; }

			public int ALLORA_attendi { get; set; }
			public int ALLORA_attendi_2 { get; set; }



			public string TESTO { get { return SE_NOME + " " + SE_condizione.ToString() + " " + SE_valore + Environment.NewLine + ((ALLORA_attendi > 0) ? "-- > ATTENDI " + ALLORA_attendi + " sec" + Environment.NewLine : "") + "-- > " + ALLORA_NOME + " " + ALLORA_condizione.ToString() + " " + ALLORA_valore + Environment.NewLine + ((ALLORA_attendi_2 > 0) ? "-- > ATTENDI " + ALLORA_attendi_2 + " sec" + Environment.NewLine : "") + "-- > " + ALLORA_NOME + " " + ALLORA_condizione_2.ToString() + " " + ALLORA_valore_2; } }

			public enum enumValore
			{
				off=0,
				on=1,
				uguale=2,
				minore=3,
				maggiore=4
			}
		}

	}
}
