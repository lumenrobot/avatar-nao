using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
namespace LumenAPI
{
    class LumenSonar
    {
        private Socket handlerSocket;
        private string ip;
        private int port;
        private string methodFlag;
        public LumenSonar(string serverIp, int serverPort)
        {
            this.ip = serverIp;
            this.port = serverPort;
            connectToServer();
        }
        private void connectToServer()
        {
            handlerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(this.ip), this.port);
            bool isConnected = false;
            //attempt to start connection
            try
            {
                handlerSocket.Connect(server);
                isConnected = true;
            }
            catch
            {
                //throw exception if attempting failed, ask user to check IP address and Port
                isConnected = false;
                throw new ArgumentException("unable to create LumenSonar", "serverIp");
            }

            //sending flag to server to create an motion object
            if (isConnected)
            {
                MemoryStream msSend = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(msSend);
                byte[] buffer;
                bw.Write("LumenSonar");
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

        #region to define all methods in LumenSonar class
        public float getDistance(string sensor)
        {
            methodFlag = "getDistance";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            float distance = new float();
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;
            if (sensor == "left") { }
            else if (sensor == "right") { }
            else
            {
                throw new ArgumentOutOfRangeException("sensor", "only 'left' and 'right' allowed");
            }

            bw.Write(methodFlag);
            bw.Write(sensor);
            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "getDistance")
                {
                    distance = br.ReadSingle();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return distance;
        }
        #endregion
    }
}
