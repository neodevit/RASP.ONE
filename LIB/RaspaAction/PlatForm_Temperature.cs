using RaspaEntity;
using RaspaTools;
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
using Windows.UI.Xaml;

namespace RaspaAction
{
	public class PlatForm_Temperature : IPlatform
	{
		GpioPin gpioPIN = null;
		MQTT mqTT = null;
		RaspaProtocol Protocol;
		private IDht _dht = null;
		private Timer _timer = null;
		private int PinNumber=0;
		private PlatformNotify notify;

		public PlatForm_Temperature()
		{
		}

		public RaspaResult RUN(MQTT mqtt, GpioPin gpio, Dictionary<int, bool> EVENTS,RaspaProtocol protocol)
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
				enumTEMPOption tipo = (enumTEMPOption)Convert.ToInt32(Protocol.Destinatario.Options);
				if (tipo == enumTEMPOption.dht11)
					_dht = new Dht11(gpioPIN, GpioPinDriveMode.Input);
				else
					_dht = new Dht22(gpioPIN, GpioPinDriveMode.Input);
				#endregion

				//-------------------
				// SCEGLI AZIONE
				//-------------------
				switch (Protocol.Azione)
				{
					case enumAzione.off:
						// stop timer
						_timer?.Change(Timeout.Infinite, Timeout.Infinite);

						// dispose sensor
						_dht = null;
						notify.ActionNotify(Protocol,true, "Read Temperature", enumSubribe.central, enumComponente.temperature, enumComando.notify, enumAzione.off, PinNumber, new List<string>());

						break;
					case enumAzione.value:
					case enumAzione.read:
						// Istanzia sensor
						if (tipo == enumTEMPOption.dht11)
							_dht = new Dht11(gpioPIN, GpioPinDriveMode.Input);
						else
							_dht = new Dht22(gpioPIN, GpioPinDriveMode.Input);

						// read value
						Timer_Tick();

						break;
					case enumAzione.on:
					case enumAzione.readRepetitive:
						// ------------------------------------------------------------
						// se value è un numero >0 allora indica ogni quanti secondi 
						// bisogna ripetere la lettura
						// ------------------------------------------------------------
						// Istanzia sensor
						if (tipo == enumTEMPOption.dht11)
							_dht = new Dht11(gpioPIN, GpioPinDriveMode.Input);
						else
							_dht = new Dht22(gpioPIN, GpioPinDriveMode.Input);


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
			double Temperature = 0;
			double Humidity = 0;

			//if (Monitor.TryEnter(_dht))
			//{
				try
				{
					DhtReading reading = readSensorAsync().Result;

					if (reading.IsValid)
					{
						Temperature = reading.Temperature;
						Humidity = reading.Humidity;

						List<string> result = new List<string>();
						result.Add(Temperature.ToString());
						result.Add(Humidity.ToString());

						notify.ActionNotify(Protocol, true, "Read Temperature", enumSubribe.central, enumComponente.temperature, enumComando.notify, enumAzione.value, PinNumber, result);

					}
				}
				catch (Exception ex)
				{
					if (Debugger.IsAttached) Debugger.Break();
					System.Diagnostics.Debug.WriteLine("SASSO API TEST - SET : " + ex.Message);
				}
			//	finally
			//	{
			//		Monitor.Exit(_dht);
			//	}
			//}
		}

		private async Task<DhtReading> readSensorAsync()
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
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - GET TEMPERATURE : " + ex.Message);
			}

			return reading;
		}

	}
}
