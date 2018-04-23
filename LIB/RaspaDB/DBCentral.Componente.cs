using MySql.Data.MySqlClient;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace RaspaDB
{
    public partial class DBCentral
    {
		// verifica che non esista altro nodo che quel num eccetto se stesso
		public bool existAltroComponenteConStessoNodeNum(int? IDCorrente, int nodeNum)
		{
			bool res = false;
			try
			{
				Componenti nodeNums = GetComponenteByNodeNum(nodeNum);
				if (nodeNums.Count == 0 || 
				   (IDCorrente.HasValue && nodeNums.Count == 1 && nodeNums[0].ID == IDCorrente.Value) ||
				   (IDCorrente.HasValue==false && nodeNums.Count == 1))
					res = false;
				else
					res = true;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public bool existAltroComponenteConStessoNodeNum(int? IDCorrente, int nodeNum,enumComponente tipo)
		{
			bool res = false;
			try
			{
				Componenti nodeNums = GetComponenteByNodeNumAndTipo(nodeNum, tipo);
				if (nodeNums.Count == 0 ||
				   (IDCorrente.HasValue && nodeNums.Count == 1 && nodeNums[0].ID == IDCorrente.Value) ||
				   (IDCorrente.HasValue == false && nodeNums.Count == 1))
					res = false;
				else
					res = true;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public bool existAltroComponenteConStessoNome(int? IDCorrente, string Nome)
		{
			bool res = false;
			try
			{
				Componenti nodeNums = GetComponenteByNome(Nome);
				if (nodeNums.Count == 0 ||
				   (IDCorrente.HasValue && nodeNums.Count == 1 && nodeNums[0].ID == IDCorrente.Value) ||
				   (IDCorrente.HasValue == false && nodeNums.Count == 1))
					res = false;
				else
					res = true;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public bool existAltroComponenteConStessoIPv4(int? IDCorrente, string IPv4)
		{
			bool res = false;
			try
			{
				Componenti nodeNums = GetComponentiByIPv4(IPv4);
				if (nodeNums.Count == 0 ||
				   (IDCorrente.HasValue && nodeNums.Count == 1 && nodeNums[0].ID == IDCorrente.Value) ||
				   (IDCorrente.HasValue == false && nodeNums.Count == 1))
					res = false;
				else
					res = true;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public bool existAltroComponenteConStessoIPv4(int? IDCorrente, string IPv4, enumComponente tipo)
		{
			bool res = false;
			try
			{
				Componenti nodeNums = GetComponentiByIPv4(IPv4, tipo);
				if (nodeNums.Count == 0 ||
				   (IDCorrente.HasValue && nodeNums.Count == 1 && nodeNums[0].ID == IDCorrente.Value) ||
				   (IDCorrente.HasValue == false && nodeNums.Count == 1))
					res = false;
				else
					res = true;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public bool existAltroComponenteConStessoNodeNumAndPin(int? IDCorrente, int nodeNum,int Pin)
		{
			bool res = false;
			try
			{
				Componenti nodeNums = GetComponenteByNodeNumAndNode_Pin(nodeNum, Pin);
				if (nodeNums.Count == 0 ||
				   (IDCorrente.HasValue && nodeNums.Count == 1 && nodeNums[0].ID == IDCorrente.Value) ||
				   (IDCorrente.HasValue == false && nodeNums.Count == 1))
					res = false;
				else
					res = true;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public bool existAltroComponenteConStessoNodeNumAndPinEccettoTipo(int? IDCorrente, int nodeNum, int Pin, enumComponente Tipo)
		{
			bool res = false;
			try
			{
				Componenti nodeNums = GetComponenteByNodeNumAndNode_PinEccettoTipo(nodeNum, Pin,Tipo);
				if (nodeNums.Count == 0 ||
				   (IDCorrente.HasValue && nodeNums.Count == 1 && nodeNums[0].ID == IDCorrente.Value) ||
				   (IDCorrente.HasValue == false && nodeNums.Count == 1))
					res = false;
				else
					res = true;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponenti()
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " ORDER BY Nome";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componente GetComponenteByID(int ID)
		{
			Componente res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						Componenti result = GetRecComponenti(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponenteNODOorCENTRALE()
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IDComponenteTipo = @nodo";
				sql += " OR IDComponenteTipo = @centrale";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@nodo", (int)enumComponente.nodo);
						mySqlCommand.Parameters.AddWithValue("@centrale", (int)enumComponente.centrale);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componenti GetComponente_ECCETTO_NODOorCENTRALE()
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IDComponenteTipo <> @nodo";
				sql += " AND IDComponenteTipo <> @centrale";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@nodo", (int)enumComponente.nodo);
						mySqlCommand.Parameters.AddWithValue("@centrale", (int)enumComponente.centrale);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componenti GetComponente_ECCETTO_NODO()
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IDComponenteTipo <> @nodo";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@nodo", (int)enumComponente.nodo);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}



		public Componenti GetComponentiByIPv4(string IPv4)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IPv4 = @IPv4";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componente GetComponenteByIPv4(string IPv4)
		{
			Componente res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IPv4 = @IPv4";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4);
						mySqlCommand.Connection.Open();

						Componenti results = GetRecComponenti(mySqlCommand);
						if (results.Count > 0)
							res = results[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componente GetComponenteByHWAddress(string HWAddress)
		{
			Componente res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE HWAddress = @HWAddress";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@HWAddress", HWAddress);
						mySqlCommand.Connection.Open();

						Componenti results = GetRecComponenti(mySqlCommand);
						if (results.Count > 0)
							res = results[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponentiByIPv4(string IPv4, enumComponente Tipo)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IPv4 = @IPv4";
				sql += " AND   IDComponenteTipo = @IDComponenteTipo";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4);
						mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)Tipo);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componente GetComponenteByIPv4(string IPv4, enumComponente Tipo)
		{
			Componente res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IPv4 = @IPv4";
				sql += " AND   IDComponenteTipo = @IDComponenteTipo";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4);
						mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)Tipo);
						mySqlCommand.Connection.Open();

						Componenti result = GetRecComponenti(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponentedByTipoAndEnableAndTrustedAndAttivoOrderByNode(enumComponente Tipo,bool? Enabled,bool? Trusted, bool? Attivo)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IDComponenteTipo = @IDComponenteTipo";
				if (Enabled.HasValue)
					sql += " AND Enabled = @Enabled";
				if (Trusted.HasValue)
					sql += " AND Trusted = @Trusted";
				if (Attivo.HasValue)
					sql += " AND Attivo = @Attivo";
				sql += " ORDER BY Node_Num";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)Tipo);
						if (Enabled.HasValue)
							mySqlCommand.Parameters.AddWithValue("@Enabled", Enabled.Value);
						if (Trusted.HasValue)
							mySqlCommand.Parameters.AddWithValue("@Trusted", Trusted.Value);
						if (Attivo.HasValue)
							mySqlCommand.Parameters.AddWithValue("@Attivo", Attivo.Value);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componenti GetComponenteByNodeNum(int Node_Num)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE Node_Num = @Node_Num";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Node_Num", Node_Num);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componenti GetComponenteByNodeNumAndTipo(int Node_Num, enumComponente Tipo)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE Node_Num = @Node_Num";
				sql += " AND   IDComponenteTipo = @IDComponenteTipo";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Node_Num", Node_Num);
						mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)Tipo);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponenteByNode_Pin(int Node_Pin)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE Node_Pin = @Node_Pin";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Node_Pin", Node_Pin);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponenteByNodeNumAndNode_Pin(int Node_Num,int Node_Pin)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE Node_Num = @Node_Num";
				sql += " AND Node_Pin = @Node_Pin";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Node_Num", Node_Num);
						mySqlCommand.Parameters.AddWithValue("@Node_Pin", Node_Pin);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponenteByNodeNumAndNode_PinEccettoTipo(int Node_Num, int Node_Pin, enumComponente Tipo)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE Node_Num = @Node_Num";
				sql += " AND Node_Pin = @Node_Pin";
				sql += " AND IDComponenteTipo <> @IDComponenteTipo";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Node_Num", Node_Num);
						mySqlCommand.Parameters.AddWithValue("@Node_Pin", Node_Pin);
						mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)Tipo);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponenteByNodeNumAndNode_Pin(int Node_Num, int Node_Pin,string Value)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE Node_Num = @Node_Num";
				sql += " AND Node_Pin = @Node_Pin";
				sql += " WHERE TRIM(LOWER(Value)) = @Value";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Node_Num", Node_Num);
						mySqlCommand.Parameters.AddWithValue("@Node_Pin", Node_Pin);
						mySqlCommand.Parameters.AddWithValue("@Value", Value.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public Componenti GetComponenteByNome(string Nome)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE TRIM(LOWER(Nome)) = @Nome";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Nome", Nome.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public Componenti GetComponenteByIDComponenteTipo(enumComponente tipo)
		{
			Componenti res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `70_COMPONENTE`";
				sql += " WHERE IDComponenteTipo = @IDComponenteTipo";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)tipo);
						mySqlCommand.Connection.Open();

						res = GetRecComponenti(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		private Componenti GetRecComponenti(MySqlCommand mySqlCommand)
		{
			Componenti res = new Componenti();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Componente item = new Componente();

						item.ID = reader.GetInt32("ID");
						if (!reader.IsDBNull(reader.GetOrdinal("Enabled")))
							item.Enabled = reader.GetBoolean("Enabled");
						else
							item.Enabled = false;
						if (!reader.IsDBNull(reader.GetOrdinal("Trusted")))
							item.Trusted = reader.GetBoolean("Trusted");
						else
							item.Trusted = false;

						if (!reader.IsDBNull(reader.GetOrdinal("IDComponenteTipo")))
						{
							item.IDComponenteTipo = reader.GetInt32("IDComponenteTipo");
							item.Tipo = (enumComponente)reader.GetInt32("IDComponenteTipo");
						}
						else
						{
							item.IDComponenteTipo = 0;
							item.Tipo = enumComponente.nessuno;
						}

						if (!reader.IsDBNull(reader.GetOrdinal("Stato")))
							item.Stato = (enumStato)reader.GetInt32("Stato");
						else
							item.Stato = enumStato.nessuno;

						if (!reader.IsDBNull(reader.GetOrdinal("repeat")))
							item.repeat = reader.GetBoolean("repeat");
						else
							item.repeat = false;

						if (!reader.IsDBNull(reader.GetOrdinal("repeatTime_sec")))
							item.repeatTime.totaleSS = reader.GetInt32("repeatTime_sec");
						else
							item.repeatTime.totaleSS = 0;


						if (!reader.IsDBNull(reader.GetOrdinal("HostName")))
							item.Nome = reader.GetString("HostName");
						else
							item.HostName = "";

						if (!reader.IsDBNull(reader.GetOrdinal("Nome")))
							item.Nome = reader.GetString("Nome");
						else
							item.Nome = "";
						if (!reader.IsDBNull(reader.GetOrdinal("Descrizione")))
							item.Descrizione = reader.GetString("Descrizione");
						else
							item.Descrizione = "";

						if (!reader.IsDBNull(reader.GetOrdinal("OSVersion")))
							item.OSVersion = reader.GetString("OSVersion");
						else
							item.OSVersion = "";
						if (!reader.IsDBNull(reader.GetOrdinal("NodeSWVersion")))
							item.NodeSWVersion = reader.GetString("NodeSWVersion");
						else
							item.NodeSWVersion = "";
						if (!reader.IsDBNull(reader.GetOrdinal("SystemProductName")))
							item.SystemProductName = reader.GetString("SystemProductName");
						else
							item.SystemProductName = "";
						if (!reader.IsDBNull(reader.GetOrdinal("SystemID")))
							item.SystemID = reader.GetString("SystemID");
						else
							item.SystemID = "";
						

						// POSITION
						if (!reader.IsDBNull(reader.GetOrdinal("PositionTop")))
							item.PositionTop = reader.GetDouble("PositionTop");
						else
							item.PositionTop = 10;
						if (!reader.IsDBNull(reader.GetOrdinal("PositionLeft")))
							item.PositionLeft = reader.GetDouble("PositionLeft");
						else
							item.PositionLeft = 10;
						if (!reader.IsDBNull(reader.GetOrdinal("PositionBottom")))
							item.PositionBottom = reader.GetDouble("PositionBottom");
						else
							item.PositionBottom = 10;
						if (!reader.IsDBNull(reader.GetOrdinal("PositionRight")))
							item.PositionRight = reader.GetDouble("PositionRight");
						else
							item.PositionRight = 10;


						if (!reader.IsDBNull(reader.GetOrdinal("Node_Num")))
							item.Node_Num = reader.GetInt32("Node_Num");
						else
							item.Node_Num = 0;
						if (!reader.IsDBNull(reader.GetOrdinal("Node_Pin")))
							item.Node_Pin = reader.GetInt32("Node_Pin");
						else
							item.Node_Pin = 0;


						// VALUE
						if (!reader.IsDBNull(reader.GetOrdinal("Value")))
							item.ValueFor_readDB(reader.GetString("Value"));
						else
							item.Value = new List<string>();



						// OPTIONS
						if (!reader.IsDBNull(reader.GetOrdinal("Options")))
							item.Options = reader.GetString("Options");
						else
							item.Options = "";

						if (!reader.IsDBNull(reader.GetOrdinal("IPv4")))
							item.IPv4 = reader.GetString("IPv4");
						else
							item.IPv4 = "";
						if (!reader.IsDBNull(reader.GetOrdinal("IPv6")))
							item.IPv6 = reader.GetString("IPv6");
						else
							item.IPv6 = "";
						if (!reader.IsDBNull(reader.GetOrdinal("HWAddress")))
							item.HWAddress = reader.GetString("HWAddress");
						else
							item.HWAddress = "";
						if (!reader.IsDBNull(reader.GetOrdinal("BlueTooth")))
							item.BlueTooth = reader.GetString("BlueTooth");
						else
							item.BlueTooth = "";
						

						// FOLLOW
						if (!reader.IsDBNull(reader.GetOrdinal("UserIns")))
							item.follow.UserIns = reader.GetString("UserIns");
						else
							item.follow.UserIns = "";

						if (!reader.IsDBNull(reader.GetOrdinal("UserMod")))
							item.follow.UserMod = reader.GetString("UserMod");
						else
							item.follow.UserMod = "";
						if (!reader.IsDBNull(reader.GetOrdinal("DataIns")))
							item.follow.DataIns = reader.GetDateTime("DataIns");
						else
							item.follow.DataIns = null;

						if (!reader.IsDBNull(reader.GetOrdinal("DataMod")))
							item.follow.DataMod = reader.GetDateTime("DataMod");
						else
							item.follow.DataMod = null;

						res.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetComponenti(Componente value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			Componente nodo = null;
			MySqlConnection conn = null;
			MySqlTransaction trans = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				// TRANS
				// TRANSACTION START
				conn = new MySqlConnection(GetConnectionString());
				if (conn.State != ConnectionState.Open)
					conn.Open();
				trans = conn.BeginTransaction(IsolationLevel.Serializable);

				// SAVE COMPINENTE
				if (value.ID.HasValue)
					nodo = GetComponenteByID(value.ID.Value);
				if (nodo == null)
					res = InsComponenti(value, Utente,conn,trans);
				else
					res = ModComponenti(value, Utente,conn,trans);

				// se fallisce esco
				if (!res.Esito)
					return res;

				// se manca ID componente esco
				if (!res.ID.HasValue)
				{
					res.Esito = false;
					res.Message += "- Non determino ID componente";
					return res;
				}

				// aggiorno ip di tutti i componenti legati a quel nodo
				if (value.Tipo == enumComponente.nodo)
				{
					res = ModComponentiChangeNodeIP(res.ID.Value,value.Node_Num, value.IPv4, Utente,conn,trans);
					if (!res.Esito)
						return res;
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
				if (trans != null)
					trans.Rollback();
			}
			finally
			{
				if (trans != null)
				{
					if (res.Esito)
						trans.Commit();
					else
						trans.Rollback();
					trans.Dispose();
				}
				if (conn != null)
				{
					conn.Close();
					conn.Dispose();
				}
			}
			return res;
		}
		public RaspaResult InsComponenti(Componente value, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{
				string sql = "";
				sql += "INSERT INTO `70_COMPONENTE`";
				sql += " (`Enabled`,`Trusted`,`IDComponenteTipo`,`Stato`,`repeat`,repeatTime_sec,`HostName`,`OSVersion`,`NodeSWVersion`,`SystemProductName`,`SystemID`,`Nome`,`Descrizione`,`PositionLeft`,`PositionTop`,`PositionBottom`,`PositionRight`,`Node_Num`,`Node_Pin`,`Value`,`IPv4`,`IPv6`,`HWAddress`,`BlueTooth`,`Options`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@Enabled,@Trusted,@IDComponenteTipo,@Stato,@repeat,@repeatTime_sec,@HostName,@OSVersion,@NodeSWVersion,@SystemProductName,@SystemID,@Nome,@Descrizione,@PositionLeft,@PositionTop,@PositionBottom,@PositionRight,@Node_Num,@Node_Pin,@Value,@IPv4,@IPv6,@HWAddress,@BlueTooth,@Options,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				if (conn != null)
					mySqlConnection = conn;
				else
					mySqlConnection = new MySqlConnection(GetConnectionString());

				using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
				{
					mySqlCommand.CommandText = sql;
					mySqlCommand.Parameters.AddWithValue("@Enabled", value.Enabled);
					mySqlCommand.Parameters.AddWithValue("@Trusted", value.Enabled);
					mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)value.Tipo);

					mySqlCommand.Parameters.AddWithValue("@Stato", value.Stato);
					mySqlCommand.Parameters.AddWithValue("@repeat", value.repeat);
					mySqlCommand.Parameters.AddWithValue("@repeatTime_sec", value.repeatTime.totaleSS);

					mySqlCommand.Parameters.AddWithValue("@HostName", value.HostName);
					mySqlCommand.Parameters.AddWithValue("@OSVersion", value.OSVersion);
					mySqlCommand.Parameters.AddWithValue("@NodeSWVersion", value.NodeSWVersion);
					mySqlCommand.Parameters.AddWithValue("@SystemProductName", value.SystemProductName);
					mySqlCommand.Parameters.AddWithValue("@SystemID", value.SystemID);

					mySqlCommand.Parameters.AddWithValue("@Nome", value.Nome);
					mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);

					mySqlCommand.Parameters.AddWithValue("@PositionLeft", value.PositionLeft);
					mySqlCommand.Parameters.AddWithValue("@PositionTop", value.PositionTop);
					mySqlCommand.Parameters.AddWithValue("@PositionBottom", value.PositionBottom);
					mySqlCommand.Parameters.AddWithValue("@PositionRight", value.PositionRight);

					mySqlCommand.Parameters.AddWithValue("@Node_Num", value.Node_Num);
					mySqlCommand.Parameters.AddWithValue("@Node_Pin", value.Node_Pin);
					mySqlCommand.Parameters.AddWithValue("@Value", value.ValueFor_writeDB());

					mySqlCommand.Parameters.AddWithValue("@IPv4", value.IPv4);
					mySqlCommand.Parameters.AddWithValue("@IPv6", value.IPv6);
					mySqlCommand.Parameters.AddWithValue("@HWAddress", value.IPv6);
					mySqlCommand.Parameters.AddWithValue("@BlueTooth", value.BlueTooth);

					mySqlCommand.Parameters.AddWithValue("@Options", value.Options);

					mySqlCommand.Parameters.AddWithValue("@Utente", Utente);

					if (trans != null)
						mySqlCommand.Transaction = trans;

					if (conn.State == ConnectionState.Closed)
						mySqlCommand.Connection.Open();

					using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
					{
						if (reader.HasRows)
						{
							reader.Read();
							int ID = reader.GetInt32("ID");
							res = new RaspaResult(true, "Inserimento COMPONENTI " + ID + "eseguito con successo");
							res.ID = ID;
							value.ID = ID;
						}
					}
				}
				
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
				if (trans != null)
					trans.Rollback();
			}
			finally
			{
				if (trans == null && conn != null)
				{
					conn.Close();
					conn.Dispose();
				}
			}
			return res;
		}
		public RaspaResult ModComponenti(Componente value, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE NODE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `70_COMPONENTE`";
				sql += " SET `Enabled` = @Enabled";
				sql += "    ,`Trusted` = @Trusted";
				sql += "    ,`IDComponenteTipo` = @IDComponenteTipo";

				sql += "    ,`Stato` = @Stato";
				sql += "    ,`repeat` = @repeat";
				sql += "    ,`repeatTime_sec` = @repeatTime_sec";

				sql += "    ,`HostName` = @HostName";
				sql += "    ,`OSVersion` = @OSVersion";
				sql += "    ,`NodeSWVersion` = @NodeSWVersion";
				sql += "    ,`SystemProductName` = @SystemProductName";
				sql += "    ,`SystemID` = @SystemID";
				

				sql += "    ,`Nome` = @Nome";
				sql += "    ,`Descrizione` = @Descrizione";

				sql += "    ,`PositionLeft` = @PositionLeft";
				sql += "    ,`PositionTop` = @PositionTop";
				sql += "    ,`PositionBottom` = @PositionBottom";
				sql += "    ,`PositionRight` = @PositionRight";

				sql += "    ,`Node_Num` = @Node_Num";
				sql += "    ,`Node_Pin` = @Node_Pin";
				sql += "    ,`Value` = @Value";

				sql += "    ,`IPv4` = @IPv4";
				sql += "    ,`IPv6` = @IPv6";
				sql += "    ,`HWAddress` = @HWAddress";
				sql += "    ,`BlueTooth` = @BlueTooth";
				
				sql += "    ,`Options` = @Options";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				if (conn != null)
					mySqlConnection = conn;
				else
					mySqlConnection = new MySqlConnection(GetConnectionString());

				using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
				{
					mySqlCommand.CommandText = sql;
					mySqlCommand.Parameters.AddWithValue("@ID", value.ID.Value);
					mySqlCommand.Parameters.AddWithValue("@Enabled", value.Enabled);
					mySqlCommand.Parameters.AddWithValue("@Trusted", value.Trusted);
					mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)value.Tipo);

					mySqlCommand.Parameters.AddWithValue("@Stato", value.Stato);
					mySqlCommand.Parameters.AddWithValue("@repeat", value.repeat);
					mySqlCommand.Parameters.AddWithValue("@repeatTime_sec", value.repeatTime.totaleSS);

					mySqlCommand.Parameters.AddWithValue("@HostName", value.HostName);
					mySqlCommand.Parameters.AddWithValue("@OSVersion", value.OSVersion);
					mySqlCommand.Parameters.AddWithValue("@NodeSWVersion", value.NodeSWVersion);
					mySqlCommand.Parameters.AddWithValue("@SystemProductName", value.SystemProductName);
					mySqlCommand.Parameters.AddWithValue("@SystemID", value.SystemID);

					mySqlCommand.Parameters.AddWithValue("@Nome", value.Nome);
					mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);

					mySqlCommand.Parameters.AddWithValue("@PositionLeft", value.PositionLeft);
					mySqlCommand.Parameters.AddWithValue("@PositionTop", value.PositionTop);
					mySqlCommand.Parameters.AddWithValue("@PositionBottom", value.PositionBottom);
					mySqlCommand.Parameters.AddWithValue("@PositionRight", value.PositionRight);

					mySqlCommand.Parameters.AddWithValue("@Node_Num", value.Node_Num);
					mySqlCommand.Parameters.AddWithValue("@Node_Pin", value.Node_Pin);
					mySqlCommand.Parameters.AddWithValue("@Value", value.ValueFor_writeDB());

					mySqlCommand.Parameters.AddWithValue("@IPv4", value.IPv4);
					mySqlCommand.Parameters.AddWithValue("@IPv6", value.IPv6);
					mySqlCommand.Parameters.AddWithValue("@HWAddress", value.HWAddress);
					mySqlCommand.Parameters.AddWithValue("@BlueTooth", value.BlueTooth);

					mySqlCommand.Parameters.AddWithValue("@Options", value.Options);

					mySqlCommand.Parameters.AddWithValue("@Utente", Utente);

					if (trans != null)
						mySqlCommand.Transaction = trans;

					if (conn.State == ConnectionState.Closed)
						mySqlCommand.Connection.Open();
					mySqlCommand.ExecuteNonQuery();
				}
				

				res = new RaspaResult(true, "Update COMPONENTI ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
				if (trans != null)
					trans.Rollback();
			}
			finally
			{
				if (trans == null && conn != null)
				{
					conn.Close();
					conn.Dispose();
				}
			}
			return res;
		}
		public RaspaResult ModComponentiChangeNodeIP(int idNodo,int Node_Num, string IPv4, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{

				string sql = "";
				sql += "UPDATE `70_COMPONENTE`";
				sql += " SET `IPv4` = @IPv4";
				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `Node_Num` = @Node_Num";
				sql += " AND `IDComponenteTipo` <> @IDComponenteTipo";

				if (conn != null)
					mySqlConnection = conn;
				else
					mySqlConnection = new MySqlConnection(GetConnectionString());
				using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
				{
					mySqlCommand.CommandText = sql;
					mySqlCommand.Parameters.AddWithValue("@Node_Num", Node_Num);
					mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4);
					mySqlCommand.Parameters.AddWithValue("@IDComponenteTipo", (int)enumComponente.nodo);
					mySqlCommand.Parameters.AddWithValue("@Utente", Utente);

					if (trans != null)
						mySqlCommand.Transaction = trans;

					if (conn.State == ConnectionState.Closed)
						mySqlCommand.Connection.Open();
					mySqlCommand.ExecuteNonQuery();
				}
				

				res = new RaspaResult(true, "Update VALUE COMPONENTI nodenum " + Node_Num + "Eseguito");
				res.ID = idNodo;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
				if (trans != null)
					trans.Rollback();
			}
			finally
			{
				if (trans == null && conn != null)
				{
					conn.Close();
					conn.Dispose();
				}
			}
			return res;
		}


		public RaspaResult ModComponentiValue(int ID,string Value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (ID==0)
					return new RaspaResult(false, "UPDATE NODE VALUE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `70_COMPONENTE`";
				sql += " SET `Value` = @Value";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Parameters.AddWithValue("@Value", Value);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update VALUE COMPONENTI ID " + ID + "Eseguito");
				res.ID = ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModComponentiStato(int ID, enumStato Stato, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (ID==0)
					return new RaspaResult(false, "UPDATE NODE VALUE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `70_COMPONENTE`";
				sql += " SET `Stato` = @Stato";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Parameters.AddWithValue("@Stato", (int)Stato);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update VALUE COMPONENTI ID " + ID + "Eseguito");
				res.ID = ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModComponentiValueAndStato(int ID, string Value, enumStato Stato, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (ID == 0)
					return new RaspaResult(false, "UPDATE NODE VALUE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `70_COMPONENTE`";
				sql += " SET `Value` = @Value";
				sql += "    ,`Stato` = @Stato";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Parameters.AddWithValue("@Value", Value);
						mySqlCommand.Parameters.AddWithValue("@Stato", (int)Stato);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update VALUE COMPONENTI ID " + ID + "Eseguito");
				res.ID = ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DelComponentiByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `70_COMPONENTE`";
				sql += " WHERE ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTI : " + ex.Message);
			}
			return res;
		}
	}
}
