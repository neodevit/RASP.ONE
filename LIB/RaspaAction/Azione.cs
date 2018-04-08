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
	public delegate void Notifica(bool Esito,string Message, enumSubribe subscribe, enumComponente componente, enumComando comando, enumAzione azione,int Pin,string Value);

	public class Azione
	{
		public event Notifica ActionNotify;

		GpioController GPIO = null;
		Dictionary<int,GpioPin> PIN = null;
		Dictionary<int,bool> action_EVENTS = null;
		Dictionary<int,bool> platform_EVENTS = null;
		RaspaProtocol Protocol;
		public Azione(GpioController gpio, Dictionary<int, GpioPin> pin, Dictionary<int, bool> action_events, Dictionary<int, bool> platform_events)
		{
			// ASSIGN
			GPIO = gpio;
			PIN = pin;
			action_EVENTS = action_events;
			platform_EVENTS = platform_events;
		}
		public void Execute(RaspaProtocol protocol)
		{
			GpioPin gpioPIN;
			IPlatform Platform;
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
				// AZIONE
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

								Platform = new PlatForm_Light();
								if (!action_EVENTS.ContainsKey(gpioPIN.PinNumber) || !action_EVENTS[gpioPIN.PinNumber])
								{
									Platform.ActionNotify -= PlatformNotify;
									Platform.ActionNotify += PlatformNotify;
									// memorizzo che ho già impostato evento
									action_EVENTS[gpioPIN.PinNumber] = true;
								}
								res = Platform.RUN(gpioPIN, platform_EVENTS, Protocol);
								break;
							case enumComponente.pir:
								// Prendo il pin
								gpioPIN = GetPIN(pin);
								if (gpioPIN == null)
								{
									if (Debugger.IsAttached) Debugger.Break();
									return;
								}

								Platform = new PlatForm_PIR();
								if (!action_EVENTS.ContainsKey(gpioPIN.PinNumber) || !action_EVENTS[gpioPIN.PinNumber])
								{
									Platform.ActionNotify -= PlatformNotify;
									Platform.ActionNotify += PlatformNotify;
									// memorizzo che ho già impostato evento
									action_EVENTS[gpioPIN.PinNumber] = true;
								}
								res = Platform.RUN(gpioPIN, platform_EVENTS, Protocol);
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

								Platform = new PlatForm_Temperature();
								if (!action_EVENTS.ContainsKey(gpioPIN.PinNumber) || !action_EVENTS[gpioPIN.PinNumber])
								{
									Platform.ActionNotify -= PlatformNotify;
									Platform.ActionNotify += PlatformNotify;
									// memorizzo che ho già impostato evento
									action_EVENTS[gpioPIN.PinNumber] = false; // faccio in modo che con FALSE ridichiara sempre l'evento se così funziona bisogna togliere che l'evento lo dichiara solo 1 volta ma dichiararlo sempre
								}
								res = Platform.RUN(gpioPIN, platform_EVENTS, Protocol);
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
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false,ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - EXECUTE : " + ex.Message);
			}
		}





		private void PlatformNotify(bool Esito, string Messaggio, enumSubribe subscribe, enumComponente componente, enumComando comando, enumAzione azione, int pin, string value)
		{
			try
			{
				ActionNotify(Esito, Messaggio,subscribe, componente, comando, azione, pin, value);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - PIN VALUE CHANGE : " + ex.Message);
			}
		}


		#region GET PIN
		// GET PIN con RETRY
		private GpioPin GetPIN(int pin, GpioPinValue? InitValue = GpioPinValue.High, GpioSharingMode? sharingMode = GpioSharingMode.SharedReadOnly)
		{
			GpioPin res=null;
			try
			{
				for (int i = 0; i <= 5; i++)
				{
					res = GetPIN_single(pin, InitValue, sharingMode);
					if (res == null)
					{
						// wait random da 1 a 3 secondi
						Random rnd = new Random();
						int ms = rnd.Next(1, 3000);
						Task.Delay(ms);
					}
					else
						break;
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
