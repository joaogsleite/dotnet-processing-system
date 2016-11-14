using System;
using System.Collections.Generic;

namespace DADStorm
{
	public class CountOp : Operator
	{

		public CountOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			: base(id, input_ops, input_files, routing, replicas_url, options) { }


	}
}
