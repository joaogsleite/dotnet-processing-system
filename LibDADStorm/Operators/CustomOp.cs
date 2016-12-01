using System;
using System.Collections.Generic;
using System.IO;
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

		public override List<Tuple> execute(Tuple input){

            List<Tuple> output = new List<Tuple>();

            byte[] code = File.ReadAllBytes(dllName);
            Assembly assembly = Assembly.Load(code);

            // Walk through each type in the assembly looking for our class
            foreach (Type type in assembly.GetTypes()) {
                if (type.IsClass == true) {
                    if (type.FullName.EndsWith("." + className)) {

                        // create an instance of the object
                        object ClassObj = Activator.CreateInstance(type);

                        // Dynamically Invoke the method
                        List<string> l = input.toList();

                        object[] methodArgs = new object[] { l };
                        object resultObject = type.InvokeMember(methodName,
                          BindingFlags.Default | BindingFlags.InvokeMethod,
                               null,
                               ClassObj,
                               methodArgs);
                        IList<IList<string>> result = (IList<IList<string>>)resultObject;
                        Console.WriteLine("Custom operator result was: ");
                        foreach (IList<string> tuple in result) 
                            output.Add(new Tuple(tuple));
                        
                        return output;
                    }
                }
            }
            return output;
            /*
            Assembly assembly = Assembly.LoadFrom(dllName);
            Type type = assembly.GetType("DADStorm."+className);
            object ClassObj = Activator.CreateInstance(type);

            object[] args = new object[] { tuple.toList() };

            object output = type.InvokeMember(methodName,BindingFlags.Default | BindingFlags.InvokeMethod, 
                                                null, ClassObj, args);

            List<IList<string>> result = (List<IList<string>>) output;

            List<Tuple> res = new List<Tuple>();
            foreach (List<string> list in result)
                res.Add(new Tuple(list));      

            return (List<Tuple>) res;

            */
        }
	}
}
