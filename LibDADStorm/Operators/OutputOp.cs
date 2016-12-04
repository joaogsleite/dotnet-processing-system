using System;
using System.IO;

namespace DADStorm{

    public class OutputOp{
        private string op_id;
        private int repl_id;

        public OutputOp(string op_id, int repl_id){
            this.op_id = op_id;
            this.repl_id = repl_id;
        }

        public void execute(Tuple input){
            using (StreamWriter file = new StreamWriter(op_id + "-"+repl_id+"-output.txt", true)){
                file.WriteLine(input);
            }
        }
    }
}
