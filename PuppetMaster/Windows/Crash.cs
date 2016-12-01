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
    public partial class Crash : Form {
        PuppetMaster pm;
        public Crash(PuppetMaster pm) {
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
                    pm.Crash(op_id, repl_id);
                    pm.log(">> Crash " + op_id + " " + repl_id);
                } catch (Exception) {
                    pm.log("Operator not found!");
                }
            }).Start();
            this.Close();
        }

        private void cancel_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e) {

        }

        private void label2_Click(object sender, EventArgs e) {

        }

        private void repl_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void op_SelectedIndexChanged(object sender, EventArgs e) {

        }
    }
}
