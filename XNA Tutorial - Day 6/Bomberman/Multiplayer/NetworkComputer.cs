using System;
using System.Collections.Generic;
using System.Text;

namespace Artificial.XNATutorial.Bomberman
{
    public class NetworkComputer
    {
        public string Name;
        public string IP;
        public int Port;
        public bool Connected;
        public NetworkComputer(string name, string ip, int port)
        {
            Name = name;
            IP = ip;
            Port = port;
        }
    }
}
