using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
            try {
                pm.Interval(op.Text,Int32.Parse(interval_value.Text));
            }catch(Exception){
                pm.log("Operator not found!");
            }
            this.Close();
        }
    }
}
