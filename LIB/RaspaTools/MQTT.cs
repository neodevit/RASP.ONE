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

		public MQTT(int NodeID,string NodeName,string NodeIP)
		{
			ClientID = NodeID.ToString();
			ClientIP = NodeIP;
			ClientName = NodeName;
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
		IManagedMqttClient CLIENT;
		public async void createClient()
		{
			try
			{
				writeLog("CLIENT Creating ...");

				// CLIENT
				CLIENT = new MqttFactory().CreateManagedMqttClient();

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

				//OPTIONS
				// Create TCP based options using the builder.
				var options = new ManagedMqttClientOptionsBuilder()
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
				await CLIENT.StartAsync(options.Build());

				/// CONNECTED
				CLIENT.Connected += Client_Connected;
				//await CLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic(createTopic(ClientIP)).Build());

				// RICEZIONE MESSAGI
				CLIENT.ApplicationMessageReceived += Client_ApplicationMessageReceived;

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

				await CLIENT.StopAsync();

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
				writeLog("CLIENT Receinving ...");

				Receive?.Invoke(e.ApplicationMessage.Topic,
								Encoding.UTF8.GetString(e.ApplicationMessage.Payload),
								e.ApplicationMessage.QualityOfServiceLevel,
								e.ApplicationMessage.Retain);

				writeLog("CLIENT REVEICE " + e.ApplicationMessage.Topic);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("CLIENT RECEIVE - Error : " + ex.Message);
			}
		}

		private async void Client_Connected(object sender, MQTTnet.Client.MqttClientConnectedEventArgs e)
		{
			try
			{
				writeLog("CLIENT CONNECT");

				// ADD SUBSCRIPTION
				string TopicSubscript = createTopic(ClientIP);
				await CLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic(TopicSubscript).Build());
				writeLog("CLIENT SUBSCRIPTION " + TopicSubscript);
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
			try
			{
				writeLog("CLIENT Sending ... " + protocollo.Destinatario.IPv4);
				publishcommand(createTopic(protocollo.Destinatario.IPv4), protocollo.BuildJson());
				writeLog("CLIENT SENDED " + protocollo.Destinatario.IPv4);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("CLIENT SEND - Error : " + ex.Message);
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

				await CLIENT.PublishAsync(message);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
				writeLog("CLIENT SEND - Error : " + ex.Message);
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
				await CLIENT.SubscribeAsync(new TopicFilterBuilder().WithTopic(Topic).Build());
				writeLog("CLIENT SUBSCRIBED " + Topic);
			}
			catch (Exception ex)
			{
				if (Debugger.IsAttached) Debugger.Break();
			}
		}
		#endregion

		private string createTopic(string IP)
		{
			return "RAPSPA/" + IP;
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
