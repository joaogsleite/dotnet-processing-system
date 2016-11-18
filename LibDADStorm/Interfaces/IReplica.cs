
using System;

namespace DADStorm{
	public interface IReplica{
		string toString();
		Boolean ready();
		void Freeze();
		void Unfreeze();
		void Crash();
		string Status();
		void Start();
		void Interval(int time);
	}
}
