using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaAction
{
	public class PlatformNotify
	{
		MQTT mqTT;
		public PlatformNotify(MQTT mqtt)
		{
			mqTT = mqtt;
		}
		public void ActionNotify(RaspaProtocol Original, bool Esito, string Messaggio, enumSubribe subscribe, enumComponente componente, enumComando comando, enumAzione azione, int pin)
		{
			try
			{
				ActionNotify(Original, Esito, Messaggio, subscribe, componente, comando, azione, pin, new List<string>());
			}
			catch { }
		}
		public void ActionNotify(RaspaProtocol Original,bool Esito, string Messaggio, enumSubribe subscribe, enumComponente componente, enumComando comando, enumAzione azione, int pin, List<string> value )
		{
			RaspaProtocol Protocol = new RaspaProtocol();
			try
			{
				// se sono messaggi che si automanda il nodo in fase actual allora esco
				if (Original.Mittente == null ||
					Original.Destinatario == null)
					return;

				// prepara il messaggio da inviare
				Protocol.Mittente = Original.Destinatario;
				Protocol.Destinatario = Original.Mittente;
				Protocol.SubcribeDestination = subscribe;
				Protocol.SubcribeResponse = enumSubribe.IPv4;
				Protocol.Comando = comando;
				Protocol.Azione = azione;
				Protocol.Esito = Esito;
				Protocol.Message = Messaggio;
				Protocol.Value = value ?? new List<string>();
				mqTT.Publish(Protocol);

			}
			catch { }
		}

	}
}
