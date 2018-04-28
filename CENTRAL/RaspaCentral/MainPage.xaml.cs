using RaspaDB;
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
using Windows.Media.SpeechSynthesis;
using Windows.Media.Playback;
using System.Threading;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspaCentral
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		public string Utente = "Fabio";
		Dictionary<int,Timer> timer = new Dictionary<int, Timer>();

		public MainPage()
        {
            this.InitializeComponent();

		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			// CENTRAL
			StartCentral();
			// MAPPA
			InitProperty();
			initMappa();


		}


		#region CENTRAL
		private static Componente centrale = null;

		bool flgMQTT = true;
		bool flgSubscription_central = true;
		bool flgSubscription_rules = true;
		bool flgSubscription_hearbit = true;
		bool flgSubscription_reload = true;
		bool flgINIT_NODO_Component = true;
		public async void StartCentral()
		{

			// --------------------------------------
			// SYNC NODE TIME
			// --------------------------------------
			SyncDateTime();

			// --------------------------------------
			// CENTRALE
			// --------------------------------------
			// Inizializza centrale
			RaspaResult resNode = INIT_CENTRALE();
			if (!resNode.Esito)
				messaggio.Text = resNode.Message;
			else
				CENTRALE_SHOW();


			// --------------------------------------
			// PREPARE SUBSCRIPTION
			// --------------------------------------
			List<string> ConnectedSubscription = new List<string>();

			if (flgSubscription_central)
				ConnectedSubscription.Add(enumSubribe.central.ToString());
			if (flgSubscription_rules)                                      // solo un centrale può eseguire i rules
				ConnectedSubscription.Add(enumSubribe.rules.ToString()); 
			if (flgSubscription_hearbit)
				ConnectedSubscription.Add(enumSubribe.heartbit.ToString()); 
			if (flgSubscription_reload)                                     // solo un centrale può richiedere il reload dei nodi
				ConnectedSubscription.Add(enumSubribe.nodeINIT.ToString());

			// --------------------------------------
			// MTMQ
			// --------------------------------------
			if (flgMQTT)
				INIT_MQTT(ConnectedSubscription);

			// --------------------------------------
			// INIT NODI
			// --------------------------------------
			// quando parte il centrale richiede i dati al nodo
			if (flgINIT_NODO_Component)
				CentraleInitNode();

		}
		#region SYNC DATE TIME
		public void SyncDateTime()
		{
			RaspBerry RB = new RaspBerry();
			RB.SyncDateTime();
		}
		#endregion

		private RaspaResult INIT_CENTRALE()
		{
			RaspaResult res = new RaspaResult(true);

			try
			{
				RaspBerry client = new RaspBerry();
				RaspaInfo info = client.GetRaspInfo();

				DBCentral db = new DBCentral();
				centrale = db.GetComponenteByIPv4(info.Network_IPv4, enumComponente.centrale);

				if (centrale == null)
					return new RaspaResult(false, "Centrale non riconosciuto nel sistema");

				centrale.HostName = info.HostName;
				centrale.IPv4 = info.Network_IPv4;
				centrale.IPv6 = info.Network_IPv6;
				centrale.HWAddress = info.Network_Serial;
				centrale.BlueTooth = "";
				centrale.OSVersion = info.OSVersion;
				centrale.NodeSWVersion = info.RaspaVersion;

				centrale.SystemProductName = info.Productname;
				centrale.SystemID = info.HW_ID;

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
		private void CENTRALE_SHOW()
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
		private void INIT_MQTT(List<string> subscription)
		{
			try
			{
				mqTT = new MQTT(centrale.SystemID, centrale.HostName, centrale.IPv4,subscription);

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
		private void INIT_MQTT_subctribe(enumSubribe tipoSubscribe)
		{
			try
			{
				mqTT.Subscribe(tipoSubscribe.ToString());
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
			var ignore = Trasmission.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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
			});
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


		// QUANDO IL CENTRALE PARTE RICHIEDE LO STATO ATTUALE AI NODI
		#region CENTRALE INIT
		private void CentraleInitNode()
		{
			messaggio.Text = "";
			try
			{
				// Inizializza componenti mappa leggendoli dal DB
				DBCentral DB = new DBCentral();
				Componenti recs = DB.GetComponenti(mappa_num);
				foreach (Componente rec in recs)
					CentraleInitNode(rec);
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		public void CentraleInitNode(Componente componente)
		{
			try
			{
				if (componente.Enabled)
				{
					enumStato stato = enumStato.nessuno;

					switch (componente.Tipo)
					{
						case enumComponente.nessuno:
						case enumComponente.centrale:
						case enumComponente.nodo:
							stato = enumStato.nessuno;
							break;
						case enumComponente.pir:
							stato = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.light:
							stato = (componente.Enabled) ? enumStato.read : enumStato.off;
							break;
						case enumComponente.bell:
							stato = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.moisture:
							stato = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.push:
							stato = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.temperature:
							stato = (componente.Enabled) ? ((componente.repeat) ? enumStato.readRepetitive : enumStato.read) : enumStato.off;
							break;
						case enumComponente.umidity:
							stato = (componente.Enabled) ? ((componente.repeat) ? enumStato.readRepetitive : enumStato.read) : enumStato.off;
							break;
						case enumComponente.webcam_ip:
							stato = enumStato.nessuno;
							break;
						case enumComponente.webcam_rasp:
							stato = enumStato.nessuno;
							//stato = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
					}


					if (stato != enumStato.nessuno)
					{
						RaspaProtocol protocol = new RaspaProtocol();
						protocol.Comando = enumComando.comando;
						protocol.Mittente = centrale;
						protocol.Destinatario = componente;
						protocol.Azione = enumStato.read;
						protocol.SubcribeDestination = enumSubribe.IPv4;
						protocol.SubcribeResponse = enumSubribe.central;

						MQTTRSend(protocol);
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

		// QUANDO IL CENTRALE FA REFRESH DELLA PAGINA DEE RILEGGERE I NODI
		// NON USATO 
		#region CENTRALE READ
		public void CentraleReadNodeComponent(Componente componente)
		{
			try
			{
				if (componente.Enabled)
				{
					enumStato azione = enumStato.nessuno;

					switch (componente.Tipo)
					{
						case enumComponente.nessuno:
						case enumComponente.centrale:
						case enumComponente.nodo:
							azione = enumStato.nessuno;
							break;
						case enumComponente.pir:
							azione = (componente.Enabled) ? enumStato.read : enumStato.off;
							break;
						case enumComponente.light:
							azione = (componente.Enabled) ? enumStato.read : enumStato.off;
							break;
						case enumComponente.bell:
							azione = (componente.Enabled) ? enumStato.read : enumStato.off;
							break;
						case enumComponente.moisture:
							azione = (componente.Enabled) ? enumStato.read : enumStato.off;
							break;
						case enumComponente.push:
							azione = (componente.Enabled) ? enumStato.read : enumStato.off;
							break;
						case enumComponente.temperature:
							azione = (componente.Enabled) ? ((componente.repeat) ? enumStato.readRepetitive : enumStato.read) : enumStato.off;
							break;
						case enumComponente.umidity:
							azione = (componente.Enabled) ? ((componente.repeat) ? enumStato.readRepetitive : enumStato.read) : enumStato.off;
							break;
						case enumComponente.webcam_ip:
							azione = enumStato.nessuno;
							break;
						case enumComponente.webcam_rasp:
							azione = enumStato.nessuno;
							//stato = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
					}


					if (azione != enumStato.nessuno)
					{
						RaspaProtocol protocol = new RaspaProtocol();
						protocol.Comando = enumComando.comando;
						protocol.Mittente = centrale;
						protocol.Destinatario = componente;
						protocol.Azione = azione;
						protocol.SubcribeDestination = enumSubribe.IPv4;
						protocol.SubcribeResponse = enumSubribe.central;

						MQTTRSend(protocol);
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

		// QUANDO IL NODO PARTE RICHIEDE L'INIZIALIZZAZIONE
		#region NODE INIT
		private void NodeInit(string IPv4)
		{
			DBCentral DB = new DBCentral();
			try
			{
				// leggo il nodo da reload sul DB
				Componenti componenti = DB.GetComponentiByIPv4(IPv4);

				// analizza i componenti
				foreach (Componente componente in componenti)
					NodeInit(componente);
			}
			catch (Exception ex)
			{
				messaggio.Text = ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void NodeInit(Componente componente)
		{
			try
			{
				if (componente.Enabled)
				{
					enumStato azione = enumStato.nessuno;

					switch (componente.Tipo)
					{
						case enumComponente.nessuno:
						case enumComponente.centrale:
						case enumComponente.nodo:
							azione = enumStato.nessuno;
							break;
						case enumComponente.pir:
							azione = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.light:
							azione = (componente.Enabled) ? componente.Stato : enumStato.off;
							break;
						case enumComponente.bell:
							azione = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.moisture:
							azione = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.push:
							azione = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
						case enumComponente.temperature:
							azione = (componente.Enabled) ? ((componente.repeat)?enumStato.readRepetitive: enumStato.read) : enumStato.off;
							break;
						case enumComponente.umidity:
							azione = (componente.Enabled) ? ((componente.repeat) ? enumStato.readRepetitive : enumStato.read) : enumStato.off;
							break;
						case enumComponente.webcam_ip:
							azione = enumStato.nessuno;
							break;
						case enumComponente.webcam_rasp:
							azione = enumStato.nessuno;
							//stato = (componente.Enabled) ? enumStato.on : enumStato.off;
							break;
					}


					if (azione != enumStato.nessuno)
					{
						RaspaProtocol protocol = new RaspaProtocol();
						protocol.Comando = enumComando.comando;
						protocol.Mittente = centrale;
						protocol.Destinatario = componente;
						protocol.Azione = azione;
						protocol.RepetiteTime = (azione == enumStato.readRepetitive) ? componente.repeatTime : null;
						protocol.SubcribeDestination = enumSubribe.IPv4;
						protocol.SubcribeResponse = enumSubribe.central;

						MQTTRSend(protocol);
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

				// se il messaggio più vecchio di 5 minuti lo salto
				if (protocol.MessaggeMQTT_IsExpired())
					return;

				if (protocol.Comando == enumComando.nodeSaveConfig)
				{
					// leggo il nodo da iniziare sul DB
					Componente nodo2INIT = DB.GetComponenteByIPv4(protocol.Mittente.IPv4, enumComponente.nodo);
					// se il componente è effettivamente un nodo
					if (nodo2INIT.Tipo == enumComponente.nodo && 
						protocol.Mittente != null)
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
				}
				else if (protocol.Comando == enumComando.nodeReload)
				{
					NodeInit(protocol.Mittente.IPv4);
				}
				else if (protocol.Comando == enumComando.notify)
				{
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
							// NOTIFY
							#region NOTIFY

							img.Source = choseImageByComponente(protocol);

							switch (protocol.Mittente.Tipo)
							{
								case enumComponente.light:
									#region LIGHT
									switch (protocol.Azione)
									{
										case enumStato.nessuno:
											DB.ModComponentiStato(tag.ID, enumStato.nessuno, Utente);
											break;
										case enumStato.off:
											DB.ModComponentiStato(tag.ID, enumStato.off, Utente);
											break;
										case enumStato.on:
											DB.ModComponentiStato(tag.ID, enumStato.on, Utente);
											break;
										case enumStato.errore:
											DB.ModComponentiStato(tag.ID, enumStato.errore, Utente);
											break;
									}
									#endregion
									break;
								case enumComponente.pir:
									#region PIR
									switch (protocol.Azione)
									{
										case enumStato.nessuno:
											DB.ModComponentiStato(tag.ID, enumStato.nessuno, Utente);
											break;
										case enumStato.off:
											DB.ModComponentiStato(tag.ID, enumStato.off, Utente);
											break;
										case enumStato.on:
										case enumStato.signalOFF:
											DB.ModComponentiStato(tag.ID, enumStato.on, Utente);
											break;
										case enumStato.signal:
											DB.ModComponentiStato(tag.ID, enumStato.signal, Utente);
											break;
										case enumStato.errore:
											DB.ModComponentiStato(tag.ID, enumStato.errore, Utente);
											break;
									}
									#endregion
									break;
								case enumComponente.bell:
									#region BELL
									switch (protocol.Azione)
									{
										case enumStato.nessuno:
											DB.ModComponentiStato(tag.ID, enumStato.nessuno, Utente);
											break;
										case enumStato.off:
											DB.ModComponentiStato(tag.ID, enumStato.off, Utente);
											break;
										case enumStato.on:
										case enumStato.signalOFF:
											DB.ModComponentiStato(tag.ID, enumStato.on, Utente);
											break;
										case enumStato.signal:
											DB.ModComponentiStato(tag.ID, enumStato.signal, Utente);
											break;
										case enumStato.errore:
											DB.ModComponentiStato(tag.ID, enumStato.errore, Utente);
											break;
									}
									#endregion
									break;
								case enumComponente.moisture:
									#region MOISTURE
									switch (protocol.Azione)
									{
										case enumStato.nessuno:
											DB.ModComponentiStato(tag.ID, enumStato.nessuno, Utente);
											break;
										case enumStato.off:
											DB.ModComponentiStato(tag.ID, enumStato.off, Utente);
											break;
										case enumStato.on:
										case enumStato.signalOFF:
											DB.ModComponentiStato(tag.ID, enumStato.on, Utente);
											break;
										case enumStato.signal:
											DB.ModComponentiStato(tag.ID, enumStato.signal, Utente);
											break;
										case enumStato.errore:
											DB.ModComponentiStato(tag.ID, enumStato.errore, Utente);
											break;
									}
									#endregion
									break;

								case enumComponente.umidity:
									string umidity = protocol.getUmidityValue();
									Decimal? umidityVal = protocol.getUmidity();
									string TooltipUmidity = "Last : " + DateTime.Now.ToString("HH:mm:ss.f") + Environment.NewLine + "Umidity : " + umidity;

									#region UMIDITY
									switch (protocol.Azione)
									{
										case enumStato.nessuno:
											ToolTipService.SetToolTip(img, "---");
											DB.ModComponentiValueAndStato(tag.ID, "", enumStato.nessuno, Utente);
											break;
										case enumStato.off:
											ToolTipService.SetToolTip(img, "OFF");
											DB.ModComponentiStato(tag.ID, enumStato.off, Utente);
											DB.ModComponentiValueAndStato(tag.ID, "", enumStato.off, Utente);
											break;
										case enumStato.on:
											ToolTipService.SetToolTip(img, TooltipUmidity);
											DB.ModComponentiValueAndStato(tag.ID, protocol.ValueFor_writeDB(), enumStato.on, Utente);
											break;
										case enumStato.value:
											ToolTipService.SetToolTip(img, TooltipUmidity);
											DB.ModComponentiValueAndStato(tag.ID, protocol.ValueFor_writeDB(), enumStato.on, Utente);
											break;
										case enumStato.errore:
											ToolTipService.SetToolTip(img, "Error");
											DB.ModComponentiValueAndStato(tag.ID, "", enumStato.errore, Utente);
											break;
									}
									#endregion
									break;
								case enumComponente.temperature:

									string temperatura = protocol.getTemperatureValue();
									Decimal? temperaturaVal = protocol.getTemperature();
									string TooltipTemperature = "Last : " + DateTime.Now.ToString("HH:mm:ss.f") + Environment.NewLine + "Temperature : " + temperatura;

									#region TEMPERATURE
									switch (protocol.Azione)
									{
										case enumStato.nessuno:
											ToolTipService.SetToolTip(img, "---");
											DB.ModComponentiValueAndStato(tag.ID, "", enumStato.nessuno, Utente);
											break;
										case enumStato.off:
											ToolTipService.SetToolTip(img, "OFF");
											DB.ModComponentiStato(tag.ID, enumStato.off, Utente);
											DB.ModComponentiValueAndStato(tag.ID, "", enumStato.off, Utente);
											break;
										case enumStato.on:
											ToolTipService.SetToolTip(img, TooltipTemperature);
											DB.ModComponentiValueAndStato(tag.ID, protocol.ValueFor_writeDB(), enumStato.on, Utente);
											break;
										case enumStato.value:
											ToolTipService.SetToolTip(img, TooltipTemperature);
											DB.ModComponentiValueAndStato(tag.ID, protocol.ValueFor_writeDB(), enumStato.on, Utente);
											break;
										case enumStato.errore:
											ToolTipService.SetToolTip(img, "Error");
											DB.ModComponentiValueAndStato(tag.ID, "", enumStato.errore, Utente);
											break;
									}
									#endregion
									break;

								case enumComponente.push:
									switch (protocol.Azione)
									{
										case enumStato.signal:
											SpeechService speek = new SpeechService();
											speek.parla("disse la vacca al mulo come ti puzza il culo");
											break;
									}
									break;
							}
							#endregion
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
						#region PIR
						switch(componente.Stato)
						{
							case enumStato.nessuno:
							case enumStato.errore:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.read;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("<-- READ " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " GET ");
								MQTTRSend(protocol);
								break;
							case enumStato.off:
								// ---------------------------------
								// chiama nodo per PIR ON
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.on;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET ON ");
								MQTTRSend(protocol);
								break;
							case enumStato.on:
								// ---------------------------------
								// chiama nodo per PIR OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.off;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET OFF ");
								MQTTRSend(protocol);

								break;
							case enumStato.signal:
							case enumStato.signalOFF:
								img.Source = loadImage("pir_on");
								DB.ModComponentiStato(componente.ID.Value, enumStato.on, Utente);

								break;
						}
#endregion
						break;
					case enumComponente.light:
						switch (componente.Stato)
						{
							case enumStato.nessuno:
							case enumStato.errore:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.read;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("<-- READ " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " GET ");
								MQTTRSend(protocol);
								break;
							case enumStato.off:
								// ---------------------------------
								// chiama nodo per PIR ON
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.on;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> OFF " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET ON ");
								MQTTRSend(protocol);
								break;
							case enumStato.on:
								// ---------------------------------
								// chiama nodo per PIR OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.off;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> ON " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET OFF ");
								MQTTRSend(protocol);

								break;
						}

						break;
					case enumComponente.umidity:
						switch (componente.Stato)
						{
							case enumStato.nessuno:
							case enumStato.errore:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = (componente.repeat) ? enumStato.readRepetitive : enumStato.read;
								protocol.RepetiteTime = (componente.repeat) ? componente.repeatTime : null;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("<-- READ " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " READ UMIDITY ");
								MQTTRSend(protocol);
								break;
							case enumStato.off:
								// ---------------------------------
								// chiama nodo per UMIDITY ON
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = (componente.repeat) ? enumStato.readRepetitive : enumStato.read;
								protocol.RepetiteTime = (componente.repeat) ? componente.repeatTime : null;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " READ UMIDITY ");
								MQTTRSend(protocol);
								break;
							case enumStato.on:
								// ---------------------------------
								// chiama nodo per UMIDITY OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = (componente.repeat) ? enumStato.readRepetitive : enumStato.read;
								protocol.RepetiteTime = (componente.repeat) ? componente.repeatTime : null;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " READ UMIDITY ");
								MQTTRSend(protocol);

								break;

						}
						break;
					case enumComponente.temperature:

						switch (componente.Stato)
						{
							case enumStato.nessuno:
							case enumStato.errore:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = (componente.repeat) ? enumStato.readRepetitive : enumStato.read;
								protocol.RepetiteTime = (componente.repeat) ? componente.repeatTime : null;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("<-- READ " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " READ TEMP ");
								MQTTRSend(protocol);
								break;
							case enumStato.off:
								// ---------------------------------
								// chiama nodo per TEMPERATURE ON
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = (componente.repeat) ? enumStato.readRepetitive : enumStato.read;
								protocol.RepetiteTime = (componente.repeat) ? componente.repeatTime : null;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " READ TEMP ");
								MQTTRSend(protocol);
								break;
							case enumStato.on:
								// ---------------------------------
								// chiama nodo per TEMPERATURE OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = (componente.repeat) ? enumStato.readRepetitive : enumStato.read;
								protocol.RepetiteTime = (componente.repeat) ? componente.repeatTime : null;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " READ TEMP ");
								MQTTRSend(protocol);

								break;

						}

						break;
					case enumComponente.push:
						switch (componente.Stato)
						{
							case enumStato.nessuno:
							case enumStato.errore:
							case enumStato.off:
							case enumStato.on:
								// ---------------------------------
								// chiama nodo per TEMPERATURE OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.on;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " READ BUTTON PUSH ");
								MQTTRSend(protocol);
								break;
						}
						break;
					case enumComponente.bell:
						#region BELL
						switch (componente.Stato)
						{
							case enumStato.nessuno:
							case enumStato.errore:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.read;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("<-- READ " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " GET ");
								MQTTRSend(protocol);
								break;
							case enumStato.off:
								// ---------------------------------
								// chiama nodo per PIR ON
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.on;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET ON ");
								MQTTRSend(protocol);
								break;
							case enumStato.on:
								// ---------------------------------
								// chiama nodo per PIR OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.off;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET OFF ");
								MQTTRSend(protocol);

								break;
							case enumStato.signal:
								img.Source = loadImage("bell_on");
								DB.ModComponentiStato(componente.ID.Value, enumStato.on, Utente);
								break;
						}
#endregion
						break;
					case enumComponente.moisture:
						#region MOISTURE
						switch (componente.Stato)
						{
							case enumStato.nessuno:
							case enumStato.errore:
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.read;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("<-- READ " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " GET ");
								MQTTRSend(protocol);
								break;
							case enumStato.off:
								// ---------------------------------
								// chiama nodo per PIR ON
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.on;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET ON ");
								MQTTRSend(protocol);
								break;
							case enumStato.on:
								// ---------------------------------
								// chiama nodo per PIR OFF
								// ---------------------------------
								protocol = new RaspaProtocol();
								protocol.Mittente = centrale;
								protocol.Destinatario = componente;
								protocol.Comando = enumComando.comando;
								protocol.Azione = enumStato.off;
								protocol.SubcribeDestination = enumSubribe.IPv4;
								protocol.SubcribeResponse = enumSubribe.central;

								writeLogVideo("--> " + protocol.Destinatario.IPv4 + " - " + protocol.Destinatario.Tipo.ToString() + " SET OFF ");
								MQTTRSend(protocol);

								break;
							case enumStato.signal:
							case enumStato.signalOFF:
								img.Source = loadImage("moisture_on");
								DB.ModComponentiStato(componente.ID.Value, enumStato.on, Utente);
								break;
						}
						#endregion
						break;
					case enumComponente.webcam_ip:
						ipcam1.playIPCam(componente.Nome, componente.getIPCAMAddress());
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

				// carica combo mappa
				initComboMappa();
				COMBO_MAPPA.SelectedValue = "0";

				// Prepara frame CAM
				GridCAM.Visibility = Visibility.Visible;
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}

		// DRAW MAPS and POPOLATE GRAPHICS FROM DB and NODE
		#region MAPPA
		int mappa_num = 0;
		int mappa_max = 2;
		private void initComboMappa()
		{
			try
			{
				for (int i = 0; i <= mappa_max; i++)
					COMBO_MAPPA.Items.Add(i.ToString());
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void MAPPA_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				ComboBox cMappa = sender as ComboBox;
				if (cMappa == null)
					return;

				if (cMappa.SelectedValue != null)
				{
					// Leggo MAPPA NUM selected
					mappa_num = Convert.ToInt32(cMappa.SelectedValue.ToString());

					// cambio mappa
					Mappa.Source = new BitmapImage(new Uri("ms-appx:///Assets/mappa" + mappa_num + ".png"));

					// spegni toolbar
					ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);

					// Elimina componenti in sospeso
					eliminaActualImageSoloSeProvvisioria();

					// toglie evidenziazione
					evidenzia(false);

					// carica componenti
					popolateComponenti();

					// reinizializza componenti dal nodo
					reINITMapsComponentFromNode();

					// inizializza controllo property
					ComponentProperty.mappa_num = mappa_num;
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion

		// POPOLATE GRAPHICS FROM DB
		#region reinit from DB
		private void btnRefresh_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// Visualizza Mappa
			TabsShow(enumShowTabs.mappa);
			// rilegge dal nodo i componenti
			popolateComponenti();
		}
		private void popolateComponenti()
		{
			messaggio.Text = "";
			try
			{
				// pulisci interfaccia 
				CleanInterface();

				// Inizializza componenti mappa leggendoli dal DB
				DBCentral DB = new DBCentral();
				Componenti recs = DB.GetComponenti(mappa_num);
				foreach (Componente rec in recs)
				{
					rec.Stato = enumStato.nessuno;
					rec.Value = null;

					// disegno
					create_object(rec.Tipo, rec);
				}
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion

		// POPOLATE GRAPHICS FROM NODE
		#region reinit from node
		private void btnReinit_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// Visualizza Mappa
			TabsShow(enumShowTabs.mappa);
			// rilegge dal nodo i componenti
			reINITMapsComponentFromNode();
		}
		private void reINITMapsComponentFromNode()
		{
			messaggio.Text = "";
			try
			{
				// pulisci interfaccia 
				CleanInterface();

				// Inizializza componenti mappa leggendoli dal DB
				DBCentral DB = new DBCentral();
				Componenti recs = DB.GetComponenti(mappa_num);
				foreach (Componente rec in recs)
					CentraleInitNode(rec);
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion
		
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
				ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);

				if (editing.IsChecked != null && editing.IsChecked.Value)
				{
					RaspaMode = enumMode.edit;
				}
				else
				{
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
						ToolTipCustom = "NODO : " + Actualcomponente.Node_Num + " - " + Actualcomponente.IPv4;

						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

						break;
					case enumComponente.centrale:
						Actualcomponente.Nome = (item == null) ? "CENTRALE " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "192.168.1." : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "CENTRALE : " + Actualcomponente.Node_Num + " - " + Actualcomponente.IPv4;

						Actualcomponente.Value = (item == null) ? new List<string>(): item.Value;

						break;
					case enumComponente.light:
						Actualcomponente.Nome = (item == null) ? "LUCE " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "LIGHT : " + Actualcomponente.Node_Num + "/" + Actualcomponente.Node_Pin + " - " + Actualcomponente.IPv4;

						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

						break;
					case enumComponente.pir:
						Actualcomponente.Nome = (item == null) ? "PIR " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "PIR : " + Actualcomponente.Node_Num + "/" + Actualcomponente.Node_Pin + " - " + Actualcomponente.IPv4;

						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

						break;
					case enumComponente.push:
						Actualcomponente.Nome = (item == null) ? "PUSH BUTTON " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "PUSH : " + Actualcomponente.Node_Num + "/" + Actualcomponente.Node_Pin + " - " + Actualcomponente.IPv4;

						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

						break;
					case enumComponente.moisture:
						Actualcomponente.Nome = (item == null) ? "MOISTURE " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "MOISTURE : " + Actualcomponente.Node_Num + "/" + Actualcomponente.Node_Pin + " - " + Actualcomponente.IPv4;

						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

						break;
					case enumComponente.bell:
						Actualcomponente.Nome = (item == null) ? "BELL " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "BELL : " + Actualcomponente.Node_Num + "/" + Actualcomponente.Node_Pin + " - " + Actualcomponente.IPv4;

						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

						break;

					case enumComponente.umidity:
						Actualcomponente.Nome = (item == null) ? "UMIDITA' " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = (item == null) ? "---" : "Umidità : " + item.getUmidityValue();
						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;
						break;
					case enumComponente.temperature:
						Actualcomponente.Nome = (item == null) ? "TEMPERATURE " : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = (item == null) ? "---" : "Temperature : " + item.getTemperatureValue();
						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

						break;
					case enumComponente.webcam_ip:
						Actualcomponente.Nome = (item == null) ? "WEBCAM" : item.Nome;
						Actualcomponente.IPv4 = (item == null) ? "" : item.IPv4;
						Actualcomponente.IPv6 = (item == null) ? "" : item.IPv6;
						Actualcomponente.HWAddress = (item == null) ? "" : item.HWAddress;
						ToolTipCustom = "WEBCAM : " + Actualcomponente.Nome + " - " + Actualcomponente.IPv4;
						Actualcomponente.Value = (item == null) ? new List<string>() : item.Value;

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
				Image FoundComponent = GetComponentByTag(Tag);
				if (FoundComponent != null)
					immagine = FoundComponent;

				// Creo Immagine da Tools
				immagine.Name = (item != null && item.ID.HasValue) ? "C_" + item.ID.Value : "C_0";
				immagine.Tag = Tag;
				immagine.Margin = new Thickness(Actualcomponente.PositionLeft, Actualcomponente.PositionTop, Actualcomponente.PositionRight, Actualcomponente.PositionBottom);
				immagine.Width = 16;
				immagine.Height = 16;
				immagine.CanDrag = true;
				immagine.Source = choseImageByComponente(((item!=null)?item:new Componente(true,comp, enumStato.on)));
				immagine.VerticalAlignment = VerticalAlignment.Top;
				immagine.HorizontalAlignment = HorizontalAlignment.Left;


				// TOOLTIP
				ToolTip ttip = new ToolTip();
				ttip.Content = ToolTipCustom;
				ttip.VerticalOffset = 10;
				ttip.PlacementTarget = immagine;
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

				a1 = new MenuFlyoutItem { Text = "Reset", IsEnabled = Actualcomponente.Action.Reset, Tag = Actualcomponente.ID };
				a2 = new MenuFlyoutItem { Text = "Enabled", IsEnabled = Actualcomponente.Action.Enabled, Tag = Actualcomponente.ID };
				a3 = new MenuFlyoutItem { Text = "Disabled", IsEnabled = Actualcomponente.Action.Disabled, Tag = Actualcomponente.ID };
				a4 = new MenuFlyoutItem { Text = "Remove", IsEnabled = Actualcomponente.Action.Remove, Tag = Actualcomponente.ID };
				a5 = new MenuFlyoutItem { Text = "Property", IsEnabled = Actualcomponente.Action.Property, Tag = Actualcomponente.ID };


				a1.Click += MenuFlyoutItem_Click;
				a2.Click += MenuFlyoutItem_Click;
				a3.Click += MenuFlyoutItem_Click;
				a4.Click += MenuFlyoutItem_Click;
				a5.Click += MenuFlyoutItem_Click;
				myFlyout.Items.Add(a1);
				myFlyout.Items.Add(a2);
				myFlyout.Items.Add(a3);
				myFlyout.Items.Add(new MenuFlyoutSeparator());
				myFlyout.Items.Add(a4);
				myFlyout.Items.Add(new MenuFlyoutSeparator());
				myFlyout.Items.Add(a5);

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
		private BitmapImage loadImage(string name)
		{
			BitmapImage res = new BitmapImage(new Uri("ms-appx:///Assets/cross.png"));
			try
			{
				// LOAD
				res = new BitmapImage(new Uri("ms-appx:///Assets/"+ name + ".png"));
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				res = new BitmapImage(new Uri("ms-appx:///Assets/error.png"));
			}
			return res;
		}

		private ImageSource choseImageByComponente(RaspaProtocol protocol)
		{
			string valore = "";
			switch (protocol.Mittente.Tipo)
			{
				case enumComponente.temperature:
					decimal? temp = protocol.getTemperature();
					valore = (temp.HasValue) ? temp.ToString() : "";
					break;
				case enumComponente.umidity:
					decimal? umidity = protocol.getUmidity();
					valore = (umidity.HasValue) ? umidity.ToString() : "";
					break;
			}
			return choseImageByComponente(protocol.Mittente.Tipo, protocol.Azione, valore);
		}
		private ImageSource choseImageByComponente(Componente oggetto)
		{
			string valore = "";
			switch (oggetto.Tipo)
			{
				case enumComponente.temperature:
					decimal? temp = oggetto.getTemperature();
					valore = (temp.HasValue) ? temp.ToString() : "";
					break;
				case enumComponente.umidity:
					decimal? umidity = oggetto.getTemperature();
					valore = (umidity.HasValue) ? umidity.ToString() : "";
					break;
			}
			return choseImageByComponente(oggetto.Tipo, oggetto.Stato, valore, oggetto.Enabled, oggetto.Trusted = true, oggetto.Error);
		}
		private ImageSource choseImageByComponente(enumComponente tipo, enumStato stato,string valore, bool Enabled=true, bool Trusted = true,bool Error=false)
		{
			ImageSource res = loadImage("exclamation");

			switch (tipo)
			{
				case enumComponente.nessuno:
					res = loadImage("exclamation");
					break;
				case enumComponente.light:
					if (!Enabled)
						res = loadImage("light_disabled");
					else
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("light_err");
								break;
							case enumStato.off:
								res = loadImage("light_off");
								break;
							case enumStato.on:
								res = loadImage("light_on");
								break;
							case enumStato.errore:
								res = loadImage("light_err");
								break;
						}
					break;
				case enumComponente.pir:
					if (!Enabled)
						res = loadImage("pir_disabled");
					else
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("pir_err");
								break;
							case enumStato.off:
								res = loadImage("pir_off");
								break;
							case enumStato.on:
							case enumStato.signalOFF:
								res = loadImage("pir_on");
								break;
							case enumStato.signal:
								res = loadImage("pir_active");
								break;
							case enumStato.errore:
								res = loadImage("pir_err");
								break;
						}

					break;
				case enumComponente.moisture:
					if (!Enabled)
						res = loadImage("moisture_disabled");
					else
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("moisture_err");
								break;
							case enumStato.off:
								res = loadImage("moisture_off");
								break;
							case enumStato.on:
							case enumStato.signalOFF:
								res = loadImage("moisture_on");
								break;
							case enumStato.signal:
								res = loadImage("moisture_active");
								break;
							case enumStato.errore:
								res = loadImage("moisture_err");
								break;
						}

					break;
				case enumComponente.bell:
					if (!Enabled)
						res = loadImage("bell_disabled");
					else
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("bell_err");
								break;
							case enumStato.off:
								res = loadImage("bell_off");
								break;
							case enumStato.on:
							case enumStato.signalOFF:
								res = loadImage("bell_on");
								break;
							case enumStato.signal:
								res = loadImage("bell_active");
								break;
							case enumStato.errore:
								res = loadImage("bell_err");
								break;
						}

					break;

				case enumComponente.push:
					if (!Enabled)
						res = loadImage("push_disabled");
					else
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("push_err");
								break;
							case enumStato.off:
								res = loadImage("push_off");
								break;
							case enumStato.on:
							case enumStato.signalOFF:
								res = loadImage("push_on");
								break;
							case enumStato.signal:
								res = loadImage("push_active");
								break;
							case enumStato.errore:
								res = loadImage("push_err");
								break;
						}

					break;

				case enumComponente.umidity:
					if (!Enabled)
						res = loadImage("umidity_disabled");
					else
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("umidity_err");
								break;
							case enumStato.off:
								res = loadImage("umidity_off");
								break;
							case enumStato.on:
							case enumStato.value:
								if (valore=="")
									res = loadImage("umidity_null");
								else
								{
									decimal umidity = Convert.ToDecimal(valore);

									if (umidity <= 5)                               // BASSA
										res = loadImage("umidity_0");
									else if (umidity > 5 && umidity <= 30)          // MINIMA
										res = loadImage("umidity_1");
									else if (umidity > 31 && umidity <= 40)         // SECCO
										res = loadImage("umidity_2");
									else if (umidity > 40 && umidity <= 60)         // NORMALE
										res = loadImage("umidity_3");
									else if (umidity > 60 && umidity <= 78)         // UMIDO
										res = loadImage("umidity_4");
									else if (umidity > 78)                          // ALTA
										res = loadImage("umidity_5");
								}
								break;
							case enumStato.errore:
								res = loadImage("umidity_err");
								break;
						}
					break;

				case enumComponente.temperature:
					if (!Enabled)
						res = loadImage("temp_disabled");
					else
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("temp_err");
								break;
							case enumStato.off:
								res = loadImage("temp_disabled");
								break;
							case enumStato.on:
							case enumStato.value:
								if (valore == "")
									res = loadImage("temp_null");
								else
								{
									decimal temperatura = Convert.ToDecimal(valore);

									if (temperatura <= 3)                                   // GELO
										res = loadImage("temp_0");
									else if (temperatura > 3 && temperatura <= 13)          // MINIMA
										res = loadImage("temp_1");
									else if (temperatura > 13 && temperatura <= 19)         // FREDDO
										res = loadImage("temp_2");
									else if (temperatura > 19 && temperatura <= 25)         // NORMALE
										res = loadImage("temp_3");
									else if (temperatura > 25 && temperatura <= 30)         // CALDO
										res = loadImage("temp_4");
									else if (temperatura > 30)                              // MASSIMA
										res = loadImage("temp_5");
								}
								break;
							case enumStato.errore:
								res = loadImage("temp_err");
								break;
						}
					break;

				case enumComponente.nodo:
					if (!Enabled)
						res = loadImage("raspberry_dusabled");
					else if (!Trusted)
						res = loadImage("raspberry_untrusted");
					else if (Error)
						res = loadImage("raspberry_err");
					else
					{
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("raspberry_on");
								break;
							case enumStato.off:
								res = loadImage("raspberry_off");
								break;
							case enumStato.signal:
							case enumStato.on:
								res = loadImage("raspberry_on");
								break;
							case enumStato.errore:
								res = loadImage("raspberry_err");
								break;
						}
					}
					break;
				case enumComponente.centrale:
					if (!Enabled)
						res = loadImage("central_disabled");
					else if (!Trusted)
						res = loadImage("central_untrusted");
					else if (Error)
						res = loadImage("central_err");
					else
					{
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("central_on");
								break;
							case enumStato.off:
								res = loadImage("central_off");
								break;
							case enumStato.signal:
							case enumStato.on:
								res = loadImage("central_on");
								break;
							case enumStato.errore:
								res = loadImage("central_err");
								break;
						}
					}
					break;
				case enumComponente.webcam_ip:
					if (!Enabled)
						res = loadImage("webcam_disabled");
					else if (!Trusted)
						res = loadImage("webcam_untrusted");
					else if (Error)
						res = loadImage("webcam_err");
					else
					{
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("webcam_on");
								break;
							case enumStato.off:
								res = loadImage("webcam_off");
								break;
							case enumStato.on:
							case enumStato.signal:
							case enumStato.signalOFF:
								res = loadImage("webcam_on");
								break;
							case enumStato.errore:
								res = loadImage("webcam_err");
								break;
						}
					}
					break;
				case enumComponente.webcam_rasp:
					if (!Enabled)
						res = loadImage("webcam_rasp_disabled");
					else if (!Trusted)
						res = loadImage("webcam_rasp_untrusted");
					else if (Error)
						res = loadImage("webcam_rasp_err");
					else
					{
						switch (stato)
						{
							case enumStato.nessuno:
								res = loadImage("webcam_rasp_err");
								break;
							case enumStato.off:
								res = loadImage("webcam_rasp_off");
								break;
							case enumStato.on:
							case enumStato.signal:
							case enumStato.signalOFF:
								res = loadImage("webcam_rasp_on");
								break;
							case enumStato.errore:
								res = loadImage("webcam_rasp_err");
								break;
						}
					}
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
					ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);
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

							ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);
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
								// --> toolbar info property
								OpenComponenteInfo(ID);
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
							// --> toolbar info property
							OpenComponenteInfo(ID);
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
					case "Property":
						OpenComponenteInfo(IDComponente);
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

		#region PROPERTY
		private void OpenComponenteInfo(int IDComponente)
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
					OpenComponenteInfo(Actualcomponente);
				}
				else
					messaggio.Text = "Non rilevato ID Componente";

			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void OpenComponenteInfo(Componente componente)
		{
			messaggio.Text = "";
			try
			{
				if (componente == null || componente.ID.HasValue==false)
					messaggio.Text = "Non rilevato Componente";
				else
				{
					PropertyText.Text = componente.Tipo.ToString().ToUpperInvariant().Replace("_"," ");

					OpenProperty(componente.ID.Value);
					ShowSchema();
					OpenRules(componente.ID.Value);
					ToolbarPropertyShow(enumShowToolbarScheda.Property);
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

				// visualizzare propeerty
				RaspaResult res = ComponentProperty.showProperty(IDComponente, Actualcomponente.Tipo, Actualcomponente.Margin, this);
				if (res.Esito)
					Actualcomponente = ComponentProperty.componente;
				else
					messaggio.Text = res.Message;

				// evidenziazione
				evidenzia(true);

			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
                if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void ShowSchema()
		{
			try
			{
				// TIPO
				switch (Actualcomponente.Tipo)
				{
					case enumComponente.nessuno:
					case enumComponente.nodo:
					case enumComponente.centrale:
						gpio.clear();
						break;
					case enumComponente.light:
						gpio.drawGPIO_LIGHT(Actualcomponente.Node_Pin);
						break;
					case enumComponente.pir:
						gpio.drawGPIO_PIR(Actualcomponente.Node_Pin);
						break;
					case enumComponente.umidity:
						gpio.drawGPIO_UMIDITY(Actualcomponente.Node_Pin);
						break;
					case enumComponente.temperature:
						gpio.drawGPIO_TEMP(Actualcomponente.Node_Pin);
						break;
					case enumComponente.push:
						gpio.drawGPIO_PUSH(Actualcomponente.Node_Pin);
						break;
					case enumComponente.bell:
						gpio.drawGPIO_BELL(Actualcomponente.Node_Pin);
						break;
					case enumComponente.moisture:
						gpio.drawGPIO_MOISTURE(Actualcomponente.Node_Pin);
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
		private void OpenRules(int IDComponente)
		{
			try
			{
				Regole.INIT(IDComponente, this);

			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		private void esci_property_Tapped(object sender, TappedRoutedEventArgs e)
		{
			ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);

			// Pulisci schermo
			eliminaActualImageSoloSeProvvisioria();

			// toglie evidenziazione
			evidenzia(false);

			// azzera componente attuale
			azzeraActualComponent();

		}



		#endregion


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

				ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);

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

				ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);

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
		private Image GetComponentByTag(string Tag)
		{
			Image res = null;
			try
			{
				foreach (var control in GridMappa.Children)
				{
					var img = control as Image;
					if (img != null && img.Tag != null && img.Tag.ToString() == Tag)
					{
						res = img;
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
		enumDrag dragObj = enumDrag.nessuno;
		public enum enumDrag
		{
			nessuno=0,
			image = 1,
			property=2,
		}

		// -----------------------------
		//  IMAGE
		// -----------------------------
		private void Button_DragStarting(UIElement sender, DragStartingEventArgs args)
		{
			try
			{
				// -----------------------------
				//  IMAGE
				// -----------------------------
				if (RaspaMode == enumMode.working)
					return;

				dragObj = enumDrag.image;

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
		PointerPoint selectedPointer = null;
		private void Img_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			try
			{
				// -----------------------------
				//  IMAGE
				// -----------------------------
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

		// -----------------------------
		//  PROPERTY
		// -----------------------------
		private void Toolbar_DragStarting(UIElement sender, DragStartingEventArgs args)
		{
			try
			{
				// -----------------------------
				//  PROPERTY
				// -----------------------------
				dragObj = enumDrag.property;
				args.Data.SetText("Muovi");
				args.Data.RequestedOperation = DataPackageOperation.Move;
				args.AllowedOperations = DataPackageOperation.Move;
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		PointerPoint toolbardPointer = null;
		private void Toolbar_PointerPressed(object sender, PointerRoutedEventArgs e)
		{
			try
			{
				RelativePanel pointer = sender as RelativePanel;
				if (pointer != null)
					toolbardPointer = e.GetCurrentPoint(pointer);
			}
			catch (Exception ex)
			{
				messaggio.Text = "Errore : " + ex.Message;
				if (Debugger.IsAttached) Debugger.Break();
			}
		}

		// -----------------------------
		//  DRAG OVER
		// -----------------------------
		private void Mappa_DragOver(object sender, DragEventArgs e)
		{
			messaggio.Text = "";
			try
			{
				if (dragObj == enumDrag.image && RaspaMode == enumMode.working)
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

		// -----------------------------
		//  DROP
		// -----------------------------
		private void Mappa_Drop(object sender, DragEventArgs e)
		{
			messaggio.Text = "";
			Componente item = null;
			try
			{
				// -----------------------------
				//  PROPERTY
				// -----------------------------
				if (dragObj == enumDrag.property)
				{
					e.AcceptedOperation = DataPackageOperation.Move;

					Grid b = sender as Grid;
					if (b == null)
						return;

					var p = e.GetPosition(main);
					var X = (toolbardPointer != null && toolbardPointer.Position != null) ? toolbardPointer.Position.X : 0;
					var Y = (toolbardPointer != null && toolbardPointer.Position != null) ? toolbardPointer.Position.Y : 0;
					ToolBar.Margin = new Thickness(p.X - X - 4, p.Y - Y - 4, 0, 0);
				}
				else
				{
					// -----------------------------
					//  IMAGE
					// -----------------------------
					if (dragObj == enumDrag.image && RaspaMode == enumMode.working)
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
						ActualImage.Margin = new Thickness(p.X - X - 4, p.Y - Y - 4, 0, 0);
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
							ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);
						else
						{
							// evidenziazione
							evidenzia(true);

							// show property
							OpenComponenteInfo(Actualcomponente);
						}

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
					ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);

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

			ToolbarPropertyShow(enumShowToolbarScheda.Nessuno);

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
		public enum enumShowToolbarScheda
		{
			Nessuno=0,
			Property = 1,
			Schema = 2,
			Rules = 3,
		}
		public void ToolbarPropertyShow(enumShowToolbarScheda show)
		{
			switch(show)
			{
				case enumShowToolbarScheda.Nessuno:
					ToolBar.Visibility = Visibility.Collapsed;
					TabComponent.Visibility = Visibility.Collapsed;
					break;
				case enumShowToolbarScheda.Property:
					TabComponent.SelectedIndex = 0;
					TabComponent.Visibility = Visibility.Visible;
					ToolBar.Visibility = Visibility.Visible;
					break;
				case enumShowToolbarScheda.Schema:
					TabComponent.SelectedIndex = 1;
					TabComponent.Visibility = Visibility.Visible;
					ToolBar.Visibility = Visibility.Visible;
					break;
				case enumShowToolbarScheda.Rules:
					TabComponent.SelectedIndex = 2;
					TabComponent.Visibility = Visibility.Visible;
					ToolBar.Visibility = Visibility.Visible;
					break;

			}
		}



		#endregion

		#endregion


	}
}
