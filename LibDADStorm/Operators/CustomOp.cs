using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace DADStorm
{
	[Serializable]
	public class CustomOp : Operator{

		private string dllName;
		private string className;
		private string methodName;

		public CustomOp(string id, List<Operator> input_ops, List<string> input_files, string routing, List<string> replicas_url, string options)
			: base(id, input_ops, input_files, routing, replicas_url, options) {

			this.dllName = options.Split(',')[0];
			this.className = options.Split(',')[1];
			this.methodName = options.Split(',')[2];
		}

		public override Tuple execute(Tuple tuple){
			Assembly assembly = Assembly.LoadFrom(dllName);
			Type type = assembly.GetType("DADStorm."+className);
			object ClassObj = Activator.CreateInstance(type);

			object[] args = new object[] { tuple };

			object result = type.InvokeMember(methodName,BindingFlags.Default | BindingFlags.InvokeMethod, 
			                                  null, ClassObj, args);


			return (Tuple) result;
		}
	}
}
