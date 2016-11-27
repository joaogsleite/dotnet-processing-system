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
        public static MainWindow instance = null;

        public MainWindow(PuppetMaster pm) {
            InitializeComponent();
            this.pm = pm;
            pm.log_box = this.logs;
			logs.Text = "Welcome!";
            instance = this;
        }

        [STAThread]
        public static void Main() {

            PuppetMaster pm = new PuppetMaster();

            new Thread(() => {
                BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
                provider.TypeFilterLevel = TypeFilterLevel.Full;
                IDictionary props = new Hashtable();
                props["port"] = 10001;
                props["name"] = "tcp10001";
                TcpServerChannel channel = new TcpServerChannel(props, provider);
                ChannelServices.RegisterChannel(channel, false);
                RemotingServices.Marshal(pm, "pm", typeof(IPM));
            }).Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow(pm));
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
				try{
				    Parser p = new Parser(@file);
                    if (!conf_loaded){
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
                }
				catch (Exception err){
					log(err.ToString());
				}
            }
            else {
                log("Config file selected not valid!");
            }
        }

        private void load_script_Click(object sender, EventArgs e) {
            DialogResult result = open_file.ShowDialog();
            if (result == DialogResult.OK) {
                log("Loading script file...");
                string file = open_file.FileName;
                Parser p = new Parser(@file);
                pm.LoadCommands(p.commands());
            }
            else {
                log("Script file selected not valid!");
            }
        }

        private void status_Click(object sender, EventArgs e) {
            pm.Status();
            log(">> Status");
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

		public void log(string text){
            this.Invoke((MethodInvoker)delegate (){
                logs.AppendText("\r\n"+text);
                Application.DoEvents();
            });
        }

        private void step_button_Click(object sender, EventArgs e){
            pm.executeCommand();
        }

        private void all_button_Click(object sender, EventArgs e){
            Boolean more = true;
            while (more){
                more = pm.executeCommand();
                Thread.Sleep(500);
            }
        }
    }
}
