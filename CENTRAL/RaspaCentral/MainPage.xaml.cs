﻿using RaspaDB;
using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Input;
using Windows.UI.Xaml.Shapes;
using System.Reflection;
using MQTTnet.Protocol;
using Windows.Security.ExchangeActiveSyncProvisioning;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspaCentral
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		public string Utente = "Fabio";

		public MainPage()
        {
            this.InitializeComponent();

		}

		bool flgINIT_NODO_Component = false;
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			// CENTRAL
			StartCentral();
			// MAPPA
			InitProperty();
			initMappa();
			// Init from node
			if (flgINIT_NODO_Component)
				initComponentiFromNode();

		}

		#region CENTRAL
		private static Componente centrale = null;

		bool flgMQTT = true;
		public async void StartCentral()
		{

			// --------------------------------------
			// NODO
			// --------------------------------------
			// Inizializza nodo
			RaspaResult resNode = INIT_NODE();
			if (!resNode.Esito)
				messaggio.Text = resNode.Message;
			else
				NODE_SHOW();

			// --------------------------------------
			// MTMQ
			// --------------------------------------
			if (flgMQTT)
				INIT_MQTT();

		}
		private RaspaResult INIT_NODE()
		{
			RaspaResult res = new RaspaResult(true);

			try
			{
				RaspBerry client = new RaspBerry();
				string IPv4 = client.GetLocalIPv4();

				DBCentral db = new DBCentral();
				centrale = db.GetComponenteByIPv4(IPv4, enumComponente.centrale);

				if (centrale == null)
					res = new RaspaResult(false, "Centrale non riconosciuto nel sistema");

				centrale.HostName = client.GetHostName();
				centrale.IPv4 = IPv4;
				centrale.IPv6 = "";
				centrale.HWAddress = client.GetHWAddress();
				centrale.BlueTooth = "";
				centrale.OSVersion = client.GetOSVersion();
				centrale.NodeSWVersion = client.GetRASPANodeVersion();

				centrale.SystemProductName = client.SystemProductName();
				centrale.SystemID = client.GetHardwareID();

				res = db.SetComponenti(centrale, Utente);

			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("INIT_NODE : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		private void NODE_SHOW()
		{
			try
			{

				NodeNum.Text = centrale.Node_Num.ToString();
				NodeName.Text = centrale.Nome ?? "";

				CENTRALHostName.Text = centrale.HostName ?? "";
				CENTRAL_IPv4.Text = centrale.IPv4 ?? "";
				CENTRAL_IPv6.Text = centrale.IPv6 ?? "";
				CENTRAL_OS.Text = "Windows (" + centrale.OSVersion + ")" ?? "";
				CENTRAL_Platform.Text = centrale.SystemProductName ?? "";
				CENTRAL_Serial.Text = centrale.SystemID ?? "";

				CENTRAL_SWVer.Text = "RASP.one CENTRALE : ver " + centrale.NodeSWVersion ?? "";

			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("UPDATE_NODE : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
		}

		#region MQTT
		bool FlgMQTT_CentralServer = false;
		MQTT mqTT;
		private void INIT_MQTT()
		{
			try
			{
				mqTT = new MQTT(centrale.SystemID, centrale.HostName, centrale.IPv4);

				if (FlgMQTT_CentralServer)
				{
					mqTT.createServer();
					mqTT.startServer();
				}

				mqTT.createClient();
				mqTT.startClient();

				mqTT.Receive -= MQTTReceive;
				mqTT.Receive += MQTTReceive;
				mqTT.Logging -= MQTTLogging;
				mqTT.Logging += MQTTLogging;

			}
			catch (Exception ex)
			{
				messaggio.Text = ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void MQTTLogging(string Messaggio)
		{
			var ignore = Trasmission.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				writeLogVideo(Messaggio);
			});
		}

		private void writeLogVideo(string Messaggio)
		{
			// scrivi log
			Trasmission.Items.Insert(0, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")+ " " + Messaggio);
			// pulisci log vecchi
			try
			{
				if (Trasmission.Items.Count > 50)
					for (int i = 50; i <= Trasmission.Items.Count; i++)
						Trasmission.Items.RemoveAt(i);
			}
			catch { }
		}

		#endregion


		#region SEND
		private async void MQTTRSend(RaspaProtocol protocol)
		{
			try
			{
				// send message
				mqTT.Publish(protocol);
			}
			catch (Exception ex)
			{
				messaggio.Text = ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion

		#region INIT
		private void initComponentiFromNode()
		{
			RaspaProtocol protocol;
			messaggio.Text = "";
			try
			{
				// Inizializza componenti mappa leggendoli dal DB
				DBCentral DB = new DBCentral();
				Componenti recs = DB.GetComponenti();
				foreach (Componente rec in recs)
				{
					switch (rec.Tipo)
					{
						case enumComponente.centrale:
							break;
						case enumComponente.nodo:
							break;
						case enumComponente.light:
							protocol = new RaspaProtocol();
							protocol.Mittente = centrale;
							protocol.Destinatario = rec;
							protocol.Comando = enumComando.get;

							writeLogVideo("INIT --> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " value : " + protocol.Value);
							MQTTRSend(protocol);

							break;
						case enumComponente.pir:
							if (rec.Enabled)
							{
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = rec;
								protocol.Comando = enumComando.comando;
								protocol.Value = ((int)enumPirValue.on).ToString();

								writeLogVideo("INIT --> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " value : " + protocol.Value);
								MQTTRSend(protocol);
							}
							break;
						case enumComponente.webcam_ip:
							break;
						case enumComponente.webcam_rasp:
							break;

					}
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}

		#endregion

		#region RECEVICE
		private void MQTTReceive(string Topic, string Payload, MqttQualityOfServiceLevel QoS, bool Retain)
		{
			var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				AggiornaFormConIRisultati(Payload);
			});
		}
		private void AggiornaFormConIRisultati(string message)
		{
			RaspaProtocol protocol = new RaspaProtocol(message);
			try
			{
				DBCentral DB = new DBCentral();
				// Modifica interfaccia a fronte della notifica
				foreach (var control in GridMappa.Children)
				{
					var img = control as Image;
					if (img == null)
						continue;

					if (img.Tag == null)
						continue;

					RaspaTag tag = new RaspaTag(img.Tag.ToString());
					if (tag.CompareMittente(protocol))
					{
						switch (protocol.Comando)
						{
							case enumComando.notify:
								#region NOTIFY

								// -----------------------------------------------
								// Aggiorno value sul DB
								// -----------------------------------------------
								RaspaResult res = DB.ModComponentiValue(tag.ID, protocol.Value, Utente);
								if (!res.Esito)
								{
									messaggio.Text = "Errore Receive command but not found ID " + tag.ID + " on DB";
									return;
								}

								// aggiorno immagine
								switch (protocol.Mittente.Tipo)
								{
									case enumComponente.light:
										enumPINValue valueLight = (enumPINValue)Convert.ToInt32(protocol.Value);
										switch (valueLight)
										{
											case enumPINValue.nessuno:
												img.Source = light_err.Source;
												break;
											case enumPINValue.off:
												img.Source = light_off.Source;
												break;
											case enumPINValue.on:
												img.Source = light_on.Source;
												break;
											case enumPINValue.err:
												img.Source = light_err.Source;
												break;
										}
										break;
									case enumComponente.pir:
										enumPirValue valuePir = (enumPirValue)Convert.ToInt32(protocol.Value);

										switch (valuePir)
										{
											case enumPirValue.nessuno: // nessuno
												img.Source = bell_err.Source;
												break;
											case enumPirValue.off: // spento
												img.Source = bell_off.Source;
												break;
											case enumPirValue.on: // acceso
												img.Source = bell_on.Source;
												break;
											case enumPirValue.signal: // segnale passaggio
												img.Source = bell_active.Source;
												break;
											case enumPirValue.err: // segnale passaggio
												img.Source = bell_err.Source;
												break;
										}
										break;
								}
								break;
								#endregion
							case enumComando.get:
								#region GET
								//-------------------------------------
								//               GET
								//-------------------------------------
								switch (protocol.Mittente.Tipo)
								{
									case enumComponente.light:
										// -----------------------------------------------
										// Aggiorno value sul DB
										// -----------------------------------------------
										RaspaResult resGet = DB.ModComponentiValue(tag.ID, protocol.Value, Utente);
										if (!resGet.Esito)
										{
											messaggio.Text = "Errore Receive GET but not found ID " + tag.ID + " on DB";
											return;
										}

										enumPINValue valueGET = (enumPINValue)Convert.ToInt32(protocol.Value);
										switch (valueGET)
										{
											case enumPINValue.nessuno:
												img.Source = light_err.Source;
												break;
											case enumPINValue.off:
												img.Source = light_off.Source;
												break;
											case enumPINValue.on:
												img.Source = light_on.Source;
												break;
											case enumPINValue.err:
												img.Source = light_err.Source;
												break;
										}
										break;
									case enumComponente.pir:
										enumPirValue valuePir = (enumPirValue)Convert.ToInt32(protocol.Value);

										switch (valuePir)
										{
											case enumPirValue.nessuno: // nessuno
												img.Source = bell_err.Source;
												break;
											case enumPirValue.off: // spento
												img.Source = bell_off.Source;
												break;
											case enumPirValue.on: // acceso
												img.Source = bell_on.Source;
												break;
											case enumPirValue.signal: // segnale passaggio
												img.Source = bell_active.Source;
												break;
											case enumPirValue.err: // segnale passaggio
												img.Source = bell_err.Source;
												break;
										}
										break;
								}
								#endregion
								break;
							case enumComando.set:
								#region SET
								#endregion
								break;
							case enumComando.comando:
								#region comando
								#endregion
								break;
							case enumComando.nodeInit:
								#region NODE INIT
								// leggo il nodo da iniziare sul DB
								Componente nodo2INIT = DB.GetComponenteByIPv4(protocol.Mittente.IPv4, enumComponente.nodo);

								// se il componente è effettivamente un nodo
								if (nodo2INIT.Tipo== enumComponente.nodo && protocol.Mittente != null)
								{
									// Aggiorno i dati del nodo
									nodo2INIT.HostName = protocol.Mittente.HostName;
									nodo2INIT.HWAddress = protocol.Mittente.HWAddress;
									nodo2INIT.IPv4 = protocol.Mittente.IPv4;
									nodo2INIT.IPv6 = protocol.Mittente.IPv6;
									nodo2INIT.NodeSWVersion = protocol.Mittente.NodeSWVersion;
									nodo2INIT.OSVersion = protocol.Mittente.OSVersion;
									nodo2INIT.SystemID = protocol.Mittente.SystemID;
									nodo2INIT.SystemProductName = protocol.Mittente.SystemProductName;

									// salvo i dati aggiornati sul database
									RaspaResult resNodeInit = DB.SetComponenti(nodo2INIT, Utente);
								}
								break;

							#endregion
							case enumComando.nodeReload:
								#region NODE RELOAD
								// leggo il nodo da reload sul DB
								Componenti componenti = DB.GetComponentiByIPv4(protocol.Mittente.IPv4);

								// analizza i componenti
								foreach (Componente componente in componenti)
								{
									switch (componente.Tipo)
									{
										case enumComponente.nessuno:
										case enumComponente.centrale:
										case enumComponente.nodo:
											break;
										case enumComponente.pir:
											// manda messaggio al nodo per inizializzare questo PIR
											// con i dati presenti nel DB

											break;
										case enumComponente.light:
											// manda messaggio al nodo per inizializzare questa LIGHT
											// con i dati presenti nel DB
											RaspaProtocol initLight = new RaspaProtocol();
											protocol.Comando = enumComando.comando;
											protocol.Mittente = centrale;
											protocol.Destinatario = componente;
											protocol.Value = componente.Value;

											MQTTRSend(protocol);
											break;
									}
								}
								
								#endregion
								break;
							default:
								break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion


		#endregion

		#region WORKING
		private RaspaResult Comando(Image img,int IDComponente)
		{
			RaspaProtocol protocol;
			RaspaResult res = new RaspaResult(true);
			try
			{
				DBCentral DB = new DBCentral();
				Componente componente = DB.GetComponenteByID(IDComponente);
				if (componente == null)
				{
					messaggio.Text = "Nessun comando per il componente ID " + IDComponente + " non trovato sul DB";
					return new RaspaResult(false, "Nessun comando per il componente ID " + IDComponente + " non trovato sul DB");
				}

				switch (componente.Tipo)
				{
					case enumComponente.centrale:
						break;
					case enumComponente.nodo:
						break;
					case enumComponente.pir:

						enumPirValue valuePIR = (enumPirValue)Convert.ToInt32(componente.Value);
						switch(valuePIR)
						{
							case enumPirValue.nessuno:
								// ---------------------------------
								//            PIR ERR
								// ---------------------------------
								if (Debugger.IsAttached) Debugger.Break();
								break;
							case enumPirValue.err:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.get;
								protocol.Value = ((int)enumPirValue.off).ToString();

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " GET ");
								MQTTRSend(protocol);
								break;
							case enumPirValue.off:
								// ---------------------------------
								// chiama nodo per PIR ON
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Value = ((int)enumPirValue.on).ToString();

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " value : " + protocol.Value);
								MQTTRSend(protocol);
								break;
							case enumPirValue.on:
								// ---------------------------------
								// chiama nodo per PIR OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Value = ((int)enumPirValue.off).ToString();

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " value : " + protocol.Value);
								MQTTRSend(protocol);

								break;
							case enumPirValue.signal:
								// ---------------------------------
								//         CANCEL SIGNAL
								// aggiorna solo la form risultati
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = componente;
								protocol.Destinatario = centrale;
								protocol.Comando = enumComando.notify;
								protocol.Value = ((int)enumPirValue.on).ToString();
								AggiornaFormConIRisultati(protocol.BuildJson());

								break;
						}
						break;
					case enumComponente.light:
						enumPINValue valueLIGHT = (enumPINValue)Convert.ToInt32(componente.Value);

						switch(valueLIGHT)
						{
							case enumPINValue.nessuno:
								// ---------------------------------
								//         LIGT ERR
								// ---------------------------------
								if (Debugger.IsAttached) Debugger.Break();
								break;
							case enumPINValue.err:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.get;
								protocol.Value = ((int)enumPINValue.off).ToString();

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " GET ");
								MQTTRSend(protocol);
								break;
							case enumPINValue.off:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Value = ((int)enumPINValue.on).ToString();

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " value : " + protocol.Value);
								MQTTRSend(protocol);
								break;
							case enumPINValue.on:
								// ---------------------------------
								// chiama nodo per LIGHT OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Value = ((int)enumPINValue.off).ToString();

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " value : " + protocol.Value);
								MQTTRSend(protocol);
								break;
						}
						break;
					case enumComponente.webcam_ip:
						ipcam1.playIPCam(componente.Nome, componente.Value);
						ipcam1.Visibility = Visibility.Visible;
						break;
					case enumComponente.webcam_rasp:
						break;
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, ex.Message);
			}
			return res;
		}
		#endregion

		#region MAPPA INTERATIVA
		public Componente Actualcomponente;
		public Image ActualImage;
		enumComponente ActualTipoComponente;
		Image selectedTool = null;
		PointerPoint selectedPointer = null;
		private void initMappa()
		{
			messaggio.Text = "";
			try
			{
				// Visualizza Mappa
				TabsShow(enumShowTabs.mappa);
				// mode
				working.IsChecked = true;
				editing.IsChecked = false;
				changeMode();
				// popolate componenti
				popolateComponenti();
				// Prepara frame CAM
				GridCAM.Visibility = Visibility.Visible;
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void popolateComponenti()
		{
			messaggio.Text = "";
			try
			{
				// Inizializza componenti mappa leggendoli dal DB
				DBCentral DB = new DBCentral();
				Componenti recs = DB.GetComponenti();
				foreach (Componente rec in recs)
					create_object(rec.Tipo, rec);
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void btnRefresh_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// Visualizza Mappa
			TabsShow(enumShowTabs.mappa);
			// pulisci interfaccia 
			CleanInterface();
			// ricarica componenti
			popolateComponenti();
		}

		private void CleanInterface()
		{
			RaspaResult esito = new RaspaResult(true);
			List<Image> daCancellare = new List<Image>();
			try
			{
				foreach (var control in GridMappa.Children)
				{
					var img = control as Image;
					if (isComponentRaspone(img))
						daCancellare.Add(img);
				}
				foreach(Image img in daCancellare)
					// rimuove oggetto dalla mappa
					GridMappa.Children.Remove((UIElement)img);

				ActualImage = null;

				// toglie evidenziazione
				evidenziazione.Visibility = Visibility.Collapsed;

				// update layout
				GridMappa.UpdateLayout();
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore Eliminazione : " + ex.Message;
			}
		}
		#region Editing/Working
		public enum enumMode
		{
			working,
			edit,
		}
		enumMode RaspaMode = enumMode.working;
		private void mode_Tapped(object sender, TappedRoutedEventArgs e)
		{
			try
			{
				ToggleButton but = sender as ToggleButton;
				if (but != working)
					working.IsChecked = !working.IsChecked;
				if (but != editing)
					editing.IsChecked = !editing.IsChecked;
				changeMode();
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void changeMode()
		{
			messaggio.Text = "";
			try
			{
				if (editing.IsChecked != null && editing.IsChecked.Value)
				{
					ToolbarShow(enumShowToolbar.componenti);
					RaspaMode = enumMode.edit;
				}
				else
				{
					ToolbarShow(enumShowToolbar.help);
					RaspaMode = enumMode.working;

					// Elimina componenti in sospeso
					eliminaActualImageSoloSeProvvisioria();
					// toglie evidenziazione
					evidenzia(false);
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}


		#endregion
		#region CREATE OBJECT
		private string initActualComponente(enumComponente comp,Componente item=null)
		{
			string ToolTipCustom = "";
			try
			{
				// Leggo dal DB
				Actualcomponente = new Componente();
				Actualcomponente.Tipo = comp;
				Actualcomponente.ID = (item == null) ? 0 : item.ID;
				Actualcomponente.Enabled = (item == null) ? true : item.Enabled;
				Actualcomponente.Trusted = (item == null) ? true : item.Trusted;
				Actualcomponente.Descrizione = (item == null) ? "" : item.Descrizione;
				Actualcomponente.Node_Num = (item == null) ? 0 : item.Node_Num;
				Actualcomponente.Node_Pin = (item == null) ? 0 : item.Node_Pin;

				if (item != null)
				{
					Actualcomponente.PositionLeft = item.PositionLeft;
					Actualcomponente.PositionTop = item.PositionTop;
					Actualcomponente.PositionRight = item.PositionRight;
					Actualcomponente.PositionBottom = item.PositionBottom;
				}

				switch (comp)
				{
					case enumComponente.nodo:
						Actualcomponente.Nome = (item == null) ? "NODO " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "192.168.1." : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "Enabled : " + ((Actualcomponente.Enabled) ? "SI" : "NO") + Environment.NewLine +
										"Trusted : " + ((Actualcomponente.Trusted) ? "SI" : "NO") +
										"HOSTNAME : " + Actualcomponente.Nome + Environment.NewLine +
										"NODE NUM : " + Actualcomponente.Node_Num + Environment.NewLine +
										"IP : " + Actualcomponente.IPv4 + Environment.NewLine +
										"HW Address : " + Actualcomponente.HWAddress + Environment.NewLine;

						Actualcomponente.Value = (item == null) ? "" : item.Value;

						break;
					case enumComponente.centrale:
						Actualcomponente.Nome = (item == null) ? "CENTRALE " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "192.168.1." : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "Enabled : " + ((Actualcomponente.Enabled) ? "SI" : "NO") + Environment.NewLine +
										"Trusted : " + ((Actualcomponente.Trusted) ? "SI" : "NO") +
										"HOSTNAME : " + Actualcomponente.Nome + Environment.NewLine +
										"NODE NUM : " + Actualcomponente.Node_Num + Environment.NewLine +
										"IP : " + Actualcomponente.IPv4 + Environment.NewLine +
										"HW Address : " + Actualcomponente.HWAddress + Environment.NewLine;

						Actualcomponente.Value = (item == null) ? "" : item.Value;

						break;
					case enumComponente.light:
						Actualcomponente.Nome = (item == null) ? "LUCE " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "Enabled : " + ((Actualcomponente.Enabled) ? "SI" : "NO") + Environment.NewLine +
										"Attivo : " + ((Actualcomponente.Attivo == enumStato.on) ? "SI" : "NO") + Environment.NewLine +
										"Trusted : " + ((Actualcomponente.Trusted) ? "SI" : "NO") +
										"NOME : " + Actualcomponente.Nome + Environment.NewLine +
										"NODE NUM : " + Actualcomponente.Node_Num + Environment.NewLine +
										"NODE PIN : " + Actualcomponente.Node_Pin + Environment.NewLine +
										"IP : " + Actualcomponente.IPv4 + Environment.NewLine +
										"HW Address : " + Actualcomponente.HWAddress + Environment.NewLine;

						Actualcomponente.Value = (item == null) ? "1" : item.Value;

						break;
					case enumComponente.pir:
						Actualcomponente.Nome = (item == null) ? "PIR " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "Enabled : " + ((Actualcomponente.Enabled) ? "SI" : "NO") + Environment.NewLine +
										"Attivo : " + ((Actualcomponente.Attivo == enumStato.on) ? "SI" : "NO") + Environment.NewLine +
										"Trusted : " + ((Actualcomponente.Trusted) ? "SI" : "NO") +
										"NOME : " + Actualcomponente.Nome + Environment.NewLine +
										"NODE NUM : " + Actualcomponente.Node_Num + Environment.NewLine +
										"NODE PIN : " + Actualcomponente.Node_Pin + Environment.NewLine +
										"IP : " + Actualcomponente.IPv4 + Environment.NewLine +
										"HW Address : " + Actualcomponente.HWAddress + Environment.NewLine;

						Actualcomponente.Value = (item == null) ? "2" : item.Value;

						break;
					case enumComponente.webcam_ip:
						Actualcomponente.Nome = (item == null) ? "WEBCAM" : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "Enabled : " + ((Actualcomponente.Enabled) ? "SI" : "NO") + Environment.NewLine +
										"Attivo : " + ((Actualcomponente.Attivo == enumStato.on) ? "SI" : "NO") + Environment.NewLine +
										"Trusted : " + ((Actualcomponente.Trusted) ? "SI" : "NO") +
										"NOME : " + Actualcomponente.Nome + Environment.NewLine +
										"IP : " + Actualcomponente.IPv4 + Environment.NewLine +
										"HW Address : " + Actualcomponente.HWAddress + Environment.NewLine;

						Actualcomponente.Value = (item == null) ? "" : item.Value;

						break;

				}

				Actualcomponente.calcAction();
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
			return ToolTipCustom;
		}
		public Image create_object(enumComponente comp, Componente item = null)
		{
			Image immagine = new Image();
			try
			{
				// init actual componente
				string ToolTipCustom = initActualComponente(comp, item);

				// creo TAG : ID_VALUE_COMP
				string Tag = "RASP.ONE_" + ((item != null && item.ID.HasValue) ? item.ID.Value + "_" + ((int)comp).ToString() : "0_" + ((int)comp).ToString() );
				// se esiste già una immagine con quel tag non la ricreo
				if (ExistComponentWithTagInPage(Tag))
					return null;

				// Creo Immagine da Tools
				immagine.Name = (item != null && item.ID.HasValue) ? "C_" + item.ID.Value : "C_0";
				immagine.Tag = Tag;
				immagine.Margin = new Thickness(Actualcomponente.PositionLeft, Actualcomponente.PositionTop, Actualcomponente.PositionRight, Actualcomponente.PositionBottom);
				immagine.Width = 16;
				immagine.Height = 16;
				immagine.CanDrag = true;
				immagine.Source = choseImageByComponente(Actualcomponente);
				immagine.VerticalAlignment = VerticalAlignment.Top;
				immagine.HorizontalAlignment = HorizontalAlignment.Left;


				// TOOLTIP
				ToolTip ttip = new ToolTip();
				ttip.Content = "COMPONENTE " + comp.ToString().ToUpperInvariant() + Environment.NewLine +
								"ID : " + ((Actualcomponente.ID.HasValue) ? Actualcomponente.ID.Value.ToString() : "-") + Environment.NewLine +
								ToolTipCustom;
				ToolTipService.SetToolTip(immagine, ttip);

				// eventi
				immagine.Tapped += componente_LeftTapped;
				immagine.DragStarting += Button_DragStarting;
				immagine.PointerPressed += Img_PointerPressed;
				immagine.RightTapped += componente_RightTapped;

				// Menu
				MenuFlyout myFlyout = new MenuFlyout();
				MenuFlyoutItem a1 = new MenuFlyoutItem();
				MenuFlyoutItem a2 = new MenuFlyoutItem();
				MenuFlyoutItem a3 = new MenuFlyoutItem();
				MenuFlyoutItem a4 = new MenuFlyoutItem();
				MenuFlyoutItem a5 = new MenuFlyoutItem();
				MenuFlyoutItem a6 = new MenuFlyoutItem();
				MenuFlyoutItem a7 = new MenuFlyoutItem();

				a1 = new MenuFlyoutItem { Text = "Reset", IsEnabled = Actualcomponente.Action.Reset, Tag = Actualcomponente.ID };
				a2 = new MenuFlyoutItem { Text = "Enabled", IsEnabled = Actualcomponente.Action.Enabled, Tag = Actualcomponente.ID };
				a3 = new MenuFlyoutItem { Text = "Disabled", IsEnabled = Actualcomponente.Action.Disabled, Tag = Actualcomponente.ID };
				a4 = new MenuFlyoutItem { Text = "Schema", IsEnabled = Actualcomponente.Action.Schema, Tag = Actualcomponente.ID };
				a5 = new MenuFlyoutItem { Text = "Regole", IsEnabled = Actualcomponente.Action.Regole, Tag = Actualcomponente.ID };
				a6 = new MenuFlyoutItem { Text = "Property", IsEnabled = Actualcomponente.Action.Property, Tag = Actualcomponente.ID };
				a7 = new MenuFlyoutItem { Text = "Remove", IsEnabled = Actualcomponente.Action.Remove, Tag = Actualcomponente.ID };


				a1.Click += MenuFlyoutItem_Click;
				a2.Click += MenuFlyoutItem_Click;
				a3.Click += MenuFlyoutItem_Click;
				a4.Click += MenuFlyoutItem_Click;
				a5.Click += MenuFlyoutItem_Click;
				a6.Click += MenuFlyoutItem_Click;
				a7.Click += MenuFlyoutItem_Click;
				myFlyout.Items.Add(a1);
				myFlyout.Items.Add(a2);
				myFlyout.Items.Add(a3);
				myFlyout.Items.Add(a4);
				myFlyout.Items.Add(a5);
				myFlyout.Items.Add(a6);

				// aggiungi menu al bottone
				FlyoutBase.SetAttachedFlyout((FrameworkElement)immagine, myFlyout);

				// ADD PAGINA
				GridMappa.Children.Add(immagine);
				GridMappa.UpdateLayout();

			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
			return immagine;
		}

		private BitmapImage choseImageByComponente(Componente oggetto)
		{
			BitmapImage res = new BitmapImage(new Uri("ms-appx:///Assets/error.png"));

			switch (oggetto.Tipo)
			{
				case enumComponente.nessuno:
					res = new BitmapImage(new Uri("ms-appx:///Assets/error.png"));
					break;
				case enumComponente.light:
					enumPINValue valueLight = (enumPINValue)Convert.ToInt32(oggetto.Value);
					switch (valueLight)
					{
						case enumPINValue.nessuno:
							res = new BitmapImage(new Uri("ms-appx:///Assets/light_disabled.png"));
							break;
						case enumPINValue.off:
						res = new BitmapImage(new Uri("ms-appx:///Assets/light_off.png"));
							break;
						case enumPINValue.on:
							res = new BitmapImage(new Uri("ms-appx:///Assets/light_on.png"));
							break;
						case enumPINValue.err:
							res = new BitmapImage(new Uri("ms-appx:///Assets/light_err.png"));
							break;
					}
					break;
				case enumComponente.pir:
					int val = (!string.IsNullOrEmpty(oggetto.Value)) ? Convert.ToInt32(oggetto.Value) : 0;
					enumPirValue valuePIR = (enumPirValue)val;
					switch(valuePIR)
					{
						case enumPirValue.nessuno:
							res = new BitmapImage(new Uri("ms-appx:///Assets/bell_err.png"));
							break;
						case enumPirValue.off:
							res = new BitmapImage(new Uri("ms-appx:///Assets/bell_off.png"));
							break;
						case enumPirValue.on:
							res = new BitmapImage(new Uri("ms-appx:///Assets/bell_on.png"));
							break;
						case enumPirValue.signal:
							res = new BitmapImage(new Uri("ms-appx:///Assets/bell_active.png"));
							break;
						case enumPirValue.err:
							res = new BitmapImage(new Uri("ms-appx:///Assets/bell_err.png"));
							break;
					}

					break;
				case enumComponente.nodo:
					res = new BitmapImage(new Uri("ms-appx:///Assets/raspberry.png"));
					if (!oggetto.Enabled)
						res = new BitmapImage(new Uri("ms-appx:///Assets/raspberry_off.png"));
					if (!oggetto.Trusted)
						res = new BitmapImage(new Uri("ms-appx:///Assets/raspberry_untrusted.png"));
					if (oggetto.Error)
						res = new BitmapImage(new Uri("ms-appx:///Assets/raspberry_err.png"));
					break;
				case enumComponente.centrale:
					res = new BitmapImage(new Uri("ms-appx:///Assets/central.png"));
					if (!oggetto.Enabled)
						res = new BitmapImage(new Uri("ms-appx:///Assets/central_off.png"));
					if (!oggetto.Trusted)
						res = new BitmapImage(new Uri("ms-appx:///Assets/central_untrusted.png"));
					if (oggetto.Error)
						res = new BitmapImage(new Uri("ms-appx:///Assets/central_err.png"));
					break;
				case enumComponente.webcam_ip:
					res = new BitmapImage(new Uri("ms-appx:///Assets/webcam.png"));
					if (!oggetto.Enabled)
						res = new BitmapImage(new Uri("ms-appx:///Assets/webcam_off.png"));
					if (!oggetto.Trusted)
						res = new BitmapImage(new Uri("ms-appx:///Assets/webcam_untrusted.png"));
					if (oggetto.Error)
						res = new BitmapImage(new Uri("ms-appx:///Assets/webcam_error.png"));
					break;
				case enumComponente.webcam_rasp:
					res = new BitmapImage(new Uri("ms-appx:///Assets/webcam.rasp.png"));
					if (!oggetto.Enabled)
						res = new BitmapImage(new Uri("ms-appx:///Assets/webcam.rasp_off.png"));
					if (!oggetto.Trusted)
						res = new BitmapImage(new Uri("ms-appx:///Assets/webcam.rasp_untrusted.png"));
					if (oggetto.Error)
						res = new BitmapImage(new Uri("ms-appx:///Assets/webcam_error.png"));
					break;

			}
			return res;
		}
		#endregion

		#region CLICK
		private void Mappa_Tapped(object sender, TappedRoutedEventArgs e)
		{
			messaggio.Text = "";
			try
			{
				if (RaspaMode == enumMode.working)
					return;

				// deseleziono
				evidenzia(false);

				if (RaspaMode == enumMode.edit)
				{

					// visualizza toolbar componenti
					ToolbarShow(enumShowToolbar.componenti);
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
			}
		}
		private void componente_LeftTapped(object sender, TappedRoutedEventArgs e)
		{
			int ID = 0;
			messaggio.Text = "";
			try
			{
				Image selImage = sender as Image;
				// CLICCATO NULLA
				// --> esco
				if (selImage == null)
					return;


				if (RaspaMode == enumMode.edit)
				{
					#region EDIT
					// CLICCATO STESSO ELEMENTO
					if (selImage == ActualImage)
					{
						ActualImage = selImage;

						//----------------------------
						// CLICCATO è già SELEZIONATO
						// --> DESELEZIONA
						// --> show TOOLBAR LOGO
						//----------------------------
						if (evidenziazione.Visibility == Visibility.Visible)
						{
							// deseleziono
							evidenzia(false);
							// visualizza toolbar componenti
							ToolbarShow(enumShowToolbar.componenti);
						}
						else
						{
							//----------------------------
							// CLICCATO NON è SELEZIONATO
							// --> LEGGI ID
							// --> SELEZIONA
							// --> show PROPERTY
							//----------------------------
							ID = getImageID(ActualImage);
							if (ID > 0)
							{
								// --> seleziona
								evidenzia(true);
								// --> property
								OpenProperty(ID);
							}
						}
					}
					else
					{
						ActualImage = selImage;

						//----------------------------
						// CLICCATO ALTRO ELEMENTO
						// --> LEGGI ID
						// --> SELEZIONA
						// --> show PROPERTY
						//----------------------------
						// seleziona
						evidenzia(true);
						// leggi ID immagine
						ID = getImageID(ActualImage);
						if (ID > 0)
						{
							// show property
							OpenProperty(ID);
						}
					}
					#endregion
				}
				else
				{
					ActualImage = selImage;
					ID = getImageID(ActualImage);
					if (ID > 0)
						Comando(ActualImage,ID);
					else
						messaggio.Text = "Nessun ID rilevato";
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
			}
		}

		#endregion
		#region CONTEXT MENU
		private void Img_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			try
			{
				Image pointerImage = sender as Image;
				if (pointerImage != null)
					selectedPointer = e.GetCurrentPoint(pointerImage);
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void componente_RightTapped(object sender, RightTappedRoutedEventArgs e)
		{
			try
			{
				Image tapped = sender as Image;
				// se non ho richiesto menu dell'attuale
				if (tapped != null && ActualImage != tapped)
					// se l'attuale era provvisoria
					eliminaActualImageSoloSeProvvisioria();

				ActualImage = tapped;
				selectedTool = ActualImage;

				if (ActualImage != null)
				{
					FlyoutBase FB = FlyoutBase.GetAttachedFlyout((FrameworkElement)ActualImage);
					FB.ShowAt((FrameworkElement)ActualImage);
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
		{
			MenuFlyoutItem selectedItemFlyout = sender as MenuFlyoutItem;
			messaggio.Text = "";
			try
			{
				int IDComponente = getImageID(selectedTool);

				switch (selectedItemFlyout.Text.ToString())
				{
					case "Enabled":
					case "Disabled":
					case "Reset":
					case "Schema":
						ShowSchema(IDComponente);
						break;
					case "Regole":
						Regole.INIT(IDComponente,this);
						ToolbarShow(enumShowToolbar.regole);
						break;
					case "Property":
						OpenProperty(IDComponente);
						break;
					case "Remove":
						remove(IDComponente);
						break;
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void OpenProperty(int IDComponente)
		{
			messaggio.Text = "";
			try
			{
				// preleggo da db
				if (IDComponente != 0)
				{
					// leggere oggetto da db
					DBCentral DB = new DBCentral();
					Actualcomponente = DB.GetComponenteByID(IDComponente);
				}

				// visualizzare propeerty
				showProperty(IDComponente, Actualcomponente.Tipo, Actualcomponente.Margin);

				// evidenziazione
				evidenzia(true);


			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void remove(int IDComponente)
		{
			messaggio.Text = "";
			try
			{
				if (RaspaMode == enumMode.working)
					return;

				int ID = getImageID(selectedTool);
				ActualTipoComponente = getImageComponenteType(selectedTool);
				if (ID > 0)
					EliminaDBActualComponente(ID);
				else
					messaggio.Text = "Non ho decodificato il componente selezionato";

				ToolbarShow(enumShowToolbar.componenti);

			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore Eliminazione : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}

		}
		private void EliminaDBActualComponente(int ID)
		{
			RaspaResult esito = new RaspaResult(true);
			messaggio.Text = "";
			try
			{
				if (RaspaMode == enumMode.working)
				{
					messaggio.Text = "For delete go in Editing Mode";
					return;
				}

				// DELETE DB
				if (ID>0)
				{
					DBCentral DB = new DBCentral();
					esito = DB.DelComponentiByID(ID);

					// verifica esito
					if (!esito.Esito)
					{
						messaggio.Text = "Errore Eliminazione : " + esito.Message;
						return;
					}
				}

				// Pulisci schermo
				eliminaActualImage();

				// Pivot a Toolbar
				ToolbarShow(enumShowToolbar.componenti);

				// azzera componente attuale
				azzeraActualComponent();
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				messaggio.Text = "Errore Eliminazione : " + ex.Message;
			}
		}

		#endregion

		#region SCHEMA COMPONENT
		private void ShowSchema(int ID)
		{
			try
			{
				// rileggo dati dal db
				DBCentral DB = new DBCentral();
				Componente item = DB.GetComponenteByID(ID);

				// TIPO
				switch (item.Tipo)
				{
					case enumComponente.nessuno:
						break;
					case enumComponente.nodo:
						break;
					case enumComponente.centrale:
						break;
					case enumComponente.light:
						gpio.drawGPIO_LIGHT(item.Node_Pin);
						ToolbarShow(enumShowToolbar.schema);
						break;
					case enumComponente.pir:
						gpio.drawGPIO_PIR(item.Node_Pin);
						ToolbarShow(enumShowToolbar.schema);
						break;
					case enumComponente.webcam_ip:
						break;
					case enumComponente.webcam_rasp:
						break;
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion
	
		#region TAG
		private bool isComponentRaspone(Image image)
		{
			bool res = false;
			try
			{
				if (image != null && image.Tag != null && image.Tag.ToString().StartsWith("RASP.ONE_"))
					res = true;
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		private bool ExistComponentWithTagInPage(string Tag)
		{
			bool res = false;
			try
			{
				foreach (var control in GridMappa.Children)
				{
					var img = control as Image;
					if (img != null && img.Tag != null && img.Tag.ToString()== Tag)
					{
						res = true;
						break;
					}
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore Eliminazione : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}

		/// <summary>
		/// GET IMAGE ID from TAG
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		private int getImageID(Image image)
		{
			int res = 0;
			try
			{
				if (image != null && image.Tag != null && image.Tag.ToString().StartsWith("RASP.ONE"))
				{
					RaspaTag tag = new RaspaTag(image.Tag.ToString());
					res = tag.ID;
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		private enumComponente getImageComponenteType(Image image)
		{
			enumComponente res = 0;

			try
			{
				if (image != null && image.Tag != null && image.Tag.ToString().StartsWith("RASP.ONE"))
				{
					string[] aTag = image.Tag.ToString().Split('_');
					if (aTag.Length == 3)
						res = (enumComponente)Convert.ToInt32(aTag[2]);
					else
						res = (enumComponente)Convert.ToInt32(aTag[1]);
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		#endregion

		#region DRAG DROP
		private void Button_DragStarting(UIElement sender, DragStartingEventArgs args)
		{
			try
			{
				if (RaspaMode == enumMode.working)
					return;

				evidenzia(false);

				args.Data.SetText("Muovi");

				args.Data.RequestedOperation = DataPackageOperation.Move;
				args.AllowedOperations = DataPackageOperation.Move;

				Image img = sender as Image;
				if (img != null)
				{
					selectedTool = img;
				}
				// IMG PREC
				eliminaActualImageSoloSeProvvisioria();

				// TAG
				int ID = getImageID(selectedTool);
				ActualTipoComponente = getImageComponenteType(selectedTool);

				// Se esiste prepara l'Actualcomponente dal DB
				if (ID > 0)
				{
					if (Actualcomponente == null)
					{
						DBCentral DB = new DBCentral();
						Actualcomponente = DB.GetComponenteByID(ID);
					}
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void Mappa_DragOver(object sender, DragEventArgs e)
		{
			messaggio.Text = "";
			try
			{
				if (RaspaMode == enumMode.working)
					return;

				e.AcceptedOperation = DataPackageOperation.Move;
				e.DragUIOverride.Caption = "Sposta";
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void Mappa_Drop(object sender, DragEventArgs e)
		{
			messaggio.Text = "";
			Componente item = null;
			try
			{

				if (RaspaMode == enumMode.working)
					return;

				DBCentral DB = new DBCentral();

				// azzsera immagine trascinata
				ActualImage = null;

				// evidenziazione
				evidenzia(false);

				int ID = getImageID(selectedTool);
				ActualTipoComponente = getImageComponenteType(selectedTool);
				// se è un componente sul DB
				if (ID > 0)
				{
					// rileggo dati dal db
					item = DB.GetComponenteByID(ID);

					// verifico se ha cambiato icona
					// rispetto ai dati mostrati nelle property
					if (Actualcomponente == null || Actualcomponente.ID != ID)
					{
						// reinizializzo componente attuale
						initActualComponente(ActualTipoComponente, item);
					}

					// memorizzo come immagine attuale
					ActualImage = selectedTool;
				}
				else
				{
					// crea componente nuovo
					ActualImage = create_object(ActualTipoComponente);
					ActualImage.Margin = selectedTool.Margin;
				}


				if (ActualImage != null)
				{
					e.AcceptedOperation = DataPackageOperation.Copy;


					// ADD PAGINA
					var p = e.GetPosition(Mappa);
					var X = (selectedPointer != null && selectedPointer.Position != null) ? selectedPointer.Position.X : 0;
					var Y = (selectedPointer != null && selectedPointer.Position != null) ? selectedPointer.Position.Y : 0;
					ActualImage.Margin = new Thickness(p.X - X-4, p.Y - Y-4, 0, 0);
					Actualcomponente.Margin = ActualImage.Margin;

					// SALVO POSIZIONI CAMBIATE
					if (item != null)
					{
						item.PositionLeft = ActualImage.Margin.Left;
						item.PositionTop = ActualImage.Margin.Top;
						item.PositionRight = ActualImage.Margin.Right;
						item.PositionBottom = ActualImage.Margin.Bottom;
						DB.SetComponenti(item, Utente);
					}


					//visualizza tabs property solo se nuovo
					if (ID > 0)
						ToolbarShow(enumShowToolbar.componenti);
					else
					{
						// evidenziazione
						evidenzia(true);

						// show property
						showProperty(ID, ActualTipoComponente, Actualcomponente.Margin);
					}

				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}

		}
		#endregion

		#region PROPERTY
		private void showProperty(int ID,enumComponente Tipo, Thickness Position)
		{
			RaspaResult res = ComponentProperty.showProperty(ID, Tipo, Position, this);
			if (res.Esito)
				Actualcomponente = ComponentProperty.componente;
			else
				messaggio.Text = res.Message;

			// show
			ToolbarShow(enumShowToolbar.property);
		}
		#endregion


		#region ACTUAL IMAGE/COMPONENT
		public void eliminaActualImage()
		{
			try
			{
				if (ActualImage != null)
				{
					// rimuove oggetto dalla mappa
					GridMappa.Children.Remove((UIElement)ActualImage);

					// toggliere il campo privato
					ActualImage = null;

					// spegnere tutti i pannelly property
					ToolbarShow(enumShowToolbar.componenti);

				}

				// toglie evidenziazione
				evidenzia(false);
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		public void eliminaActualImageSoloSeProvvisioria()
		{
			try
			{
				// se l'attuale era provvisoria
				int ID = getImageID(ActualImage);
				if (ID == 0)
					eliminaActualImage();
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		public void evidenzia(bool stato)
		{
			try
			{
				if (stato && ActualImage != null)
				{
					// visualizza in pagina
					evidenziazione.Margin = new Thickness(ActualImage.Margin.Left - 9, ActualImage.Margin.Top - 9, 0, 0);
					evidenziazione.Visibility = Visibility.Visible;
				}
				else
					// rendi invisibile
					evidenziazione.Visibility = Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		public void azzeraActualComponent()
		{
			try
			{
				if (Actualcomponente != null)
				{
					// toggliere il campo privato
					Actualcomponente = null;

					// azzera Tipo componente
					ActualTipoComponente = enumComponente.nessuno;

				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}

		#endregion





		#region SHOW TABS
		private void InitProperty()
		{
			// show tabs mappa
			TabsShow(enumShowTabs.mappa);
			// show toolbar componenti
			ToolbarShow(enumShowToolbar.componenti);

		}
		public enum enumShowTabs
		{
			mappa = 0,
			settings = 1
		}
		private void TabsShow(enumShowTabs show)
		{
			Tabs.SelectedIndex = (int)show;
		}
		public enum enumShowToolbar
		{
			componenti = 0,
			property = 1,
			regole = 2,
			info = 3,
			schema = 4,
			help = 5,
		}
		public void ToolbarShow(enumShowToolbar show)
		{
			ToolbarComponenti.Visibility = Visibility.Collapsed;
			ToolbarInfo.Visibility = Visibility.Collapsed;
			ToolBarProperty.Visibility = Visibility.Collapsed;
			ToolbarComponenti.Visibility = Visibility.Collapsed;
			ToolBarSchema.Visibility = Visibility.Collapsed;
			ToolBarHelp.Visibility = Visibility.Collapsed;
			ToolbarRegole.Visibility = Visibility.Collapsed;
			

			switch (show)
			{
				case enumShowToolbar.help:
					ToolBarHelp.Visibility = Visibility.Visible;
					break;

				case enumShowToolbar.componenti:
					ToolbarComponenti.Visibility = Visibility.Visible;
					break;
				case enumShowToolbar.info:
					ToolbarInfo.Visibility = Visibility.Visible;
					break;
				case enumShowToolbar.property:
					ToolBarProperty.Visibility = Visibility.Visible;
					break;
				case enumShowToolbar.regole:
					ToolbarRegole.Visibility = Visibility.Visible;
					break;
				case enumShowToolbar.schema:
					ToolBarSchema.Visibility = Visibility.Visible;
					break;
			}
		}
		#endregion



		#endregion


	}
}
