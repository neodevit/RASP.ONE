using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaAction
{
	public class PlatForm_Moisture: IPlatform
	{
		GpioPin gpioPIN = null;
		MQTT mqTT = null;
		private RaspaProtocol Protocol;
		GpioPinDriveMode Drive;
		private PlatformNotify notify;
		GpioPinValue valoreON = GpioPinValue.High;
		GpioPinValue valoreOFF = GpioPinValue.Low;

		public PlatForm_Moisture()
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


				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumAzione.off:
						// spegni input
						gpioPIN.SetDriveMode(GpioPinDriveMode.Output);

						// dispose sensor
						notify.ActionNotify(Protocol, true, "Read Moisture", enumSubribe.central, enumComponente.temperature, enumComando.notify, enumAzione.off, PinNum, new List<string>());

						break;
					case enumAzione.on:
						var r = gpioPIN.Read();
						gpioPIN.SetDriveMode(GpioPinDriveMode.Input);
						Drive = gpioPIN.GetDriveMode();

						gpioPIN.DebounceTimeout = TimeSpan.FromMilliseconds(50);

						#region EVENTS
						if (!EVENTS.ContainsKey(PinNum) || !EVENTS[PinNum])
						{
							gpioPIN.ValueChanged -= PinIn_ValueChanged;
							gpioPIN.ValueChanged += PinIn_ValueChanged;

							// memorizzo che ho già impostato evento
							EVENTS[PinNum] = true;
						}
						#endregion


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

		private void PinIn_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
		{

			if (gpioPIN.Read() == valoreON)
			{
				// need h2o
				notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumAzione.signal, sender.PinNumber);

				// SPEEK
				if (Protocol != null)
				{
					SpeechService speek = new SpeechService();
					speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " HO BISOGNO DI ACQUA");
				}
			}
			else
				// plat is ok
				notify.ActionNotify(Protocol, true, "Pir Change", enumSubribe.central, enumComponente.pir, enumComando.notify, enumAzione.signalOFF, sender.PinNumber);

		}

	}
}
