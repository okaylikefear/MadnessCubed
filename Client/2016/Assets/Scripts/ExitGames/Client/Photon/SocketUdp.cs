using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Threading;

namespace ExitGames.Client.Photon
{
	internal class SocketUdp : IPhotonSocket, IDisposable
	{
		public SocketUdp(PeerBase npeer) : base(npeer)
		{
			if (base.ReportDebugOfLevel(DebugLevel.ALL))
			{
				base.Listener.DebugReturn(DebugLevel.ALL, "CSharpSocket: UDP, Unity3d.");
			}
			base.Protocol = ConnectionProtocol.Udp;
			this.PollReceive = false;
		}

		public void Dispose()
		{
			base.State = PhotonSocketState.Disconnecting;
			if (this.sock != null)
			{
				try
				{
					if (this.sock.Connected)
					{
						this.sock.Close();
					}
				}
				catch (Exception arg)
				{
					base.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Dispose(): " + arg);
				}
			}
			this.sock = null;
			base.State = PhotonSocketState.Disconnected;
		}

		public override bool Connect()
		{
			object obj = this.syncer;
			bool result;
			lock (obj)
			{
				if (!base.Connect())
				{
					result = false;
				}
				else
				{
					base.State = PhotonSocketState.Connecting;
					new Thread(new ThreadStart(this.DnsAndConnect))
					{
						Name = "photon dns thread",
						IsBackground = true
					}.Start();
					result = true;
				}
			}
			return result;
		}

		public override bool Disconnect()
		{
			if (base.ReportDebugOfLevel(DebugLevel.INFO))
			{
				base.EnqueueDebugReturn(DebugLevel.INFO, "Disconnect()");
			}
			base.State = PhotonSocketState.Disconnecting;
			object obj = this.syncer;
			lock (obj)
			{
				if (this.sock != null)
				{
					try
					{
						this.sock.Close();
					}
					catch (Exception arg)
					{
						base.EnqueueDebugReturn(DebugLevel.INFO, "Exception in Disconnect(): " + arg);
					}
					this.sock = null;
				}
			}
			base.State = PhotonSocketState.Disconnected;
			return true;
		}

		public override PhotonSocketError Send(byte[] data, int length)
		{
			object obj = this.syncer;
			lock (obj)
			{
				if (!this.sock.Connected)
				{
					return PhotonSocketError.Skipped;
				}
				try
				{
					this.sock.Send(data, 0, length, SocketFlags.None);
				}
				catch (Exception ex)
				{
					if (base.ReportDebugOfLevel(DebugLevel.ERROR))
					{
						base.EnqueueDebugReturn(DebugLevel.ERROR, "Cannot send to: " + base.ServerAddress + ". " + ex.Message);
					}
					return PhotonSocketError.Exception;
				}
			}
			return PhotonSocketError.Success;
		}

		public override PhotonSocketError Receive(out byte[] data)
		{
			data = null;
			return PhotonSocketError.NoData;
		}

		internal void DnsAndConnect()
		{
			try
			{
				object obj = this.syncer;
				lock (obj)
				{
					this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					IPAddress ipAddress = IPhotonSocket.GetIpAddress(base.ServerAddress);
					this.sock.Connect(ipAddress, base.ServerPort);
					base.State = PhotonSocketState.Connected;
				}
			}
			catch (SecurityException ex)
			{
				if (base.ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' failed: " + ex.ToString());
				}
				base.HandleException(StatusCode.SecurityExceptionOnConnect);
				return;
			}
			catch (Exception ex2)
			{
				if (base.ReportDebugOfLevel(DebugLevel.ERROR))
				{
					base.Listener.DebugReturn(DebugLevel.ERROR, "Connect() to '" + base.ServerAddress + "' failed: " + ex2.ToString());
				}
				base.HandleException(StatusCode.ExceptionOnConnect);
				return;
			}
			new Thread(new ThreadStart(this.ReceiveLoop))
			{
				Name = "photon receive thread",
				IsBackground = true
			}.Start();
		}

		public void ReceiveLoop()
		{
			byte[] array = new byte[base.MTU];
			while (base.State == PhotonSocketState.Connected)
			{
				try
				{
					int length = this.sock.Receive(array);
					base.HandleReceivedDatagram(array, length, true);
				}
				catch (Exception ex)
				{
					if (base.State != PhotonSocketState.Disconnecting && base.State != PhotonSocketState.Disconnected)
					{
						if (base.ReportDebugOfLevel(DebugLevel.ERROR))
						{
							base.EnqueueDebugReturn(DebugLevel.ERROR, string.Concat(new object[]
							{
								"Receive issue. State: ",
								base.State,
								". Server: '",
								base.ServerAddress,
								"' Exception: ",
								ex
							}));
						}
						base.HandleException(StatusCode.ExceptionOnReceive);
					}
				}
			}
			this.Disconnect();
		}

		private Socket sock;

		private readonly object syncer = new object();
	}
}
