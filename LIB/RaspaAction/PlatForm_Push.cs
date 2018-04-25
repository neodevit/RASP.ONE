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
	public class PlatForm_Push : IPlatform
	{
		GpioPin gpioPIN = null;
		MQTT mqTT = null;
		private RaspaProtocol Protocol;
		GpioPinDriveMode Drive;
		private PlatformNotify notify;
		GpioPinValue valore;

		public PlatForm_Push()
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

				#region EVENTS
				string chiave = PinNum + "|" + ((int)Protocol.Destinatario.Tipo).ToString();
				if (!EVENTS.ContainsKey(chiave) || !EVENTS[chiave])
				{
					gpioPIN.ValueChanged -= GpioPIN_ValueChanged;
					gpioPIN.ValueChanged += GpioPIN_ValueChanged;

					// memorizzo che ho già impostato evento
					EVENTS[chiave] = true;
				}
				#endregion

				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumStato.nessuno:
					case enumStato.on:
						// Check if input pull-up resistors are supported
						// are supported if button have resitor embedded
						// else add resistor 10k from 5v to pin
						if (gpioPIN.IsDriveModeSupported(GpioPinDriveMode.InputPullDown))
							gpioPIN.SetDriveMode(GpioPinDriveMode.InputPullDown);
						else
							gpioPIN.SetDriveMode(GpioPinDriveMode.Input);
						// We also set the DebounceTimeout property to 50ms to filter out spurious events caused by electrical noise.
						// Buttons are mechanical devices and can make and break contact many times on a single button press.
						// We don't want to be overwhelmed with events so we filter these out.
						gpioPIN.DebounceTimeout = TimeSpan.FromMilliseconds(50);
						// restiutuisci esito
						notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.push, enumComando.notify, enumStato.on, gpioPIN.PinNumber);
						break;
					case enumStato.off:
						gpioPIN.SetDriveMode(GpioPinDriveMode.Output);
						// restituisci esito
						notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.push, enumComando.notify, enumStato.off, gpioPIN.PinNumber);
						break;
					case enumStato.read:
						Drive = gpioPIN.GetDriveMode();
						if (Drive == GpioPinDriveMode.InputPullUp)
						{
							valore = gpioPIN.Read();
							if (valore == GpioPinValue.High)
								notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.push, enumComando.notify, enumStato.signal, gpioPIN.PinNumber);
							else
								notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.push, enumComando.notify, enumStato.on, gpioPIN.PinNumber);
						}
						else
							notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.push, enumComando.notify, enumStato.off, gpioPIN.PinNumber);

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

		private void GpioPIN_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
		{
			if (args.Edge == GpioPinEdge.FallingEdge)
			{
				notify.ActionNotify(Protocol, true, "Push Button", enumSubribe.central, enumComponente.push, enumComando.notify, enumStato.signal, gpioPIN.PinNumber);
				// SPEEK
				if (Protocol != null)
				{
					SpeechService speek = new SpeechService();
					speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " SUONA IL CAMPANELLO");
				}
			}
			else
			{
				notify.ActionNotify(Protocol, true, "Push Button", enumSubribe.central, enumComponente.push, enumComando.notify, enumStato.signalOFF, gpioPIN.PinNumber);
			}
		}


	}
}
