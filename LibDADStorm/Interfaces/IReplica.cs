
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
        void Exit();
		void Interval(int time);
        void ack(string id);
	}
}
