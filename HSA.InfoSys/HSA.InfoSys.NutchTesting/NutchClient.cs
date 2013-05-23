

namespace HSA.InfoSys.Testing.NutchTesting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net.Sockets;
    using System.Net;

    public class NutchClient
    {
        private Socket socket;
        private string IpAdr;
        private int Port;

        public NutchClient(int port, string ipAdr)
        {
            this.Port = port;
            this.IpAdr = ipAdr;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void StartSearch()
        {

            IPAddress ipa = IPAddress.Parse(this.IpAdr);
            IPEndPoint ipe = new IPEndPoint(ipa, this.Port);

            this.socket.Connect(ipe);

            if (this.socket.Connected)
            {

                string request = "GET HTTP/1.1\r\nHost:" + this.IpAdr + "\r\nContent-Length: 0\r\n\r\n";
                Byte[] bytesSend = new ASCIIEncoding().GetBytes(request);
                socket.Send(bytesSend);
                Byte[] bytesReceived = new Byte[256];
                string response = "";
                int bytes = 0;
                do
                {
                    bytes = this.socket.Receive(bytesReceived, bytesReceived.Length, 0);
                    response += Encoding.ASCII.GetString(bytesReceived, 0, bytes);
                }
                while (bytes > 0);
                Console.WriteLine(response);
            }
        }

    }
}
