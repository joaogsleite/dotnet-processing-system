using System;
using System.Collections.Generic;
using System.Threading;

namespace DADStorm {
	public class QueryFollowersFile {

		public QueryFollowersFile() {}

		public static Tuple getFollowers(Tuple tuple){

            string user = tuple.Get(2);

            string path = "followers.dat";
            List<Tuple> file = new List<Tuple>();
            string[] lines = System.IO.File.ReadAllLines(path);
            foreach (string line in lines) 
                file.Add(new Tuple(line.Split(new string[]{", "},StringSplitOptions.None)));
            
            List<Tuple> followers = file.FindAll((t) => t.Get(1) == user);
            Tuple res = new Tuple();
            foreach (Tuple t in followers) {
                t.RemoveField(1);
                res.Merge(t);
            }
			return res;	
		}
	}
}
