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

		public GPIOPins GetGPIOPin()
		{
			GPIOPins res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `60_GPIO_PIN`";
				sql += " ORDER BY NUM asc";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecGPIOPin(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}
		public GPIOPins GetGPIOPinTipe(List<enumTipoPIN> tipi)
		{
			GPIOPins res = null;
			try
			{
				string sql = "";
				string where = "";
				sql += "SELECT *";
				sql += " FROM `60_GPIO_PIN`";
				sql += " WHERE 1=1";
				foreach (enumTipoPIN tipo in tipi)
				{
					where += (where=="")? " AND " : " OR ";
					where += " Tipo=" + (int)tipo;
				}
				sql += where;
				sql += " ORDER BY NUM asc";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecGPIOPin(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}

		public GPIOPin GetGPIOPinByID(int ID)
		{
			GPIOPin res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `60_GPIO_PIN`";
				sql += " WHERE ID = @ID";
				sql += " ORDER BY NUM";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						GPIOPins result = GetRecGPIOPin(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}

		public GPIOPin GetGPIOPinByGPIOnum(int numGPIO)
		{
			GPIOPin res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `60_GPIO_PIN`";
				sql += " WHERE GPIO = @GPIO";
				sql += " ORDER BY GPIO";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@GPIO", numGPIO);
						mySqlCommand.Connection.Open();

						GPIOPins result = GetRecGPIOPin(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}
		public GPIOPin GetGPIOPinByPINnum(int numPIN)
		{
			GPIOPin res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `60_GPIO_PIN`";
				sql += " WHERE NUM = @NUM";
				sql += " ORDER BY NUM";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NUM", numPIN);
						mySqlCommand.Connection.Open();

						GPIOPins result = GetRecGPIOPin(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}

		public GPIOPin GetGPIOPinByNomeGPIO(string NomeGPIO)
		{
			GPIOPin res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `60_GPIO_PIN`";
				sql += " WHERE TRIM(LOWER(NomeGPIO)) = @NomeGPIO";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NomeGPIO", NomeGPIO.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						GPIOPins result = GetRecGPIOPin(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}
		public GPIOPin GetGPIOPinByNomeNUM(string NomeNUM)
		{
			GPIOPin res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `60_GPIO_PIN`";
				sql += " WHERE TRIM(LOWER(NomeNUM)) = @NomeNUM";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@NomeNUM", NomeNUM.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						GPIOPins result = GetRecGPIOPin(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}

		private GPIOPins GetRecGPIOPin(MySqlCommand mySqlCommand)
		{
			GPIOPins res = new GPIOPins();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						GPIOPin item = new GPIOPin();

						item.ID = reader.GetInt32("ID");

						if (!reader.IsDBNull(reader.GetOrdinal("Tipo")))
							item.Tipo = (enumTipoPIN)reader.GetInt32("Tipo");
						else
							item.Tipo = enumTipoPIN.other;

						if (!reader.IsDBNull(reader.GetOrdinal("GPIO")))
							item.GPIO = Convert.ToInt32(reader.GetString("GPIO"));
						else
							item.GPIO = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("NUM")))
							item.NUM = reader.GetInt32("NUM");
						else
							item.NUM = 0;

						if (!reader.IsDBNull(reader.GetOrdinal("NomeGPIO")))
							item.NomeGPIO = reader.GetString("NomeGPIO");
						else
							item.NomeGPIO = "";

						if (!reader.IsDBNull(reader.GetOrdinal("NomeNUM")))
							item.NomeNUM = reader.GetString("NomeNUM");
						else
							item.NomeNUM = "";

						if (!reader.IsDBNull(reader.GetOrdinal("Descrizione")))
							item.Descrizione = reader.GetString("Descrizione");
						else
							item.Descrizione = "";

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
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetGPIOPin(GPIOPin value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			GPIOPin ele = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				if (value.ID.HasValue)
					ele = GetGPIOPinByID(value.ID.Value);
				if (ele == null)
					res = InsGPIOPin(value, Utente);
				else
					res = ModGPIOPin(value, Utente);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}
		public RaspaResult InsGPIOPin(GPIOPin value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				string sql = "";
				sql += "INSERT INTO `60_GPIO_PIN`";
				sql += " (`Tipo`,`GPIO`,`NUM`,`NomeGPIO`,`NomeGPIO`,`Descrizione`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@Tipo,@GPIO,@NUM,@NomeGPIO,@NomeGPIO,@Descrizione,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Tipo", (int)value.Tipo);
						mySqlCommand.Parameters.AddWithValue("@GPIO", value.GPIO);
						mySqlCommand.Parameters.AddWithValue("@NUM", value.NUM);
						mySqlCommand.Parameters.AddWithValue("@NomeGPIO", value.NomeGPIO);
						mySqlCommand.Parameters.AddWithValue("@NomeNUM", value.NomeNUM);
						mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();

						using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
						{
							if (reader.HasRows)
							{
								reader.Read();
								int ID = reader.GetInt32("ID");
								res = new RaspaResult(true, "Inserimento GPIO PIN " + ID + " eseguito con successo");
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
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModGPIOPin(GPIOPin value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE GPIO PIN con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `60_GPIO_PIN`";
				sql += " SET `Tipo` = @Tipo";
				sql += "    ,`GPIO` = @GPIO";
				sql += "    ,`NUM` = @NUM";
				sql += "    ,`NomeGPIO` = @NomeGPIO";
				sql += "    ,`NomeNUM` = @NomeNUM";
				sql += "    ,`Descrizione` = @Descrizione";

				sql += "    ,`UserMod` = @Utente";
				sql += "    ,`DataMod` = NOW()";
				sql += " WHERE `ID` = @ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", value.ID.Value);
						mySqlCommand.Parameters.AddWithValue("@Tipo", (int)value.Tipo);
						mySqlCommand.Parameters.AddWithValue("@GPIO", value.GPIO);
						mySqlCommand.Parameters.AddWithValue("@NUM", value.NUM);
						mySqlCommand.Parameters.AddWithValue("@NomeGPIO", value.NomeGPIO);
						mySqlCommand.Parameters.AddWithValue("@NomeNUM", value.NomeNUM);
						mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);

						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update GPIO PIN ID " + value.ID.Value + " Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DelGPIOPinByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `60_GPIO_PIN`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - GPIO PIN : " + ex.Message);
			}
			return res;
		}
	}
}
