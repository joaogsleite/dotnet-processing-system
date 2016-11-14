using System;

namespace DADStorm {
	public interface IPCS {
		string hello(string name);
		void createReplica(Operator op, string url);
	}
}
