using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Connectivity;

namespace RaspaTools
{
    public static partial class Tools
    {
		public static NetworkInfo GetDeviceNetwork()
		{
			NetworkInfo res = null;
			try
			{
				foreach (Windows.Networking.HostName name in Windows.Networking.Connectivity.NetworkInformation.GetHostNames())
				{
					switch (name.Type)
					{
						case Windows.Networking.HostNameType.DomainName:
							if (res == null)
								res = new NetworkInfo();
							if (!name.DisplayName.EndsWith("local")) 
								res.HostName = name.DisplayName;
							break;
						case Windows.Networking.HostNameType.Bluetooth:
							if (res == null)
								res = new NetworkInfo();
							res.BlueTooth = name.DisplayName;
							break;
						case Windows.Networking.HostNameType.Ipv4:
							if (res == null)
								res = new NetworkInfo();
							res.IPv4 = name.DisplayName;
							break;
						case Windows.Networking.HostNameType.Ipv6:
							if (res == null)
								res = new NetworkInfo();
							res.IPv6 = name.DisplayName;
							break;
					}
				}
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("TOOLS - NETWORK : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}



	}
}
