using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaEntity
{
	public class Tempo
	{
		private const int giorniNelMese = 31;
		private const int oreNelGiorno = 24;
		private const int minutiNelOra = 60;
		private const int secondaNelMinuti = 60;
		public Tempo()
		{
			_totaleSS = 0;
		}
		public Tempo(int d, int h, int m)
		{
			dd = d;
			hh = h;
			mm = m;
			totaleMM = (dd * oreNelGiorno * minutiNelOra) + (hh * minutiNelOra) + mm;
		}
		public Tempo(int month, int d, int h, int m)
		{
			mo = month;
			dd = d;
			hh = h;
			mm = m;
			totaleMM = (mo * giorniNelMese * oreNelGiorno * minutiNelOra) + (dd * oreNelGiorno * minutiNelOra) + (hh * minutiNelOra) + mm;
		}
		public Tempo(int month, int d, int h, int m, int s)
		{
			mo = month;
			dd = d;
			hh = h;
			mm = m;
			ss = s;
			totaleSS = (mo * giorniNelMese * oreNelGiorno * minutiNelOra * secondaNelMinuti) + (dd * oreNelGiorno * minutiNelOra * secondaNelMinuti) + (hh * minutiNelOra * secondaNelMinuti) + (mm * secondaNelMinuti) + ss;
		}

		public Tempo(double? d, double? h, double? m)
		{
			dd = (d.HasValue) ? Convert.ToInt32(d) : 0;
			hh = (h.HasValue) ? Convert.ToInt32(h) : 0;
			mm = (m.HasValue) ? Convert.ToInt32(m) : 0;
			totaleMM = (dd * oreNelGiorno * minutiNelOra) + (hh * minutiNelOra) + mm;
		}
		public Tempo(double? month, double? d, double? h, double? m)
		{
			mo = (month.HasValue) ? Convert.ToInt32(month) : 0;
			dd = (d.HasValue) ? Convert.ToInt32(d) : 0;
			hh = (h.HasValue) ? Convert.ToInt32(h) : 0;
			mm = (m.HasValue) ? Convert.ToInt32(m) : 0;
			totaleMM = (mo * giorniNelMese * oreNelGiorno * minutiNelOra) + (dd * oreNelGiorno * minutiNelOra) + (hh * minutiNelOra) + mm;
		}
		public Tempo(double? month, double? d, double? h, double? m, double? s)
		{
			mo = (month.HasValue) ? Convert.ToInt32(month) : 0;
			dd = (d.HasValue) ? Convert.ToInt32(d) : 0;
			hh = (h.HasValue) ? Convert.ToInt32(h) : 0;
			mm = (m.HasValue) ? Convert.ToInt32(m) : 0;
			ss = (s.HasValue) ? Convert.ToInt32(s) : 0;
			totaleSS = (mo * giorniNelMese * oreNelGiorno * minutiNelOra * secondaNelMinuti) + (dd * oreNelGiorno * minutiNelOra * secondaNelMinuti) + (hh * minutiNelOra * secondaNelMinuti) + (mm * secondaNelMinuti) + ss;

		}

		public Tempo(string d, string h, string m)
		{
			dd = StringToNumber(d);
			hh = StringToNumber(h);
			mm = StringToNumber(m);
			totaleMM = (dd * oreNelGiorno * minutiNelOra) + (hh * minutiNelOra) + mm;
		}
		public Tempo(string month, string d, string h, string m)
		{
			mo = StringToNumber(month);
			dd = StringToNumber(d);
			hh = StringToNumber(h);
			mm = StringToNumber(m);
			totaleMM = (mo * giorniNelMese * oreNelGiorno * minutiNelOra) + (dd * oreNelGiorno * minutiNelOra) + (hh * minutiNelOra) + mm;
		}
		public Tempo(string month, string d, string h, string m, string s)
		{
			mo = StringToNumber(month);
			dd = StringToNumber(d);
			hh = StringToNumber(h);
			mm = StringToNumber(m);
			ss = StringToNumber(s);
			totaleSS = (mo * giorniNelMese * oreNelGiorno * minutiNelOra * secondaNelMinuti) + (dd * oreNelGiorno * minutiNelOra * secondaNelMinuti) + (hh * minutiNelOra * secondaNelMinuti) + (mm * secondaNelMinuti) + ss;
		}

		#region conversion utility
		public static int StringToNumber(string valore)
		{
			int res = 0;
			try
			{
				if (isNumeric(valore))
					res = Convert.ToInt32(valore);
			}
			catch (Exception ex)
			{
				res = 0;
			}
			return res;
		}
		public static bool isNumeric(string valore)
		{
			bool res = false;
			try
			{
				int numero;
				if (int.TryParse(valore, out numero))
				{
					res = true;
				}
				else
				{
					res = false;
				}
			}
			catch (Exception ex)
			{
				res = false;
			}
			return res;
		}

		#endregion

		public Tempo(int? tot)
		{
			totaleMM = tot;
		}
		public TimeSpan getTimeSpan()
		{
			int mesiday = (mo * giorniNelMese) + dd;
			TimeSpan _res = new TimeSpan(mesiday, hh, mm, 0, 0);
			return _res;
		}

		private int? _totaleMO;
		public int? totaleMO
		{
			get { return _totaleMO; }
			set
			{
				_totaleMO = value;
				if (_totaleMO.HasValue && _totaleMO.Value < 0)
				{
					_totaleMO = null;
					_totaleDD = null;
					_totaleHH = null;
					_totaleMM = null;
					_totaleSS = null;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleMO.HasValue && _totaleMO.Value == 0)
				{
					_totaleMO = 0;
					_totaleDD = 0;
					_totaleHH = 0;
					_totaleMM = 0;
					_totaleSS = 0;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleMO.HasValue && _totaleMO.Value > 0)
				{
					TimeSpan span = TimeSpan.FromDays(_totaleMO.Value * giorniNelMese);
					mo = (span.Days != 0) ? Convert.ToInt32(span.Days / giorniNelMese) : 0;
					dd = (span.Days != 0) ? span.Days % giorniNelMese : 0;
					hh = span.Hours;
					mm = span.Minutes;
					ss = span.Seconds;
				}
				else if (_totaleMO.HasValue == false)
				{
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}

			}
		}

		private int? _totaleDD;
		public int? totaleDD
		{
			get { return _totaleDD; }
			set
			{
				_totaleDD = value;
				if (_totaleDD.HasValue && _totaleDD.Value < 0)
				{
					_totaleMO = null;
					_totaleDD = null;
					_totaleHH = null;
					_totaleMM = null;
					_totaleSS = null;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleDD.HasValue && _totaleDD.Value == 0)
				{
					_totaleMO = 0;
					_totaleDD = 0;
					_totaleHH = 0;
					_totaleMM = 0;
					_totaleSS = 0;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleDD.HasValue && _totaleDD.Value > 0)
				{
					TimeSpan span = TimeSpan.FromDays(_totaleDD.Value);
					mo = (span.Days != 0) ? Convert.ToInt32(span.Days / giorniNelMese) : 0;
					dd = (span.Days != 0) ? span.Days % giorniNelMese : 0;
					hh = span.Hours;
					mm = span.Minutes;
					ss = span.Seconds;
				}
				else if (_totaleDD.HasValue == false)
				{
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}

			}
		}

		private int? _totaleHH;
		public int? totaleHH
		{
			get { return _totaleHH; }
			set
			{
				_totaleMM = value;
				if (_totaleHH.HasValue && _totaleHH.Value < 0)
				{
					_totaleMO = null;
					_totaleDD = null;
					_totaleHH = null;
					_totaleMM = null;
					_totaleSS = null;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleHH.HasValue && _totaleHH.Value == 0)
				{
					_totaleMO = 0;
					_totaleDD = 0;
					_totaleHH = 0;
					_totaleMM = 0;
					_totaleSS = 0;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleHH.HasValue && _totaleHH.Value > 0)
				{
					TimeSpan span = TimeSpan.FromHours(_totaleHH.Value);
					mo = (span.Days != 0) ? Convert.ToInt32(span.Days / giorniNelMese) : 0;
					dd = (span.Days != 0) ? span.Days % giorniNelMese : 0;
					hh = span.Hours;
					mm = span.Minutes;
					ss = span.Seconds;
				}
				else if (_totaleHH.HasValue == false)
				{
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}

			}
		}

		private int? _totaleMM;
		public int? totaleMM
		{
			get { return _totaleMM; }
			set
			{
				_totaleMM = value;
				if (_totaleMM.HasValue && _totaleMM.Value < 0)
				{
					_totaleMO = null;
					_totaleDD = null;
					_totaleHH = null;
					_totaleMM = null;
					_totaleSS = null;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleMM.HasValue && _totaleMM.Value == 0)
				{
					_totaleMO = 0;
					_totaleDD = 0;
					_totaleHH = 0;
					_totaleMM = 0;
					_totaleSS = 0;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleMM.HasValue && _totaleMM.Value > 0)
				{
					TimeSpan span = TimeSpan.FromMinutes(_totaleMM.Value);
					mo = (span.Days != 0) ? Convert.ToInt32(span.Days / giorniNelMese) : 0;
					dd = (span.Days != 0) ? span.Days % giorniNelMese : 0;
					hh = span.Hours;
					mm = span.Minutes;
					ss = span.Seconds;
					_totaleSS = _totaleMM.Value * 60;
				}
				else if (_totaleMM.HasValue == false)
				{
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}

			}
		}

		private int? _totaleSS;
		public int? totaleSS
		{
			get { return _totaleSS; }
			set
			{
				_totaleSS = value;
				if (_totaleSS.HasValue && _totaleSS.Value < 0)
				{
					_totaleMO = null;
					_totaleDD = null;
					_totaleHH = null;
					_totaleMM = null;
					_totaleSS = null;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleSS.HasValue && _totaleSS.Value == 0)
				{
					_totaleMO = 0;
					_totaleDD = 0;
					_totaleHH = 0;
					_totaleMM = 0;
					_totaleSS = 0;
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}
				else if (_totaleSS.HasValue && _totaleSS.Value > 0)
				{
					TimeSpan span = TimeSpan.FromSeconds(_totaleSS.Value);
					mo = (span.Days != 0) ? Convert.ToInt32(span.Days / giorniNelMese) : 0;
					dd = (span.Days != 0) ? span.Days % giorniNelMese : 0;
					hh = span.Hours;
					mm = span.Minutes;
					ss = span.Seconds;
				}
				else if (_totaleSS.HasValue == false)
				{
					mo = 0;
					dd = 0;
					hh = 0;
					mm = 0;
					ss = 0;
				}

			}
		}

		public int mo { get; set; }
		public int dd { get; set; }
		public int hh { get; set; }
		public int mm { get; set; }
		public int ss { get; set; }


		public string Descrizione
		{
			get
			{
				string res = "";
				if (mo > 0)
					res += mo + " mesi ";
				if (dd > 0)
					res += dd + " giorni ";
				if (hh > 0)
					res += hh + " ore ";
				if (mm > 0)
					res += mm + " minuti ";
				if (ss > 0)
					res += mm + " secondi ";

				return res;
			}
		}
		public string DescrizioneShort
		{
			get
			{
				return "M " + mo + " G " + dd + " " + hh + ":" + mm + ":" + ss;
			}
		}


	}
}
