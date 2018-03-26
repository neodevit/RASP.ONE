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
		public RaspaResult RUN(GpioPin gpioPIN,RaspaProtocol Protocol)
		{
			RaspaResult res = new RaspaResult(true, "");
			GpioPinValue PinValue = GpioPinValue.Low;
			try
			{
				GpioPinValue valoreON = (Protocol.Destinatario.Options == "0") ? GpioPinValue.Low : GpioPinValue.High;
				GpioPinValue valoreOFF = (Protocol.Destinatario.Options == "0") ? GpioPinValue.High : GpioPinValue.Low;

				PinValue = gpioPIN.Read();
				res.Value = (PinValue == valoreON) ? "1" : "0";

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
