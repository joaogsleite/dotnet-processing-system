using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading;

namespace DADStorm {
    public partial class Replica : MarshalByRefObject, IReplica {

        private void ConnectReplicas() {
            if (subscribed) return;
            foreach (Operator input in op.input_ops) {
                foreach (string repl_url in input.replicas_url)
                    Subscribe(repl_url);
            }
            subscribed = true;
            foreach (string repl_url in op.replicas_url) {
                if (repl_url != url)
                    ConnectToReplica(repl_url);
            }
        }

        private void Subscribe(String repl_url) {
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
                        ChannelServices.RegisterChannel(channel, false);
                        Replica repl = (Replica)Activator.GetObject(typeof(Replica), repl_url);
                        repl.Send += new SendHandler(this.Receive);
                        success = true;
                        Console.WriteLine(repl_url + " subscribed!");
                    }
                    catch (Exception e) {
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
    }
}
