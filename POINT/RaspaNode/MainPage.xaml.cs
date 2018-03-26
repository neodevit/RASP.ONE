using MQTTnet.Protocol;
using RaspaAction;
using RaspaDB;
using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspaNode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		private string mqTT_Server = "192.168.1.10";


		public MainPage()
        {
            this.InitializeComponent();
			Start();
			Tabs.SelectedIndex = 2;
		}


		private const string IPCentrale = "192.168.1.10";
		RaspaProtocol OriginalMessage;

		bool flgGPIO = true;
		bool flgNodo = true;
		bool flgMQTT = true;
		public async void Start()
		{



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
			// NODO
			// --------------------------------------
			if (flgNodo)
			{
				// Inizializza nodo
				RaspaResult resNode = INIT_NODE();
				if (!resNode.Esito)
					System.Diagnostics.Debug.WriteLine("NODE not TRUSTED : " + resNode.Message);

			}

			// --------------------------------------
			// INIT MQTT
			// --------------------------------------
			if (flgMQTT)
				INIT_MQTT();

			// --------------------------------------
			// ACTION
			// --------------------------------------
			if (stato_gpio)
			{
				action = new Azione(GPIO, PIN);
				action.ActionNotify -= ActionNotify;
				action.ActionNotify += ActionNotify;
			}


		}
		#region MQTT
		MQTT mqTT;

		private void INIT_MQTT()
		{
			mqTT = new MQTT(nodo.ID.Value,nodo.Nome,nodo.IPv4);
			mqTT.createClient();
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
				writeLogVideo("Receive " + Topic + "-" + Payload + "-" + QoS.ToString() + "-" + Retain.ToString());
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
			stato.Items.Insert(0, Messaggio);
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
		Azione action = null;
		RaspaResult EsitoComando = new RaspaResult(false, "NA");
		private void MessageUDPInput(string message)
		{
			OriginalMessage  = new RaspaProtocol(message);
			EsitoComando = new RaspaResult(false, "Elaborazione Comando non inizializzata");

			if (action != null)
				EsitoComando = action.Execute(OriginalMessage);

			if (!EsitoComando.Esito)
				ActionNotify(EsitoComando.Esito, EsitoComando.Message, OriginalMessage.Destinatario.Tipo, OriginalMessage.Destinatario.Node_Pin, Convert.ToInt32(OriginalMessage.Value));

		}

		private void ActionNotify(bool Esito,string Messaggio,enumComponente componente, int pin, int value)
		{
			try
			{
				OriginalMessage.swapMittDest();
				OriginalMessage.Comando = enumComando.notify;
				OriginalMessage.Esito = Esito;
				OriginalMessage.Message = Messaggio;
				OriginalMessage.Value = value.ToString();
				mqTT.Publish(OriginalMessage);
			}
			catch { }
		}

		#endregion




		#region PHISICAL
		bool stato_gpio = false;


		#region GPIO
		internal GpioController GPIO;
		internal Dictionary<int, GpioPin> PIN;

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

		private RaspaResult INIT_NODE()
		{
			RaspaResult res = new RaspaResult(true);

			try
			{
				RaspBerry client = new RaspBerry();
				NetworkInfo net = Tools.GetDeviceNetwork();

				DBCentral db = new DBCentral();
				nodo = db.GetComponenteByIPv4(net.IPv4);

				if (nodo == null)
					res = new RaspaResult(false, "Nodo non riconosciuto nel sistema");

				nodo.HostName = net.HostName;
				nodo.IPv4 = net.IPv4;
				nodo.IPv6 = net.IPv6;
				nodo.HWAddress = net.HWAddress;
				nodo.BlueTooth = net.BlueTooth;

				res = db.SetComponenti(nodo, nodo.HostName);

			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("INIT_NODE : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		#endregion

		#endregion

	}
}
