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
        public MainPage()
        {
            this.InitializeComponent();
			Start();

		}


		private const string IPCentrale = "192.168.1.10";
		public async void Start()
		{

			bool flgGPIO = true;
			bool flgNodo = true;
			bool flgActual = false;
			bool flgUDP = true;

			// --------------------------------------
			// INIT UDP
			// --------------------------------------
			if (flgUDP)
				INIT_UDP();

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

				// Aggiorna i dati del nodo sul DB
				RaspaResult resUpdNode = UPDATE_NODE();
				if (!resUpdNode.Esito)
					System.Diagnostics.Debug.WriteLine("NODE not UPDATE on DB : " + resUpdNode.Message);

				// visualizza dati nella videata
				//NODE_SHOW();
			}

			// --------------------------------------
			// ACTUAL
			// --------------------------------------
			if (flgActual)
			{
				// ACTUALS : legge dal DB le ultime configurazioni dei pin del NODO e li rivalorizza
				RaspaResult resActual = await INIT_ACTUAL();
				if (!resActual.Esito)
					System.Diagnostics.Debug.WriteLine("ACTUAL not INITIALIZZATED GPIO : " + resActual.Message);
			}

			// --------------------------------------
			// ACTION
			// --------------------------------------
			if (stato_gpio)
			{
				action = new Azione(udp, GPIO, PIN);
				action.ActionNotify -= ActionNotify;
				action.ActionNotify += ActionNotify;
			}

			// --------------------------------------
			// START UDP LISTENER
			// --------------------------------------
			if (flgUDP)
				udp.StartListener();

		}

		#region MESSAGGING
		UDP udp;
		bool FlgUDPisON = false;
		private void INIT_UDP()
		{
			try
			{
				if (FlgUDPisON)
					return;
				udp = new UDP();
				udp.Receive -= UDPReceive;
				udp.Receive += UDPReceive;

				udp.Logging -= UDPLogging;
				udp.Logging += UDPLogging;

				udp.ConnectionResult -= UDPConnectionResult;
				udp.ConnectionResult += UDPConnectionResult;
			}
			catch (Exception ex)
			{

			}
		}
		private void btnON_Server_Click(object sender, RoutedEventArgs e)
		{
			udp.StartListener();
		}
		private void UDPConnectionResult(bool esito)
		{
			FlgUDPisON = esito;

			var ignore = flagEsito.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				flagConnection.IsChecked = esito;
			});
		}
		private void UDPReceive(string Messaggio)
		{
			var ignore = Receive.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				Receive.Text = Messaggio;
			});
			MessageUDPInput(Messaggio);
		}
		private void UDPLogging(string Messaggio)
		{
			var ignore = stato.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				stato.Items.Insert(0, Messaggio);
			});
		}
		private void Message_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
		{
			flagEsito.IsChecked = false;
		}
		private void btnSend_Click(object sender, RoutedEventArgs e)
		{
			flagEsito.IsChecked = false;
			Send(Remote.Text, Message.Text);
		}
		private async void Send(string address, string mess)
		{
			try
			{
				bool esito = await udp.SendMessage(address, mess);
				var ignore = flagEsito.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
				{
					flagEsito.IsChecked = esito;
				});
			}
			catch (Exception ex)
			{

			}
		}

		#endregion

		#region COMANDI
		Azione action = null;
		private async void MessageUDPInput(string message)
		{
			RaspaProtocol Protocol = new RaspaProtocol(message);
			RaspaResult res = new RaspaResult(false, "Elaborazione Comando non inizializzata");

			
			if (action != null)
				res = action.Execute(Protocol);

			if (!res.Esito)
				ActionNotify(res.Esito, res.Message, Protocol.Componente.Tipo, Protocol.Componente.Pin, Convert.ToInt32(Protocol.Componente.Value));

		}

		private void ActionNotify(bool Esito,string Messaggio,enumComponente componente, int pin, int value)
		{
			try
			{
				if (nodo != null && nodo.Trusted && nodo.Network != null && nodo.Network.IPv4 != null)
				{
					DBCentral DB = new DBCentral();
					RaspaResult res = DB.SetCommand(nodo.Num, pin, value.ToString(), nodo.Network.HostName);

					// MANDA MESSAGGI UDP AGLI ABBONATI
					Subscriptions subscriptions = DB.GetSubscriptionByNODOSubscriptionAndPIN(nodo.Num, pin);
					foreach (Subscription subscription in subscriptions)
					{
						// evita loop di notify a se stesso
						if (subscription.NODE_IPv4 == nodo.Network.IPv4)
							continue;


						// PREPARA MESSAGGIO
						RaspaProtocol messRule = new RaspaProtocol(Esito,
																	Messaggio,
																	subscription.ID,
																	enumComando.notify,
																	new RaspaProtocol_Nodo(nodo.ID, nodo.Enabled, nodo.Trusted, nodo.Num, nodo.Network.IPv4),
																	new RaspaProtocol_Nodo(null, null, null, subscription.NODE_subscriber, subscription.NODE_IPv4),
																	new RaspaProtocol_Componente(null, componente, null, pin, value.ToString(), null));

						

						// invia messaggio
						SendMessagePoint(subscription.NODE_IPv4, messRule.BuildJson());
					}
				}
			}
			catch { }
		}
		private async void SendMessagePoint(string address, string mess)
		{
			try
			{
				Send(address, mess);
			}
			catch (Exception ex)
			{

			}
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
		private Node nodo = null;
		private RaspaResult INIT_NODE()
		{
			RaspaResult res = new RaspaResult(true);

			try
			{
				RaspBerry client = new RaspBerry();
				string IPv4 = client.GetLocalIPv4();

				DBCentral db = new DBCentral();
				nodo = db.GetNODEByIPv4(IPv4);

				if (nodo == null)
					res = new RaspaResult(false, "Nodo non riconosciuto nel sistema");

			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("INIT_NODE : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		private RaspaResult UPDATE_NODE()
		{
			RaspaResult res = new RaspaResult(true);

			try
			{
				if (nodo == null)
				{
					nodo = new Node();
					nodo.Network = new NetworkInfo();
				}

				RaspBerry client = new RaspBerry();
				string IPv4 = client.GetLocalIPv4();

				// AGGIORNA NODO 
				NetworkInfo net = Tools.GetDeviceNetwork();
				nodo.Stato = enumStato.on;
				nodo.Network.HostName = net.HostName;
				nodo.Network.BlueTooth = net.BlueTooth;
				nodo.Network.HWAddress = "";
				nodo.Network.IPv4 = net.IPv4;
				nodo.Network.IPv6 = net.IPv6;

				// SALVA NODO
				DBCentral db = new DBCentral();
				db.SetNODE(nodo, net.HostName);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, "FAILED : " + ex.Message);
				System.Diagnostics.Debug.WriteLine("UPDATE_NODE : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}
		#endregion

		#region ACTUAL
		private Actuals actuals = null;
		private async Task<RaspaResult> INIT_ACTUAL()
		{
			RaspaResult res = new RaspaResult(true);

			try
			{


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
		#endregion

	}
}
