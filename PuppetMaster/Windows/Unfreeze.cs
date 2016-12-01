using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DADStorm {
    public partial class Unfreeze : Form {
        PuppetMaster pm;
        public Unfreeze(PuppetMaster pm) {
            this.pm = pm;
            InitializeComponent();
            foreach (string op_id in pm.operators.Keys) {
                op.Items.Add(op_id);
            }
        }

        private void start_Click(object sender, EventArgs e) {
            string op_id = op.Text;
            int repl_id = Int32.Parse(repl.Text);
            new Thread(() => {
                try {
                    pm.Unfreeze(op_id, repl_id);
                    pm.log(">> Unfreeze " + op_id + " " + repl_id);
                } catch (Exception) {
                    pm.log("Operator or Replica not found!");
                }
            }).Start();
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e) {
            this.Close();
        }
    }
}
