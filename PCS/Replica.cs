using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace DADStorm{
	public class Replica : MarshalByRefObject, IReplica {

		private Operator op;
		private string url;
		private Queue<Tuple> queue = new Queue<Tuple>();
		private Boolean processing = false;
		private Boolean subscribed = false;
		public event SendHandler Send;
		public delegate void SendHandler(Replica repl, EventArgs e);

		public Replica(Operator op, string url){
			this.op = op;
			this.url = url;
			Console.WriteLine(">> Start replica " + url + " with operator " + op.id);
			ConnectReplicas();
			ReadInputFiles();
		}

		public string toString(){
			return "Replica " + url + " of operator " + op.id;
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
			Thread.CurrentThread.Abort();
		}

		public string Status(){
			return op.id+" "+url+" => "+(processing?"processing":"");
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
			PCS.pm.log(text);
		}
	}
}
