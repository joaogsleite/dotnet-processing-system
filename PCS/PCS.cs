using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace DADStorm {
	
	public class PCS : MarshalByRefObject, IPCS{

		public PCS() : base() { }

		public string hello(string name){
			Console.WriteLine(name + " called method hello!");
			return "Hello " + name;
		}

		public void createReplica(Operator op, string url){
			Console.WriteLine("Replica " + url + " created with operator "+op.id);
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
	}
}
