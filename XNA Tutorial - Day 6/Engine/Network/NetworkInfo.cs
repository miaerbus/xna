using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Artificial.XNATutorial.Network
{
    public delegate void ReceiveServerMessage(string message);
    public delegate void ReceiveClientMessage(string message, ConnectedClient client);

    public class NetworkInfo
    {
        public static IPAddress ThisIP()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            return ipHostInfo.AddressList[0];
        }
    }
}
