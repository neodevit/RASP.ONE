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

		public Subscriptions GetSubscription(int Node_subscriber)
		{
			Subscriptions res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecSubscription(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public Subscription GetSubscriptionByID(int ID)
		{
			Subscription res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE ID = @ID";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						Subscriptions result = GetRecSubscription(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public Subscriptions GetSubscriptionByNodeSubscriber(int NODE_subscriber)
		{
			Subscriptions res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE NODE_subscriber = @NODE_subscriber";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NODE_subscriber", NODE_subscriber);
						mySqlCommand.Connection.Open();

						res = GetRecSubscription(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public Subscriptions GetSubscriptionByNODOSubscription(int NODO_Subscription)
		{
			Subscriptions res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE NODO_Subscription = @NODO_Subscription";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NODO_Subscription", NODO_Subscription);
						mySqlCommand.Connection.Open();

						res = GetRecSubscription(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public Subscriptions GetSubscriptionByNODOSubscriptionAndPIN(int NODO_Subscription,int PIN_Subscription)
		{
			Subscriptions res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE NODO_Subscription = @NODO_Subscription";
				sql += " AND   PIN_Subscription = @PIN_Subscription";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NODO_Subscription", NODO_Subscription);
						mySqlCommand.Parameters.AddWithValue("@PIN_Subscription", PIN_Subscription);
						mySqlCommand.Connection.Open();

						res = GetRecSubscription(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public Subscription GetSubscriptionByNODOSubscriberAndSubscriptionAndPin(int NODE_subscriber,int NODO_Subscription, int PIN_Subscription)
		{
			Subscription res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE NODE_subscriber = @NODE_subscriber";
				sql += " AND   NODO_Subscription = @NODO_Subscription";
				sql += " AND   PIN_Subscription = @PIN_Subscription";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NODE_subscriber", NODO_Subscription);
						mySqlCommand.Parameters.AddWithValue("@NODO_Subscription", NODO_Subscription);
						mySqlCommand.Parameters.AddWithValue("@PIN_Subscription", PIN_Subscription);
						mySqlCommand.Connection.Open();

						Subscriptions result = GetRecSubscription(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}

		public Subscriptions GetSubscriptionByNODOSubscriberHostname(string HostName)
		{
			Subscriptions res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE N.HostName = @HostName";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@HostName", HostName);
						mySqlCommand.Connection.Open();

						res = GetRecSubscription(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public Subscriptions GetSubscriptionByNODOSubscriberMacAddress(string MacAddress)
		{
			Subscriptions res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE N.MacAddress = @MacAddress";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@MacAddress", MacAddress);
						mySqlCommand.Connection.Open();

						res = GetRecSubscription(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public Subscriptions GetSubscriptionByNODOSubscriberIPv4(string IPv4)
		{
			Subscriptions res = null;
			try
			{
				string sql = "";
				sql += "SELECT A.*,N.IPv4";
				sql += " FROM `20_SUBSTRIPTION` AS A";
				sql += " LEFT join `10_NODO` AS N on N.Num = A.NODE_subscriber ";
				sql += " WHERE N.IPv4 = @IPv4";
				sql += " AND   N.Enabled = 1";
				sql += " AND   N.Trusted = 1";
				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4);
						mySqlCommand.Connection.Open();

						res = GetRecSubscription(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}


		private Subscriptions GetRecSubscription(MySqlCommand mySqlCommand)
		{
			Subscriptions res = new Subscriptions();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Subscription item = new Subscription();

						item.ID = reader.GetInt32("ID");

						
						if (!reader.IsDBNull(reader.GetOrdinal("NODE_subscriber")))
							item.NODE_subscriber = reader.GetInt32("NODE_subscriber");
						else
							item.NODE_subscriber = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("NODO_Subscription")))
							item.NODO_Subscription = reader.GetInt32("NODO_Subscription");
						else
							item.NODO_Subscription = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("PIN_Subscription")))
							item.PIN_Subscription = reader.GetInt32("PIN_Subscription");
						else
							item.PIN_Subscription = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("IPv4")))
							item.NODE_IPv4 = reader.GetString("IPv4");
						else
							item.NODE_IPv4 = "";
						

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
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetSubscription(Subscription value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			Subscription subscription = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				subscription = GetSubscriptionByNODOSubscriberAndSubscriptionAndPin(value.NODE_subscriber,value.NODO_Subscription,value.PIN_Subscription);
				if (subscription == null)
					res = InsSubscription(value, Utente);
				else
				{
					value.ID = subscription.ID;
					res = ModSubscription(value, Utente);
				}
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public RaspaResult InsSubscription(Subscription value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				string sql = "";
				sql += "INSERT INTO `20_SUBSTRIPTION`";
				sql += " (`NODE_subscriber`,`NODO_Subscription`,`PIN_Subscription`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@NODE_subscriber,@NODO_Subscription,@PIN_Subscription,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NODE_subscriber", value.NODE_subscriber);
						mySqlCommand.Parameters.AddWithValue("@NODO_Subscription", value.NODO_Subscription);
						mySqlCommand.Parameters.AddWithValue("@PIN_Subscription", value.PIN_Subscription);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();

						using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
						{
							if (reader.HasRows)
							{
								reader.Read();
								int ID = reader.GetInt32("ID");
								res = new RaspaResult(true, "Inserimento SUBSTRIPTION " + ID + "eseguito con successo");
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
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModSubscription(Subscription value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE SUBSTRIPTION con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `20_SUBSTRIPTION`";
				sql += " SET `NODE_subscriber` = @NODE_subscriber";
				sql += "    ,`NODO_Subscription` = @NODO_Subscription";
				sql += "    ,`PIN_Subscription` = @PIN_Subscription";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", value.ID.Value);
						mySqlCommand.Parameters.AddWithValue("@NODE_subscriber", value.NODE_subscriber);
						mySqlCommand.Parameters.AddWithValue("@NODO_Subscription", value.NODO_Subscription);
						mySqlCommand.Parameters.AddWithValue("@PIN_Subscription", value.PIN_Subscription);

						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update SUBSTRIPTION ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DelSubscriptionByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `20_SUBSTRIPTION`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - SUBSTRIPTION : " + ex.Message);
			}
			return res;
		}
	}
}
