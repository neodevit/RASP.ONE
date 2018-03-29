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
	public class PlatForm_GET: IPlatform
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

				PinValue = gpioPIN.Read();
				res.Value = (PinValue == valoreON) ? ((int)enumPINValue.on).ToString() : ((int)enumPINValue.off).ToString();

				// restituisci messaggio
				ActionNotify(true, "GET PIN: " + PinNum, enumComponente.nessuno, PinNum, res.Value);

			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, ex.Message, "");
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - SET : " + ex.Message);
			}
			return res;
		}

	}
}
