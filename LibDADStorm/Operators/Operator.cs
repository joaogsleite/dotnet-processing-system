using System;
using System.Collections.Generic;

namespace DADStorm{
	[Serializable]
	public class Operator {

		public string id;
		public List<Operator> input_ops;
		public List<string> input_files;
		public string routing;
		public List<string> replicas_url;
		public string mode;
		public string options;
        public Boolean last = false;

		public Operator(string id){
			this.id = id;
			this.input_files = new List<string>();
			this.input_ops = new List<Operator>();
			this.replicas_url = new List<string>();
		}

		public Operator(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options){
			this.id = id;
			this.input_ops = input_ops;
			this.input_files = input_files;
			this.routing = routing;
			this.replicas_url = replicas_url;
			this.options = options;
		}

		public void addInputFile(string file){
			input_files.Add(file);
		}
		public void addInputOperator(Operator op){
			input_ops.Add(op);
		}

		public void setRouting(string routing){
			this.routing = routing;
		}

		public void addReplica(string address){
			this.replicas_url.Add(address);
		}
		public void setMode(string mode){
			this.mode = mode;
		}
		public void setOptions(string options){
			this.options = options;	
		}

		public Operator changeMode(string mode){
			if (mode.Equals("CUSTOM")) return new CustomOp(id, input_ops, input_files, routing, replicas_url, options);
			if (mode.Equals("UNIQ")) return new UniqOp(id, input_ops, input_files, routing, replicas_url, options);
			if (mode.Equals("COUNT")) return new CountOp(id, input_ops, input_files, routing, replicas_url, options);
			if (mode.Equals("FILTER")) return new FilterOp(id, input_ops, input_files, routing, replicas_url, options);
			if (mode.Equals("DUP")) return new DupOp(id, input_ops, input_files, routing, replicas_url, options);
			return null; 
		}

		public string toString(){
			mode = this.GetType().Name;
			string res = id + "-" + mode + " : ";
			foreach (Operator op in input_ops) res += op.id + ",";
			foreach (string file in input_files) res += file + ",";
			res += " " + routing + " ";
			foreach (string url in replicas_url) res += url + ",";
			res += " " + options;
			return res;
		}

		public virtual List<Tuple> execute(Tuple tuple){
            List<Tuple> tuples = new List<Tuple>();
            tuples.Add(tuple);
            return tuples;
		}
	}
}
