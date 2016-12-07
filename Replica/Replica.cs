﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DADStorm{
	public class Replica : MarshalByRefObject, IReplica {

		private Operator op;
		private string url;
		private Queue<Tuple> input_queue = new Queue<Tuple>();
        private Queue<Tuple> output_queue = new Queue<Tuple>();
        private Boolean processing = false;
		private Boolean subscribed = false;
		public event SendHandler Send;
		private IPM pm;
        private List<Replica> sisters = new List<Replica>();
		public delegate void SendHandler(Replica repl, EventArgs e);
        public int id;
        private List<TcpChannel> channels = new List<TcpChannel>();
			

		public Replica(string op_id, string repl_url, string pm_url){
			this.url = repl_url;

			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;
			IDictionary props = new Hashtable();
			props["name"] = "tcp_pm";
			TcpChannel channel = new TcpChannel(props, null, provider);
            channels.Add(channel);
			ChannelServices.RegisterChannel(channel, false);
			pm = (IPM)Activator.GetObject(typeof(IPM), pm_url);

			this.op = pm.get_operator_by_id(op_id);

			string start_text = "[" + op_id + " " + url + "] created!";
			Console.WriteLine(start_text);
			pm.log(start_text);

			ConnectReplicas();
			ReadInputFiles();
            this.id = getMyId();
		}

        private int getMyId(){
            int i = 0;
            foreach(string repl_url in op.replicas_url){   
                if (repl_url == this.url) break;
                i++;
            }
            return i;
        }

        public string routing(){
            return op.routing;
        }

		public string toString(){
			return "[" + op.id + " " + url + "]";
		}

        public void Start() {
            processing = true;
            new Thread(() => {   
                while (processing) {
                    if (input_queue.Count > 0) {
                        List<Tuple> tuples = op.execute(input_queue.Dequeue());
                        if(tuples!=null)
                            tuples.ForEach(t => output_queue.Enqueue(t));        
                    }
                    Thread.Sleep(500);
                }         
            }).Start();
            new Thread(() => {
                while (processing){
                    if (output_queue.Count > 0 & Send != null){
                        Tuple t = output_queue.Dequeue();
                        this.SendTuple(t);
                        log("tuple " + url + ", <" + t + ">");
                    }
                    Thread.Sleep(500);
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
            CloseTcpChannels();
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
                        input_queue.Enqueue(new Tuple(line.Split(new string[] { ", " }, StringSplitOptions.None)));
                    }
						
			}
		}

        private void CloseTcpChannels() {
            foreach(TcpChannel channel in channels) {
                channel.StopListening(null);
                RemotingServices.Disconnect(this);
                ChannelServices.UnregisterChannel(channel);
            }
            channels = null;
        }

		private void ConnectReplicas(){
			if (subscribed) return;
			foreach (Operator input in op.input_ops){
				foreach(string repl_url in input.replicas_url)
				    Subscribe(repl_url);
			}
			subscribed = true;
            foreach(string repl_url in op.replicas_url) {
                if (repl_url != url)
                    ConnectToReplica(repl_url);
            }
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
                        props["name"] = repl_url + url + DateTime.Now;
                        TcpChannel channel = new TcpChannel(props, null, provider);
                        //channels.Add(channel);
                        ChannelServices.RegisterChannel(channel, false);
                        Replica repl = (Replica)Activator.GetObject(typeof(Replica), repl_url);
                        repl.Send += new SendHandler(this.Receive);
                        success = true;
                        Console.WriteLine(repl_url + " subscribed!");
                    } catch (Exception e) {
                        Console.WriteLine(e);
                        Console.WriteLine("Retrying subscribe to " + repl_url);
                    }
                    Thread.Sleep(2000);
                }
            }).Start();
		}
        private void ConnectToReplica(String repl_url) {
            new Thread(() => {
                Boolean success = false;
                while (!success) {
                    try {
                        //TcpChannel channel = new TcpChannel();
                        //ChannelServices.RegisterChannel(channel, false);
                        BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
                        provider.TypeFilterLevel = TypeFilterLevel.Full;
                        IDictionary props = new Hashtable();
                        props["name"] = repl_url + url + DateTime.Now;
                        TcpChannel channel = new TcpChannel(props, null, provider);
                        //channels.Add(channel);
                        ChannelServices.RegisterChannel(channel, false);
                        Replica repl = (Replica)Activator.GetObject(typeof(Replica), repl_url);
                        sisters.Add(repl);
                        success = true;
                        Console.WriteLine(repl_url + " connected!");
                    }
                    catch (Exception e) {
                        Console.WriteLine(e);
                        Console.WriteLine("Retrying connect to " + repl_url);
                    }
                    Thread.Sleep(2000);
                }
            }).Start();
        }

        public void SendTuple(Tuple tuple){
            string routing = ((Replica)(this.Send.Target)).routing();

            if (routing.Contains("primary")) {
                //Console.WriteLine("Routing: primary");
                int repl_id = -1;
                Boolean success = false;
                while (!success) {
                    repl_id++;
                    try {
                        if (this.Send.GetInvocationList().Length - 1 < repl_id) return;
                        SendHandler send = (SendHandler)Array.Find(this.Send.GetInvocationList(),
                                                   r => ((Replica)(r.Target)).id == repl_id);
                        send(this, (EventArgs)tuple);
                        success = true;
                    }
                    catch (Exception) {
                        Console.WriteLine("Failed to send tuple to replica " + repl_id);
                        Console.WriteLine("Trying to send tuple to replica " + (repl_id++)+"...");
                        success = false;
                    }
                }
            }
            else if (routing.Contains("hashing")) {
                int field_index = Int32.Parse(routing.Split('(')[1].Split(')')[0]);
                //Console.WriteLine("Routing: hashing(" + field + ")");

                string field_value = tuple.Get(field_index);
                MD5 md5Hasher = MD5.Create();
                byte[] hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(field_value));
                int hash = BitConverter.ToInt32(hashed, 0);

                List<SendHandler> handlers = new List<SendHandler>();
                foreach (Delegate handler in this.Send.GetInvocationList())
                    handlers.Add((SendHandler)handler);

                Boolean success = false;
                int index = -1;
                while (!success) {
                    try {
                        if (handlers.Count == 0) { return; }
                        index = hash % handlers.Count;
                        SendHandler send = handlers[index];
                        send(this, (EventArgs)tuple);
                        success = true;
                    }
                    catch (Exception) {
                        if (index == -1) { return; }
                        Console.WriteLine("Failed to send tuple to replica " + index);  
                        handlers.RemoveAt(index);
                        Console.WriteLine("Trying to send to another random replica...");
                    }
                }
            }
            else if (routing.Contains("random")) {
                //Console.WriteLine("Routing: random");

                List<SendHandler> handlers = new List<SendHandler>();
                foreach(Delegate handler in this.Send.GetInvocationList())
                    handlers.Add((SendHandler)handler);
                
                Boolean success = false;
                int index = -1;
                while (!success) {
                    try {
                        if (handlers.Count == 0) { return; }
                        index = new Random().Next(0, handlers.Count);
                        SendHandler send = handlers[index];
                        send(this, (EventArgs)tuple);
                        success = true;
                    } catch (Exception) {
                        Console.WriteLine("Failed to send tuple to replica " + index);
                        if (index == -1) { return; }
                        handlers.RemoveAt(index);
                        Console.WriteLine("Trying to send to another random replica...");
                    }
                }
            } 
        }
		public void Receive(Replica repl, EventArgs e){
            new Thread(() => {
                input_queue.Enqueue((Tuple)e);
            }).Start(); 
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

			int port = Int32.Parse(repl_url.Split(':')[2].Split('/')[0]);
			string uri = repl_url.Split('/')[repl_url.Split('/').Length - 1];

			BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
			provider.TypeFilterLevel = TypeFilterLevel.Full;
			IDictionary props = new Hashtable();
			props["port"] = port;
			props["name"] = "tcp" + port;

			TcpChannel channel = new TcpChannel(props,null, provider);
			ChannelServices.RegisterChannel(channel, false);

			Replica replica = new Replica(op_id,repl_url,pm_url);
            replica.channels.Add(channel);
			RemotingServices.Marshal(replica, uri, typeof(Replica));

			// Dont close console
			Console.ReadLine();

            // Close TcpChannel
            channel.StopListening(null);
            RemotingServices.Disconnect(replica);
            ChannelServices.UnregisterChannel(channel);
            channel = null;
        }
	}
}
