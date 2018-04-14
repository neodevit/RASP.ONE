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
	public class PlatForm_Bell : IPlatform
	{
		GpioPin gpioPIN = null;
		MQTT mqTT = null;
		private RaspaProtocol Protocol;
		GpioPinDriveMode Drive;
		private PlatformNotify notify;

		public PlatForm_Bell()
		{
		}

		public RaspaResult RUN(MQTT mqtt, GpioPin gpio, Dictionary<int, bool> EVENTS,RaspaProtocol protocol)
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
				if (!EVENTS.ContainsKey(PinNum) || !EVENTS[PinNum])
				{
					gpioPIN.ValueChanged -= GpioPIN_ValueChanged;
					gpioPIN.ValueChanged += GpioPIN_ValueChanged;

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
							notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.bell, enumComando.notify, enumAzione.on, gpioPIN.PinNumber);
						else
							notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.bell, enumComando.notify, enumAzione.off, gpioPIN.PinNumber);

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
			if (args.Edge == GpioPinEdge.RisingEdge)
			{
				notify.ActionNotify(Protocol, true, "Push Button", enumSubribe.central, enumComponente.bell, enumComando.notify, enumAzione.signal, gpioPIN.PinNumber);
				// SPEEK
				if (Protocol != null)
				{
					SpeechService speek = new SpeechService();
					speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " SUONA IL CAMPANELLO");
				}
			}
		}


	}
}
