using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public class Follow
	{
		public string UserIns { get; set; }
		public DateTime? DataIns { get; set; }
		public string UserMod { get; set; }
		public DateTime? DataMod { get; set; }

		public Follow SetFollow(string user)
		{
			UserIns = user;
			DataIns = DateTime.Now;
			UserMod = user;
			DataMod = DateTime.Now;
			return this;
		}
		public string getDataIns()
		{
			return (DataIns.HasValue) ? DataIns.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-";
		}
		public string getDataMod()
		{
			return (DataMod.HasValue) ? DataMod.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-";
		}


	}
}
