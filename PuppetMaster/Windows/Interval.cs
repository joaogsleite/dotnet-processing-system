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
    public partial class Interval : Form {
        PuppetMaster pm;
        public Interval(PuppetMaster pm) {
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
            int interval = Int32.Parse(interval_value.Text);
            new Thread(() => {
                try {
                    pm.Interval(op_id, interval);
                    pm.log(">> Interval " + op_id + " " + interval);
                } catch (Exception) {
                    pm.log("Operator not found!");
                }
            }).Start();
            this.Close();
        }
    }
}
