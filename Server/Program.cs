using Nest;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Program : Client
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Type your port :");
            string port = Console.ReadLine();
            int formatted_port;
            try
            {
                formatted_port = Convert.ToInt32(port);

                Console.WriteLine("provide ip address like: 127.0.0.1 - and press enter");
                string ipAddress = Console.ReadLine();
                Socket serverListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), formatted_port);

                serverListener.Bind(ep);
                serverListener.Listen(50);

                Console.WriteLine("Server Listening to port " + formatted_port);
                Socket clientSocket = default(Socket);
                int clientCounter = 0;                
                List<Client> connectedClients = new List<Client>();
                Program p = new Program();
                Console.WriteLine("type >>> exit <<< to shutdown server or press Enter if you wanna continue");
                string quitServ = Console.ReadLine();
                if (quitServ?.ToLower() == "exit")
                {
                    p.shuttingDownServer(serverListener);
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine(clientCounter + " Clients Connected - pls start client application");
                }

                while (true)
                {
                    clientCounter++;
                    clientSocket = serverListener.Accept();
                    Console.WriteLine("Hello, Dear Client! You're connected!");
                    Console.WriteLine(clientCounter + " Client(s) Connected");
                    Console.WriteLine("                                     ");
                    Client cl = new Client() { sumOfNums = 0, ipAddress = ((IPEndPoint)clientSocket.LocalEndPoint).Address.ToString(), port = formatted_port.ToString(), status = "Active" };
                    Thread clientThread = new Thread(new ThreadStart(() => p.Client(clientSocket, cl)));
                    clientThread.Start();
                    connectedClients.Add(cl);

                    string answer;
                    do
                    {
                        Console.WriteLine("Type >>> list <<< To see all active connections(or type exit to shutdown server)");
                        Console.WriteLine("Type >>> exit <<< to shutdown server)");
                        Console.WriteLine("Type >>> accept <<< accept next connection)");

                        answer = Console.ReadLine();
                        var wantCountLower = answer?.ToLower();
                        if ((wantCountLower == "list"))
                        {
                            Console.WriteLine("                         ");
                            Console.WriteLine("******List of connected clients***************");
                            foreach (var s in connectedClients)
                            {
                                if (s.status != "Disconnected")
                                {
                                    Console.WriteLine("GUID:" + s.getId().ToString() + "IP Address: " + s.ipAddress + " | Sum of Numbers: " + s.sumOfNums);
                                }
                            }
                            Console.WriteLine("                         ");
                        }
                        else if (answer?.ToLower() == "exit")
                        {
                            p.shuttingDownServer(serverListener);

                        }
                        else if ((answer?.ToLower() == "accept")){
                            Console.WriteLine("awaiting new client application....");
                            break; }

                    } while (true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void shuttingDownServer(Socket srv)
        {
            try
            {
                srv.SafeHandle.Close();
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Client(Socket client, Client conClient)
        {
            try
            {                
                conClient.sumOfNums = 0;
                while (true)
                {
                    byte[] msg = new byte[1024];
                    int size = client.Receive(msg);

                    string clientMsg = DecodeClientMsg(msg);
                    if (clientMsg?.TrimEnd('\0').ToLower() == "exit")
                    {
                        conClient.status = "Disconnected";
                        client.Send(msg, 0, size, SocketFlags.None);
                    }
                    else
                    {
                        int clientInputValue = parseReturnInt(clientMsg);
                        string respondToClient = "";

                        if (clientInputValue != -1)
                        {
                            conClient.sumOfNums += clientInputValue;
                            respondToClient = "Sum of all numbers typed while active session: " + conClient.sumOfNums;
                            byte[] serverMsgToClient = EncodeClientMsg(respondToClient);
                            client.Send(serverMsgToClient, 0, serverMsgToClient.Length, SocketFlags.None);
                        }
                        else
                        {
                            respondToClient = "Invalid data type, pls type only integers!";
                            byte[] serverMsgToClient = EncodeClientMsg(respondToClient);
                            client.Send(serverMsgToClient, 0, serverMsgToClient.Length, SocketFlags.None);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public byte[] EncodeClientMsg(string msg)
        {
            return System.Text.Encoding.UTF8.GetBytes(msg, 0, msg.Length);
        }

        public string DecodeClientMsg(byte[] msg)
        {
            return System.Text.Encoding.UTF8.GetString(msg, 0, msg.Length);
        }
        public int parseReturnInt(string msgParam)
        {
            int clientNumber = -1;
            if (msgParam != null || msgParam != string.Empty)
            {
                try
                {
                    return clientNumber = int.Parse(msgParam);
                }
                catch (Exception)
                {
                    return clientNumber;
                }
            }
            else
            {
                return clientNumber;
            }
        }
    }
}
