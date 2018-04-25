using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RaspaEntity
{
	public class Registro
	{
		public Registro()
		{
		}


		public DateTime? Data { get; set; }
		public enumComponente Tipo { get; set; }
		public int IDComponente { get; set; }
		public enumStato Stato { get; set; }

		public int Node_Num { get; set; }
		public int Node_Pin { get; set; }

		#region VALUE
		public List<string> Value { get; set; }

		#region VALUE for DB
		public void ValueFor_readDB(string val)
		{
			if (!string.IsNullOrEmpty(val))
				Value = val.Split('§').ToList<string>(); 
		}
		public string ValueFor_writeDB()
		{
			string res = "";
			if (Value!=null)
				res = string.Join("§", Value);
			return res;
		}
		#endregion
		#endregion

		public string IPv4 { get; set; }
		public string IPv6 { get; set; }

	}

	public class Registri : Collection<Registro>
	{

	}

}
