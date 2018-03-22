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
		int numEventi = 0;
		public RaspaResult RUN(GpioPin gpioPIN, GpioPinEdge? Edge, int value)
		{
			RaspaResult res = new RaspaResult(true, "");
			GpioPinValue PinValue = GpioPinValue.Low;
			numEventi = 0;
			try
			{
				PinValue = gpioPIN.Read();
				res.Value = (PinValue == GpioPinValue.Low) ? "1" : "0";
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
