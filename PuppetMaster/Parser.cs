using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DADStorm
{
	public class Parser{

		private string content;
		private char[] delimiters = { ' ', '\n', '\t' };

		public Parser(string path){
			string[] lines = System.IO.File.ReadAllLines(path);
			this.content = "";
			foreach (string line in lines){
				if (!line.Contains("%"))
					this.content += line + "\n";
			}
		}

		public string logging_level(){
			return content.Split(new string[]{"LoggingLevel "},StringSplitOptions.None)[1].Split(delimiters)[0];
		}

		public List<string> commands(){
			string[] lines = content.Split('\n');
			List<string> res = new List<string>();
			foreach (string line in lines){
				Regex r = new Regex("Start|Status|Crash|Freeze|Unfreeze|Interval|Wait");
				if (r.IsMatch(line)) res.Add(line);
			}
			return res;
		}

		public List<Operator> operators(){

			string[] ops = content.Split(new string[]{"input ops"}, StringSplitOptions.None);
			Dictionary<string,Operator> res = new Dictionary<string,Operator>();

			string id = ops[0].Split(delimiters)[ops[0].Split(delimiters).Length - 2];
			Operator op = new Operator(id);

			for (int i = 1; i < ops.Length; i++){
				string[] inputs = ops[i].Split(new string[] { "rep fact" }, StringSplitOptions.None)[0].Split(',');
				foreach (string input in inputs){
					if (input.Contains(".")) op.addInputFile(input.Trim(delimiters));
					else op.addInputOperator(res[input.Trim(delimiters)]);
				}
				op.setRouting(ops[i].Split(new string[] { "routing" }, StringSplitOptions.None)[1].Split(' ')[1]);
				string[] address = ops[i].Split(new string[] { "address" }, StringSplitOptions.None)[1].Split(new string[] { "operator spec" }, StringSplitOptions.None)[0].Split(',');
				foreach (string a in address) op.addReplica(a.Trim(delimiters));

				string[] spec = ops[i].Split(new string[] { "operator spec" }, StringSplitOptions.None)[1].Split(delimiters);
				op.setOptions(spec[2]);

				res.Add(id, op.changeMode(spec[1]));
				id = ops[i].Split(delimiters)[ops[i].Split(delimiters).Length - 2];
				op = new Operator(id);
			}
			return new List<Operator>(res.Values);
		}
	}
}