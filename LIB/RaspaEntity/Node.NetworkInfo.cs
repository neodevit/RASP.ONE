using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public class NetworkInfo
	{
		// comando per cambiare host name : setcomputername SASSO_NODE
		public string HostName { get; set; }
		public string BlueTooth { get; set; }
		public string MacAddress { get; set; }
		public string HWAddress { get; set; }
		public string IPv4 {get; set; }
		public string IPv6 { get; set; }

	}
}
