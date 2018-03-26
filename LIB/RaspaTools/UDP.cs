using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Networking;
using RaspaEntity;
using Windows.ApplicationModel.Core;

namespace RaspaTools
{
	public delegate void SocketMessage(string messaggio);
	public delegate void SocketEsito(bool esito);
	public class UDP
	{
		StreamSocketListener SoketListener;
		private static string PortNumberReceive = "6969";
		private static string PortNumberConnect = "7070";
		public event SocketMessage Receive = delegate { };
		public event SocketMessage Logging = delegate { };
		public event SocketEsito ConnectionResult = delegate { };

		public UDP()
		{
			try
			{
			}
			catch (Exception ex)
			{

			}
		}
		#region LISTENER
		public async void StartListener()
		{
			try
			{
				writeLog("Connection ...");

				if (CoreApplication.Properties.ContainsKey("listener"))
					return;

				SoketListener = new StreamSocketListener();
				SoketListener.ConnectionReceived -= this.listener_ConnectionReceived;
				SoketListener.ConnectionReceived += this.listener_ConnectionReceived;

				// da valutare
				SoketListener.Control.KeepAlive = true;

				await SoketListener.BindServiceNameAsync(PortNumberReceive);

				CoreApplication.Properties.Add("listener", SoketListener);

				writeLog("Connected");
				ConnectionResult(true);
			}
			catch (Exception ex)
			{
				SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
				writeLog("Connection Error : " + ((webErrorStatus != null) ? webErrorStatus.ToString() : "") + " -" + ((ex != null) ? ex.Message : ""));
				ConnectionResult(false);
			}
		}
		private async void listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
		{
			string request = "";
			try
			{

				writeLog("Receiving ...");
				using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
				{
					request = await streamReader.ReadToEndAsync();
				}


				// Echo the request back as the response.
				//using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
				//{
				//	using (var streamWriter = new StreamWriter(outputStream))
				//	{
				//		await streamWriter.WriteLineAsync("OK");
				//		await streamWriter.FlushAsync();
				//	}
				//}

				// Evento di ricezione del risultato
				Receive?.Invoke(request);
				writeLog("Receive message");

			}
			catch (Exception ex)
			{
				writeLog("Connection Error : " + ex.Message);
			}
		}
		#endregion

		#region SENDER
		public async Task<bool> SendMessage(RaspaProtocol protocol)
		{
			bool esito = false;
			try
			{
				writeLog("Sending ...");

				// Create the StreamSocket and establish a connection to the echo server.
				using (StreamSocket socket = new StreamSocket())
				{
					// Connect
					socket.Control.KeepAlive = false;
					var hostName = new HostName(protocol.Destinatario.IPv4);
					await socket.ConnectAsync(hostName, PortNumberConnect);

					// Send a request to the echo server.
					using (Stream outputStream = socket.OutputStream.AsStreamForWrite())
					{
						using (var streamWriter = new StreamWriter(outputStream))
						{
							await streamWriter.WriteLineAsync(protocol.BuildJson());
							await streamWriter.FlushAsync();
						}
					}

					writeLog("Send to " + protocol.Destinatario.IPv4);

					// Read data from the echo server.
					//string response;
					//using (Stream inputStream = socket.InputStream.AsStreamForRead())
					//{
					//	using (StreamReader streamReader = new StreamReader(inputStream))
					//	{
					//		response = await streamReader.ReadLineAsync();
					//	}
					//}
					//esito = (response.ToUpperInvariant() == "OK") ? true : false;

					//writeLog("Send : " + response.ToUpperInvariant());

				}

			}
			catch (Exception ex)
			{
				SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
				writeLog("Connection Error : " + ((webErrorStatus != null) ? webErrorStatus.ToString() : "") + " -" + ((ex != null) ? ex.Message : ""));
			}
			return esito;
		}
		#endregion

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
