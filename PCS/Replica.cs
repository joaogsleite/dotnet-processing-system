using System;
namespace DADStorm
{
	public class Replica : MarshalByRefObject {

		private Operator op;
		private string url;

		public Replica(Operator op, string url){
			this.op = op;
			this.url = url;
		}

		public string toString(){
			return "Replica " + url + " of operator " + op.id;
		}
	}
}
