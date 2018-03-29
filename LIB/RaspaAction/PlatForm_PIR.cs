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
	public class PlatForm_PIR: IPlatform
	{
		public event ActionNotify ActionNotify;
		public RaspaResult RUN(GpioPin gpioPIN, Dictionary<int, bool> EVENTS,RaspaProtocol Protocol)
		{
			RaspaResult res = new RaspaResult(false, "NA");
			try
			{
				enumPIROption option = (enumPIROption)Convert.ToInt32(Protocol.Destinatario.Options);
				GpioPinEdge edge = (option == enumPIROption.FallingEdge) ? GpioPinEdge.FallingEdge : GpioPinEdge.RisingEdge;

				enumPirValue value = (enumPirValue)Convert.ToInt32(Protocol.Value);
				int PinNum = gpioPIN.PinNumber;

				switch (value)
				{
					case enumPirValue.on: // ACCENDO
						res.Value = ((int)enumPirValue.on).ToString();
						res.Esito = true;
						res.Message = "PIR ON";

						if (!EVENTS.ContainsKey(PinNum) || !EVENTS[PinNum])
						{
							gpioPIN.ValueChanged -= Pir_FallingEdge_ValueChanged;
							gpioPIN.ValueChanged -= Pir_RisingEdge_ValueChanged;
							if (edge == GpioPinEdge.FallingEdge)
								gpioPIN.ValueChanged += Pir_FallingEdge_ValueChanged;
							else
								gpioPIN.ValueChanged += Pir_RisingEdge_ValueChanged;

							// memorizzo che ho già impostato evento
							EVENTS[PinNum] = true;
						}

						gpioPIN.SetDriveMode(GpioPinDriveMode.Input);
						ActionNotify(res.Esito, res.Message, enumComponente.pir, PinNum, res.Value);
						break;
					case enumPirValue.off: // SPENGO
						//valore restituito
						res.Value = ((int)enumPirValue.off).ToString();
						res.Esito = true;
						res.Message = "PIR OFF";
						gpioPIN.ValueChanged -= Pir_FallingEdge_ValueChanged;
						gpioPIN.ValueChanged -= Pir_RisingEdge_ValueChanged;
						ActionNotify(res.Esito, res.Message, enumComponente.pir, PinNum, res.Value);
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

		private void Pir_FallingEdge_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			// se rilevato mando messaggio
			if (e.Edge == GpioPinEdge.FallingEdge)
				ActionNotify(true, "Pir Change", enumComponente.pir, sender.PinNumber, ((int)enumPirValue.signal).ToString());
		}
		private void Pir_RisingEdge_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			// se rilevato mando messaggio
			if (e.Edge == GpioPinEdge.RisingEdge)
				ActionNotify(true, "Pir Change", enumComponente.pir, sender.PinNumber, ((int)enumPirValue.signal).ToString());
		}

	}
}
