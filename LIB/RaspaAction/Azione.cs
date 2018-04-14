using RaspaDB;
using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.UI.Core;

namespace RaspaAction
{
	public class Azione
	{
		RaspaProtocol Protocol;
		GpioController GPIO = null;
		MQTT mqTT = null;

		Dictionary<int,GpioPin> PIN = null;
		Dictionary<int,bool> platform_EVENTS = null;
		Dictionary<int,IPlatform> platform_Engine = null;
		IPlatform Platform = null;

		public Azione(MQTT mqtt,GpioController gpio, Dictionary<int, GpioPin> pin, Dictionary<int, IPlatform> platform_engine, Dictionary<int, bool> platform_events)
		{
			// ASSIGN
			GPIO = gpio;
			PIN = pin;
			mqTT = mqtt;

			platform_Engine = platform_engine;
			platform_EVENTS = platform_events;
		}

		public void Execute(RaspaProtocol protocol)
		{
			GpioPin gpioPIN=null;
			RaspaResult res = new RaspaResult(true);
			try
			{
				Protocol = protocol;

				//-----------------------------------------
				// PREPARA INPUT
				//-----------------------------------------
				// PIN
				int pin = Convert.ToInt32(Protocol.Destinatario.Node_Pin);
				if (pin == 0)
					return;
				// GPIO
				if (PIN == null || !PIN.ContainsKey(pin))
					return;

				//-----------------------------------------
				// PLATFORM
				//-----------------------------------------
				if (!platform_Engine.ContainsKey(pin) || platform_Engine[pin] == null)
				{
					// CHOSE PLATFORM
					switch (Protocol.Destinatario.Tipo)
					{
						case enumComponente.light:
							Platform = new PlatForm_Light();
							break;
						case enumComponente.pir:
							Platform = new PlatForm_PIR();
							break;
						case enumComponente.bell:
							Platform = new PlatForm_Bell();
							break;
						case enumComponente.temperature:
						case enumComponente.umidity:
						case enumComponente.temperatureAndumidity:
							Platform = new PlatForm_Temperature();
							break;
					}

					// ADD PLATFORM
					if (Platform != null)
						platform_Engine.Add(pin, Platform);
				}
				else
				{
					// REUSE PLATFORM
					Platform = platform_Engine[pin];
				}

				//-----------------------------------------
				// PIN
				//-----------------------------------------
				switch (Protocol.Comando)
				{
					case enumComando.nodeInit:
						break;
					case enumComando.nodeReload:
						break;
					case enumComando.notify:
						break;
					case enumComando.comando:

						switch (Protocol.Destinatario.Tipo)
						{
							case enumComponente.light:
								// Prendo il pin
								gpioPIN = GetPIN(pin);
								if (gpioPIN == null)
								{
									if (Debugger.IsAttached) Debugger.Break();
									return;
								}
								break;
							case enumComponente.pir:
								// Prendo il pin
								gpioPIN = GetPIN(pin);
								if (gpioPIN == null)
								{
									if (Debugger.IsAttached) Debugger.Break();
									return;
								}
								break;
							case enumComponente.bell:
								// Prendo il pin
								gpioPIN = GetPIN(pin);
								if (gpioPIN == null)
								{
									if (Debugger.IsAttached) Debugger.Break();
									return;
								}
								break;

							case enumComponente.temperatureAndumidity:
							case enumComponente.temperature:
							case enumComponente.umidity:
								// Prendo il pin
								gpioPIN = GetPIN(pin,null, GpioSharingMode.Exclusive);
								if (gpioPIN == null)
								{
									if (Debugger.IsAttached) Debugger.Break();
									return;
								}
								break;
							default:
								res = new RaspaResult(false, "COMPONENTE : " + Protocol.Destinatario.Tipo.ToString() + " non esistente", "");
								break;
						}
						break;
					default:
						res =  new RaspaResult(false, "COMANDO : " + Protocol.Comando.ToString() + " non esistente", "");
						break;
				}

				//-----------------------------------------
				// EXECUTE
				//-----------------------------------------
				if (gpioPIN != null)
					res = Platform.RUN(mqTT, gpioPIN, platform_EVENTS, Protocol);
				else
				{
					if (Debugger.IsAttached) Debugger.Break();
					return;
				}

			}
			catch (Exception ex)
			{
				res = new RaspaResult(false,ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - EXECUTE : " + ex.Message);
			}
		}

		#region GET PIN
		// GET PIN con RETRY
		private GpioPin GetPIN(int pin, GpioPinValue? InitValue = GpioPinValue.High, GpioSharingMode? sharingMode = GpioSharingMode.SharedReadOnly)
		{
			GpioPin res=null;
			int attemp = 0;
			try
			{
				// Loop finchè non legge un valore valido
				while (res==null && attemp < 5)
				{
					res = GetPIN_single(pin, InitValue, sharingMode);
					attemp++;

					// wait random da 0 a 1 secondi
					Random rnd = new Random();
					int ms = rnd.Next(0, 1000);
					Task.Delay(ms);
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - PIN VALUE CHANGE : " + ex.Message);
			}
			return res;
		}
		// get PIN
		private GpioPin GetPIN_single(int pin, GpioPinValue? InitValue = GpioPinValue.High, GpioSharingMode? sharingMode = GpioSharingMode.SharedReadOnly)
		{
			GpioPin res = null;
			try
			{
				if (PIN.ContainsKey(pin))
				{
					if (PIN[pin] == null)
					{
						// open pin
						PIN[pin] = GPIO.OpenPin(pin);



						// init value
						if (InitValue.HasValue)
						{
							PIN[pin].Write(InitValue.Value);
							PIN[pin].SetDriveMode(GpioPinDriveMode.Output);
						}

						if (sharingMode.HasValue)
						{
							PIN[pin] = GPIO.OpenPin(pin, sharingMode.Value);
						}
					}

					res = PIN[pin];
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - GET PIN : " + ex.Message);
			}
			return res;
		}
		#endregion

	}
}
