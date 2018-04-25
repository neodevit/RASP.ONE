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
		sound_basketball_buzzer = 0,
		sound_banana_peel_slip  = 1,
		sound_Bleep = 2,
		sound_Beep=3,
		sound_bells=4,
		sound_Click_Button=5,
		sound_sms_alert_1=6,
		sound_sms_alert_2=7,
		sound_Sniff=8,
		sound_Time=9,
		sound_Woop = 10,
		sound_Bike_Horn=11,
		sound_Computer_Error_Alert=12,
		sound_Cuckoo_Clock=13,
		sound_Metronome=14,
		sound_Large_Metal_Pan = 15,
		sound_Torch=16,


		sound_Buzz = 100,
		sound_Buzz_Fade_In = 101,
		sound_Buzz_Fade_Out =102,
		sound_Doorbell=103,
		sound_Buzz2 =104,
		sound_Door_Buzzer=105,
		sound_old_fashioned_door_bell=106,
		sound_Door_Chime=107,
		sound_old_fashioned_school_bell=108,
		sound_Rooster=109,
		sound_dixie_horn=110,
		sound_Yahoo=111,

		sound_alarm_clock = 200,
		sound_Police=201,
		sound_Smoke_Alarm=202,
		sound_Phone_Vibrating=203,
		sound_Siren=204,
	}




	public enum enumComando
	{
		nessuno = 0,
		notify  = 1,
		comando = 4,
		nodeInit=5,
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
		hearbit =3,         
		reload =4,			// only one in network
	}
}
