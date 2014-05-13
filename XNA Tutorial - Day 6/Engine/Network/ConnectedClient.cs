using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Artificial.XNATutorial.Network
{
    public class ConnectedClient
    {
        private Socket socket = null;
        private Server server = null;
        private byte[] buffer = new byte[1024];

        public ConnectedClient(Socket socket, Server server)
        {
            this.socket = socket;
            this.server = server;
        }

        public EndPoint RemoteEndPoint
        {
            get
            {
                return socket.RemoteEndPoint;
            }
        }

        public void Receive()
        {
            socket.BeginReceive(buffer, 0, buffer.Length, 0, new AsyncCallback(EndReceive), this);
        }

        private void EndReceive(IAsyncResult asyncResult)
        {
            int index = socket.EndReceive(asyncResult);
            string data = System.Text.Encoding.Unicode.GetString(buffer, 0, index);
            server.DecodeReadData(data, this);
        }

        public void Send(string data)
        {
            byte[] buffer = System.Text.Encoding.Unicode.GetBytes(data);
            socket.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(EndSend), socket);
        }

        private void EndSend(IAsyncResult asyncResult)
        {
            int bytesSent = socket.EndSend(asyncResult);
        }

    }
}
