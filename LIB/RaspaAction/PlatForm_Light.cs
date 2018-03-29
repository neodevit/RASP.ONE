using RaspaEntity;
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
		public event ActionNotify ActionNotify;
		GpioPinValue valoreON = GpioPinValue.Low;
		GpioPinValue valoreOFF = GpioPinValue.High;

		public RaspaResult RUN(GpioPin gpioPIN, Dictionary<int, bool> EVENTS,RaspaProtocol Protocol)
		{
			RaspaResult res = new RaspaResult(true, "");
			GpioPinValue PinValue = GpioPinValue.Low;
			try
			{
				enumPINValue value = (enumPINValue)Convert.ToInt32(Protocol.Value);
				int PinNum = gpioPIN.PinNumber;


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
				GpioPinValue? nuovo_valore = (Protocol.Value == ((int)enumPINValue.on).ToString()) ? valoreON : valoreOFF;

				if (!EVENTS.ContainsKey(PinNum) || !EVENTS[PinNum])
				{
					gpioPIN.ValueChanged -= Light_ValueChanged;
					gpioPIN.ValueChanged += Light_ValueChanged;

					// memorizzo che ho già impostato evento
					EVENTS[PinNum] = true;
				}

				// Se il valore da impostare è = al valore corrente
				// restituisco errore di operazione non eseguita
				if (nuovo_valore.HasValue && nuovo_valore.Value != gpioPIN.Read())
				{
					gpioPIN.Write(nuovo_valore.Value);
					gpioPIN.SetDriveMode(GpioPinDriveMode.Output);

					// RILEGGO
					PinValue = gpioPIN.Read();
					if (PinValue != nuovo_valore)
						ActionNotify(false, "Non sono riuscito a settare SET.valore ", enumComponente.light, PinNum, Protocol.Value);
				}
				else
				{
					res = new RaspaResult(true, "Warning : Valore da impostare " + value.ToString() + " è già il valore corrente del pin " + PinNum);
					res.Warning = true;
					// notifico perchè il centrale deve aggiornare lo stato
					string valueInvariato = (nuovo_valore.Value == valoreON) ? ((int)enumPINValue.on).ToString() : ((int)enumPINValue.off).ToString();
					ActionNotify(true, "Pin SET invariato", enumComponente.light, PinNum, valueInvariato);
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
			string value = (sender.Read() == valoreON) ? ((int)enumPINValue.on).ToString() : ((int)enumPINValue.off).ToString();

			ActionNotify(true, "Pin SET Change", enumComponente.light, sender.PinNumber, value);
		}

	}
}
