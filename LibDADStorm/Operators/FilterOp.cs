using System;
using System.Collections.Generic;

namespace DADStorm
{
	[Serializable]
	public class FilterOp : Operator{

		private int field = 0;
		private string compare = "=";
		private string val = "dog";

		public FilterOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			: base(id, input_ops, input_files, routing, replicas_url, options) { }

		public override Tuple execute(Tuple tuple){
			if(compare=="="){
				if(tuple.Get(field)==val)
					return tuple;
			}
			if(compare==">"){
				if(Int32.Parse(tuple.Get(field)) > Int32.Parse(val))
					return tuple;
			}
			if(compare=="<"){
				if(Int32.Parse(tuple.Get(field)) < Int32.Parse(val))
					return tuple;
			}
			return null;
		}
	}
}
