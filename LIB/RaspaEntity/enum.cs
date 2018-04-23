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
