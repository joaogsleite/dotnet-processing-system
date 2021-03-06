﻿using System;
using System.Collections.Generic;

namespace DADStorm
{
	[Serializable]
	public class FilterOp : Operator{

		private int field;
		private string compare;
		private string val;

		public FilterOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			: base(id, input_ops, input_files, routing, replicas_url, options) { 

			this.field = Int32.Parse(options.Split(',')[0]);
			this.compare = options.Split(',')[1];
			this.val = options.Split(',')[2];

			/*if(this.val.Substring(0,1)=="'" || this.val.Substring(0,1)=="\"")
				this.val = this.val.Substring(1);	
			if(this.val.Substring(this.val.Length-1)=="'" || this.val.Substring(this.val.Length-1)=="\"")
				this.val = this.val.Substring(0,this.val.Length-1);*/
		}

		public override List<Tuple> execute(Tuple tuple){

            List<Tuple> res = new List<Tuple>();

			if(compare=="="){
                if (tuple.Get(field).Equals(val))
                    res.Add(tuple);
			}
			if(compare==">"){
				try{
					if (float.Parse(tuple.Get(field)) > float.Parse(val))
						res.Add(tuple);
				}catch (Exception){
					if (string.Compare(tuple.Get(field), val) > 0)
						res.Add(tuple);
				}

			}
			if(compare=="<"){
				try{
					if (float.Parse(tuple.Get(field)) < float.Parse(val))
						res.Add(tuple);
				}catch (Exception){
					if (string.Compare(tuple.Get(field), val) < 0)
                        res.Add(tuple);
                }
			}
			return res;
		}
	}
}
