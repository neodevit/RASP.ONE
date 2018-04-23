using RaspaEntity;
using RaspaTools;
using Sensors;
using Sensors.Dht;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.Devices.SerialCommunication;
using Windows.UI.Xaml;

namespace RaspaAction
{
	public class PlatForm_Umidity : IPlatform
	{
		GpioPin gpioPIN = null;
		MQTT mqTT = null;
		RaspaProtocol Protocol;
		private IDht _dht = null;

		private Timer _timer = null;
		private int PinNumber=0;
		private PlatformNotify notify;
		enumTEMPOption hardware;

		public PlatForm_Umidity()
		{
		}

		public RaspaResult RUN(MQTT mqtt, GpioPin gpio, Dictionary<string, bool> EVENTS,RaspaProtocol protocol)
		{
			RaspaResult res = new RaspaResult(true, "");
			try
			{
				// controlli formali
				if (protocol.Comando != enumComando.comando)
					return new RaspaResult(false, "Platform deve eseguire solo comandi");

				// ripetitive timer
				_timer = new Timer(_ => Timer_Tick(), null,-1, Timeout.Infinite);

				// GPIO
				gpioPIN = gpio;

				// memorizzo il protocol
				Protocol = protocol;

				// Memorizzo MTQTT
				mqTT = mqtt;

				// istanzio notify
				notify = new PlatformNotify(mqTT);

				// Pin
				PinNumber = gpioPIN.PinNumber;

				#region Options
				hardware = (enumTEMPOption)Convert.ToInt32(Protocol.Destinatario.Options);
				switch (hardware)
				{
					case enumTEMPOption.dht11:
						_dht = new Dht11(gpioPIN, GpioPinDriveMode.Input);
						break;
					case enumTEMPOption.dht22:
						_dht = new Dht22(gpioPIN, GpioPinDriveMode.Input);
						break;
				}
				#endregion

				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumStato.off:
						// stop timer
						_timer?.Change(Timeout.Infinite, Timeout.Infinite);

						// dispose sensor
						_dht = null;

						// notify OFF
						notify.ActionNotify(Protocol,true, "Read ", enumSubribe.central, enumComponente.umidity, enumComando.notify, enumStato.off, PinNumber, new List<string>());

						break;
					case enumStato.value:
					case enumStato.read:
						// read value
						Timer_Tick();

						break;
					case enumStato.on:
					case enumStato.readRepetitive:
						// ------------------------------------------------------------
						// se value è un numero >0 allora indica ogni quanti secondi 
						// bisogna ripetere la lettura
						// ------------------------------------------------------------
						// start timer
						if (Protocol.RepetiteTime != null)
						{ 
							int tempo = Protocol.RepetiteTime.mm * 60 * 1000;
							_timer?.Change(0, tempo);
						}
						else
							Timer_Tick();

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

		private void Timer_Tick()
		{
			double Humidity = 0;
			bool isValid = false;
			try
			{
				switch(hardware)
				{
					case enumTEMPOption.dht11:
					case enumTEMPOption.dht22:
						DhtReading reading = read_BHT_SensorAsync().Result;
						if (reading.IsValid)
						{
							Humidity = reading.Humidity;
							isValid = true;
						}
						break;
				}

				// COMUNICA ESITO
				if (isValid)
				{
					List<string> resultU = new List<string>();
					resultU.Add(Humidity.ToString());
					notify.ActionNotify(Protocol, true, "Read Umidity", enumSubribe.central, enumComponente.umidity, enumComando.notify, enumStato.value, PinNumber, resultU);
				}
				else
				{
					notify.ActionNotify(Protocol, true, "Read Umidity", enumSubribe.central, enumComponente.umidity, enumComando.notify, enumStato.value, PinNumber, null);
				}

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - SET : " + ex.Message);
			}
		}

		private async Task<DhtReading> read_BHT_SensorAsync()
		{
			DhtReading reading = new DhtReading();
			int attemp = 0;
			try
			{
				// Loop finchè non legge un valore valido
				while (!reading.IsValid && attemp<5)
				{
					reading = await _dht.GetReadingAsync();
					attemp++;

					// wait random da 0 a 1 secondi
					Random rnd = new Random();
					int ms = rnd.Next(0, 1000);
					await Task.Delay(ms);
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - GET UMIDITY : " + ex.Message);
			}

			return reading;
		}

	}
}
