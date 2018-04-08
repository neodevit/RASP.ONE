using RaspaEntity;
using RaspaTools;
using Sensors.Dht;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace RaspaAction
{
	public class PlatForm_Temperature : IPlatform
	{
		public event Notifica ActionNotify;
		RaspaProtocol Protocol;
		private IDht _dht = null;
		private DispatcherTimer _timer;
		private bool _timerDelegate;
		private int PinNumber=0;
		public RaspaResult RUN(GpioPin gpioPIN, Dictionary<int, bool> EVENTS,RaspaProtocol protocol)
		{
			RaspaResult res = new RaspaResult(true, "");
			try
			{
				// controlli formali
				if (protocol.Comando != enumComando.comando)
					return new RaspaResult(false, "Platform deve eseguire solo comandi");

				// memorizzo il protocol
				Protocol = protocol;

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
						if (_timer != null)
							_timer.Stop();

						// dispose sensor
						_dht = null;
						ActionNotify?.Invoke(true, "Read Temperature", enumSubribe.central, enumComponente.temperature, enumComando.notify, enumAzione.off, PinNumber, "");

						break;
					case enumAzione.read:
						// Istanzia sensor
						if (tipo == enumTEMPOption.dht11)
							_dht = new Dht11(gpioPIN, GpioPinDriveMode.Input);
						else
							_dht = new Dht22(gpioPIN, GpioPinDriveMode.Input);

						// read value
						var task = readSensorAsync();
						notifySensor(task.Result);

						break;
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
						_timer = new DispatcherTimer();
						_timer.Interval = TimeSpan.FromSeconds(Protocol.RepetiteTime.mm);
						if (!_timerDelegate)
						{
							_timer.Tick += this.Timer_Tick;
							_timerDelegate = true;
						}
						// leggi subito il dato
						var taskRep = readSensorAsync();
						notifySensor(taskRep.Result);

						// aspetta un secondo
						Task.Delay(1000);

						// fai partire il timer
						_timer.Start();
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

		private void Timer_Tick(object sender, object e)
		{
			var taskRep = readSensorAsync();
			notifySensor(taskRep.Result);
		}

		private void readData()
		{
			try
			{

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - GET TEMPERATURE : " + ex.Message);
			}
		}
		private async Task<resTempUmidity> readSensorAsync()
		{
			resTempUmidity sensorRes = null;
			int attemp = 0;
			try
			{
				// Loop finchè non legge un valore valido
				DhtReading reading = new DhtReading();
				while (!reading.IsValid && attemp<1000)
				{
					reading = await _dht.GetReadingAsync();
					attemp++;
				}

				// se sono uscito dal while
				if (reading.IsValid)
				{
					sensorRes = new resTempUmidity();
					sensorRes.temperature = Convert.ToDecimal(reading.Temperature);
					sensorRes.umidity = Convert.ToDecimal(reading.Humidity);
				}

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - GET TEMPERATURE : " + ex.Message);
			}

			return sensorRes;
		}
		private void notifySensor(resTempUmidity sensorRes)
		{
			try
			{
				if (sensorRes != null)
				{
					Notifica handler = ActionNotify;

					if (Protocol.Destinatario.Tipo == enumComponente.temperature || Protocol.Destinatario.Tipo == enumComponente.temperatureAndumidity)
						this.ActionNotify?.Invoke(true, "Read Temperature", enumSubribe.central, enumComponente.temperature, enumComando.notify, enumAzione.value, PinNumber, sensorRes.temperature.ToString());


					if (Protocol.Destinatario.Tipo == enumComponente.umidity || Protocol.Destinatario.Tipo == enumComponente.temperatureAndumidity)
						this.ActionNotify?.Invoke(true, "Read Humidity", enumSubribe.central, enumComponente.umidity, enumComando.notify, enumAzione.value, PinNumber, sensorRes.umidity.ToString());
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - GET TEMPERATURE : " + ex.Message);
			}
		}

	}
	public class resTempUmidity
	{
		public decimal temperature { get; set; }
		public decimal umidity { get; set; }

	}
}
