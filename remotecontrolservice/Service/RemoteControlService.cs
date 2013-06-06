using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Net.Sockets;

namespace remotecontrolservice
{
    public class RemoteControlService
    {
        public static void Run()
        {
			string listeningIP = System.Configuration.ConfigurationManager.AppSettings.Get("ListeningIP").Trim();
			if(string.IsNullOrEmpty(listeningIP))
			{
				listeningIP = GetLocalIP();
			}

            int listeningPort = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings.Get("ListeningPort"));
            string pwd = System.Configuration.ConfigurationManager.AppSettings.Get("Password");

			IPEndPoint ep = new IPEndPoint(IPAddress.Parse(listeningIP), listeningPort);
            UdpClient listener = new UdpClient(ep);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listeningPort);

			Console.WriteLine("Remote control service started at {0}:{1}", listeningIP, listeningPort.ToString());

            try
            {
                while (true)
                {
                    byte[] bytes = listener.Receive(ref groupEP);
                    string data = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

                    Command cmd = BuildCommand(data);
                    string result = "";
                    if (cmd.Password != pwd)
                    {
                        result = "wrong password.";
                    }
                    else
                    {
                        result = RemoteControl.SendCmd(cmd);
                    }
                    Console.WriteLine("[{0}]({1}): {2}", groupEP.ToString(), data, result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Close();
                Run();
            }
        }

        public static string GetLocalIP()
        {
            string ip = "";
            IPAddress[] addresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            foreach (IPAddress address in addresses)
            {
				if(address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
					&& !address.IsIPv6LinkLocal
					&& !address.IsIPv6Multicast
					&& !address.IsIPv6SiteLocal)
                {
                    ip = address.ToString();
					break;
                }
            }

            return ip;
        }

        public static Command BuildCommand(string data)
        {
            string name = "";
            string password = "";
            string value = "";

            string[] vals = data.Split('|');
            name = vals[0];
            password = vals[1];
            value = data.Substring(name.Length + password.Length + 2);

            return new Command
            {
                Name = name,
                Password = password,
                Value = value
            };
        }
    }
}
