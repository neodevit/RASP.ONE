using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaAction
{
	interface IPlatform
	{
		RaspaResult RUN(GpioPin gpioPIN, RaspaProtocol protocol);
		event ActionNotify ActionNotify;
	}
}
