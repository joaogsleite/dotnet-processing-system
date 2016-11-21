using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections;
using System.Threading;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;

namespace DADStorm {
    public class PuppetMaster : MarshalByRefObject, IPM {
    private Dictionary<string, IPCS> pcs = new Dictionary<string, IPCS>();
		private List<string> replicas_url = new List<string>();
		private Dictionary<string,IReplica> replicas_by_url = new Dictionary<string, IReplica>();
		public Dictionary<string,Operator> operators = new Dictionary<string,Operator>();
		private delegate void RemoteAsyncDelegate();
		private delegate void IntervalRemoteAsyncDelegate(int interval);
		private delegate string StatusRemoteAsyncDelegate();
		private Boolean full_logging = false;
        public Func<string,string> print_log;

		public PuppetMaster() {}

		public Operator get_operator_by_id(string op_id){
			return operators[op_id];
		}

		public void CreateOperators(List<Operator> ops) {
			foreach (Operator op in ops){
				operators.Add(op.id,op);
				foreach (string replica_url in op.replicas_url){
					new Thread(()=>{ CreateReplica(op, replica_url); }).Start();
					replicas_url.Add(replica_url);
				}	
			}
		}

		public void ConnectToReplicas(){
			foreach(string repl_url in replicas_url){
				new Thread(()=>{ connectToReplica(repl_url); }).Start();
			}
			while(replicas_by_url.Count != replicas_url.Count){
				Thread.Sleep(1000);
			}
		}

		private void connectToReplica(string repl_url){
			Boolean connected = false;
			while(!connected){
				try{
					IReplica repl = (IReplica)Activator.GetObject(typeof(IReplica), repl_url);
					replicas_by_url.Add(repl_url, repl);
					connected = true;
				}catch(Exception){
					print_log("Retring connect to "+repl_url);
				}
				Thread.Sleep(1000);
			}
		}

		private void CreateReplica(Operator op, string replica_url){
			string[] parts = replica_url.Split(':');
			string machine = parts[1];

			string pcs_url = "tcp:" + machine + ":10000/pcs";

			try {
				Monitor.Enter(pcs);
				if (!pcs.ContainsKey(pcs_url)){
					//TcpChannel channel = new TcpChannel();
					//ChannelServices.RegisterChannel(channel, false);
					BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
					provider.TypeFilterLevel = TypeFilterLevel.Full;
					IDictionary props = new Hashtable();
					props["name"] = replica_url;
					TcpServerChannel channel = new TcpServerChannel(props,provider);
					ChannelServices.RegisterChannel(channel, false);
					pcs.Add(pcs_url, (IPCS)Activator.GetObject(typeof(IPCS), pcs_url));
				}
				pcs[pcs_url].createReplica(op.id,replica_url);
				Monitor.Exit(pcs);
			}
			catch (RemotingException e){
				print_log(e.ToString());
			}
			catch (SocketException){
				print_log("Could not locate server");
			}
		}

		public void LoadCommands(List<string> commands){
			ThreadPool.QueueUserWorkItem(a => {
				foreach(string cmd in commands){
					if(cmd.Contains("Start"))
						StartOp(cmd.Split(' ')[1]);
					if(cmd.Contains("Interval"))
						Interval(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
					if(cmd.Contains("Status"))
						Status();
					if(cmd.Contains("Crash"))
						Crash(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
					if(cmd.Contains("Freeze"))
						Freeze(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
					if(cmd.Contains("Unfreeze"))
						Unfreeze(cmd.Split(' ')[1],Int32.Parse(cmd.Split(' ')[2]));
					if(cmd.Contains("Wait"))
						Thread.Sleep(Int32.Parse(cmd.Split(' ')[1]));
					Thread.Sleep(1000);
				}
			});
		}

		public void StartOp(string id){
			foreach(string repl_url in operators[id].replicas_url){
				RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[repl_url].Start);
				RemoteDel.BeginInvoke(null, null);
			}
			print_log(">> Start " + id);
		}
		public void Status(){
			foreach (string op_id in operators.Keys){
				foreach (string repl_url in operators[op_id].replicas_url){
					try{
						//AsyncCallback cb = new AsyncCallback(StatusCallBack);
						StatusRemoteAsyncDelegate RemoteDel = new StatusRemoteAsyncDelegate(replicas_by_url[repl_url].Status);
						//RemoteDel.BeginInvoke(cb, null);
						RemoteDel.BeginInvoke(null, null);
					}
					catch (Exception){
						log("[" + op_id + " " + repl_url + "] crashed!");
					}

				}
			}
			print_log(">> Status ");
		}
		public void StatusCallBack(IAsyncResult ar){
			StatusRemoteAsyncDelegate d = (StatusRemoteAsyncDelegate)((AsyncResult)ar).AsyncDelegate;
			log(d.EndInvoke(ar));
		}
		public void Interval(string id, int interval){
			foreach(string repl_url in operators[id].replicas_url){
				IntervalRemoteAsyncDelegate RemoteDel = new IntervalRemoteAsyncDelegate(replicas_by_url[repl_url].Interval);
				RemoteDel.BeginInvoke(interval, null, null);
			}
			print_log(">> Interval " + id+ " " +interval);
		}
		public void Crash(string op_id, int repl_id){
			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[operators[op_id].replicas_url[repl_id]].Crash);
			RemoteDel.BeginInvoke(null, null);
			print_log(">> Crash " + op_id + " " + repl_id);
		}
		public void Freeze(string op_id, int repl_id){
			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[operators[op_id].replicas_url[repl_id]].Freeze);
			RemoteDel.BeginInvoke(null, null);
			print_log(">> Freeze " + op_id + " " + repl_id);
		}
		public void Unfreeze(string op_id, int repl_id){
			RemoteAsyncDelegate RemoteDel = new RemoteAsyncDelegate(replicas_by_url[operators[op_id].replicas_url[repl_id]].Unfreeze);
			RemoteDel.BeginInvoke(null, null);
			print_log(">> Unfreeze " + op_id + " " + repl_id);
		}
		public void LoggingLevel(string level){
			if(level=="full")
				this.full_logging = true;
			if(level=="light")
				this.full_logging = false;
		}

		public void log(string text){
			if(full_logging && print_log!=null)
				print_log(text);
		}
    }
}
