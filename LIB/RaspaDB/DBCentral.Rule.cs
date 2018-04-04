using MySql.Data.MySqlClient;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace RaspaDB
{
    public partial class DBCentral
    {
		#region CONTROLLER
		public RecRule GetRulesByID(int ID)
		{
			RecRule res = null;
			try
			{
				res = GetRuleByID(ID);
				if (res != null && res.ID.HasValue)
					res = getRecRules(res);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}
		public ObservableCollection<RecRule> GetRulesByID_Componente(int ID_Componente)
		{
			ObservableCollection<RecRule> res = null;
			try
			{
				res = GetRuleByIDComponente_SE(ID_Componente);
				foreach(RecRule rec in res)
					if (rec != null && rec.ID.HasValue)
						getRecRules(rec);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}
		private RecRule getRecRules(RecRule rec)
		{
			RecRule res = null;
			try
			{
				res = rec;
				res.ITEM = GetRules_itemByID_RULE(rec.ID.Value);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetRule(RecRule value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			MySqlConnection conn = null;
			MySqlTransaction trans = null;

			try
			{
				// CONTROLLI FORMALI
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				// TRANSACTION START
				conn = new MySqlConnection(GetConnectionString());
				if (conn.State != ConnectionState.Open)
					conn.Open();
				trans = conn.BeginTransaction(IsolationLevel.Serializable);

				// SAVE RULES
				SetRule(value, Utente, conn, trans);
				if (!res.Esito)
					return res;

				// non trovo id rules esco con errore
				if (!value.ID.HasValue)
					return new RaspaResult(false, "ID Rules non presente");

				// IMPOSTA ID RULES
				value.setIDRule();

				// SAVE ITEM
				res = SetRule_item(value.ITEM, Utente, conn, trans);
				if (!res.Esito)
					return res;


			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
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

		#endregion

		#region RULES
		private RecRule GetRuleByID(int ID)
		{
			RecRule res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `50_RULES`";
				sql += " WHERE ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						ObservableCollection<RecRule> result = GetRecRule(mySqlCommand);
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

		public ObservableCollection<RecRule> GetRuleByIDComponente_SE(int ID_Componente)
		{
			ObservableCollection<RecRule> res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `50_RULES` A";
				sql += " LEFT JOIN `51_RULES_ITEM` B on B.ID_RULE = A.ID";
				sql += " WHERE B.ID_Componente = @ID_Componente";
				sql += " AND B.Tipo = @Tipo";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID_Componente", ID_Componente);
						mySqlCommand.Parameters.AddWithValue("@Tipo", (int)enumRulesType.se);
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


		private ObservableCollection<RecRule> GetRecRule(MySqlCommand mySqlCommand)
		{
			ObservableCollection<RecRule> res = new ObservableCollection<RecRule>();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						RecRule item = new RecRule();

						item.ID = reader.GetInt32("ID");


						// NOME
						if (!reader.IsDBNull(reader.GetOrdinal("NOME")))
							item.NOME = reader.GetString("NOME");
						else
							item.NOME = "";

						if (!reader.IsDBNull(reader.GetOrdinal("DESCRIZIONE")))
							item.DESCRIZIONE = reader.GetString("DESCRIZIONE");
						else
							item.DESCRIZIONE = "";


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

		private RaspaResult SetRule(RecRule value, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(0);
			RecRule role = null;

			try
			{
				// CONTROLLI FORMALI
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				// SAVE RULES
				if (value.ID.HasValue)
					role = GetRulesByID(value.ID.Value);
				if (role == null)
					res = InsRule(value, Utente,conn,trans);
				else
					res = ModRule(value, Utente, conn, trans);

				// non riesco a salvare rules esco con errore
				if (!res.Esito)
					return res;

				// non trovo id rules esco con errore
				if (!value.ID.HasValue)
					return new RaspaResult(false, "ID Rules non presente");

			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
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
		public RaspaResult InsRule(RecRule value, string Utente, MySqlConnection conn=null, MySqlTransaction trans=null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{
				string sql = "";
				sql += "INSERT INTO `50_RULES`";
				sql += " (`NOME`,`DESCRIZIONE`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@NOME,@DESCRIZIONE,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				if (conn != null)
					mySqlConnection = conn;
				else
					mySqlConnection = new MySqlConnection(GetConnectionString());

				using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
				{
					mySqlCommand.CommandText = sql;
					mySqlCommand.Parameters.AddWithValue("@NOME", value.NOME);
					mySqlCommand.Parameters.AddWithValue("@DESCRIZIONE", value.DESCRIZIONE);
					mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
					mySqlCommand.Connection.Open();

					if (trans != null)
						mySqlCommand.Transaction = trans;

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
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
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
		public RaspaResult ModRule(RecRule value, string Utente, MySqlConnection conn=null, MySqlTransaction trans=null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE RULE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `50_RULES`";
				sql += " SET `NOME` = @NOME";
				sql += "    ,`DESCRIZIONE` = @DESCRIZIONE";
				
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
					mySqlCommand.Parameters.AddWithValue("@NOME", value.NOME);
					mySqlCommand.Parameters.AddWithValue("@DESCRIZIONE", value.DESCRIZIONE);

					mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
					mySqlCommand.Connection.Open();
					mySqlCommand.ExecuteNonQuery();
				}
				

				res = new RaspaResult(true, "Update RULE ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE : " + ex.Message);
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

		#endregion


		#region RULES SE
		public RecRules_item GetRules_itemByID(int ID)
		{
			RecRules_item res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `51_RULES_ITEM`";
				sql += " WHERE ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						ObservableCollection<RecRules_item> result = GetRecRule_item(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE SE : " + ex.Message);
			}
			return res;
		}
		public ObservableCollection<RecRules_item> GetRules_itemByID_RULE(int ID_RULE)
		{
			ObservableCollection<RecRules_item> res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `51_RULES_ITEM`";
				sql += " WHERE ID_RULE = @ID_RULE";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID_RULE", ID_RULE);
						mySqlCommand.Connection.Open();

						res = GetRecRule_item(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE ITEM : " + ex.Message);
			}
			return res;
		}

		private ObservableCollection<RecRules_item> GetRecRule_item(MySqlCommand mySqlCommand)
		{
			ObservableCollection<RecRules_item> res = new ObservableCollection<RecRules_item>();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						RecRules_item item = new RecRules_item();

						item.ID = reader.GetInt32("ID");

						if (!reader.IsDBNull(reader.GetOrdinal("ID_RULE")))
							item.ID_RULE = reader.GetInt32("ID_RULE");
						else
							item.ID_RULE = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("ID_Componente")))
							item.ID_Componente = reader.GetInt32("ID_Componente");
						else
							item.ID_Componente = 0;


						if (!reader.IsDBNull(reader.GetOrdinal("Tipo")))
							item.Tipo =  (enumRulesType)reader.GetInt32("Tipo");
						else
							item.Tipo = enumRulesType.nessuna;

						if (!reader.IsDBNull(reader.GetOrdinal("POS")))
							item.POS = reader.GetInt32("POS");
						else
							item.POS = 0;

						// NOME
						if (!reader.IsDBNull(reader.GetOrdinal("NOME")))
							item.NOME = reader.GetString("NOME");
						else
							item.NOME = "";

						if (!reader.IsDBNull(reader.GetOrdinal("Condizione")))
							item.Condizione = (enumRulesValore)reader.GetInt32("Condizione");
						else
							item.Condizione = enumRulesValore.nessuna;

						if (!reader.IsDBNull(reader.GetOrdinal("Valore")))
							item.Valore = reader.GetString("Valore");
						else
							item.Valore = "";

						res.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE SE : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetRule_item(ObservableCollection<RecRules_item> values, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(0);
			try
			{
				if (values == null)
					return new RaspaResult(false, "oggetto non impostato");

				foreach (RecRules_item rec in values)
				{
					res = SetRule_item(rec, Utente, conn, trans);
					if (!res.Esito)
						return res;
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE ITEM : " + ex.Message);
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
		public RaspaResult SetRule_item(RecRules_item value, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(0);
			RecRules_item role = null;
            MySqlConnection mySqlConnection = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				if (value.ID.HasValue)
					role = GetRules_itemByID(value.ID.Value);
				if (role == null)
					res = InsRule_item(value, Utente, conn, trans);
				else
				{
					res = ModRule_item(value, Utente, conn, trans);
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE ITEM : " + ex.Message);
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
		public RaspaResult InsRule_item(RecRules_item value, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{
				string sql = "";
				sql += "INSERT INTO `51_RULES_ITEM`";
				sql += " (`ID_RULE`,`Tipo`,`ID_Componente`,`POS`,`NOME`,`Condizione`,`Valore`)";
				sql += " VALUES";
				sql += " (@ID_RULE,@Tipo,@ID_Componente,@POS,@NOME,@Condizione,@Valore);";
				sql += " select LAST_INSERT_ID() as ID;";

				if (conn != null)
					mySqlConnection = conn;
				else
					mySqlConnection = new MySqlConnection(GetConnectionString());

				using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
				{
					mySqlCommand.CommandText = sql;
					mySqlCommand.Parameters.AddWithValue("@ID_RULE", value.ID_RULE);
					mySqlCommand.Parameters.AddWithValue("@Tipo", value.Tipo);
					mySqlCommand.Parameters.AddWithValue("@ID_Componente", value.ID_Componente);
					mySqlCommand.Parameters.AddWithValue("@POS", value.POS);
					mySqlCommand.Parameters.AddWithValue("@NOME", value.NOME);
					mySqlCommand.Parameters.AddWithValue("@Condizione", value.Condizione);
					mySqlCommand.Parameters.AddWithValue("@Valore", value.Valore);
					mySqlCommand.Connection.Open();

					using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
					{
						if (reader.HasRows)
						{
							reader.Read();
							int ID = reader.GetInt32("ID");
							res = new RaspaResult(true, "Inserimento RULE ITEM " + ID + "eseguito con successo");
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
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE SE : " + ex.Message);
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
		public RaspaResult ModRule_item(RecRules_item value, string Utente, MySqlConnection conn = null, MySqlTransaction trans = null)
		{
			RaspaResult res = new RaspaResult(false);
            MySqlConnection mySqlConnection = null;
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE RULE ITEM con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `51_RULES_ITEM`";
				sql += " SET `ID_RULE` = @ID_RULE";
				sql += "    ,`Tipo` = @Tipo";
				sql += "    ,`ID_Componente` = @ID_Componente";
				sql += "    ,`POS` = @POS";
				sql += "    ,`NOME` = @NOME";
				sql += "    ,`Condizione` = @Condizione";
				sql += "    ,`Valore` = @Valore";

				sql += " WHERE `ID` = @ID;";

				if (conn != null)
					mySqlConnection = conn;
				else
					mySqlConnection = new MySqlConnection(GetConnectionString());

				using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
				{
					mySqlCommand.CommandText = sql;
					mySqlCommand.Parameters.AddWithValue("@ID", value.ID);
					mySqlCommand.Parameters.AddWithValue("@ID_RULE", value.ID_RULE);
					mySqlCommand.Parameters.AddWithValue("@Tipo", value.Tipo);
					mySqlCommand.Parameters.AddWithValue("@ID_Componente", value.ID_Componente);
					mySqlCommand.Parameters.AddWithValue("@POS", value.POS);
					mySqlCommand.Parameters.AddWithValue("@NOME", value.NOME);
					mySqlCommand.Parameters.AddWithValue("@Condizione", value.Condizione);
					mySqlCommand.Parameters.AddWithValue("@Valore", value.Valore);

					mySqlCommand.Connection.Open();
					mySqlCommand.ExecuteNonQuery();
				}
				
				res = new RaspaResult(true, "Update RULE ITEM ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE ITEM : " + ex.Message);
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

		public RaspaResult DelRule_itemByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `51_RULES_ITEM`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - RULE ITEM : " + ex.Message);
			}
			return res;
		}

		#endregion


	}
}
