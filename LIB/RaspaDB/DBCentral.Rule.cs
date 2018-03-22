using MySql.Data.MySqlClient;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaDB
{
    public partial class DBCentral
    {

		public Rule GetRulesByID(int ID)
		{
			Rule res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `50_RULES` AS A";
				sql += " LEFT join `20_SUBSTRIPTION` AS S on S.ID = A.IDSubscription ";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE ";
				sql += " WHERE N.Enabled = 1";
				sql += " AND   N.Trusted = 1";
				sql += " AND   A.ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						Rules result = GetRecRule(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}
		public Rules GetRulesByIDSubscription(int IDSubscription,string ValueSubscription)
		{
			Rules res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `50_RULES` AS A";
				sql += " LEFT join `20_SUBSTRIPTION` AS S on S.ID = A.IDSubscription ";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE ";
				sql += " WHERE N.Enabled = 1";
				sql += " AND   N.Trusted = 1";
				sql += " AND   A.IDSubscription = @IDSubscription";
				sql += " AND   A.ValueSubscription = @ValueSubscription";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDSubscription", IDSubscription);
						mySqlCommand.Parameters.AddWithValue("@ValueSubscription", IDSubscription);
						mySqlCommand.Connection.Open();

						res = GetRecRule(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}


		private Rules GetRecRule(MySqlCommand mySqlCommand)
		{
			Rules res = new Rules();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Rule item = new Rule();

						item.ID = reader.GetInt32("ID");

						
						if (!reader.IsDBNull(reader.GetOrdinal("IDSubscription")))
							item.IDSubscription = reader.GetInt32("IDSubscription");
						else
							item.IDSubscription = 0;
						if (!reader.IsDBNull(reader.GetOrdinal("ValueSubscription")))
							item.ValueSubscription = reader.GetString("ValueSubscription");
						else
							item.ValueSubscription = "";
						

						if (!reader.IsDBNull(reader.GetOrdinal("NODE")))
							item.NODE = reader.GetInt32("NODE");
						else
							item.NODE = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("PIN")))
							item.PIN = reader.GetInt32("PIN");
						else
							item.PIN = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("Edge")))
							item.Edge = (GpioPinEdge)reader.GetInt32("Edge");
						else
							item.Edge = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("COMANDO")))
							item.COMANDO = (enumComando)reader.GetInt32("COMANDO");
						else
							item.COMANDO = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("COMPONENTE")))
							item.COMPONENTE = (enumComponente)reader.GetInt32("COMPONENTE");
						else
							item.COMPONENTE = 0;


						if (!reader.IsDBNull(reader.GetOrdinal("VALUE")))
							item.VALUE = reader.GetString("VALUE");
						else
							item.VALUE = "";

						if (!reader.IsDBNull(reader.GetOrdinal("IPv4")))
							item.IPv4 = reader.GetString("IPv4");
						else
							item.IPv4 = "";


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
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetRule(Rule value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			Rule role = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				if (value.ID.HasValue)
					role = GetRulesByID(value.ID.Value);
				if (role == null)
					res = InsRule(value, Utente);
				else
				{
					res = ModRule(value, Utente);
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}
		public RaspaResult InsRule(Rule value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				string sql = "";
				sql += "INSERT INTO `50_RULES`";
				sql += " (`IDSubscription`,`ValueSubscription`,`NODE`,`PIN`,`COMANDO`,`COMPONENTE`,`Edge`,`VALUE`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@IDSubscription,@ValueSubscription,@NODE,@PIN,@COMANDO,@COMPONENTE,@Edge,@VALUE,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDSubscription", value.IDSubscription);
						mySqlCommand.Parameters.AddWithValue("@ValueSubscription", value.ValueSubscription);
						mySqlCommand.Parameters.AddWithValue("@NODE", value.NODE);
						mySqlCommand.Parameters.AddWithValue("@PIN", value.PIN);
						mySqlCommand.Parameters.AddWithValue("@COMANDO", value.COMANDO);
						mySqlCommand.Parameters.AddWithValue("@COMPONENTE", value.COMPONENTE);
						mySqlCommand.Parameters.AddWithValue("@Edge", value.Edge);
						mySqlCommand.Parameters.AddWithValue("@VALUE", value.VALUE);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();

						using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
						{
							if (reader.HasRows)
							{
								reader.Read();
								int ID = reader.GetInt32("ID");
								res = new RaspaResult(true, "Inserimento RULE " + ID + "eseguito con successo");
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
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModRule(Rule value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE RULE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `50_RULES`";
				sql += " SET `IDSubscription` = @IDSubscription";
				sql += "    ,`ValueSubscription` = @ValueSubscription";
				sql += "    ,`NODE` = @NODE";
				sql += "    ,`PIN` = @PIN";
				sql += "    ,`COMANDO` = @COMANDO";
				sql += "    ,`COMPONENTE` = @COMPONENTE";
				sql += "    ,`Edge` = @Edge";
				sql += "    ,`VALUE` = @VALUE";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", value.ID.Value);
						mySqlCommand.Parameters.AddWithValue("@IDSubscription", value.IDSubscription);
						mySqlCommand.Parameters.AddWithValue("@ValueSubscription", value.ValueSubscription);
						mySqlCommand.Parameters.AddWithValue("@NODE", value.NODE);
						mySqlCommand.Parameters.AddWithValue("@PIN", value.PIN);
						mySqlCommand.Parameters.AddWithValue("@COMANDO", value.COMANDO);
						mySqlCommand.Parameters.AddWithValue("@COMPONENTE", value.COMPONENTE);
						mySqlCommand.Parameters.AddWithValue("@Edge", value.Edge);
						mySqlCommand.Parameters.AddWithValue("@VALUE", value.VALUE);

						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update RULE ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DelRuleByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `50_RULES`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}
	}
}
