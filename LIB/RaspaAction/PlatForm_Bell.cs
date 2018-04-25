using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;

namespace RaspaAction
{
	public class PlatForm_Bell : IPlatform
	{
		MQTT mqTT = null;
		private RaspaProtocol Protocol;
		private PlatformNotify notify;

		public PlatForm_Bell()
		{
		}

		public RaspaResult RUN(MQTT mqtt, GpioPin gpio, Dictionary<string, bool> EVENTS,RaspaProtocol protocol)
		{
			RaspaResult res = new RaspaResult(false, "NA");
			try
			{

				// controlli formali
				if (protocol.Comando != enumComando.comando)
					return new RaspaResult(false, "Platform deve eseguire solo comandi");

				// memorizzo il protocol
				Protocol = protocol;

				// Memorizzo MTQTT
				mqTT = mqtt;

				// istanzio notify
				notify = new PlatformNotify(mqTT);

				#region OPTIONS
				enumBellOption option = (enumBellOption)Convert.ToInt32(Protocol.Destinatario.Options);
				#endregion

				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumStato.nessuno:
					case enumStato.signal:

						var mediaElement = new MediaElement();
						mediaElement.Source = new Uri("ms-appx:///Assets/"+ option.ToString() + ".mp3");
						mediaElement.Play();

						// restiutuisci esito
						notify.ActionNotify(Protocol, true, "Push Button Read", enumSubribe.central, enumComponente.bell, enumComando.notify, enumStato.signalOFF, 0);
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



	}
}
