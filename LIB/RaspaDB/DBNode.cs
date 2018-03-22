using SQLite.Net;
using SQLite.Net.Attributes;
using SQLite.Net.Platform.WinRT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaDB
{
    public partial class DBNode
    {
		String DB_NAME = "node.sqlite";

		string Server = "192.168.1.69";
		string User = "SassoSec";
		string Password = "123456";
		string Istance = "SassoSec";
		string Port = "3307";
		string IPCentrale = "192.168.1.10";

		public DBNode()
		{
			if (!File.Exists(GetPathDB()))
				CreateDB();

		}

		public string GetPathDB()
		{
			return Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, DB_NAME);
		}
		public void CreateDB()
		{
				CreateDB_Table_Config();
		}
		public void CreateDB_Table_Config()
		{
			using (SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), GetPathDB()))
			{
				conn.CreateTable<NodeConfig>();
				// DB Central Mysql
				conn.Insert(new NodeConfig() { Key = "Mysql_Server", Value = "192.168.1.69" });
				conn.Insert(new NodeConfig() { Key = "Mysql_Istance", Value = "SassoSec" });
				conn.Insert(new NodeConfig() { Key = "Mysql_User", Value = "SassoSec" });
				conn.Insert(new NodeConfig() { Key = "Mysql_Password", Value = "123456" });
				conn.Insert(new NodeConfig() { Key = "Mysql_Port", Value = "3307" });
				// Message Server
				conn.Insert(new NodeConfig() { Key = "Message_Server", Value = "192.168.1.10" });
			}
		}

		#region MYSQL CENTRAL
		public string getConnectionStringDBCentral()
		{
			string res = "";
			try
			{
				NodeConfig configSetting = null;
				// Server
				configSetting = getNodeConfigByKey("Mysql_Server");
				if (configSetting == null)
					return res;
				Server = configSetting.Value;

				// ISTANCE
				configSetting = getNodeConfigByKey("Mysql_Istance");
				if (configSetting == null)
					return res;
				Istance = configSetting.Value;

				// USER
				configSetting = getNodeConfigByKey("Mysql_User");
				if (configSetting == null)
					return res;
				User = configSetting.Value;

				// PASS
				configSetting = getNodeConfigByKey("Mysql_Password");
				if (configSetting == null)
					return res;
				Password = configSetting.Value;

				// PORT
				configSetting = getNodeConfigByKey("Mysql_Port");
				if (configSetting == null)
					return res;
				Port = configSetting.Value;

				// COMPOSE CONN STRING MYSQL CENTRAL
				res = "server=" + Server + ";user=" + User + ";database=" + Istance + ";port=" + Port + ";password=" + Password + ";SslMode=None;Convert Zero Datetime=True;";
			}
			catch (Exception ex)
			{

			}
			return res;
		}
		public DBCentral getDBCentral()
		{
			DBCentral res = null;
			try
			{
				NodeConfig configSetting = null;
				// Server
				configSetting = getNodeConfigByKey("Mysql_Server");
				if (configSetting == null)
					return res;
				Server = configSetting.Value;

				// ISTANCE
				configSetting = getNodeConfigByKey("Mysql_Istance");
				if (configSetting == null)
					return res;
				Istance = configSetting.Value;

				// USER
				configSetting = getNodeConfigByKey("Mysql_User");
				if (configSetting == null)
					return res;
				User = configSetting.Value;

				// PASS
				configSetting = getNodeConfigByKey("Mysql_Password");
				if (configSetting == null)
					return res;
				Password = configSetting.Value;

				// PORT
				configSetting = getNodeConfigByKey("Mysql_Port");
				if (configSetting == null)
					return res;
				Port = configSetting.Value;

				// COMPOSE CONN STRING MYSQL CENTRAL
				res = new DBCentral(Server, Istance, User, Password, Port);
			}
			catch (Exception ex)
			{

			}
			return res;
		}
		public string getMessageServer()
		{
			string res = "";
			try
			{
				NodeConfig configSetting = null;
				// Server
				configSetting = getNodeConfigByKey("Message_Server");
				if (configSetting == null)
					return res;
				res = configSetting.Value;
			}
			catch (Exception ex)
			{

			}
			return res;
		}

		#endregion
		private void insNodeConfig(string key, string value)
		{
			using (SQLiteConnection conn = new SQLite.Net.SQLiteConnection(new SQLitePlatformWinRT(), GetPathDB()))
			{
				var s = conn.Insert(new NodeConfig()
				{
					Key = key,
					Value = value
				});
			}
		}
		private NodeConfig getNodeConfigByKey(string key)
		{
			NodeConfig res = null;
			try
			{
				using (SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), GetPathDB()))
				{
					List<NodeConfig> recs = conn.Query<NodeConfig>("select * from NodeConfig where Key =?",key).ToList();
					if (recs.Count > 0)
						res = recs[0];
				}
			}
			catch (Exception ex)
			{

			}
			return res;
		}
		private bool delNodeConfigByKey(string key)
		{
			bool res = false;
			try
			{
				using (SQLiteConnection conn = new SQLiteConnection(new SQLitePlatformWinRT(), GetPathDB()))
				{
					//List<NodeConfig> recs = conn.Table<NodeConfig>().ToList();
					int i = conn.Execute("DELETE FROM NodeConfig WHERE Key = ?", key);
					res = (i == 0) ? false : true;
				}
			}
			catch (Exception ex)
			{
				res = false;
			}
			return res;
		}





	}

	public class NodeConfig
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }
		public string Key { get; set; }
		public string Value { get; set; }
	}
}
