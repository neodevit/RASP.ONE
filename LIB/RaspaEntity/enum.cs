	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

namespace RaspaEntity
{
	public enum enumPirValue
	{
		nessuno = 0,
		off = 1,
		on = 2,
		signal = 3,
		err = 4,
	}
	public enum enumPIROption
	{
		nessuno = 0,
		FallingEdge = 1,
		RisingEdge = 2,
	}

	public enum enumPINValue
	{
		nessuno = 0,
		off = 1,
		on = 2,
		err = 3,
	}
	public enum enumPINOptionIsON
	{
		nessuno = 0,
		low = 1,
		hight = 2,
	}
	public enum enumStato
	{
		nessuno=0,
		off = 1,
		on = 2,
	}
	public enum enumComando
	{
		nessuno = 0,
		notify  = 1,
		get  = 2,
		set = 3,
		comando = 4,
		nodeInit=5,
		nodeReload = 6,
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
	}
	public enum enumAzione
	{
		nessuno = 0,
		get = 1,
		set = 2,
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
}
