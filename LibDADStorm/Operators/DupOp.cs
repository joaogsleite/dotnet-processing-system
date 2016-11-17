﻿using System;
using System.Collections.Generic;

namespace DADStorm
{
	[Serializable]
	public class DupOp : Operator {

		public DupOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			:base(id,input_ops,input_files,routing,replicas_url,options){ }

		public override Tuple execute(Tuple tuple){
			return tuple;
		}
	}
}
