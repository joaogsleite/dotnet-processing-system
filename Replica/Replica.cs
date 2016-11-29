using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.IO;

namespace DADStorm{
	public class Replica : MarshalByRefObject, IReplica {

		private Operator op;
		private string url;
		private Queue<Tuple> queue = new Queue<Tuple>();
		private Boolean processing = false;
		private Boolean subscribed = false;
		public event SendHandler Send;
		private IPM pm;
		public delegate void SendHandler(Replica repl, EventArgs e);
		private Boolean last_repl = false;
			

		public Replica(string op_id, string repl_url, string pm_url, Boolean last_repl){
			this.url = repl_url;
			this.last_repl = last_repl;

			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;
			IDictionary props = new Hashtable();
			props["name"] = "tcp_pm";
			TcpServerChannel channel = new TcpServerChannel(props,provider);
			ChannelServices.RegisterChannel(channel, false);
			pm = (IPM)Activator.GetObject(typeof(IPM), pm_url);

			this.op = pm.get_operator_by_id(op_id);

			string start_text = "[" + op_id + " " + url + "] created!";
			Console.WriteLine(start_text);
			pm.log(start_text);

			ConnectReplicas();
			ReadInputFiles();
		}

		public string toString(){
			return "[" + op.id + " " + url + "]";
		}

		public void Start(){
            new Thread(() => {
                processing = true;
                while (processing) {
                    if (queue.Count > 0 & Send != null) {
                        Console.WriteLine("tuples to process...");
                        List<Tuple> tuples = op.execute(queue.Dequeue());
                        Console.WriteLine("tuples: ");
                        Console.WriteLine(tuples);
                        Console.WriteLine(tuples.Count);
                        if (tuples != null) {
                            if (tuples.Count > 0) {
                                foreach (Tuple t in tuples) {
                                    Console.WriteLine("new tuple: " + t);
                                    this.Send(this, (EventArgs)t);
                                    Console.WriteLine("new tuple2s: " + t);
                                    Console.WriteLine(t);
                                    log("tuple " + url + ", <" + t + ">");
                                }
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            }).Start();
		}

		public void Freeze(){
            this.Status();
			processing = false;
		}

		public void Unfreeze(){
			Start();
            this.Status();
		}

		public void Crash(){
            //Environment.Exit(1);
            Process.GetCurrentProcess().Kill();
		}
        public void Exit(){
            Environment.Exit(0);
            Process.GetCurrentProcess().Kill();
        }

		public string Status(){
			string text = "[" + op.id + " " + url + "] ";
			text += processing ? "processing " : "frozen";
            log(text);
            Console.WriteLine(text);     
			return text;
		}

		public void Interval(int time){
			Thread.Sleep(time);
		}

		private void ReadInputFiles(){
			foreach(string path in op.input_files){
				string[] lines = System.IO.File.ReadAllLines(@path);
				foreach (string line in lines)
                    if (!line.Contains("%")) {
                        Console.WriteLine("Reading line from " + @path + "...");
                        queue.Enqueue(new Tuple(line.Split(new string[] { ", " }, StringSplitOptions.None)));
                    }
						
			}
		}

		private void ConnectReplicas(){
			if (subscribed) return;
			foreach (Operator input in op.input_ops)
			{
				//foreach(string repl_url in input.replicas_url)
				//PRIMARY TODO !!!
				Subscribe(input.replicas_url[0]);

			}
			subscribed = true;

			if (last_repl)
				this.Send += new SendHandler(this.Output);
		}
		public Boolean ready(){
			return subscribed;
		}

		private void Subscribe(String repl_url){
            new Thread(() => {
                Boolean success = false;
                while (!success) {
                    try {
                        //TcpChannel channel = new TcpChannel();
                        //ChannelServices.RegisterChannel(channel, false);
                        BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
                        provider.TypeFilterLevel = TypeFilterLevel.Full;
                        IDictionary props = new Hashtable();
                        props["name"] = repl_url + url;
                        TcpServerChannel channel = new TcpServerChannel(props, provider);
                        ChannelServices.RegisterChannel(channel, false);
                        Replica repl = (Replica)Activator.GetObject(typeof(Replica), repl_url);
                        repl.Send += new SendHandler(this.Receive);
                        success = true;
                        Console.WriteLine(repl_url + " subscribed!");
                    } catch (Exception) {
                        Console.WriteLine("Retrying connect to " + repl_url);
                    }
                    Thread.Sleep(1000);
                }
            }).Start();
		}

		public void Receive(Replica repl, EventArgs e){
            new Thread(() => {
                Console.WriteLine("tuple received!");
                queue.Enqueue((Tuple)e);
            }).Start(); 
		}
		private void Output(Replica repl, EventArgs e){
			using (StreamWriter file = new StreamWriter(@op.id+"-output.txt",true)){
				file.WriteLine((Tuple)e);
			}
		}

		private void log(string text){
			pm.log(text);
		}

		public static string exe_path(){
			return @Environment.CurrentDirectory+"/Replica.exe";
		}

		public static void Main(string[] args){

			string repl_url = args[0];
			string op_id = args[1];
			string pm_url = args[2];
			Boolean last_repl = args[3] == "true" ? true : false;

			int port = Int32.Parse(repl_url.Split(':')[2].Split('/')[0]);
			string uri = repl_url.Split('/')[repl_url.Split('/').Length - 1];

			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;
			IDictionary props = new Hashtable();
			props["port"] = port;
			props["name"] = "tcp" + port;

			TcpServerChannel channel = new TcpServerChannel(props,provider);
			ChannelServices.RegisterChannel(channel, false);

			Replica replica = new Replica(op_id,repl_url,pm_url,last_repl);
			RemotingServices.Marshal(replica, uri, typeof(Replica));

			// Dont close console
			Console.ReadLine();
		}
	}
}
