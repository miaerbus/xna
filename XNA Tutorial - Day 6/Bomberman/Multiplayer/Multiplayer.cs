using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Artificial.XNATutorial.Network;

namespace Artificial.XNATutorial.Bomberman
{
    public class Multiplayer
    {
        int port = 50014;
        int port2 = 50015;
        Server server;
        public List<NetworkComputer> TheOthers = new List<NetworkComputer>();
        Dictionary<NetworkComputer,Client> clients = new Dictionary<NetworkComputer,Client>();
        Thread thread;
        bool stopConnect;
        int hostPriority;

        Semaphore connectWait = new Semaphore(0, 1);
        Semaphore connectWaiting = new Semaphore(0, 1);
        volatile bool connectionAccepted;

        class Messages
        {
            public const string Connect = "CONNECT";
            public const string ConnectAccepted = "CONNECTOK";
            public const string NoHost = "NOHOST";            
        }

        public Multiplayer(bool primaryPort)
        {
            hostPriority = Bomberman.Random.Next(0, 1000000);

            if (primaryPort)
            {
                server = new Server(port, ServerReceiver);
                TheOthers.Add(new NetworkComputer("Retro@TestPort", "212.235.178.162", port2));
            }
            else
            {
                server = new Server(port2, ServerReceiver);
                TheOthers.Add(new NetworkComputer("Retro", "212.235.178.162", port));
            }

            // Create clients
            for (int i = 0; i < TheOthers.Count; i++)
            {
                clients.Add(TheOthers[i], new Client(TheOthers[i].IP, TheOthers[i].Port, ClientReceiver));
            }

            thread = new Thread(new ThreadStart(ConnectWithTheOthers));
            thread.IsBackground = true;
            thread.Start();
        }

        public void Disconnect()
        {
            server.Disconnect();
            for (int i = 0; i < TheOthers.Count; i++)
            {
                clients[TheOthers[i]].Disconnect();
            }
            stopConnect = true;
        }

        void ConnectWithTheOthers()
        {
            while (!stopConnect)
            {
                Thread.Sleep(2000);
                for (int i = 0; i < TheOthers.Count; i++)
                {
                    TheOthers[i].Connected = clients[TheOthers[i]].Connected;
                    if (!clients[TheOthers[i]].Connected)
                    {
                        clients[TheOthers[i]].Connect();
                    }
                }
            }
        }

        public bool ConnectToGame(NetworkComputer computer)
        {
            Client c = clients[computer];
            connectionAccepted = false;
            c.Send(Messages.Connect);
            return connectionAccepted;
        }

        void ServerReceiver(string message, ConnectedClient client)
        {
            string[] parts = message.Split(new char[] { ':' });
            switch (parts[0])
            {
                case Messages.Connect:
                    client.Send(Messages.ConnectAccepted);
                    break;              
            }
        }

        void ClientReceiver(string message)
        {
            string[] parts = message.Split(new char[] { ':' });
            switch (parts[0])
            {
                case Messages.ConnectAccepted:
                    connectionAccepted = true;
                    break;
            }
        }

    }
}
