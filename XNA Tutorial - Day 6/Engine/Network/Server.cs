using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Artificial.XNATutorial.Network
{
    public class Server
    {
        Socket socket;
        IPEndPoint endPoint;

        ReceiveClientMessage receiveMethod;

        public Server(int port, ReceiveClientMessage receiveMethod)
        {
            this.receiveMethod = receiveMethod;
            endPoint = new IPEndPoint(NetworkInfo.ThisIP(), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(endPoint);
            socket.Listen(10);
            socket.BeginAccept(new AsyncCallback(AcceptClient), socket);
        }

        void AcceptClient(IAsyncResult asyncResult)
        {
            Socket clientSocket = (Socket)asyncResult.AsyncState;
            if (socket == null || !socket.Connected) return;

            clientSocket.BeginAccept(new AsyncCallback(AcceptClient), socket);
            ConnectedClient cc = new ConnectedClient(clientSocket.EndAccept(asyncResult), this);
            Console.WriteLine(cc.RemoteEndPoint.ToString() + " connected");
            cc.Receive();
        }

        internal void DecodeReadData(string data, ConnectedClient client)
        {
            Console.WriteLine("Received from " + client.RemoteEndPoint.ToString() + ": " + data);
            receiveMethod(data, client);
            client.Receive();
        }

        public void Disconnect()
        {
            socket.Close();
        }
    }
}
