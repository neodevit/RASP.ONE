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

		public ComponenteTipi GetComponenteTipo()
		{
			ComponenteTipi res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `41_COMPONENTE_TIPO`";
				sql += " ORDER BY ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Connection.Open();

						res = GetRecComponenteTipo(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}
		public ComponenteTipi GetComponenteTipoEnabled(bool Enabled)
		{
			ComponenteTipi res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `41_COMPONENTE_TIPO`";
				sql += " WHERE Enabled=@Enabled";
				sql += " ORDER BY ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Enabled", Enabled);
						mySqlCommand.Connection.Open();

						res = GetRecComponenteTipo(mySqlCommand);
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}

		public ComponenteTipo GetComponenteTipoByID(int ID)
		{
			ComponenteTipo res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `41_COMPONENTE_TIPO`";
				sql += " WHERE ID = @ID";
				sql += " ORDER BY ID";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@ID", ID);
						mySqlCommand.Connection.Open();

						ComponenteTipi result = GetRecComponenteTipo(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}

		public ComponenteTipo GetComponenteTipoByNome(string Nome)
		{
			ComponenteTipo res = null;
			try
			{
				string sql = "";
				sql += "SELECT *";
				sql += " FROM `41_COMPONENTE_TIPO`";
				sql += " WHERE TRIM(LOWER(Nome)) = @Nome";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Nome", Nome.Trim().ToLowerInvariant());
						mySqlCommand.Connection.Open();

						ComponenteTipi result = GetRecComponenteTipo(mySqlCommand);
						if (result.Count > 0)
							res = result[0];
					}
				}
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}

		private ComponenteTipi GetRecComponenteTipo(MySqlCommand mySqlCommand)
		{
			ComponenteTipi res = new ComponenteTipi();
			try
			{
				using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
				{
					while (reader.Read())
					{
						ComponenteTipo item = new ComponenteTipo();

						item.ID = reader.GetInt32("ID");

						if (!reader.IsDBNull(reader.GetOrdinal("Enabled")))
							item.Enabled = reader.GetBoolean("Enabled");
						else
							item.Enabled = false;

						if (!reader.IsDBNull(reader.GetOrdinal("Nome")))
							item.Nome = reader.GetString("Nome");
						else
							item.Nome = "";

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
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}

		public RaspaResult SetComponenteTipo(ComponenteTipo value, string Utente)
		{
			RaspaResult res = new RaspaResult(0);
			ComponenteTipo ele = null;
			try
			{
				if (value == null)
					return new RaspaResult(false, "oggetto non impostato");

				if (value.ID.HasValue)
					ele = GetComponenteTipoByID(value.ID.Value);
				if (ele == null)
					res = InsComponenteTipo(value, Utente);
				else
					res = ModComponenteTipo(value, Utente);
			}
			catch (Exception ex)
			{
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				if (Debugger.IsAttached) Debugger.Break();
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}
		public RaspaResult InsComponenteTipo(ComponenteTipo value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				string sql = "";
				sql += "INSERT INTO `41_COMPONENTE_TIPO`";
				sql += " (`Enabled`,`Nome`,`Descrizione`,`UserIns`,`DataIns`,`UserMod`,`DataMod`)";
				sql += " VALUES";
				sql += " (@Enabled,@Nome,@Descrizione,@Utente,NOW(),@Utente,NOW());";
				sql += " select LAST_INSERT_ID() as ID;";

				using (MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString()))
				{
					using (MySqlCommand mySqlCommand = mySqlConnection.CreateCommand())
					{
						mySqlCommand.CommandText = sql;
						mySqlCommand.Parameters.AddWithValue("@Enabled", value.Enabled);
						mySqlCommand.Parameters.AddWithValue("@Nome", value.Nome);
						mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);
						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();

						using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
						{
							if (reader.HasRows)
							{
								reader.Read();
								int ID = reader.GetInt32("ID");
								res = new RaspaResult(true, "Inserimento COMPONENTE TIPO " + ID + " eseguito con successo");
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
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}
		public RaspaResult ModComponenteTipo(ComponenteTipo value, string Utente)
		{
			RaspaResult res = new RaspaResult(false);
			try
			{
				if (!value.ID.HasValue)
					return new RaspaResult(false, "UPDATE COMPONENTE TIPO con ID non valorizzato");

				string sql = "";
				sql += "UPDATE `41_COMPONENTE_TIPO`";
				sql += " SET `Enabled` = @Enabled";
				sql += "    ,`Nome` = @Nome";
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
						mySqlCommand.Parameters.AddWithValue("@Enabled", value.Enabled);
						mySqlCommand.Parameters.AddWithValue("@Nome", value.Nome);
						mySqlCommand.Parameters.AddWithValue("@Descrizione", value.Descrizione);

						mySqlCommand.Parameters.AddWithValue("@Utente", Utente);
						mySqlCommand.Connection.Open();
						mySqlCommand.ExecuteNonQuery();
					}
				}

				res = new RaspaResult(true, "Update COMPONENTE TIPO ID " + value.ID.Value + " Eseguito");
				res.ID = value.ID;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				res = new RaspaResult(false, enumLevel.error, ex.Message);
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}

		public RaspaResult DelComponenteTipoByID(int ID)
		{
			RaspaResult res = new RaspaResult(true);
			try
			{
				string sql = "";
				sql += "DELETE FROM `41_COMPONENTE_TIPO`";
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
				System.Diagnostics.Debug.WriteLine("DBCentral - COMPONENTE TIPO : " + ex.Message);
			}
			return res;
		}
	}
}
