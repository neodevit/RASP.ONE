using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaAction
{
	public class PlatForm_Light: IPlatform
	{
		public event ActionNotify ActionNotify;
		int numEventi = 0;
		public RaspaResult RUN(GpioPin gpioPIN, GpioPinEdge? Edge, int value)
		{
			RaspaResult res = new RaspaResult(true, "");
			GpioPinValue PinValue = GpioPinValue.Low;
			numEventi = 0;
			try
			{
				GpioPinValue? nuovo_valore = (value == 1) ? GpioPinValue.Low : GpioPinValue.High;
				gpioPIN.ValueChanged -= Light_ValueChanged;
				gpioPIN.ValueChanged += Light_ValueChanged;

				// Se il valore da impostare è = al valore corrente
				// restituisco errore di operazione non eseguita
				if (nuovo_valore.HasValue && nuovo_valore.Value != gpioPIN.Read())
				{
					gpioPIN.Write(nuovo_valore.Value);
					gpioPIN.SetDriveMode(GpioPinDriveMode.Output);

					// RILEGGO
					PinValue = gpioPIN.Read();
					if (PinValue != nuovo_valore)
						return new RaspaResult(false, "Non sono riuscito a settare SET.valore " + nuovo_valore.ToString(), "0");
					else
						res.Value = (PinValue == GpioPinValue.Low) ? "1" : "0";
				}
				else
				{
					res = new RaspaResult(false, "Warning : Valore da impostare " + value + " è già il valore corrente del pin " + gpioPIN.PinNumber);
					res.Warning = false;
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
			numEventi++;
			if (numEventi > 1)
				return;

			int value = (sender.Read() == GpioPinValue.Low) ? 1 : 0;
			this.ActionNotify(true, "Pin SET Change", enumComponente.light, sender.PinNumber, value);
		}

	}
}
