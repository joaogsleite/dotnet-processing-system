using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace DADStorm {
	
	public class PCS : MarshalByRefObject, IPCS{

		public string pm_url = "tcp://localhost:10001/pm";

		public PCS() : base() {}

		public void createReplica(string op_id, string repl_url){

			string args = repl_url+" "+op_id+" "+pm_url;
			string exe_path = Replica.exe_path();

			ProcessStartInfo info = new ProcessStartInfo(exe_path, args); 
			info.CreateNoWindow = false; 

			Process.Start(info);
		}

		public static string GetLocalIPAddress(){
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			return "localhost";
		}

		public static void Main(string[] args){

			Console.Clear();
			Console.WriteLine();
			Console.WriteLine("PCS");
			Console.WriteLine("listening on tcp://"+PCS.GetLocalIPAddress() + ":10000/pcs");

			TcpChannel channel = new TcpChannel(10000);
			ChannelServices.RegisterChannel(channel, false);

			PCS pcs = new PCS();
			RemotingServices.Marshal(pcs, "pcs", typeof(IPCS));

			// Dont close console
			Console.ReadLine();
		}
	}
}
