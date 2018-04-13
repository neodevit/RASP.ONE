using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors.Custom;

namespace Sensors
{
	// metano
    public class CO2
	{
		// The following ID is defined by vendors and is unique to a custom sensor type. Each custom sensor driver should define one unique ID.
		// The ID below is defined in the custom sensor driver sample available in the SDK. It identifies the custom sensor CO2 emulation sample driver.
		Guid GUIDCustomSensorDeviceVendorDefinedTypeID = new Guid("4025a865-638c-43aa-a688-98580961eeae");

		// A property key is defined by vendors for each datafield property a custom sensor driver exposes. Property keys are defined
		// per custom sensor driver and is unique to each custom sensor type.
		// 
		// The following property key represents the CO2 level as defined in the custom sensor CO2 emulation driver sample available in the WDK.
		// In this example only one key is defined, but other drivers may define more than one key by rev'ing up the property key index.
		const String CO2LevelKey = "{74879888-a3cc-45c6-9ea9-058838256433} 1";

		private CustomSensor customSensor;
		private uint desiredReportInterval;
		private DeviceWatcher watcher;
		private DeviceAccessInformation deviceAccessInformation;

		public CO2()
		{
			String customSensorSelector = "";


			customSensorSelector = CustomSensor.GetDeviceSelector(GUIDCustomSensorDeviceVendorDefinedTypeID);
			watcher = DeviceInformation.CreateWatcher(customSensorSelector);
			watcher.Added += OnCustomSensorAdded;
			watcher.Start();

			// Register to be notified when the user disables access to the custom sensor through privacy settings.
			deviceAccessInformation = DeviceAccessInformation.CreateFromDeviceClassId(GUIDCustomSensorDeviceVendorDefinedTypeID);
			deviceAccessInformation.AccessChanged += OnAccessChanged;


			// Re-enable sensor input (no need to restore the desired reportInterval... it is restored for us upon app resume)
			customSensor.ReadingChanged += OnReadingChanged;
		}

		/// <summary>
		/// Invoked when the device watcher finds a matching custom sensor device 
		/// </summary>
		/// <param name="watcher">device watcher</param>
		/// <param name="customSensorDevice">device information for the custom sensor that was found</param>
		private async void OnCustomSensorAdded(DeviceWatcher watcher, DeviceInformation customSensorDevice)
		{
			try
			{
				customSensor = await CustomSensor.FromIdAsync(customSensorDevice.Id);
				if (customSensor != null)
				{
					CustomSensorReading reading = customSensor.GetCurrentReading();
					if (!reading.Properties.ContainsKey(CO2LevelKey))
					{
						//rootPage.NotifyUser("The found custom sensor doesn't provide CO2 reading", NotifyType.ErrorMessage);
						customSensor = null;
					}
					else
					{
						// Select a report interval that is both suitable for the purposes of the app and supported by the sensor.
						// This value will be used later to activate the sensor.
						// In the case below, we defined a 200ms report interval as being suitable for the purpose of this app.
						UInt32 minReportInterval = customSensor.MinimumReportInterval;
						desiredReportInterval = minReportInterval > 200 ? minReportInterval : 200;
					}

				}
				else
				{
					//rootPage.NotifyUser("No custom sensor found", NotifyType.ErrorMessage);
				}
			}
			catch (Exception e)
			{
				//rootPage.NotifyUser("The user may have denied access to the custom sensor. Error: " + e.Message, NotifyType.ErrorMessage);
			}
		}

		/// <summary>
		/// This is the event handler for AccessChanged events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void OnAccessChanged(DeviceAccessInformation sender, DeviceAccessChangedEventArgs e)
		{
			var status = e.Status;
			if (status != DeviceAccessStatus.Allowed)
			{
				//rootPage.NotifyUser("Custom sensor access denied", NotifyType.ErrorMessage);
				customSensor = null;
			}
		}

		/// <summary>
		/// This is the event handler for ReadingChanged events.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void OnReadingChanged(CustomSensor sender, CustomSensorReadingChangedEventArgs e)
		{
			CustomSensorReading reading = e.Reading;

			string CO2LevelString = String.Format("{0,5:0.00}", reading.Properties[CO2LevelKey]);
		}
	}
}
