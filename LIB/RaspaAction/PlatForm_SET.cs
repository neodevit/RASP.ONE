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
	public class PlatForm_SET: IPlatform
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
				// value
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
					gpioPIN.ValueChanged -= PinSet_ValueChanged;
					gpioPIN.ValueChanged += PinSet_ValueChanged;

					// memorizzo che ho già impostato evento
					EVENTS[PinNum] = true;
				}

				if (nuovo_valore.HasValue)
					gpioPIN.Write(nuovo_valore.Value);
				gpioPIN.SetDriveMode(GpioPinDriveMode.Output);

				// RILEGGO
				PinValue = gpioPIN.Read();
				if (PinValue != nuovo_valore)
					ActionNotify(false, "Non sono riuscito a settare SET.valore " + nuovo_valore.ToString(), enumComponente.nessuno, PinNum, Protocol.Value);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, ex.Message, "");
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - SET : " + ex.Message);
			}
			return res;
		}
		private void PinSet_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			try
			{
				string value = (sender.Read() == valoreON) ? ((int)enumPINValue.on).ToString() : ((int)enumPINValue.off).ToString();
				ActionNotify(true, "Pin SET Change", enumComponente.nessuno, sender.PinNumber, value);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - PIN VALUE CHANGE : " + ex.Message);
			}
		}

	}
}
