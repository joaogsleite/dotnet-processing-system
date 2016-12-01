using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting;

namespace DADStorm {
    public partial class MainWindow : Form {

        private PuppetMaster pm;
        private Boolean conf_loaded = false;

        public MainWindow(PuppetMaster pm) {
            InitializeComponent();
            this.pm = pm;
			logs.Text = "Welcome!";
        }

        private void exit_Click(object sender, EventArgs e) {
            pm.exit();
            this.Close();
        }

        private void load_config_Click(object sender, EventArgs e) {
            DialogResult result = open_file.ShowDialog();
            if (result == DialogResult.OK) {

                log("Loading config file...");
                string file = open_file.FileName;
                try {
                    new Thread(() => {
                        Parser p = new Parser(@file);
                        if (!conf_loaded) {
                            conf_loaded = true;
                            pm.LoggingLevel(p.logging_level());
                            log("Logging level: " + p.logging_level());
                            pm.CreateOperators(p.operators());
                            log("Creating operators...");
                            pm.ConnectToReplicas();
                            log("Connecting to replicas...");
                            Thread.Sleep(2000);
                        }
                        pm.LoadCommands(p.commands());
                        log("Loading commands to execute...");
                    }).Start();
                } catch (Exception err) {
                    log(err.ToString());
                }
            } else {
                log("Config file selected not valid!");
            }
        }

        private void load_script_Click(object sender, EventArgs e) {
            DialogResult result = open_file.ShowDialog();
            if (result == DialogResult.OK) {
                string file = open_file.FileName;
                new Thread(() => {
                    log("Loading script file...");
                    Parser p = new Parser(@file);
                    pm.LoadCommands(p.commands());
                }).Start();
            }
            else {
                log("Script file selected not valid!");
            }
        }

        private void status_Click(object sender, EventArgs e) {
            new Thread(() => {
                log(">> Status");
                pm.Status();
            }).Start();
        }

        private void start_op_Click(object sender, EventArgs e) {
          	StartOp startop = new StartOp(pm);
          	startop.Show();
        }

        private void interval_Click(object sender, EventArgs e) {
            Interval interv = new Interval(pm);
            interv.Show();
        }

        private void crash_Click(object sender, EventArgs e) {
            Crash crsh = new Crash(pm);
            crsh.Show();
        }

        private void freeze_Click(object sender, EventArgs e) {
            Freeze frze = new Freeze(pm);
            frze.Show();
        }

        private void unfreeze_Click(object sender, EventArgs e) {
            Unfreeze unfrze = new Unfreeze(pm);
            unfrze.Show();
        }

        private void step_button_Click(object sender, EventArgs e){
            new Thread(() => {
                pm.executeCommand();
            }).Start();
        }

        private void all_button_Click(object sender, EventArgs e){
            new Thread(() => {
                Boolean more = true;
                while (more) {
                    more = pm.executeCommand();
                    Thread.Sleep(500);
                }
            }).Start();
        }

        delegate void UpdateLog(string text);
        private void update_log(string text) {
            logs.AppendText("\r\n" + text);
        }
        public void log(string text) {
            if (logs.InvokeRequired) {
                UpdateLog update = update_log;
                this.Invoke(update, text);
            } else update_log(text);
        }
    }
}
