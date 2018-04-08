using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaAction
{
	public class PlatForm_PIR: IPlatform
	{
		GpioPin gpioPIN = null;
		MQTT mqTT = null;
		private RaspaProtocol Protocol;
		GpioPinDriveMode Drive;
		private PlatformNotify notify;

		public PlatForm_PIR()
		{
		}

		public RaspaResult RUN(MQTT mqtt, GpioPin gpi, Dictionary<int, bool> EVENTS,RaspaProtocol protocol)
		{
			RaspaResult res = new RaspaResult(false, "NA");
			try
			{

				// controlli formali
				if (protocol.Comando != enumComando.comando)
					return new RaspaResult(false, "Platform deve eseguire solo comandi");

				// memorizzo il protocol
				Protocol = protocol;

				// GPIO
				gpioPIN = gpi;

				// Memorizzo MTQTT
				mqTT = mqtt;

				// istanzio notify
				notify = new PlatformNotify(mqTT);

				// PIN
				int PinNum = gpioPIN.PinNumber;

				#region OPTIONS
				enumPIROption option = (enumPIROption)Convert.ToInt32(Protocol.Destinatario.Options);
				GpioPinEdge edge = (option == enumPIROption.FallingEdge) ? GpioPinEdge.FallingEdge : GpioPinEdge.RisingEdge;
				#endregion
				#region EVENTS
				if (!EVENTS.ContainsKey(PinNum) || !EVENTS[PinNum])
				{
					gpioPIN.ValueChanged -= Pir_FallingEdge_ValueChanged;
					gpioPIN.ValueChanged -= Pir_RisingEdge_ValueChanged;
					if (edge == GpioPinEdge.FallingEdge)
						gpioPIN.ValueChanged += Pir_FallingEdge_ValueChanged;
					else
						gpioPIN.ValueChanged += Pir_RisingEdge_ValueChanged;

					// memorizzo che ho già impostato evento
					EVENTS[PinNum] = true;
				}
				#endregion

				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumAzione.on:
						gpioPIN.SetDriveMode(GpioPinDriveMode.Input);
						break;
					case enumAzione.off:
						gpioPIN.SetDriveMode(GpioPinDriveMode.Output);
						break;
					case enumAzione.read:
						Drive = gpioPIN.GetDriveMode();
						if (Drive == GpioPinDriveMode.Input)
							notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumAzione.on, gpioPIN.PinNumber, new List<string>());
						else
							notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumAzione.off, gpioPIN.PinNumber, new List<string>());

						break;

				}


			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, ex.Message, "");
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - SET : " + ex.Message);
			}
			return res;
		}

		private void Pir_FallingEdge_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			// se rilevato mando messaggio
			if (e.Edge == GpioPinEdge.FallingEdge)
			{
				// NOTIFY OK
				notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumAzione.signal, sender.PinNumber, new List<string>());
				// SPEEK
				if (Protocol != null)
				{
					SpeechService speek = new SpeechService();
					speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " PRESENZA RILEVATA");
				}
			}
		}
		private void Pir_RisingEdge_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			// se rilevato mando messaggio
			if (e.Edge == GpioPinEdge.RisingEdge)
			{
				// NOTIFY OK
				notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumAzione.signal, sender.PinNumber, new List<string>());

				// SPEEK
				if (Protocol != null)
				{
					SpeechService speek = new SpeechService();
					speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " PRESENZA RILEVATA");
				}
			}
		}

	}
}
