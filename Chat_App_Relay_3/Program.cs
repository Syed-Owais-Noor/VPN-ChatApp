using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Chat_App_Relay_3
{
    class Program
    {
        public static Hashtable clientsList = new Hashtable();

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("192.168.1.103");
            TcpListener serverSocket = new TcpListener(ip, 13000);
            TcpClient clientSocket = default(TcpClient);
            
            int counter = 0, randNum = 0, count = 0;

            int[] port = new[] { 11000, 12000 };

            serverSocket.Start();
            Console.WriteLine("Chat Room Relay Server 3 is Up....");
            counter = 0;

            StreamWriter writer;

            Random rand = new Random();

            while ((true))
            {
                int randPort = port[rand.Next(port.Length)];

                counter += 1;
                clientSocket = serverSocket.AcceptTcpClient();

                string dataFromClient = "";

                StreamReader reader = new StreamReader(clientSocket.GetStream());

                dataFromClient = reader.ReadLine();

                string[] dataFromClientS = dataFromClient.Split("$");

                randNum = Convert.ToInt32(dataFromClientS[1]);

                string name = dataFromClientS[2];

                dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                if (clientsList.ContainsKey(name))
                {

                }
                else
                {
                    clientsList.Add(name, clientSocket);
                    count = 0;
                }

                if (randNum != 0)
                {
                    randNum -= 1;
                }
                else
                {
                    randPort = 8888;
                }

                TcpClient client = new TcpClient("192.168.1.103", randPort);

                writer = new StreamWriter(client.GetStream());

                string outStream = dataFromClient + "$" + randNum + "$" + name;
                writer.WriteLine(outStream);
                writer.Flush();

                if (clientsList.Contains(name) && count == 1)
                {
                    broadcast(dataFromClient, dataFromClient, true, name);
                    Console.WriteLine("From client - " + name + " : " + dataFromClient);
                }
                else
                {
                    broadcast(dataFromClient + " Joined ", dataFromClient, false, name);
                    Console.WriteLine(dataFromClient + " Joined chat room ");
                    count = 1;
                }

                handleClinet clientHandle = new handleClinet();
                clientHandle.startClient(clientSocket, dataFromClient, clientsList, randNum, randPort, name);
            }

            clientSocket.Close();
            serverSocket.Stop();
            Console.WriteLine("exit");
            Console.ReadLine();
        }

        public static void broadcast(string msg, string uName, bool flag, string name)
        {
            foreach (DictionaryEntry Item in clientsList)
            {
                TcpClient broadcastSocket;
                broadcastSocket = (TcpClient)Item.Value;
                StreamWriter writer = new StreamWriter(broadcastSocket.GetStream());

                string broadcastBytes = "";

                if (flag == true)
                {
                    broadcastBytes = name + " says : " + msg;
                }
                else
                {
                    broadcastBytes = msg;
                }

                writer.WriteLine(broadcastBytes);
                writer.Flush();
            }
        }  //end broadcast function
    }

    public class handleClinet
    {
        TcpClient clientSocket;
        string clNo, name;
        Hashtable clientsList;

        int randNum, randPort;

        StreamWriter writer;

        Random rand = new Random();

        public void startClient(TcpClient inClientSocket, string clineNo, Hashtable cList, int randNum, int randPort, string name)
        {
            this.clientSocket = inClientSocket;
            this.clNo = clineNo;
            this.clientsList = cList;
            this.randNum = randNum;
            this.randPort = randPort;
            this.name = name;
            Thread ctThread = new Thread(doChat);
            ctThread.Start();
        }

        private void doChat()
        {
            int requestCount = 0;
            string dataFromClient = null;
            //Byte[] sendBytes = null;
            //string serverResponse = null;
            string rCount = null;
            requestCount = 0;

            while ((true))
            {
                try
                {
                    requestCount = requestCount + 1;
                    if (clientSocket.Connected == true)
                    {
                        StreamReader reader = new StreamReader(clientSocket.GetStream());

                        dataFromClient = reader.ReadLine();

                        string[] dataFromClientS = dataFromClient.Split("$");

                        dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));
                        Console.WriteLine("From client - " + clNo + " : " + dataFromClient);
                        rCount = Convert.ToString(requestCount);

                        if (randNum != 0)
                        {
                            randNum -= 1;
                        }
                        else
                        {
                            randPort = 8888;
                        }

                        TcpClient client = new TcpClient("192.168.1.103", randPort);

                        writer = new StreamWriter(client.GetStream());

                        string outStream = dataFromClient + "$" + randNum + "$" + name;
                        writer.WriteLine(outStream);
                        writer.Flush();

                        Program.broadcast(dataFromClient, clNo, true, name);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }//end while
        }//end doChat
    } //end class handleClinet
}