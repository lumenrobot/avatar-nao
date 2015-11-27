using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
namespace LumenAPI
{
    public class LumenVideoDevice
    {
        private Socket handlerSocket;
        private string ip;
        private int port;
        private string methodFlag;
        public LumenVideoDevice(string serverIp, int serverPort)
        {
            this.ip = serverIp;
            this.port = serverPort;
            connectToServer();   
        }
        //handling connection to server
        private void connectToServer()
        {
            bool isConnected;
            handlerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
            //attempt to start connection
            try
            {
                handlerSocket.Connect(server);
                isConnected = true;
            }
            catch
            {
                isConnected = false;
                //throw exception if failed
                throw new ArgumentException("unable create LumenVideoDevice", "serverIp");
            }

            if (isConnected)
            {
                MemoryStream msSend = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(msSend);
                byte[] buffer;
                bw.Write("LumenVideoDevice");
                buffer = msSend.ToArray();
                trySend:
                try
                {
                    handlerSocket.Send(buffer);
                }
                catch
                {
                    goto trySend;
                }
            }
        }

        #region to define all method in LumenVideoDevice class

        public byte[] getImageRemote()
        {
            methodFlag = "getImageRemote";
            MemoryStream msSend = new MemoryStream();
            byte[] imageData = new byte[230400];
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] bufferSend;
            
            bw.Write(methodFlag);
            bufferSend = msSend.ToArray();
            try
            {
                tryReceive:
                handlerSocket.Send(bufferSend);
                int bufferSize = handlerSocket.Receive(imageData);
                if (bufferSize != 230400)
                {
                    goto tryReceive;
                }
            }
            catch
            {
                throw new InvalidOperationException("unable to execute, check connection");
            }
            return imageData;
        }
        #endregion
    }
}
