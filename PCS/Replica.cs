using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;

namespace DADStorm
{
	public class Replica : MarshalByRefObject {

		private Operator op;
		private string url;

		public event SendHandler Send;
		public EventArgs e = null;
		public delegate void SendHandler(Replica repl, EventArgs e);

		public Replica(Operator op, string url){
			this.op = op;
			this.url = url;
		}

		public void Start(){
			System.Threading.Thread.Sleep(10000);
			foreach(Operator input in op.input_ops){
				foreach(string repl_url in input.replicas_url){
					Replica repl = (Replica)Activator.GetObject(typeof(Replica), url);
					Subscribe(repl);
				}
			}
			while (true){
				System.Threading.Thread.Sleep(3000);
				if (Send != null)
					Send(this, e);
			}
		}

		public void Subscribe(Replica repl){
			repl.Send += new Replica.SendHandler(Receive);
		}
		private void Receive(Replica repl, EventArgs e){
			System.Console.WriteLine("OP"+op.id+" received new tuple from OP"+repl.op.id);
		}

		public string toString(){
			return "Replica " + url + " of operator " + op.id;
		}
	}
}
