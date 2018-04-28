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

		public RaspaResult RUN(MQTT mqtt, GpioPin gpio, Dictionary<string, bool> EVENTS,RaspaProtocol protocol)
		{
			RaspaResult res = new RaspaResult(false, "NA");
			try
			{

				// controlli formali
				if (protocol.Comando != enumComando.comando)
					return new RaspaResult(false, "Platform deve eseguire solo comandi");
				if (gpio == null)
					return new RaspaResult(false, "Platform deve avere valorizzato GPIO Pin");

				// memorizzo il protocol
				Protocol = protocol;

				// GPIO
				gpioPIN = gpio;

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

				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumStato.on:
					case enumStato.signal:
					case enumStato.signalOFF:
						gpioPIN.DebounceTimeout = TimeSpan.FromMilliseconds(500);
						gpioPIN.SetDriveMode(GpioPinDriveMode.Input);
						notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.on, gpioPIN.PinNumber);
						break;
					case enumStato.off:
						gpioPIN.SetDriveMode(GpioPinDriveMode.Output);
						notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.off, gpioPIN.PinNumber);
						break;
					case enumStato.read:
						Drive = gpioPIN.GetDriveMode();
						if (Drive == GpioPinDriveMode.Input)
							notify.ActionNotify(Protocol, true, "Pir read", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.on, gpioPIN.PinNumber);
						else
							notify.ActionNotify(Protocol, true, "Pir read", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.off, gpioPIN.PinNumber);

						break;

				}

				#region EVENTS
				string chiave = PinNum + "|" + ((int)Protocol.Destinatario.Tipo).ToString();
				if (!EVENTS.ContainsKey(chiave) || !EVENTS[chiave])
				{
					gpioPIN.ValueChanged -= Pir_FallingEdge_ValueChanged;
					gpioPIN.ValueChanged -= Pir_RisingEdge_ValueChanged;
					if (edge == GpioPinEdge.FallingEdge)
						gpioPIN.ValueChanged += Pir_FallingEdge_ValueChanged;
					else
						gpioPIN.ValueChanged += Pir_RisingEdge_ValueChanged;

					// memorizzo che ho già impostato evento
					EVENTS[chiave] = true;
				}
				#endregion


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
				notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.signal, sender.PinNumber);
				// SPEEK
				if (Protocol != null)
				{
					SpeechService speek = new SpeechService();
					speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " PRESENZA RILEVATA");
				}
			}
			//else
			//	notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.signalOFF, sender.PinNumber);
		}
		private void Pir_RisingEdge_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			// se rilevato mando messaggio
			if (e.Edge == GpioPinEdge.RisingEdge)
			{
				// NOTIFY OK
				notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.signal, sender.PinNumber);

				// SPEEK
				if (Protocol != null)
				{
					SpeechService speek = new SpeechService();
					speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " PRESENZA RILEVATA");
				}
			}
			//else
			//	notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumStato.signalOFF, sender.PinNumber);
		}

	}
}
