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

		public Nodes GetNodes()
		{
			Nodes res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecNode(mySqlCommand);
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
		public Nodes GetNODEEnabled(bool Enabled)
		{
			Nodes res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE Enabled = @Enabled";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Enabled", Enabled);
						mySqlCommand.Connection.Open();

						res = GetRecNode(mySqlCommand);
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


		public Node GetNODEByID(int ID)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE ID = @ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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
		public Node GetNODEByNum(int Num)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE Num = @Num";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Num", Num);
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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

		public Node GetNODEByNome(string Nome)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE TRIM(LOWER(Nome)) = @Nome";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Nome", Nome.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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
		public Node GetNODEByHostName(string HostName)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE TRIM(LOWER(HostName)) = @HostName";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@HostName", HostName.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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
		public Node GetNODEByMacAddress(string MacAddress)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE TRIM(LOWER(MacAddress)) = @MacAddress";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@MacAddress", MacAddress.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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
		public Node GetNODEByHWAddress(string HWAddress)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE TRIM(LOWER(HWAddress)) = @HWAddress";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@HWAddress", HWAddress.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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

		public Node GetNODEByIPv4(string IPv4)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE TRIM(LOWER(IPv4)) = @IPv4";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv4", IPv4.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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
		public Node GetNODEByIPv6(string IPv6)
		{
			Node res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `10_NODO`";
				sql += " WHERE TRIM(LOWER(IPv6)) = @IPv6";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@IPv6", IPv6.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						Nodes result = GetRecNode(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
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

		private Nodes GetRecNode(MySqlCommand mySqlCommand)
		{
			Nodes res = new Nodes();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						Node item = new Node();

						item.ID = reader.GetInt32("ID");

						
						if (!reader.IsDBNull(reader.GetOrdinal("Num")))
							item.Num = reader.GetInt32("Num");
						else
							item.Num = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("Stato")))
							item.Stato = (enumStato)reader.GetInt32("Stato");
						else
							item.Stato = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("Enabled")))
							item.Enabled = reader.GetBoolean("Enabled");
						else
							item.Enabled = false;

						if (!reader.IsDBNull(reader.GetOrdinal("Trusted")))
							item.Trusted = reader.GetBoolean("Trusted");
						else
							item.Trusted = false;

						if (!reader.IsDBNull(reader.GetOrdinal("Nome")))
							item.Nome = reader.GetString("Nome");
						else
							item.Nome = "";

						if (!reader.IsDBNull(reader.GetOrdinal("Descrizione")))
							item.Descrizione = reader.GetString("Descrizione");
						else
							item.Descrizione = "";

						// NETWORK
						item.Network = new NetworkInfo();
						if (!reader.IsDBNull(reader.GetOrdinal("HostName")))
							item.Network.HostName = reader.GetString("HostName");
						else
							item.Network.HostName = "";
						
						if (!reader.IsDBNull(reader.GetOrdinal("MacAddress")))
							item.Network.MacAddress = reader.GetString("MacAddress");
						else
							item.Network.MacAddress = "";

						if (!reader.IsDBNull(reader.GetOrdinal("HWAddress")))
							item.Network.HWAddress = reader.GetString("HWAddress");
						else
							item.Network.HWAddress = "";

						if (!reader.IsDBNull(reader.GetOrdinal("BlueTooth")))
							item.Network.BlueTooth = reader.GetString("BlueTooth");
						else
							item.Network.BlueTooth = "";

						if (!reader.IsDBNull(reader.GetOrdinal("IPv4")))
							item.Network.IPv4 = reader.GetString("IPv4");
						else
							item.Network.IPv4 = "";

						if (!reader.IsDBNull(reader.GetOrdinal("IPv6")))
							item.Network.IPv6 = reader.GetString("IPv6");
						else
							item.Network.IPv6 = "";


						if (!reader.IsDBNull(reader.GetOrdinal("IPv6")))
							item.Network.IPv6 = reader.GetString("IPv6");
						else
							item.Network.IPv6 = "";



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

		public RaspaResult SetNODE(Node value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			Node nodo = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				if (value.ID.HasValue)
					nodo = GetNODEByID(value.ID.Value);
				if (nodo == null)
					res = InsNODE(value, Utente);
				else
					res = ModNODE(value, Utente);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - NODES : " + ex.Message);
			}
			return res;
		}
		public RaspaResult InsNODE(Node value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				string sql = "";
				sql += "INSERT INTO `10_NODO`";
				sql += " (`Enabled`,`Stato`,`Nome`,`Descrizione`,`HostName`,`MacAddress`,`HWAddress`,`BlueTooth`,`IPv4`,`IPv6`,`PositionLeft`,`PositionTop`,`PositionBottom`,`PositionRight,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@Enabled,@Stato,@Nome,@Descrizione,@HostName,@MacAddress,@HWAddress,@BlueTooth,@IPv4,@IPv6,@PositionLeft,@PositionTop,@PositionBottom,@PositionRight,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Enabled", value.Enabled);
						mySqlCommand.Parameters.AddWithValue("@Stato", ((value.Stato == enumStato.on) ? true : false));
						mySqlCommand.Parameters.AddWithValue("@Nome", value.Nome);
						mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);
						if (value.Network != null)
						{
							mySqlCommand.Parameters.AddWithValue("@HostName", value.Network.HostName);
							mySqlCommand.Parameters.AddWithValue("@MacAddress", value.Network.MacAddress);
							mySqlCommand.Parameters.AddWithValue("@HWAddress", value.Network.HWAddress);
							mySqlCommand.Parameters.AddWithValue("@BlueTooth", value.Network.BlueTooth);
							mySqlCommand.Parameters.AddWithValue("@IPv4", value.Network.IPv4);
							mySqlCommand.Parameters.AddWithValue("@IPv6", value.Network.IPv6);
						}
						mySqlCommand.Parameters.AddWithValue("@PositionLeft", value.PositionLeft);
						mySqlCommand.Parameters.AddWithValue("@PositionTop", value.PositionTop);
						mySqlCommand.Parameters.AddWithValue("@PositionBottom", value.PositionBottom);
						mySqlCommand.Parameters.AddWithValue("@PositionRight", value.PositionRight);


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
				System.Diagnostics.Debug.WriteLine("DBCentral - NODES : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModNODE(Node value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE NODE con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `10_NODO`";
				sql += " SET `Enabled` = @Enabled";
				sql += "    ,`Stato` = @Stato";
				
				sql += "    ,`Nome` = @Nome";
				sql += "    ,`Descrizione` = @Descrizione";

				sql += "    ,`HostName` = @HostName";
				sql += "    ,`MacAddress` = @MacAddress";
				sql += "    ,`HWAddress` = @HWAddress";
				sql += "    ,`BlueTooth` = @BlueTooth";
				sql += "    ,`IPv4` = @IPv4";
				sql += "    ,`IPv6` = @IPv6";

				sql += "    ,`PositionLeft` = @PositionLeft";
				sql += "    ,`PositionTop` = @PositionTop";
				sql += "    ,`PositionBottom` = @PositionBottom";
				sql += "    ,`PositionRight` = @PositionRight";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", value.ID.Value);
						mySqlCommand.Parameters.AddWithValue("@Enabled", value.Enabled);
						mySqlCommand.Parameters.AddWithValue("@Stato", ((value.Stato == enumStato.on) ? true : false));
						mySqlCommand.Parameters.AddWithValue("@Nome", value.Nome);
						mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);

						if (value.Network != null)
						{
							mySqlCommand.Parameters.AddWithValue("@HostName", value.Network.HostName);
							mySqlCommand.Parameters.AddWithValue("@MacAddress", value.Network.MacAddress);
							mySqlCommand.Parameters.AddWithValue("@HWAddress", value.Network.HWAddress);
							mySqlCommand.Parameters.AddWithValue("@BlueTooth", value.Network.BlueTooth);
							mySqlCommand.Parameters.AddWithValue("@IPv4", value.Network.IPv4);
							mySqlCommand.Parameters.AddWithValue("@IPv6", value.Network.IPv6);
						}

						mySqlCommand.Parameters.AddWithValue("@PositionLeft", value.PositionLeft);
						mySqlCommand.Parameters.AddWithValue("@PositionTop", value.PositionTop);
						mySqlCommand.Parameters.AddWithValue("@PositionBottom", value.PositionBottom);
						mySqlCommand.Parameters.AddWithValue("@PositionRight", value.PositionRight);

						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update NODE ID " + value.ID.Value + "Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - NODES : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DelNODEByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `10_NODO`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - NODES : " + ex.Message);
			}
			return res;
		}
	}
}
