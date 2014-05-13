using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Artificial.XNATutorial.Network
{
    public class Client
    {
        Socket socket = null;
        EndPoint serverEndPoint = null;
        byte[] buffer = new byte[1024];

        bool connected = false;
        public bool Connected
        {
            get
            {
                return connected;
            }
        }

        ReceiveServerMessage receiveMethod;

        public Client(string serverIP, int serverPort, ReceiveServerMessage receiveMethod)
        {
            this.receiveMethod = receiveMethod;
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);
        }

        public void Connect()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(serverEndPoint, new AsyncCallback(EndConnect), socket);
        }

        private void EndConnect(IAsyncResult asyncResult)
        {
            try
            {
                socket.EndConnect(asyncResult);
                Console.WriteLine("Connected to: " + socket.RemoteEndPoint.ToString());
                connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Receive()
        {
            if (socket.Connected)
            {
                socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(EndReceive), this);
            }
        }

        private void EndReceive(IAsyncResult asyncResult)
        {
            if (socket != null && socket.Connected)
            {
                try
                {

                    int index = socket.EndReceive(asyncResult);
                    string data = System.Text.Encoding.Unicode.GetString(buffer, 0, index);
                    Console.WriteLine(data);
                    receiveMethod(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void Send(string data)
        {
            if (socket.Connected)
            {
                try
                {
                    byte[] buff = System.Text.Encoding.Unicode.GetBytes(data);
                    socket.BeginSend(buff, 0, buff.Length, 0, new AsyncCallback(EndSend), socket);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void EndSend(IAsyncResult asyncResult)
        {
            int bytesSent = socket.EndSend(asyncResult);
            Receive();
        }

        public void Disconnect()
        {
            if (socket != null) socket.Close();
        }
    }
}
