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
    public partial class StartOp : Form {
        PuppetMaster pm;
        public StartOp(PuppetMaster pm) {
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
            string op_id = op.Text;
            new Thread(() => {
                try {
                    pm.StartOp(op_id);
                    pm.log(">> Start " + op_id);
                } catch (Exception) {
                    pm.log("Operator not found!");
                }
            }).Start();
            this.Close();
        }
    }
}
