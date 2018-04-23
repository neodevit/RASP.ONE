using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public enum IANAtype
	{
		Eternet = 6,
		Wify = 71,
		Mobile_3g = 143,
	}
	public class RaspaInfo
	{
		public string Adapter { get; set; }
		public string Connection { get; set; }
		public string HostName { get; set; }

		public IANAtype NetworkType { get; set; }
		public string Network_IPv4 { get; set; }
		public string Network_IPv6 { get; set; }
		public string Network_Serial { get; set; }

		public string HW_ID { get; set; }
		public string OSVersion { get; set; }
		public string Productname { get; set; }
		public string RaspaVersion { get; set; }
	}
}
