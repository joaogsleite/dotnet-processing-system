using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace DADStorm{
	public class Replica : MarshalByRefObject {

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
			ReadInputFiles();
		}

		public string toString(){
			return "Replica " + url + " of operator " + op.id;
		}

		public void Start(){

			SubscribeAll();

			processing = true;
			while(processing){
				if(queue.Count > 0 & Send != null){
					Tuple output = op.execute(queue.Dequeue());
					Console.WriteLine("OUTPUT"+output);
					if(output != null)
						Send(this, (EventArgs)output);
				}
				Thread.Sleep(1000);
			}
		}

		private void ReadInputFiles(){
			foreach(string path in op.input_files){
				string[] lines = System.IO.File.ReadAllLines(@path);
				foreach (string line in lines)
					queue.Enqueue(new Tuple(line.Split(',')));
			}
		}

		private void SubscribeAll(){
			if(subscribed) return;
			foreach(Operator input in op.input_ops){
				foreach(string repl_url in input.replicas_url)
					Subscribe((Replica)Activator.GetObject(typeof(Replica), repl_url));		
			}
			subscribed = true;
		}
		public void Subscribe(Replica repl){
			repl.Send += new Replica.SendHandler(this.Receive);
		}
		private void Receive(Replica repl, EventArgs e){
			Tuple tuple = (Tuple)e;
			queue.Enqueue(tuple);
			Console.WriteLine(op.id+url+" received new tuple from "+repl.op.id+repl.url+" => "+e);
		}
	}
}
