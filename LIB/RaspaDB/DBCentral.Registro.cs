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

		public Registri GetRegistri(int Limit=1000)
		{
			Registri res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_REGISTRO`";
				sql += " ORDER BY Nome";
				sql += " LIMIT @Limit";
				
				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Limit", Limit);
						mySqlCommand.Connection.Open();

						res = GetRecRegistri(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - REGISTRO : " + ex.Message);
			}
			return res;
		}
		public Registri GetRegistriByIDComponente(int IDComponente)
		{
			Registri res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_REGISTRO`";
				sql += " WHERE IDComponente = @IDComponente";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDComponente", IDComponente);
						mySqlCommand.Connection.Open();

						res = GetRecRegistri(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - REGISTRO : " + ex.Message);
			}
			return res;
		}
		public Registri GetRegistriByDataAndComponente(int IDComponente,DateTime from , DateTime to)
		{
			Registri res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_REGISTRO`";
				sql += " WHERE IDComponente = @IDComponente";
				sql += " AND Data BETWEEN @from AND $to";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDComponente", IDComponente);
						mySqlCommand.Parameters.AddWithValue("@from", from);
						mySqlCommand.Parameters.AddWithValue("@to", to);
						mySqlCommand.Connection.Open();

						res = GetRecRegistri(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - REGISTRO : " + ex.Message);
			}
			return res;
		}



		private Registri GetRecRegistri(MySqlCommand mySqlCommand)
		{
			Registri res = new Registri();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Registro item = new Registro();

						if (!reader.IsDBNull(reader.GetOrdinal("Data")))
							item.Data = reader.GetDateTime("Data");
						else
							item.Data = null;

						if (!reader.IsDBNull(reader.GetOrdinal("IDComponente")))
							item.IDComponente = reader.GetInt32("IDComponente");
						else
							item.IDComponente = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("Tipo")))
							item.Tipo = (enumComponente)reader.GetInt32("Tipo");
						else
							item.Tipo = enumComponente.nessuno;

						if (!reader.IsDBNull(reader.GetOrdinal("Stato")))
							item.Stato = (enumStato)reader.GetInt32("Stato");
						else
							item.Stato = enumStato.nessuno;

						// NODE
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

						// RETE
						if (!reader.IsDBNull(reader.GetOrdinal("IPv4")))
							item.IPv4 = reader.GetString("IPv4");
						else
							item.IPv4 = "";
						if (!reader.IsDBNull(reader.GetOrdinal("IPv6")))
							item.IPv6 = reader.GetString("IPv6");
						else
							item.IPv6 = "";

						res.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - REGISTRO : " + ex.Message);
			}
			return res;
		}

		public RaspaResult InsRegistro(Registro value, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{
				string sql = "";
				sql += "INSERT INTO `10_REGISTRO`";
				sql += " (`Data`,`Tipo`,`IDComponente`,`Stato`,`Node_Num`,Node_Pin,`Value`,`IPv4`,`IPv6`)";
				sql += " VALUES";
				sql += " (NOW(),@Tipo,@IDComponente,@Stato,@Node_Num,@Node_Pin,@Value,@IPv4,@IPv6);";
				sql += " select LAST_INSERT_ID() as ID;";

				if (conn != null)
					mySqlConnection = conn;
				else
					mySqlConnection = new MySqlConnection(GetConnectionString());

				using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
				{
					mySqlCommand.CommandText = sql;
					mySqlCommand.Parameters.AddWithValue("@Tipo", (int)value.Tipo);
					mySqlCommand.Parameters.AddWithValue("@IDComponente", value.IDComponente);
					mySqlCommand.Parameters.AddWithValue("@Stato", (int)value.Stato);
					mySqlCommand.Parameters.AddWithValue("@Node_Num", value.Node_Num);
					mySqlCommand.Parameters.AddWithValue("@Node_Pin", value.Node_Pin);
					mySqlCommand.Parameters.AddWithValue("@Value", value.ValueFor_writeDB());
					mySqlCommand.Parameters.AddWithValue("@IPv4", value.IPv4);
					mySqlCommand.Parameters.AddWithValue("@IPv6", value.IPv6);
					mySqlCommand.Parameters.AddWithValue("@IPv4", value.IPv4);
					mySqlCommand.Parameters.AddWithValue("@IPv4", value.IPv4);


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
							res = new RaspaResult(true, "Inserimento REGISTRO " + ID + "eseguito con successo");
						}
					}
				}
				
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - REGISTRO : " + ex.Message);
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



		public RaspaResult DelRegistriBydate(DateTime from,DateTime to)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `10_REGISTRO`";
				sql += " WHERE Data BETWEEN @from AND $to";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@from", from);
						mySqlCommand.Parameters.AddWithValue("@to", to);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - REGISTRO : " + ex.Message);
			}
			return res;
		}
	}
}
