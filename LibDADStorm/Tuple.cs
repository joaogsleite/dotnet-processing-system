using System;
using System.Collections.Generic;

namespace DADStorm{

	[Serializable()]
	public class Tuple : EventArgs {

		List<string> items;

		public Tuple(params string[] args){
			items = new List<string>(args);
		}

        public Tuple() {
            items = new List<string>();
        }

        public void Merge(Tuple t) {
            for (int i = 1; i <= t.Count(); i++)
                items.Add(t.Get(i));
        }

        public string Get(int index){
			return items[index-1];
		}

		public Boolean Contains(string item){
			return items.Contains(item);
		}

        public void RemoveField(int index) {
            items.RemoveAt(index - 1);
        }

        public int Count() {
            return items.Count;
        }

		public void Add(string item){
			items.Add(item);
		}

		public override string ToString(){
			return string.Join(",", items.ToArray());
		}
	}
}
