using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;

namespace DADStorm{
	public class Replica : MarshalByRefObject, IReplica {

		private Operator op;
		private string url;
		private Queue<Tuple> queue = new Queue<Tuple>();
		private Boolean processing = false;
		private Boolean subscribed = false;
		public event SendHandler Send;
		private IPM pm;
		public delegate void SendHandler(Replica repl, EventArgs e);

		public Replica(string op_id, string repl_url, string pm_url){
			this.url = repl_url;

			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;
			IDictionary props = new Hashtable();
			props["name"] = "tcp_pm";
			TcpServerChannel channel = new TcpServerChannel(props,provider);
			ChannelServices.RegisterChannel(channel, false);
			pm = (IPM)Activator.GetObject(typeof(IPM), pm_url);

			this.op = pm.get_operator_by_id(op_id);

			string start_text = "[" + op_id + " " + url + "] started!";
			Console.WriteLine(start_text);
			pm.log(start_text);

			ConnectReplicas();
			ReadInputFiles();
		}

		public string toString(){
			return "[" + op.id + " " + url + "]";
		}

		public void Start(){

			processing = true;
			while(processing){
				if(queue.Count > 0 & Send != null){
					Tuple tuple = op.execute(queue.Dequeue());
					if(tuple != null){
						Send(this, (EventArgs)tuple);
						log("tuple "+url+", <"+tuple+">");
					}
				}
				Thread.Sleep(1000);
			}
		}

		public void Freeze(){
			processing = false;
		}

		public void Unfreeze(){
			Start();
		}

		public void Crash(){
			Environment.Exit(1);
		}

		public string Status(){
			string text = "[" + op.id + " " + url + "] ";
			text += processing ? "processing " : " ";
			Console.WriteLine(text);
			return text;
		}

		public void Interval(int time){
			Thread.Sleep(time);
		}

		private void ReadInputFiles(){
			foreach(string path in op.input_files){
				string[] lines = System.IO.File.ReadAllLines(@path);
				foreach (string line in lines)
					queue.Enqueue(new Tuple(line.Split(',')));
			}
		}

		private void ConnectReplicas(){
			if(subscribed) return;
			foreach(Operator input in op.input_ops){
				//foreach(string repl_url in input.replicas_url)
				//PRIMARY TODO !!!
				new Thread(()=>{
					Subscribe(input.replicas_url[0]);	
				}).Start();

			}
			subscribed = true;
		}

		public Boolean ready(){
			return subscribed;
		}

		private void Subscribe(String repl_url){
			Replica repl = null;
			Boolean success = false;
			while(!success){
				try{
					repl = (Replica)Activator.GetObject(typeof(Replica), repl_url);
					repl.Send += new Replica.SendHandler(this.Receive);
					success = true;
				} catch(Exception){
					Console.WriteLine("Retrying connect to "+repl_url);	
				}
				Thread.Sleep(1000);
			}

		}
		private void Receive(Replica repl, EventArgs e){
			queue.Enqueue((Tuple)e);
		}

		private void log(string text){
			pm.log(text);
		}

		public static string exe_path(){
			return @Environment.CurrentDirectory+"/Replica.exe";
		}

		public static void Main(string[] args){

			string repl_url = args[0];
			string op_id = args[1];
			string pm_url = args[2];

			int port = Int32.Parse(repl_url.Split(':')[2].Split('/')[0]);
			string uri = repl_url.Split('/')[repl_url.Split('/').Length - 1];

			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;
			IDictionary props = new Hashtable();
			props["port"] = port;
			props["name"] = "tcp" + port;

			TcpServerChannel channel = new TcpServerChannel(props,provider);
			ChannelServices.RegisterChannel(channel, false);

			Replica replica = new Replica(op_id,repl_url,pm_url);
			RemotingServices.Marshal(replica, uri, typeof(Replica));

			// Dont close console
			Console.ReadLine();
		}
	}
}
