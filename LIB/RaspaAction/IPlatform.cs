using RaspaEntity;
using RaspaTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaAction
{
	public interface IPlatform
	{
		RaspaResult RUN(MQTT mqtt, GpioPin gpioPIN, Dictionary<int, bool> EVENTS, RaspaProtocol protocol);
	}
}
