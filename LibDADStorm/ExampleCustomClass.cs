using System;
using System.Threading;

namespace DADStorm {
	public class ExampleCustomClass {

		public ExampleCustomClass() {}

		public static Tuple getFollowers(Tuple tuple){
			Monitor.Enter(tuple);
			tuple.Add("custom");
			Monitor.Exit(tuple);
			return tuple;	
		}
	}
}
