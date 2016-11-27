using System;
using System.Collections.Generic;

namespace DADStorm
{
	[Serializable]
	public class CountOp : Operator{

		private List<Tuple> tuples = new List<Tuple>();

		public CountOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			: base(id, input_ops, input_files, routing, replicas_url, options) { }

		public override List<Tuple> execute(Tuple tuple){
			tuples.Add(tuple);
            int n = tuples.Count;
            List<Tuple> res = new List<Tuple>();
            res.Add(new Tuple(n+""));
            return res;
		}
	}
}
