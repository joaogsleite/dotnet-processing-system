using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Sockets;

namespace DADStorm {
    class PuppetMaster {

		Dictionary<string, IPCS> pcs = new Dictionary<string, IPCS>();

		public PuppetMaster(){

			// Windows.Forms GUI
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new Form1());

		}

		void createOperators(List<Operator> ops) {
			foreach (Operator op in ops){
				Console.Write("\n" + op.id + ": ");
				foreach (string replica_url in op.replicas_url){
					Console.Write(replica_url + ", ");
					createReplica(op, replica_url);
				}	
			}
		}

		void createReplica(Operator op, string replica_url){
			string[] parts = replica_url.Split(':');
			string machine = parts[1];

			string pcs_url = "tcp:" + machine + ":10000/pcs";

			try{
				if (!pcs.ContainsKey(pcs_url)){
					TcpChannel channel = new TcpChannel();
					ChannelServices.RegisterChannel(channel, false);
					pcs.Add(pcs_url, (IPCS)Activator.GetObject(typeof(IPCS), pcs_url));
				}
				pcs[pcs_url].createReplica(op,replica_url);
			}
			catch (RemotingException e){
				Console.WriteLine(e.ToString());
			}
			catch (SocketException){
				System.Console.WriteLine("Could not locate server");
			}
		}

		[STAThread]
        static void Main() {

			Console.WriteLine("PUPPET MASTER");

			PuppetMaster pm =new PuppetMaster();

			Parser p = new Parser(@"dadstorm.config");
			pm.createOperators(p.operators());

			Console.ReadLine();
        }
    }
}
