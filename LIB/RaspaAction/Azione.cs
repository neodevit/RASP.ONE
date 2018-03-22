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
	public delegate void ActionNotify(bool Esito,string Messaggio,enumComponente componente, int pin, int value);
	public class Azione
	{
		public event ActionNotify ActionNotify;

		GpioController GPIO = null;
		Dictionary<int,GpioPin> PIN = null;
		UDP udpRoot;
		RaspaProtocol Protocol;
		public Azione(UDP udp,GpioController gpio, Dictionary<int, GpioPin> pin)
		{
			// ASSIGN
			udpRoot = udp;
			GPIO = gpio;
			PIN = pin;

		}
		public RaspaResult Execute(RaspaProtocol Message)
		{
			IPlatform Platform;
			RaspaResult res = new RaspaResult(true);
			Protocol = Message;
			try
			{
				//-----------------------------------------
				// PREPARA INPUT
				//-----------------------------------------
				// PIN
				int pin = Convert.ToInt32(Protocol.Componente.Pin);
				if (pin == 0)
					return new RaspaResult(false, "PIN zero", "");
				// value
				int value = Convert.ToInt32(Protocol.Componente.Value);
				// EDGE
				GpioPinEdge? edge = Protocol.Componente.Edge;
				// GPIO
				if (PIN == null || !PIN.ContainsKey(pin))
					return new RaspaResult(false, "PIN " + pin + " invalid", "");
				// Prendo il pin
				GpioPin gpioPIN = GetPIN(pin);
				if (gpioPIN==null)
					return new RaspaResult(false, "GPIO PIN " + pin + " non inizializzate", "");

				//-----------------------------------------
				// AZIONE
				//-----------------------------------------
				switch (Protocol.Comando)
				{
					case enumComando.notify:
						break;
					case enumComando.get:
						Platform = new PlatForm_GET();
						Platform.ActionNotify -= PlatformNotify;
						Platform.ActionNotify += PlatformNotify;
						res = Platform.RUN(gpioPIN, edge, value);
						break;
					case enumComando.set:
						Platform = new PlatForm_SET();
						Platform.ActionNotify -= PlatformNotify;
						Platform.ActionNotify += PlatformNotify;
						res = Platform.RUN(gpioPIN, edge, value);
						break;
					case enumComando.comando:
						switch(Protocol.Componente.Tipo)
						{
							case enumComponente.light:
								Platform = new PlatForm_Light();
								Platform.ActionNotify -= PlatformNotify;
								Platform.ActionNotify += PlatformNotify;
								res = Platform.RUN(gpioPIN, edge, value);
								break;
							case enumComponente.pir:
								Platform = new PlatForm_PIR();
								Platform.ActionNotify -= PlatformNotify;
								Platform.ActionNotify += PlatformNotify;
								res = Platform.RUN(gpioPIN, edge, value);
								break;
							default:
								res = new RaspaResult(false, "COMPONENTE : " + Protocol.Componente.Tipo.ToString() + " non esistente", "");
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
			return res;
		}

		private void PlatformNotify(bool Esito, string Messaggio, enumComponente componente, int pin, int value)
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
