using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace RaspaAction
{
	public class PlatForm_Light: IPlatform
	{
		GpioPin gpioPIN = null;
		MQTT mqTT = null;
		GpioPinValue valoreON = GpioPinValue.Low;
		GpioPinValue valoreOFF = GpioPinValue.High;
		RaspaProtocol Protocol;
		private PlatformNotify notify;
		GpioPinValue valore;
		int PinNumber = 0;

		public PlatForm_Light()
		{
		}

		public RaspaResult RUN(MQTT mqtt, GpioPin gpio, Dictionary<int, bool> EVENTS,RaspaProtocol protocol)
		{
			RaspaResult res = new RaspaResult(true, "");
			try
			{
				// controlli formali
				if (protocol.Comando != enumComando.comando)
					return new RaspaResult(false, "Platform deve eseguire solo comandi");

				// GPIO
				gpioPIN = gpio;

				// pin
				PinNumber = gpioPIN.PinNumber;

				// memorizzo il protocol
				Protocol = protocol;

				// Memorizzo MTQTT
				mqTT = mqtt;

				// istanzio notify
				notify = new PlatformNotify(mqTT);

				// PIN NUMBER
				int PinNum = gpioPIN.PinNumber;

				#region CALCOLA OPTIONS

				if (Protocol.Destinatario.Options == ((int)enumPINOptionIsON.low).ToString())
				{
					valoreON = GpioPinValue.Low;
					valoreOFF = GpioPinValue.High;
				}
				else if (Protocol.Destinatario.Options == ((int)enumPINOptionIsON.hight).ToString())
				{
					valoreON = GpioPinValue.High;
					valoreOFF = GpioPinValue.Low;
				}
				#endregion
				#region EVENTS
				if (!EVENTS.ContainsKey(PinNum) || !EVENTS[PinNum])
				{
					gpioPIN.ValueChanged -= Light_ValueChanged;
					gpioPIN.ValueChanged += Light_ValueChanged;

					// memorizzo che ho già impostato evento
					EVENTS[PinNum] = true;
				}
				#endregion

				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumStato.on:
						valore = valoreON;
						if (valore != gpioPIN.Read())
						{
							//-------------------
							// AZIONE
							//-------------------
							gpioPIN.Write(valore);
							gpioPIN.SetDriveMode(GpioPinDriveMode.Output);

						}
						else
							notify.ActionNotify(Protocol, true, "Nuovo valore impostato ", enumSubribe.central, enumComponente.light, enumComando.notify, enumStato.on, PinNumber);

						break;
					case enumStato.off:
						valore = valoreOFF;
						if (valore != gpioPIN.Read())
						{
							//-------------------
							// AZIONE
							//-------------------
							gpioPIN.Write(valore);
							gpioPIN.SetDriveMode(GpioPinDriveMode.Output);

						}
						else
							notify.ActionNotify(Protocol, true, "Nuovo valore impostato ", enumSubribe.central, enumComponente.light, enumComando.notify, enumStato.off, PinNumber);

						break;
					case enumStato.read:
						valore = gpioPIN.Read();
						if (valore == valoreON)
							notify.ActionNotify(Protocol, true, "Nuovo valore impostato ", enumSubribe.central, enumComponente.light, enumComando.notify, enumStato.on, PinNumber);
						else
							notify.ActionNotify(Protocol, true, "Nuovo valore impostato ", enumSubribe.central, enumComponente.light, enumComando.notify, enumStato.off, PinNumber);

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


		private void Light_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			SpeechService speek = new SpeechService();
			try
			{

				//-------------------
				// RILEGGO
				//-------------------
				GpioPinValue valore_impostato = sender.Read();

				//-------------------
				// RISPONDO
				//-------------------
				// ON/OFF 
				if (valore_impostato == valoreON)
				{
					// NOTIFY ON
					notify.ActionNotify(Protocol, true, "Nuovo valore impostato ", enumSubribe.central, enumComponente.light, enumComando.notify, enumStato.on, sender.PinNumber);
					// SPEEK
					if (Protocol != null)
						speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " Azione : ON ");
				}
				else
				{
					// NOTIFY OFF
					notify.ActionNotify(Protocol, true, "Nuovo valore impostato ", enumSubribe.central, enumComponente.light, enumComando.notify, enumStato.off, sender.PinNumber);
					// SPEEK
					if (Protocol != null)
						speek.parla(" NODO " + Protocol.Destinatario.Node_Num + " PIN " + Protocol.Destinatario.Node_Pin + " componente " + Protocol.Mittente.Nome + " Azione : OFF ");
				}

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - LIGHT : " + ex.Message);
			}
		}

	}
}
