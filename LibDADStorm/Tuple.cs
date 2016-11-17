using System;
using System.Collections.Generic;

namespace DADStorm{

	[Serializable()]
	public class Tuple : EventArgs {

		List<string> items;

		public Tuple(params string[] args){
			items = new List<string>(args);
		}

		public string Get(int index){
			return items.GetRange(index,1)[0];
		}

		public Boolean Contains(string item){
			return items.Contains(item);
		}

		public void Add(string item){
			items.Add(item);
		}

		public override string ToString(){
			return string.Join(",", items.ToArray());
		}
	}
}
