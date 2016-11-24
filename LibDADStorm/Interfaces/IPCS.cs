using System;

namespace DADStorm {
	public interface IPCS {
		void createReplica(string op_id, string url, Boolean last_repl);
	}
}
