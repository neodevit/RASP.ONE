﻿using RaspaDB;
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
using static RaspaCentral.MainPage;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RaspaCentral
{
	public sealed partial class Property : UserControl
	{
		public Property()
		{
			this.InitializeComponent();
		}
		public Componente componente;
		public MainPage chiamante;
		public RaspaResult esito;

		public RaspaResult showProperty(int ID, enumComponente tipo, MainPage pagina)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				chiamante = pagina;

				DBCentral DB = new DBCentral();
				componente = DB.GetComponenteByID(ID);
				if (componente == null)
				{
					componente = new Componente();
					componente.Tipo = tipo;
				}


				// TIPO
				switch (componente.Tipo)
				{
					case enumComponente.nessuno:
						break;
					case enumComponente.nodo:
						ToolbarPropertyShow(componente.Tipo);

						NODO_ID.Text = (componente.ID.HasValue) ? componente.ID.Value.ToString() : "-";
						NODO_ENABLED.IsChecked = componente.Enabled;
						NODO_TRUST.IsChecked = componente.Trusted;

						NODO_NOME.Text = componente.Nome ?? "";
						NODO_DESCR.Text = componente.Descrizione ?? "";
						NODO_NUM.Text = componente.Node_Num.ToString();
						if (NODO_NUM.Text == "")
							NODO_NUM.Focus(FocusState.Pointer);
						NODO_IP.Text = componente.IPv4 ?? "";
						NODO_RETE.Text = componente.HWAddress ?? "";
						break;
					case enumComponente.centrale:
						ToolbarPropertyShow(componente.Tipo);

						CENTRAL_ID.Text = (componente.ID.HasValue) ? componente.ID.Value.ToString() : "-";
						CENTRAL_ENABLED.IsChecked = componente.Enabled;
						CENTRAL_TRUST.IsChecked = componente.Trusted;
						CENTRAL_NOME.Text = componente.Nome ?? "";
						CENTRAL_DESCR.Text = componente.Descrizione ?? "";
						CENTRAL_NUM.Text = componente.Node_Num.ToString();
						if (CENTRAL_NUM.Text == "")
							CENTRAL_NUM.Focus(FocusState.Pointer);
						CENTRAL_IP.Text = componente.IPv4 ?? "";
						CENTRAL_RETE.Text = componente.HWAddress ?? "";

						break;
					case enumComponente.light:
						ToolbarPropertyShow(componente.Tipo);

						LIGHT_ID.Text = (componente.ID.HasValue) ? componente.ID.Value.ToString() : "-";
						LIGHT_ENABLED.IsChecked = componente.Enabled;
						LIGHT_ATTIVO.IsOn = componente.Attivo == enumStato.on;
						LIGHT_NOME.Text = componente.Nome ?? "";
						LIGHT_IP.Text = componente.IPv4 ?? "";
						LIGHT_DESCRIZIONE.Text = componente.Descrizione ?? "";

						// LOAD COMBO
						initPropertyComboNodes(LIGHT_NODO);
						initPropertyComboPIN(LIGHT_PIN);

						// NODE NUM
						LIGHT_NODO.SelectedValue = componente.Node_Num;
						LIGHT_PIN.SelectedValue = componente.Node_Pin;

						break;
					case enumComponente.pir:
						ToolbarPropertyShow(componente.Tipo);

						PIR_ID.Text = (componente.ID.HasValue) ? componente.ID.Value.ToString() : "-";
						PIR_ENABLED.IsChecked = componente.Enabled;
						PIR_ATTIVO.IsOn = componente.Attivo == enumStato.on;
						PIR_NOME.Text = componente.Nome ?? "";
						PIR_IP.Text = componente.IPv4 ?? "";
						PIR_DESCRIZIONE.Text = componente.Descrizione ?? "";

						// LOAD COMBO
						initPropertyComboNodes(PIR_NODO);
						initPropertyComboPIN(PIR_PIN);

						// NODE NUM
						PIR_NODO.SelectedValue = componente.Node_Num;
						PIR_PIN.SelectedValue = componente.Node_Pin;

						break;

					case enumComponente.webcam_ip:
						ToolbarPropertyShow(componente.Tipo);
						WEBCAM_ID.Text = (componente.ID.HasValue) ? componente.ID.Value.ToString() : "-";
						WEBCAM_ENABLED.IsChecked = componente.Enabled;
						WEBCAM_NOME.Text = componente.Nome ?? "";
						WEBCAM_IP.Text = componente.IPv4 ?? "";
						WEBCAM_VALUE.Text = componente.Value ?? "";
						break;
					case enumComponente.webcam_rasp:
						ToolbarPropertyShow(componente.Tipo);
						break;
				}

				// Action
				btnPropertyAnnulla.Visibility = Visibility.Visible;
				btnPropertySalva.Visibility = Visibility.Visible;
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			finally
			{
				esito = res;
			}
			return res;
		}
		private void ToolbarPropertyShow(enumComponente show)
		{
			NODO_Property.Visibility = Visibility.Collapsed;
			CENTRAL_Property.Visibility = Visibility.Collapsed;
			LIGHT_Property.Visibility = Visibility.Collapsed;
			PIR_Property.Visibility = Visibility.Collapsed;
			WEBCAM_Property.Visibility = Visibility.Collapsed;
			switch (show)
			{
				case enumComponente.nodo:
					NODO_Property.Visibility = Visibility.Visible;
					break;
				case enumComponente.centrale:
					CENTRAL_Property.Visibility = Visibility.Visible;
					break;
				case enumComponente.light:
					LIGHT_Property.Visibility = Visibility.Visible;
					break;
				case enumComponente.pir:
					PIR_Property.Visibility = Visibility.Visible;
					break;
				case enumComponente.webcam_ip:
					WEBCAM_Property.Visibility = Visibility.Visible;
					break;
				case enumComponente.webcam_rasp:
					break;

			}
		}



		private void btnPropertyAnnulla_Click(object sender, RoutedEventArgs e)
		{
			// Pulisci schermo
			chiamante.eliminaActualImageSoloSeProvvisioria();

			// toglie evidenziazione
			chiamante.evidenzia(false);

			// spegnere tutti i pannelly property
			chiamante.ToolbarShow(enumShowToolbar.componenti);

			// azzera componente attuale
			chiamante.azzeraActualComponent();
		}

		private void btnPropertySalva_Click(object sender, RoutedEventArgs e)
		{
			chiamante.messaggio.Text = "";
			try
			{

				// TRASFORMA IN OGGETTO
				esito = saveProperty2Object();
				if (!esito.Esito)
				{
					chiamante.messaggio.Text = esito.Message;
					return;
				}

				// CONTROLLI FORMALI
				esito = ControlliFormali();
				if (!esito.Esito)
				{
					chiamante.messaggio.Text = esito.Message;
					return;
				}

				// SAVE
				esito = saveOnDB();
				if (!esito.Esito)
				{
					chiamante.messaggio.Text = esito.Message;
					return;
				}

				// MODIFICA ORIGINALPAGE
				esito = ModifyOriginalPage();
				if (!esito.Esito)
				{
					chiamante.messaggio.Text = esito.Message;
					return;
				}

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				chiamante.messaggio.Text = "Errore Salvataggio : " + ex.Message;
			}
		}
		private RaspaResult saveProperty2Object()
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				if (componente == null)
					return new RaspaResult(false, "Componente Not found");

				switch (componente.Tipo)
				{
					case enumComponente.nodo:
						componente.Enabled = (NODO_ENABLED.IsChecked.HasValue) ? NODO_ENABLED.IsChecked.Value : false;
						componente.Trusted = (NODO_TRUST.IsChecked.HasValue) ? NODO_TRUST.IsChecked.Value : false;
						componente.Nome = NODO_NOME.Text;
						componente.Descrizione = NODO_DESCR.Text;
						componente.Node_Num = Convert.ToInt32(NODO_NUM.Text);
						componente.IPv4 = NODO_IP.Text;
						componente.HWAddress = NODO_RETE.Text;
						break;
					case enumComponente.centrale:
						componente.Enabled = (CENTRAL_TRUST.IsChecked.HasValue) ? CENTRAL_TRUST.IsChecked.Value : false;
						componente.Trusted = (CENTRAL_TRUST.IsChecked.HasValue) ? CENTRAL_TRUST.IsChecked.Value : false;
						componente.Nome = CENTRAL_NOME.Text;
						componente.Descrizione = CENTRAL_DESCR.Text;
						componente.Node_Num = Convert.ToInt32(CENTRAL_NUM.Text);
						componente.IPv4 = CENTRAL_IP.Text;
						componente.HWAddress = CENTRAL_RETE.Text;
						break;
					case enumComponente.light:
						componente.Enabled = (LIGHT_ENABLED.IsChecked.HasValue) ? LIGHT_ENABLED.IsChecked.Value : false;
						componente.Nome = LIGHT_NOME.Text;
						componente.IPv4 = LIGHT_IP.Text;
						componente.Descrizione = LIGHT_DESCRIZIONE.Text;
						componente.Node_Num = Convert.ToInt32(LIGHT_NODO.SelectedValue);
						componente.Node_Pin = Convert.ToInt32(LIGHT_PIN.SelectedValue);
						componente.Value = (LIGHT_ATTIVO.IsOn) ? "1" : "0";
						break;
					case enumComponente.pir:
						componente.Enabled = (PIR_ENABLED.IsChecked.HasValue) ? PIR_ENABLED.IsChecked.Value : false;
						componente.Nome = PIR_NOME.Text;
						componente.IPv4 = PIR_IP.Text;
						componente.Descrizione = PIR_DESCRIZIONE.Text;
						componente.Node_Num = Convert.ToInt32(PIR_NODO.SelectedValue);
						componente.Node_Pin = Convert.ToInt32(PIR_PIN.SelectedValue);
						componente.Value = (PIR_ATTIVO.IsOn) ? "1" : "0";
						break;
					case enumComponente.webcam_ip:
						componente.Enabled = (WEBCAM_ENABLED.IsChecked.HasValue) ? WEBCAM_ENABLED.IsChecked.Value : false;
						componente.Nome = WEBCAM_NOME.Text;
						componente.IPv4 = WEBCAM_IP.Text;
						componente.Value = WEBCAM_VALUE.Text;
						break;
					case enumComponente.webcam_rasp:
						break;
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			finally
			{
				esito = res;
			}
			return res;
		}
		private RaspaResult ControlliFormali()
		{
			RaspaResult res = new RaspaResult(true);
			bool ExistAltroIPv4Nodo = false;
			bool ExistAltroIPv4Centrale = false;
			bool ExistAltroIPv4Cam = false;
			try
			{
				DBCentral DB = new DBCentral();
			
				// ---------------------------
				// controlli formali
				// ---------------------------
				// NOME ESISTE GIA'
				bool ExistAltroNomeUguale = DB.existAltroComponenteConStessoNome(componente.ID, componente.Nome);
				if (ExistAltroNomeUguale)
					return new RaspaResult(false, "Errore : Esiste già il NOME : " + componente.Nome);
				

				// PER TIPO COMPONENTE
				switch (componente.Tipo)
				{
					case enumComponente.nodo:
					case enumComponente.centrale:
						// NODE NUM OBBLIGATORIO
						int num = Convert.ToInt32(componente.Node_Num);
						if (num == 0)
							return new RaspaResult(false, "Errore : Specificare un NODE NUM > 0");

						// NODE NUM DOPPIO
						bool ExistAltroNodoNum = DB.existAltroComponenteConStessoNodeNum(componente.ID, componente.Node_Num, enumComponente.nodo);
						if (ExistAltroNodoNum)
							return new RaspaResult(false, "Errore : Esiste NODO con lo stesso NUM : " + componente.Node_Num);

						bool ExistAltroCentraleNum = DB.existAltroComponenteConStessoNodeNum(componente.ID, componente.Node_Num, enumComponente.centrale);
						if (ExistAltroNodoNum)
							return new RaspaResult(false, "Errore : Esiste CENTRALE con lo stesso NUM : " + componente.Node_Num);

						// IP OBBLIGATORIO
						if (string.IsNullOrEmpty(componente.IPv4))
							return new RaspaResult(false, "Errore : Indirizzo IP Obbligatorio");
						
						// IP DOPPIO
						ExistAltroIPv4Nodo = DB.existAltroComponenteConStessoIPv4(componente.ID, componente.IPv4, enumComponente.nodo);
						if (ExistAltroIPv4Nodo)
							return new RaspaResult(false, "Errore : Esiste già un NODO con IPv4 : " + componente.IPv4);

						ExistAltroIPv4Centrale = DB.existAltroComponenteConStessoIPv4(componente.ID, componente.IPv4, enumComponente.centrale);
						if (ExistAltroIPv4Centrale)
							return new RaspaResult(false, "Errore : Esiste già un CENTRALE con IPv4 : " + componente.IPv4);

						ExistAltroIPv4Cam = DB.existAltroComponenteConStessoIPv4(componente.ID, componente.IPv4, enumComponente.webcam_ip);
						if (ExistAltroIPv4Cam)
							return new RaspaResult(false, "Errore : Esiste già una IPCAM con IPv4 : " + componente.IPv4);
						break;
					case enumComponente.pir:
					case enumComponente.light:
						// NODE NUM OBBLIGATORIO
						int numN = Convert.ToInt32(componente.Node_Num);
						if (numN == 0)
							return new RaspaResult(false, "Errore : Specificare un NODE NUM > 0");

						// PIN NUM OBBLIGATORIO
						int numP = Convert.ToInt32(componente.Node_Num);
						if (numP == 0)
							return new RaspaResult(false, "Errore : Specificare un NODE PIN");

						// NoDE & PIN DOPPIO
						bool ExistAltroNodePin = DB.existAltroComponenteConStessoNodeNumAndPin(componente.ID, componente.Node_Num, componente.Node_Pin);
						if (ExistAltroNodePin)
							return new RaspaResult(false, "Errore : Esiste già il Componente con NODO : " + componente.Node_Num + " e PIN " + componente.Node_Pin);
						break;
					case enumComponente.webcam_ip:
						// IP OBBLIGATORIO
						if (string.IsNullOrEmpty(componente.IPv4))
							return new RaspaResult(false, "Errore : Indirizzo IP Obbligatorio");

						// URL OBBLIGATORIA
						if (string.IsNullOrEmpty(componente.Value))
							return new RaspaResult(false, "Errore : URL ipcam grab image è Obbligatorio");

						// IP DOPPIO
						ExistAltroIPv4Nodo = DB.existAltroComponenteConStessoIPv4(componente.ID, componente.IPv4, enumComponente.nodo);
						if (ExistAltroIPv4Nodo)
							return new RaspaResult(false, "Errore : Esiste già un NODO con IPv4 : " + componente.IPv4);

						ExistAltroIPv4Centrale = DB.existAltroComponenteConStessoIPv4(componente.ID, componente.IPv4, enumComponente.centrale);
						if (ExistAltroIPv4Centrale)
							return new RaspaResult(false, "Errore : Esiste già un CENTRALE con IPv4 : " + componente.IPv4);

						ExistAltroIPv4Cam = DB.existAltroComponenteConStessoIPv4(componente.ID, componente.IPv4, enumComponente.webcam_ip);
						if (ExistAltroIPv4Cam)
							return new RaspaResult(false, "Errore : Esiste già una IPCAM con IPv4 : " + componente.IPv4);

						break;
					case enumComponente.webcam_rasp:
						break;
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "Errore : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			finally
			{
				esito = res;
			}
			return res;
		}
		private RaspaResult saveOnDB()
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				if (componente == null)
					return new RaspaResult(false, "Componente Not found");

				DBCentral DB = new DBCentral();

				// SAVE DB
				esito = DB.SetComponenti(componente, chiamante.Utente);

				// verifica esito
				if (!esito.Esito)
					return new RaspaResult(false, "Errore Salvataggio : " + esito.Message);
				
				// verifica id 
				if (!esito.ID.HasValue)
					return new RaspaResult(false, "Errore Salvataggio : non determino id Componente");


			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			finally
			{
				esito = res;
			}
			return res;
		}
		private RaspaResult ModifyOriginalPage()
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				if (componente == null)
					return new RaspaResult(false, "Componente Not found");


				// elimina image actual
				chiamante.eliminaActualImage();

				// actual componente
				chiamante.Actualcomponente = componente;

				// actual image
				chiamante.ActualImage = chiamante.create_object(componente.Tipo, componente);

				// spegnere tutti i pannelly property
				chiamante.ToolbarShow(enumShowToolbar.componenti);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			finally
			{
				esito = res;
			}
			return res;
		}


		#region COMBO
		Dictionary<int, int> elencoNODI;
		private void initPropertyComboNodes(ComboBox comboNodo)
		{
			try
			{
				// Dictionary
				elencoNODI = new Dictionary<int, int>();
				comboNodo.Items.Clear();

				// Inizializza dal DB
				DBCentral DB = new DBCentral();
				Componenti recs = DB.GetComponentedByTipoAndEnableAndTrustedAndAttivo(enumComponente.nodo, true, null, null);
				foreach (Componente rec in recs)
					if (rec.ID.HasValue)
					{
						elencoNODI.Add(rec.ID.Value, rec.Node_Num);
						comboNodo.Items.Add(rec.Node_Num);
					}
			}
			catch (Exception ex)
			{
				chiamante.messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void NODO_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				ComboBox comboNodo = sender as ComboBox;
				if (comboNodo == null)
					return;

				if ((componente.Tipo == enumComponente.light || componente.Tipo == enumComponente.pir) && comboNodo.SelectedValue != null)
				{
					// Leggo NODO ID selected
					int Node_Num = Convert.ToInt32(comboNodo.SelectedValue.ToString());
					// leggo dal db il nodo
					DBCentral DB = new DBCentral();
					Componenti recs = DB.GetComponenteByNodeNum(Node_Num);
					if (recs.Count > 0)
						if (componente.Tipo == enumComponente.light)
							LIGHT_IP.Text = recs[0].IPv4;
						else if (componente.Tipo == enumComponente.pir)
							PIR_IP.Text = recs[0].IPv4;
				}
			}
			catch (Exception ex)
			{
				chiamante.messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		Dictionary<int, int> elencoPIN;
		Dictionary<string, string> elencoPINvsFisico;
		private void initPropertyComboPIN(ComboBox comboPin)
		{
			try
			{

				// Dictionary
				elencoPIN = new Dictionary<int, int>();
				elencoPINvsFisico = new Dictionary<string, string>();
				comboPin.Items.Clear();

				// Inizializza dal DB
				DBCentral DB = new DBCentral();
				List<enumTipoPIN> tipi = new List<enumTipoPIN>() { enumTipoPIN.gpio, enumTipoPIN.gpioAndOther };
				GPIOPins recs = DB.GetGPIOPinTipe(tipi);
				foreach (GPIOPin rec in recs)
					if (rec.ID.HasValue)
					{
						elencoPIN.Add(rec.ID.Value, rec.GPIO);
						elencoPINvsFisico.Add(rec.GPIO.ToString(), rec.NUM.ToString());
						comboPin.Items.Add(rec.GPIO);
					}
			}
			catch (Exception ex)
			{
				chiamante.messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion
	}
}