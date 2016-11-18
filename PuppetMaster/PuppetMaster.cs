using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Sockets;
using System.Threading;

namespace DADStorm {
    class PuppetMaster {

		Dictionary<string, IPCS> pcs = new Dictionary<string, IPCS>();
		List<string> replicas_url = new List<string>();
		Dictionary<string,IReplica> replicas_by_url = new Dictionary<string, IReplica>();
		Dictionary<string,Operator> operators = new Dictionary<string,Operator>();
		public delegate void RemoteAsyncDelegate();
		public delegate void IntervalRemoteAsyncDelegate(int interval);

		public PuppetMaster() {
			// Windows.Forms GUI
			//Application.EnableVisualStyles();
			//Application.SetCompatibleTextRenderingDefault(false);
			//Application.Run(new Form1());

		}

		void CreateOperators(List<Operator> ops) {
			foreach (Operator op in ops){
				operators.Add(op.id,op);
				Console.Write("\n" + op.id + ": ");
				foreach (string replica_url in op.replicas_url){
					Console.Write(replica_url + ", ");
					CreateReplica(op, replica_url);
					replicas_url.Add(replica_url);
				}	
			}
		}

		void ConnectToReplicas(){
			int i = 0;
			foreach(string repl_url in replicas_url){
				IReplica repl = (IReplica)Activator.GetObject(typeof(IReplica), repl_url);
				replicas_by_url.Add(repl_url, repl);
				i++;
			}	
		}

		Boolean ReplicasReady(){
			foreach(KeyValuePair<string, IReplica> repl in replicas_by_url){
				if(!replicas_by_url[repl.Key].ready())
					return false;
			}	
			return true;
		}

		void CreateReplica(Operator op, string replica_url){
			string[] parts = replica_url.Split(':');
			string machine = parts[1];

			string pcs_url = "tcp:" + machine + ":10000/pcs";

			try {
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

		void LoadConfigCommands(List<string> commands){
			foreach(string cmd in commands){
				Console.WriteLine(cmd);
				if(cmd.Contains("Start"))
					StartOp(cmd.Split(' ')[1]);
				if(cmd.Contains("Interval"))
					Interval(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
				//if(cmd.Contains("Status"))
				//	Status();    TODO !!!
				if(cmd.Contains("Crash"))
					Crash(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
				if(cmd.Contains("Freeze"))
					Freeze(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
				if(cmd.Contains("Unfreeze"))
					Unfreeze(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
				if(cmd.Contains("Wait"))
					Thread.Sleep(Int32.Parse(cmd.Split(' ')[1]));
			}
		}
		private void StartOp(string id){
			foreach(string repl_url in operators[id].replicas_url){
				RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[repl_url].Start);
				RemoteDel.BeginInvoke(null, null);
			}	
		}
		private void Interval(string id, int interval){
			foreach(string repl_url in operators[id].replicas_url){
				IntervalRemoteAsyncDelegate RemoteDel = new IntervalRemoteAsyncDelegate(replicas_by_url[repl_url].Interval);
				RemoteDel.BeginInvoke(interval, null, null);
			}
		}
		private void Crash(string op_id, int repl_id){
			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[operators[op_id].replicas_url[repl_id]].Crash);
			RemoteDel.BeginInvoke(null, null);
		}
		private void Freeze(string op_id, int repl_id){
			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[operators[op_id].replicas_url[repl_id]].Freeze);
			RemoteDel.BeginInvoke(null, null);
		}
		private void Unfreeze(string op_id, int repl_id){
			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[operators[op_id].replicas_url[repl_id]].Unfreeze);
			RemoteDel.BeginInvoke(null, null);
		}

		private void asyncReplCmd(){
			


		}

		[STAThread]
        static void Main() {

			Console.WriteLine("PUPPET MASTER");

			PuppetMaster pm = new PuppetMaster();

			Parser p = new Parser(@"dadstorm.config");
			pm.CreateOperators(p.operators());

			pm.ConnectToReplicas();

			pm.LoadConfigCommands(p.commands());

			Console.ReadLine();
        }
    }
}
