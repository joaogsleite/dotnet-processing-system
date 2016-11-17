using System;
namespace DADStorm {
	public class ExampleCustomClass {

		public ExampleCustomClass() {}

		public static Tuple getFollowers(Tuple tuple){
			Console.WriteLine("CUSTOM: "+tuple);
			tuple.Add("custom");
			return tuple;	
		}
	}
}
