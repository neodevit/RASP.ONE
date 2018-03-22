using MySql.Data.MySqlClient;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaDB
{
    public partial class DBCentral
    {

		public Comandi GetComandi()
		{
			Comandi res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `40_COMMANDS`";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecCommand(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public Comando GetComandoByID(int ID)
		{
			Comando res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `40_COMMANDS`";
				sql += " WHERE ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						Comandi result = GetRecCommand(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public Comando GetComandoByIDSubscription(int IDSubscription)
		{
			Comando res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `40_COMMANDS`";
				sql += " WHERE IDSubscription = @IDSubscription";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDSubscription", IDSubscription);
						mySqlCommand.Connection.Open();

						Comandi result = GetRecCommand(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public Comando GetComandoByHostname(string HostName)
		{
			Comando res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*";
				sql += " FROM `40_COMMANDS` AS A";
				sql += " LEFT join `20_SUBSTRIPTION` AS S on S.ID = A.IDSubscription ";
				sql += " LEFT join `10_NODO` AS N on N.ID = S.NODE_subscriber ";
				sql += " WHERE N.HostName = @HostName";


				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@HostName", HostName);
						mySqlCommand.Connection.Open();

						Comandi result = GetRecCommand(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public Comando GetComandoByMacAddress(string MacAddress)
		{
			Comando res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*";
				sql += " FROM `40_COMMANDS` AS A";
				sql += " LEFT join `20_SUBSTRIPTION` AS S on S.ID = A.IDSubscription ";
				sql += " LEFT join `10_NODO` AS N on N.ID = S.NODE_subscriber ";
				sql += " WHERE N.MacAddress = @MacAddress";


				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@MacAddress", MacAddress);
						mySqlCommand.Connection.Open();

						Comandi result = GetRecCommand(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public Comando GetComandoByIPv4(string IPv4)
		{
			Comando res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.Enabled,";
				sql += " FROM `40_COMMANDS` AS A";
				sql += " LEFT join `20_SUBSTRIPTION` AS S on S.ID = A.IDSubscription ";
				sql += " LEFT join `10_NODO` AS N on N.ID = S.NODE_subscriber ";
				sql += " WHERE N.IPv4 = @IPv4";


				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4);
						mySqlCommand.Connection.Open();

						Comandi result = GetRecCommand(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}

		private Comandi GetRecCommand(MySqlCommand mySqlCommand)
		{
			Comandi res = new Comandi();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Comando item = new Comando();

						item.ID = reader.GetInt32("ID");

						
						if (!reader.IsDBNull(reader.GetOrdinal("IDSubscription")))
							item.IDSubscription = reader.GetInt32("IDSubscription");
						else
							item.IDSubscription = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("Valore")))
							item.Valore = reader.GetString("Valore");
						else
							item.Valore = "";

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
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public RaspaResult SetCommand(int Nodo,int Pin,string valore,string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			Comando comando = null;
			try
			{
				// Leggo tutti i Nodi registrati a HOST PIN
				Subscriptions subscriptions = GetSubscriptionByNODOSubscriptionAndPIN(Nodo, Pin);
				if (subscriptions != null)
				{
					// LEggo tutti i sottoscritti all'evento
					foreach (Subscription subscription in subscriptions)
					{
						if (subscription.ID.HasValue)
						{
							// Salvo il comando per i sottoscrittori
							comando = new Comando();
							comando.IDSubscription = subscription.ID.Value;
							comando.Valore = valore;
							res = InsCommand(comando, Utente);
						}
					}
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public Collection<string> ExtractMessageUdp(int Nodo, int Pin)
		{
			Collection<string> res = new Collection<string>();
			try
			{
				// Leggo tutti i Nodi registrati a HOST PIN
				Subscriptions subscriptions = GetSubscriptionByNODOSubscriptionAndPIN(Nodo, Pin);
				if (subscriptions != null)
				{
					// LEggo tutti i sottoscritti all'evento
					foreach (Subscription subscription in subscriptions)
					{
						if (subscription.ID.HasValue)
						{
							res.Add(subscription.NODE_IPv4);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetCommand(Comando value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			Comando comando = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				if (value.ID.HasValue)
					comando = GetComandoByID(value.ID.Value);
				if (comando == null)
					res = InsCommand(value, Utente);
				else
					res = ModCommand(value, Utente);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public RaspaResult InsCommand(Comando value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				string sql = "";
				sql += "INSERT INTO `40_COMMANDS`";
				sql += " (`IDSubscription`,`Valore`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@IDSubscription,@Valore,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IDSubscription", value.IDSubscription);
						mySqlCommand.Parameters.AddWithValue("@Valore", value.Valore);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();

						using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
						{
							if (reader.HasRows)
							{
								reader.Read();
								int ID = reader.GetInt32("ID");
								res = new RaspaResult(true, "Inserimento COMMAND " + ID + "eseguito con successo");
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
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModCommand(Comando value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE COMMAND con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `40_COMMANDS`";
				sql += " SET `IDSubscription` = @IDSubscription";
				sql += "    ,`Valore` = @Valore";
				
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
						mySqlCommand.Parameters.AddWithValue("@Valore", value.Valore);

						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update COMMAND ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DetCommandByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `40_COMMANDS`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - COMMAND : " + ex.Message);
			}
			return res;
		}
	}
}
