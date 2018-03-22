using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaDB
{
    public partial class DBCentral
    {
		string Server = "192.168.1.69";
		string User = "SassoSec";
		string Password = "123456";
		string Istance = "SassoSec";
		string Port = "3307";
		  
		public DBCentral()
		{

		}

		public string GetConnectionString()
		{
			return "server=" + Server + ";user=" + User + ";database=" + Istance + ";port=" + Port + ";password=" + Password + ";SslMode=None;Convert Zero Datetime=True;";
		}

	}
}
