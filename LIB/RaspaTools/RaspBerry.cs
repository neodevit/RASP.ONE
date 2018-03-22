using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Connectivity;

namespace RaspaTools
{
    public partial class RaspBerry
    {

		//----------
		// SHOOTDOWN
		//----------
		public async void ShutdownComputer()
		{
			try
			{
				String URL = "http://localhost:8080/api/control/shutdown";
				System.Diagnostics.Debug.WriteLine(URL);
				//StreamReader SR = await PostJsonStreamData(URL);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("RASPBERRY - SHUTDOWN : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
		}

		//----------
		// REBOOT
		//----------
		public async void RebootComputer()
		{
			try
			{
				String URL = "http://localhost:8080/api/control/reboot";
				System.Diagnostics.Debug.WriteLine(URL);
				//StreamReader SR = await PostJsonStreamData(URL);
			}catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine("RASPBERRY - REBOOT : " + ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
			}
		}





		// ------------
		// IP
		// ------------
		public string GetLocalIPv4()
		{
			var icp = NetworkInformation.GetInternetConnectionProfile();

			if (icp?.NetworkAdapter == null) return null;
			var hostname =
				NetworkInformation.GetHostNames()
					.SingleOrDefault(
						hn =>
							hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
							== icp.NetworkAdapter.NetworkAdapterId);

			// the ip address
			return hostname?.CanonicalName;
		}
	}
	
}
