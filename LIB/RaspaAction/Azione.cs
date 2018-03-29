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
	public delegate void ActionNotify(bool Esito,string Message, enumComponente componente,int Pin,string Value);
	public class Azione
	{
		public event ActionNotify ActionNotify;

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
		public void Execute(RaspaProtocol Protocol)
		{
			IPlatform Platform;
			RaspaResult res = new RaspaResult(true);
			try
			{
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
				// Prendo il pin
				GpioPin gpioPIN = GetPIN(pin);
				if (gpioPIN==null)
					return;

				//-----------------------------------------
				// AZIONE
				//-----------------------------------------
				switch (Protocol.Comando)
				{
					case enumComando.notify:
						break;
					case enumComando.get:
						Platform = new PlatForm_GET();
						if (!action_EVENTS.ContainsKey(gpioPIN.PinNumber) || !action_EVENTS[gpioPIN.PinNumber])
						{
							Platform.ActionNotify -= PlatformNotify;
							Platform.ActionNotify += PlatformNotify;
							// memorizzo che ho già impostato evento
							action_EVENTS[gpioPIN.PinNumber] = true;
						}
						res = Platform.RUN(gpioPIN, platform_EVENTS, Protocol);
						break;
					case enumComando.set:
						Platform = new PlatForm_SET();
						if (!action_EVENTS.ContainsKey(gpioPIN.PinNumber) || !action_EVENTS[gpioPIN.PinNumber])
						{
							Platform.ActionNotify -= PlatformNotify;
							Platform.ActionNotify += PlatformNotify;
							// memorizzo che ho già impostato evento
							action_EVENTS[gpioPIN.PinNumber] = true;
						}
						res = Platform.RUN(gpioPIN, platform_EVENTS, Protocol);
						break;
					case enumComando.comando:
						switch(Protocol.Destinatario.Tipo)
						{
							case enumComponente.light:
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

		private void PlatformNotify(bool Esito, string Messaggio, enumComponente componente, int pin, string value)
		{
			try
			{
				ActionNotify(Esito, Messaggio, componente, pin, value);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - PIN VALUE CHANGE : " + ex.Message);
			}
		}

		private GpioPin GetPIN(int pin, GpioPinValue? InitValue = GpioPinValue.High)
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
					}

					res = PIN[pin];
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("SASSO API TEST - GET PIN : " + ex.Message);
			}
			return res;
		}


	}
}
