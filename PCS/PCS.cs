using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace DADStorm {
	
	public class PCS : MarshalByRefObject, IPCS{

		//List<TcpServerChannel> open_connections = new List<TcpServerChannel>();

		public PCS() : base() { }

		public void createReplica(Operator op, string url){
			Thread thread = new Thread(() => new RegisterReplica(op,url));
			thread.Start();
		}

		public static void Main(string[] args){

			Console.WriteLine("PCS");

			TcpChannel channel = new TcpChannel(10000);
			ChannelServices.RegisterChannel(channel, false);

			PCS pcs = new PCS();
			RemotingServices.Marshal(pcs, "pcs", typeof(IPCS));

			// Dont close console
			Console.ReadLine();
		}
		private class RegisterReplica {
			public RegisterReplica(Operator op, string url){
				int port = Int32.Parse(url.Split(':')[2].Split('/')[0]);
				string uri = url.Split('/')[url.Split('/').Length - 1];

				BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
				provider.TypeFilterLevel = TypeFilterLevel.Full;
				IDictionary props = new Hashtable();
				props["port"] = port;
				props["name"] = "tcp" + port;

				TcpServerChannel channel = new TcpServerChannel(props,provider);
				ChannelServices.RegisterChannel(channel, false);

				Replica replica = new Replica(op, url);
				RemotingServices.Marshal(replica, uri, typeof(Replica));
				replica.Start();

				Console.WriteLine("Replica " + url + " created with operator " + op.id);
			}
		}
	}
}
