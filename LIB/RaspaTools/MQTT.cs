using MQTTnet;
using MQTTnet.Client;
using MQTTnet.ManagedClient;
using MQTTnet.Protocol;
using MQTTnet.Server;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaspaTools
{
	public delegate void MQTTMessage(string Topic, string Payload, MqttQualityOfServiceLevel QoS, bool Retain);
	public delegate void MQTTLog(string messaggio);

	public class MQTT
	{
		public event MQTTMessage Receive = delegate { };
		public event MQTTLog Logging = delegate { };
		public string ClientID;
		public string ClientIP;
		public string ClientName;

		public const string MQTTServer = "192.168.1.69"; // SYNOLOGY
		public const int Port = 1883;
		private const string ServerUser = "sassoribelle";
		private const string ServerPass = "123456Ab";

		private List<string> ConnectedSubscription;
		public MQTT(string NodeID,string NodeName,string NodeIP, List<string> subscription=null)
		{
			ClientID = NodeID;
			ClientIP = NodeIP;
			ClientName = NodeName;
			// --------------------
			// sottoscrizioni
			// --------------------
			ConnectedSubscription = subscription;
			// se null creo
			if (ConnectedSubscription == null)
				ConnectedSubscription = new List<string>();
			// se non sottoscrive il suo ip aggiungo
			if (!ConnectedSubscription.Contains(NodeIP))
				ConnectedSubscription.Add(NodeIP);
		}


		#region SERVER
		IMqttServer SERVER;
		public async void createServer()
		{
			try
			{
				writeLog("SERVER Creating ... " + ClientIP);

				// create server
				SERVER = new MqttFactory().CreateMqttServer();

				writeLog("SERVER CREATED " + ClientIP);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("SERVER CREATE - Error : " + ex.Message);
			}
		}
		public async void startServer()
		{
			try
			{
				writeLog("SERVER Starting ... " + ClientIP);

				// Option
				var optionsBuilder = new MqttServerOptionsBuilder()
									.WithConnectionBacklog(100)
									.WithDefaultEndpointPort(Port)
									.WithConnectionValidator(ServerSecurity)
									.WithApplicationMessageInterceptor(ServerTimeStamp)
									.WithDefaultCommunicationTimeout(TimeSpan.FromSeconds(20))
									.WithSubscriptionInterceptor(Subscription);
				// START
				await SERVER.StartAsync(optionsBuilder.Build());

				writeLog("SERVER STARTED " + ClientIP);

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("SERVER START - Error : " + ex.Message);
			}
		}
		public async void stopServer()
		{
			try
			{
				writeLog("SERVER Stoping ... " + ClientIP);

				await SERVER.StopAsync();

				writeLog("SERVER STOPPED " + ClientIP);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("SERVER STOP - Error : " + ex.Message);
			}
		}

		private void ServerSecurity(MqttConnectionValidatorContext c)
		{
			try
			{
				//if (c.ClientId.Length < 10)
				//{
				//	c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedIdentifierRejected;
				//	return;
				//}

				if (c.Username != ServerUser)
				{
					c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
					return;
				}

				if (c.Password != ServerPass)
				{
					c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
					return;
				}

				c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
			}
			catch(Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("SERVER SECURITY - Error : " + ex.Message);
			}
		}

		private void ServerTimeStamp(MqttApplicationMessageInterceptorContext c)
		{
			if (MqttTopicFilterComparer.IsMatch(c.ApplicationMessage.Topic, "/myTopic/WithTimestamp/#"))
			{
				// Replace the payload with the timestamp. But also extending a JSON 
				// based payload with the timestamp is a suitable use case.
				c.ApplicationMessage.Payload = Encoding.UTF8.GetBytes(DateTime.Now.ToString("O"));
			}
		}

		private void Subscription(MqttSubscriptionInterceptorContext c)  
		{
			try
			{
				if (c.TopicFilter.Topic.StartsWith("admin/foo/bar") && c.ClientId != "theAdmin")
				{
					c.AcceptSubscription = false;
				}

				if (c.TopicFilter.Topic.StartsWith("the/secret/stuff") && c.ClientId != "Imperator")
				{
					c.AcceptSubscription = false;
					c.CloseConnection = true;
				}
			}
			catch(Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("SERVER SUBSCRIPTION - Error : " + ex.Message);
			}
		}




		#endregion

		#region CLIENT
		bool flgManaged = true;
		IManagedMqttClient CLIENT_managed;
		IMqttClient CLIENT;
		public async void createClient()
		{
			try
			{
				writeLog("CLIENT Creating ...");

				// CLIENT MANAGED
				if (flgManaged)
					CLIENT_managed = new MqttFactory().CreateManagedMqttClient();
				else
					CLIENT = new MqttFactory().CreateMqttClient();

				writeLog("CLIENT CREATED");
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("CLIENT CREATE - Error : " + ex.Message);
			}

		}
		public async void startClient()
		{
			try
			{
				writeLog("CLIENT Starting ...");



				if (flgManaged)
				{
					//OPTIONS
					// Create TCP based options using the builder.
					var options_managed = new ManagedMqttClientOptionsBuilder()
					.WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
					.WithClientOptions(new MqttClientOptionsBuilder()
						.WithClientId(ClientID)
						.WithTcpServer(MQTTServer) //"test.mosquitto.org"
						.WithCredentials(ServerUser, ServerPass)
						//.WithTls()
						.WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
						.WithCommunicationTimeout(TimeSpan.FromSeconds(20))
						.WithCleanSession(true)
						.Build());


					// START
					await CLIENT_managed.StartAsync(options_managed.Build());

					/// CONNECTED
					CLIENT_managed.Connected += Client_Connected;
					//await CLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic(createTopic(ClientIP)).Build());

					// RICEZIONE MESSAGI
					CLIENT_managed.ApplicationMessageReceived += Client_ApplicationMessageReceived;
				}
				else
				{
					var options = new MqttClientOptionsBuilder()
						.WithClientId(ClientID)
						.WithTcpServer(MQTTServer)
						.WithCredentials(ServerUser, ServerPass)
						.WithTls()
						.WithCleanSession()
						.Build();


					await CLIENT.ConnectAsync(options);

					/// CONNECTED
					CLIENT.Connected += Client_Connected;
					//await CLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic(createTopic(ClientIP)).Build());

					// RICEZIONE MESSAGI
					CLIENT.ApplicationMessageReceived += Client_ApplicationMessageReceived;

				}

				writeLog("CLIENT STARTED");

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("CLIENT START - Error : " + ex.Message);
			}

		}
		public async void stopClient()
		{
			try
			{
				writeLog("CLIENT Stopping ...");

				if (flgManaged)
					await CLIENT_managed.StopAsync();

				writeLog("CLIENT STOPPED");

			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("CLIENT STOP - Error : " + ex.Message);
			}

		}

		private void Client_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
		{
			try
			{
				Receive?.Invoke(e.ApplicationMessage.Topic,
								Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
								e.ApplicationMessage.QualityOfServiceLevel,
								e.ApplicationMessage.Retain);

				writeLog("<-- " + e.ApplicationMessage.Topic);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("<--- Error : " + ex.Message);
			}
		}

		private async void Client_Connected(object sender, MQTTnet.Client.MqttClientConnectedEventArgs e)
		{
			try
			{
				writeLog("CLIENT CONNECT");

				if (ConnectedSubscription!=null)
					foreach (string subscription in ConnectedSubscription)
						Subscribe(subscription);

				writeLog("CLIENT SUBSCRIPTION - CLIENT IP : " + ClientIP);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("CLIENT CONNECT/SUBSRIPTION - Error : " + ex.Message);
			}
		}

		#region SEND
		public void Publish(RaspaProtocol protocollo)
		{
			string Topic = "";
			try
			{
				// ---------------------------------------
				// PREVENT LOOP
				// ---------------------------------------
				// ip loop
				if (protocollo.SubcribeDestination == enumSubribe.IPv4 &&
					protocollo.Destinatario.IPv4 == ClientIP)
				{
					writeLog("CLIENT SEND - LOOP PREVENT : " + "Destinatario e mittente hanno lo stesso IP");
					return;
				}
				// topic loop
				if (protocollo.SubcribeDestination != enumSubribe.IPv4 &&
					ConnectedSubscription.Contains(protocollo.SubcribeDestination.ToString()))
				{
					writeLog("CLIENT SEND - LOOP PREVENT : " + "Destinatario e mittente hanno lo stesso TOPIC");
					return;
				}

				// ---------------------------------------
				// CALCOLATE TOPIC
				// ---------------------------------------
				// compongo il topic se IP (punto-punto) o se famiglia di sottoscrittori (punto-molti)
				if (protocollo.SubcribeDestination == enumSubribe.IPv4)
					Topic = createTopic(protocollo.Destinatario.IPv4);
				else
					Topic = createTopic(protocollo.SubcribeDestination.ToString());

				// ---------------------------------------
				// PUBLISH
				// ---------------------------------------
				publishcommand(Topic, protocollo.BuildJson());

				writeLog("--> " + Topic + " - " + protocollo.Comando.ToString() + "Componente " + protocollo.Mittente.Tipo.ToString() +  " Azione " + protocollo.Azione.ToString() + "  value:" + protocollo.Mittente.ValueFor_writeDB());
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("--> Error : " + ex.Message);
			}
		}
		private async void publishcommand(string TOPIC,string Message)
		{
			try
			{
				var message = new MqttApplicationMessageBuilder()
					.WithTopic(TOPIC)
					.WithPayload(Message)
					.WithExactlyOnceQoS()
					.WithRetainFlag()
					.Build();

				if (flgManaged)
					await CLIENT_managed.PublishAsync(message);
				else
					await CLIENT.PublishAsync(message);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("--> Error : " + ex.Message);
			}
		}
		#endregion

		// Sottoscrivi eventi
		// RASPA/NODE/NOME
		public async void Subscribe(string Topic)
		{
			try
			{
				writeLog("CLIENT Subscribing ... " + Topic);

				string TopicSubscript = createTopic(Topic);


				if (flgManaged)
					await CLIENT_managed.SubscribeAsync(new TopicFilterBuilder().WithTopic(TopicSubscript).Build());
				else
					await CLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic(TopicSubscript).Build());

				writeLog("CLIENT SUBSCRIBED " + Topic);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
			}
		}

		public bool IsConnected()
		{
			bool res = false;
			try
			{
				if (flgManaged)
					res = CLIENT_managed.IsConnected;
				else
					res = CLIENT.IsConnected;
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
			}
			return res;
		}

		#endregion

		private string createTopic(string IP)
		{
			return "RASP.ONE/" + IP;
		}

		#region LOG
		private void writeLog(string messaggio)
		{
			try
			{
				Logging?.Invoke(messaggio);
			}
			catch { }
		}
		#endregion
	}
}
