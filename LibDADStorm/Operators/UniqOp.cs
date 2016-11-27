using System;
using System.Collections.Generic;

namespace DADStorm
{
	[Serializable]
	public class UniqOp : Operator{

		private int field = 0;

		private List<Tuple> tuples = new List<Tuple>();

		public UniqOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			: base(id, input_ops, input_files, routing, replicas_url, options) { 

			this.field = Int32.Parse(options);
		}

		public override List<Tuple> execute(Tuple tuple){
            List<Tuple> res = new List<Tuple>();
			foreach(Tuple t in tuples){
				if(t.Get(field)==tuple.Get(field))
					return null;
			}
			tuples.Add(tuple);
            res.Add(tuple);
			return res;
		}


	}
}
