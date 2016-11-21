using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DADStorm {
    public partial class Freeze : Form {
        PuppetMaster pm;
        public Freeze(PuppetMaster pm) {
            this.pm = pm;
            InitializeComponent();
            foreach (string op_id in pm.operators.Keys) {
                op.Items.Add(op_id);
            }
        }

        private void cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void start_Click(object sender, EventArgs e) {
            try {
                pm.Freeze(op.Text, Int32.Parse(repl.Text));
            }
            catch (Exception) {
                pm.print_log("Operator or Replica not found!");
            }
            this.Close();
        }
    }
}
