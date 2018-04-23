using Microsoft.Toolkit.Uwp.Helpers;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources;
using Windows.Data.Json;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage.Streams;
using Windows.System.Profile;

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


		// -----------------
		// SET CURRENT DATA
		// -----------------
		#region SYNC DATE TIME
		public async void SyncDateTime()
		{
			var socket = new DatagramSocket();
			socket.MessageReceived += SyncDateTime_SocketMessageReceived;
			await socket.ConnectAsync(new HostName("time.windows.com"), "123");

			using (var dataWriter = new DataWriter(socket.OutputStream))
			{
				var ntpData = new byte[48];
				ntpData[0] = 0x1B;
				dataWriter.WriteBytes(ntpData);
				await dataWriter.StoreAsync();
			}
		}
		private void SyncDateTime_SocketMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
		{
			try
			{
				using (var reader = args.GetDataReader())
				{
					byte[] response = new byte[48];
					reader.ReadBytes(response);

					ulong intPart = (ulong)response[40] << 24 | (ulong)response[41] << 16 | (ulong)response[42] << 8 | (ulong)response[43];
					ulong fractPart = (ulong)response[44] << 24 | (ulong)response[45] << 16 | (ulong)response[46] << 8 | (ulong)response[47];

					var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
					DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);


					SyncDateTime_SetCurrentDate(networkDateTime);
				}
			}
			catch (Exception ex)
			{
				// Handle exceptions
			}
		}
		private void SyncDateTime_SetCurrentDate(DateTime Data)
		{
			DateTime romaTime = TimeZoneInfo.ConvertTime(Data, TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"));
			Windows.System.DateTimeSettings.SetSystemDateTime(romaTime);
		}
		#endregion


		// ------------
		// IP
		// ------------
		public RaspaInfo GetRaspInfo()
		{
			RaspaInfo res = null;


			res = new RaspaInfo();

			GetCurrentIpv4Address(res);

			res.HostName = GetCurrentHostName();
			res.OSVersion = GetOSVersion();
			res.Productname = SystemProductName();
			res.RaspaVersion = GetRASPANodeVersion();
			res.HW_ID = GetHardwareID();

			return res;

		}

		private void GetCurrentIpv4Address(RaspaInfo info)
		{
			var icp = NetworkInformation.GetInternetConnectionProfile();
			if (icp != null
				  && icp.NetworkAdapter != null
				  && icp.NetworkAdapter.NetworkAdapterId != null)
			{
				var name = icp.ProfileName;
				info.NetworkType = (IANAtype)icp.NetworkAdapter.IanaInterfaceType;

				var hostnames = NetworkInformation.GetHostNames();
				foreach (var hn in hostnames)
				{
					if (hn.IPInformation != null
						&& hn.IPInformation.NetworkAdapter != null
						&& hn.IPInformation.NetworkAdapter.NetworkAdapterId
																   != null
						&& hn.IPInformation.NetworkAdapter.NetworkAdapterId
									== icp.NetworkAdapter.NetworkAdapterId
						)
					{
						if (hn.Type == HostNameType.Ipv4)
						{
							info.Network_IPv4 = hn.CanonicalName;
							info.Network_Serial = icp.NetworkAdapter.NetworkAdapterId.ToString();
						}
						if (hn.Type == HostNameType.Ipv6)
							info.Network_IPv6 = hn.CanonicalName;

					}
				}
			}

		}
		private string GetCurrentNetworkName()
		{
			var icp = NetworkInformation.GetInternetConnectionProfile();
			if (icp != null)
			{
				return icp.ProfileName;
			}

			var resourceLoader = ResourceLoader.GetForCurrentView();
			var msg = resourceLoader.GetString("NoInternetConnection");
			return msg;
		}
		private string GetCurrentHostName()
		{
			string hostName = "NA";
			var hostNames = NetworkInformation.GetHostNames();
			if (hostNames != null)
				hostName = hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "???";
			return hostName;
		}
		private string GetOSVersion()
		{
			OSVersion ver = SystemInformation.OperatingSystemVersion;
			return ver.ToString();
		}
		private string GetRASPANodeVersion()
		{
			return Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor + "." + Package.Current.Id.Version.Build + "." + Package.Current.Id.Version.Revision;
		}
		private string SystemProductName()
		{
			EasClientDeviceInformation sysInfo = new EasClientDeviceInformation();
			return sysInfo.SystemProductName;
		}
		private string GetHardwareID()
		{
			string uniqueCode = "";
			var token = HardwareIdentification.GetPackageSpecificToken(null);
			using (DataReader reader = DataReader.FromBuffer(token.Id))
			{
				byte[] bytes = new byte[token.Id.Length];
				reader.ReadBytes(bytes);
				uniqueCode = Encoding.ASCII.GetString(bytes);
			}
			return uniqueCode;
		}
	}

}
