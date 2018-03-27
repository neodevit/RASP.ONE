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
		bool firstTime = false;
		public RaspaResult RUN(GpioPin gpioPIN,RaspaProtocol Protocol)
		{
			RaspaResult res = new RaspaResult(false, "NA");
			try
			{
				GpioPinEdge edge = (Protocol.Destinatario.Options == "0") ? GpioPinEdge.FallingEdge : GpioPinEdge.RisingEdge;


				switch(Protocol.Value)
				{
					case "1": // ACCENDO
						firstTime = true; // sto accendendo non voglio una falsa segnalazione di presenza
						res.Value = "1";
						res.Esito = true;
						res.Message = "PIR ON";

						gpioPIN.ValueChanged -= Pir_FallingEdge_ValueChanged;
						gpioPIN.ValueChanged -= Pir_RisingEdge_ValueChanged;
						if (edge == GpioPinEdge.FallingEdge)
							gpioPIN.ValueChanged += Pir_FallingEdge_ValueChanged;
						else
							gpioPIN.ValueChanged += Pir_RisingEdge_ValueChanged;

						gpioPIN.SetDriveMode(GpioPinDriveMode.Input);
						ActionNotify(res.Esito, res.Message, enumComponente.pir, gpioPIN.PinNumber, res.Value);
						break;
					case "0": // SPENGO
						firstTime = true; // sto spegnendo non voglio una falsa segnalazione di presenza
						//valore restituito
						res.Value = "0";
						res.Esito = true;
						res.Message = "PIR OFF";
						gpioPIN.ValueChanged -= Pir_FallingEdge_ValueChanged;
						gpioPIN.ValueChanged -= Pir_RisingEdge_ValueChanged;
						ActionNotify(res.Esito, res.Message, enumComponente.pir, gpioPIN.PinNumber, res.Value);
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
			if (e.Edge == GpioPinEdge.FallingEdge && firstTime==false)
				ActionNotify(true, "Pir Change", enumComponente.pir, sender.PinNumber, "2");
			firstTime = false;
		}
		private void Pir_RisingEdge_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs e)
		{
			// se rilevato mando messaggio
			if (e.Edge == GpioPinEdge.RisingEdge && firstTime == false)
				ActionNotify(true, "Pir Change", enumComponente.pir, sender.PinNumber, "2");
			firstTime = false;
		}

	}
}
