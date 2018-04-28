	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

namespace RaspaEntity
{
	public enum enumPIROption
	{
		nessuno = 0,
		FallingEdge = 1,
		RisingEdge = 2,
	}

	public enum enumPINOptionIsON
	{
		nessuno = 0,
		low = 1,
		hight = 2,
	}

	public enum enumTEMPOption
	{
		dht11 = 0,
		dht22 = 1,
	}

	public enum enumBellOption
	{
		signal_basketball = 0,
		signal_peel = 1,
		signal_Bleep = 2,
		signal_Beep = 3,
		signal_bells = 4,
		signal_Click_Button = 5,
		signal_sms_alert_1 = 6,
		signal_sms_alert_2 = 7,
		signal_Sniff = 8,
		signal_Time = 9,
		signal_Computer_Error_Alert = 10,
		signal_Cuckoo_Clock = 11,
		signal_Metronome = 12,
		signal_Large_Metal_Pan = 13,
		signal_Yahoo = 14,
		signal_Vibrating = 15,


		bell_Bike_Horn = 50,
		bell_Buzz = 51,
		bell_Buzz_Fade_In = 52,
		bell_Buzz_Fade_Out = 53,
		bell_Doorbell = 54,
		bell_Door_Buzzer = 55,
		bell_old_fashioned_door_bell = 56,
		bell_Door_Chime = 57,
		bell_old_fashioned_school_bell = 58,
		bell_dixie_horn = 59,

		alarm_Woop = 100,
		alarm_Buzz2 = 101,
		alarm_clock = 102,
		alarm_Police = 103,
		alarm_Smoke = 104,
		alarm_Siren = 105,
		alarm_Torch = 106,
		alarm_Rooster = 107,
	}
	public class ChiaveValore
	{
		public ChiaveValore(string chiave,string valore)
		{
			Chiave = chiave;
			Valore = valore;
		}
		public string Chiave { get; set; }
		public string Valore { get; set; }
	}




	public enum enumComando
	{
		nessuno = 0,
		notify  = 1,
		comando = 4,
		nodeSaveConfig=5,
		nodeReload = 6,
	}
	public enum enumStato
	{
		nessuno = 0,
		on = 1,
		off = 2,
		read = 3,
		readRepetitive = 4,
		write = 5,
		writeRepetitive=6,
		signal=7,
		value=8,
		errore=9,
		signalOFF=10,
	}

	// Allineare ENUM con tabella 41_COMPONENTE_TIPO
	public enum enumComponente
	{
		nessuno = 0,
		light = 1,
		pir = 2,
		nodo = 3,
		centrale = 4,
		webcam_ip=5,
		webcam_rasp=6,
		temperature = 7,
		umidity=8,
		push=9,
		bell=10,
		moisture=11,
	}


	public enum enumTipoPIN
	{
		gpio = 0,
		power = 1,
		ground = 2,
		other = 3,
		gpioAndOther = 4,
	}

	public enum enumPinPullType
	{
		PullUp = 0,
		PullDown = 1,
	}
	public enum enumLevel
	{
		nessuno = 0,
		info = 1,
		error = 2,
		critical = 3,
		failed = 4,
		warning = 5,
		alert = 6,
		conflitto = 7,
	}

	public enum enumSubribe
	{
		IPv4=0,
		central=1,
		rules=2,            // only one in network
		heartbit =3,         
		nodeINIT =4,		// only one in network
	}
}
