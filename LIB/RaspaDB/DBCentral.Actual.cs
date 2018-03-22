using MySql.Data.MySqlClient;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaDB
{
    public partial class DBCentral
    {

		public Actuals GetActuals()
		{
			Actuals res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecActual(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public Actual GetActualByID(int ID)
		{
			Actual res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";
				sql += " WHERE ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						Actuals result = GetRecActual(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public Actuals GetActualByNum(int Num)
		{
			Actuals res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";
				sql += " WHERE node_num = @Num";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Num", Num);
						mySqlCommand.Connection.Open();

						res = GetRecActual(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public Actual GetActualByNumAndPin(int Num,int Pin)
		{
			Actual res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";
				sql += " WHERE node_num = @Num";
				sql += " AND   node_pin = @Pin";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Num", Num);
						mySqlCommand.Parameters.AddWithValue("@Pin", Pin);
						mySqlCommand.Connection.Open();

						Actuals result = GetRecActual(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public Actual GetActualByNumAndPinAndStato(enumStato stato, int Num, int Pin)
		{
			Actual res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";
				sql += " WHERE node_num = @Num";
				sql += " AND   node_pin = @Pin";
				sql += " AND   node_stato = @stato";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Num", Num);
						mySqlCommand.Parameters.AddWithValue("@Pin", Pin);
						mySqlCommand.Parameters.AddWithValue("@stato", (int)stato);
						mySqlCommand.Connection.Open();

						Actuals result = GetRecActual(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public Actuals GetActualByStato(enumStato stato)
		{
			Actuals res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";
				sql += " WHERE node_stato = @stato";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@stato", (int)stato);
						mySqlCommand.Connection.Open();

						res = GetRecActual(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public Actuals GetActualByStatoAndNum(enumStato stato,int Num)
		{
			Actuals res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";
				sql += " WHERE node_stato = @stato";
				sql += " WHERE node_num = @Num";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@stato", (int)stato);
						mySqlCommand.Parameters.AddWithValue("@Num", Num);
						mySqlCommand.Connection.Open();

						res = GetRecActual(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public Actuals GetActualByStatoAndPin(enumStato stato, int Pin)
		{
			Actuals res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `30_ACTUAL`";
				sql += " WHERE node_stato = @stato";
				sql += " WHERE node_pin = @Pin";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@stato", (int)stato);
						mySqlCommand.Parameters.AddWithValue("@Pin", Pin);
						mySqlCommand.Connection.Open();

						res = GetRecActual(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}


		private Actuals GetRecActual(MySqlCommand mySqlCommand)
		{
			Actuals res = new Actuals();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Actual item = new Actual();

						item.ID = reader.GetInt32("ID");

						
						if (!reader.IsDBNull(reader.GetOrdinal("node_num")))
							item.node_num = reader.GetInt32("node_num");
						else
							item.node_num = 0;
						

						if (!reader.IsDBNull(reader.GetOrdinal("Componente")))
							item.Componente = (enumComponente)reader.GetInt32("Componente");
						else
							item.Componente = 0;


						if (!reader.IsDBNull(reader.GetOrdinal("node_pin")))
							item.node_pin = reader.GetInt32("node_pin");
						else
							item.node_pin = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("node_stato")))
							item.node_stato = (enumStato)reader.GetInt32("node_stato");
						else
							item.node_stato = 0;


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
				System.Diagnostics.Debug.WriteLine("DBCentral - NODES : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetActual(Actual value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			Actual actual = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				actual = GetActualByNumAndPin(value.node_num,actual.node_pin);
				if (actual == null)
					res = InsActual(value, Utente);
				else
				{
					value.ID = actual.ID;
					res = ModActual(value, Utente);
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public RaspaResult InsActual(Actual value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				string sql = "";
				sql += "INSERT INTO `30_ACTUAL`";
				sql += " (`node_num`,`Componente`,`node_pin`,`node_stato`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@node_num,Componente,@node_pin,@node_stato,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@node_num", value.node_num);
						mySqlCommand.Parameters.AddWithValue("@Componente", value.Componente);
						mySqlCommand.Parameters.AddWithValue("@node_pin", value.node_pin);
						mySqlCommand.Parameters.AddWithValue("@node_stato", ((value.node_stato == enumStato.on) ? true : false));
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();

						using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
						{
							if (reader.HasRows)
							{
								reader.Read();
								int ID = reader.GetInt32("ID");
								res = new RaspaResult(true, "Inserimento NODE " + ID + "eseguito con successo");
								res.ID = ID;
								value.ID = ID;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModActual(Actual value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE NODE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `30_ACTUAL`";
				sql += " SET `node_num` = @node_num";
				sql += "    ,`Stato` = @Stato";
				
				sql += "    ,`Componente` = @Componente";
				sql += "    ,`node_pin` = @node_pin";
				sql += "    ,`Descrizione` = @Descrizione";

				sql += "    ,`HostName` = @HostName";
				sql += "    ,`MacAddress` = @MacAddress";
				sql += "    ,`HWAddress` = @HWAddress";
				sql += "    ,`BlueTooth` = @BlueTooth";
				sql += "    ,`IPv4` = @IPv4";
				sql += "    ,`IPv6` = @IPv6";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", value.ID.Value);
						mySqlCommand.Parameters.AddWithValue("@node_num", value.node_num);
						mySqlCommand.Parameters.AddWithValue("@Componente", value.Componente);
						mySqlCommand.Parameters.AddWithValue("@node_pin", value.node_pin);
						mySqlCommand.Parameters.AddWithValue("@Stato", ((value.node_stato == enumStato.on) ? true : false));

						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update ACTUAL ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DetActualByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `30_ACTUAL`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - ACTUAL : " + ex.Message);
			}
			return res;
		}
	}
}
