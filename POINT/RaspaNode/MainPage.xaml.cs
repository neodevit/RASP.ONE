using MQTTnet.Protocol;
using RaspaAction;
using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Globalization.DateTimeFormatting;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspaNode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {


		public MainPage()
        {
            this.InitializeComponent();
			Start();
			Tabs.SelectedIndex = 2;
		}

		private RaspaProtocol OriginalMessage;
		private Azione action = null;

		private const string IPCentrale = "192.168.1.10";
		private bool flgGPIO = true;
		private bool flgNodoUpdate = true;
		private bool flgMQTT = true;
		bool flgNodoSync = true;

		public async void Start()
		{
			// --------------------------------------
			// SYNC NODE TIME
			// --------------------------------------
			SyncDateTime();

			// --------------------------------------
			// NODE INIT
			// --------------------------------------
			NODE_INIT();

			// --------------------------------------
			// INIT MQTT
			// --------------------------------------
			if (flgMQTT)
				INIT_MQTT();

			// --------------------------------------
			// NODO UPDATE with sysstem info
			// --------------------------------------
			if (flgNodoUpdate)
				NODE_UPDATE();

			// --------------------------------------
			// SYNC COMPONENT from CENTRALE
			//---------------------------------------
			if (flgNodoSync)
				NODE_SYNC_COMPONENT();

			// --------------------------------------
			// GPIO
			// --------------------------------------
			// Inizializza scheda
			if (flgGPIO)
			{
				RaspaResult resGpio = INIT_GPIO();
				stato_gpio = resGpio.Esito;
			}

			// --------------------------------------
			// START MQTT
			// --------------------------------------
			if (flgMQTT)
				START_MQTT();

			// --------------------------------------
			// ACTION
			// --------------------------------------
			if (stato_gpio)
			{
				action = new Azione(mqTT,GPIO, PIN, platform_Engine, PlatForm_events);
			}

		}
		#region SYNC DATE TIME
		public void SyncDateTime()
		{
			RaspBerry RB = new RaspBerry();
			RB.SyncDateTime();
		}
		#endregion

		#region MQTT
		MQTT mqTT;

		private void INIT_MQTT()
		{
			mqTT = new MQTT(nodo.SystemID,nodo.HostName,nodo.IPv4);
			mqTT.createClient();

		}
		private void START_MQTT()
		{
			mqTT.startClient();
			mqTT.Receive -= MQTTReceive;
			mqTT.Receive += MQTTReceive;
			mqTT.Logging -= MQTTLogging;
			mqTT.Logging += MQTTLogging;

		}

		private void MQTTReceive(string Topic, string Payload, MqttQualityOfServiceLevel QoS, bool Retain)
		{
			var ignore = stato.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				writeLogVideo("<-- " + Topic + "-" + Payload + "-" + QoS.ToString() + "-" + Retain.ToString());
			});

			MessageUDPInput(Payload);
		}
		private void MQTTLogging(string Messaggio)
		{
			var ignore = stato.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				writeLogVideo(Messaggio);
			});
		}
		private void writeLogVideo(string Messaggio)
		{
			// scrivi log
			stato.Items.Insert(0, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + " " + Messaggio);
			// pulisci log vecchi
			try
			{
				if (stato.Items.Count > 50)
					for (int i = 50; i <= stato.Items.Count; i++)
						stato.Items.RemoveAt(i);
			}
			catch { }
		}

		#endregion

		#region COMANDI
		private void MessageUDPInput(string message)
		{
			// leggi protocollo
			OriginalMessage  = new RaspaProtocol(message);

			// esegue
			action?.Execute(OriginalMessage);
		}

		#endregion

		#region GPIO
		bool stato_gpio = false;


		internal GpioController GPIO;
		internal Dictionary<int, GpioPin> PIN;
		internal Dictionary<int, bool> PlatForm_events;
		internal Dictionary<int, IPlatform> platform_Engine;


		private RaspaResult INIT_GPIO()
		{
			RaspaResult res = new RaspaResult(true);

			try
			{
				// Se ci son già passato esco
				if (GPIO != null || PIN != null || (PIN != null && PIN.Count > 0))
					return new RaspaResult(true);

				// INIT GPIO
				GPIO = GpioController.GetDefault();
				if (GPIO == null)
					return new RaspaResult(false, "There is no GPIO controller on this device.", "");

				// INIT PIN_events
				PlatForm_events = new Dictionary<int, bool>();
				platform_Engine = new Dictionary<int, IPlatform>();


				// INIT PIN
				PIN = new Dictionary<int, GpioPin>();

				//GpioPin p2 = null;
				//PIN.Add(2, p2);

				//GpioPin p3 = null;
				//PIN.Add(3, p3);

				GpioPin p4 = null;
				PIN.Add(4, p4);

				GpioPin p17 = null;
				PIN.Add(17, p17);

				GpioPin p27 = null;
				PIN.Add(27, p27);

				GpioPin p22 = null;
				PIN.Add(22, p22);

				//GpioPin p10 = null;
				//PIN.Add(10, p10);

				//GpioPin p9 = null;
				//PIN.Add(9, p9);

				//GpioPin p11 = null;
				//PIN.Add(11, p11);

				GpioPin p5 = null;
				PIN.Add(5, p5);

				GpioPin p6 = null;
				PIN.Add(6, p6);

				GpioPin p13 = null;
				PIN.Add(13, p13);

				GpioPin p19 = null;
				PIN.Add(19, p19);

				GpioPin p26 = null;
				PIN.Add(26, p26);

				//GpioPin p14 = null;
				//PIN.Add(14, p14);

				//GpioPin p15 = null;
				//PIN.Add(15, p15);

				GpioPin p18 = null;
				PIN.Add(18, p18);

				GpioPin p23 = null;
				PIN.Add(23, p23);

				GpioPin p24 = null;
				PIN.Add(24, p24);

				GpioPin p25 = null;
				PIN.Add(25, p25);

				//GpioPin p8 = null;
				//PIN.Add(8, p8);

				//GpioPin p7 = null;
				//PIN.Add(7, p7);

				GpioPin p12 = null;
				PIN.Add(12, p12);

				GpioPin p16 = null;
				PIN.Add(16, p16);

				GpioPin p20 = null;
				PIN.Add(20, p20);

				GpioPin p21 = null;
				PIN.Add(21, p21);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("INIT_GPIO : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		#endregion


		#region NODE
		private Componente nodo = null;

		private void nodeUPDATE_Click(object sender, RoutedEventArgs e)
		{
			NODE_INIT();
			NODE_UPDATE();
		}
		private RaspaResult NODE_INIT()
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				// Leggi node setting from node
				RaspBerry client = new RaspBerry();
				string IPv4 = client.GetLocalIPv4();

				nodo = new Componente();
				nodo.HostName = client.GetHostName();
				nodo.IPv4 = IPv4;
				nodo.IPv6 = "";
				nodo.HWAddress = client.GetHWAddress();
				nodo.BlueTooth = "";
				nodo.OSVersion = client.GetOSVersion();
				nodo.NodeSWVersion = client.GetRASPANodeVersion();

				nodo.SystemProductName = client.SystemProductName();
				nodo.SystemID = client.GetHardwareID();
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("INIT_ACTUAL : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}

		private RaspaResult NODE_UPDATE()
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				// richiedi al centrale di salvarsi la configurazione del nodo
				RaspaProtocol protocol = new RaspaProtocol();
				protocol.Comando = enumComando.nodeInit;
				protocol.Mittente = nodo;
				protocol.Destinatario = new Componente();
				protocol.Destinatario.IPv4 = IPCentrale;
				protocol.Destinatario.Tipo = enumComponente.nessuno;
				protocol.SubcribeResponse = enumSubribe.IPv4;
				protocol.SubcribeDestination = enumSubribe.reload;
				mqTT.Publish(protocol);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("INIT_ACTUAL : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}

		/// <summary>
		/// RELOAD COMPONENT
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nodeSYNC_Click(object sender, RoutedEventArgs e)
		{
			NODE_SYNC_COMPONENT();
		}
		private RaspaResult NODE_SYNC_COMPONENT()
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				// richiedi al centrale di reinizilizzarmi
				RaspaProtocol protocol = new RaspaProtocol();
				protocol.Comando = enumComando.nodeReload;
				protocol.Mittente = nodo;
				protocol.Destinatario = new Componente();
				protocol.Destinatario.IPv4 = IPCentrale;
				protocol.Destinatario.Tipo = enumComponente.nessuno;
				protocol.SubcribeResponse = enumSubribe.IPv4;
				protocol.SubcribeDestination = enumSubribe.reload;
				mqTT.Publish(protocol);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("INIT_ACTUAL : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		#endregion

	}
}
