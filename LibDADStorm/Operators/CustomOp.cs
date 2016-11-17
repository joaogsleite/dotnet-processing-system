using System;
using System.Collections.Generic;
using System.Reflection;

namespace DADStorm
{
	[Serializable]
	public class CustomOp : Operator{

		private string dllName = "LibDADStorm.dll";
		private string className = "DADStorm.ExampleCustomClass";
		private string methodName = "getFollowers";

		public CustomOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			: base(id, input_ops, input_files, routing, replicas_url, options) { }

		public override Tuple execute(Tuple tuple){

			Assembly assembly = Assembly.LoadFrom(dllName);
			Type type = assembly.GetType(className);
			object ClassObj = Activator.CreateInstance(type);

			object[] args = new object[] { tuple };

			object result = type.InvokeMember(methodName,BindingFlags.Default | BindingFlags.InvokeMethod, 
			                                  null, ClassObj, args);

			return (Tuple) result;
		}
	}
}
