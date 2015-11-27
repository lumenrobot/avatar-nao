using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
namespace LumenAPI
{
    public class LumenBattery
    {
        private Socket handlerSocket;
        private string ip;
        private int port;
        private string methodFlag;
        public LumenBattery(string serverIp, int serverPort)
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
                throw new ArgumentException("unable to create LumenBattery", "serverIp");
            }

            //sending flag to server to create an motion object
            if (isConnected)
            {
                MemoryStream msSend = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(msSend);
                byte[] buffer;
                bw.Write("LumenBattery");
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

        #region to define all methods in LumenBattery Class

        public int getBatteryPercentage()
        {
            methodFlag = "getBatteryPercentage";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            int percentage = new int();
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            bw.Write(methodFlag);
           
            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "getBatteryPercentage")
                {
                    percentage = br.ReadInt32();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return percentage;
        }
        public bool isPlugged()
        {
            methodFlag = "isPlugged";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            bool flag  = new bool();
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            bw.Write(methodFlag);

            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "isPlugged")
                {
                    flag = br.ReadBoolean();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return flag;
        }
        public bool isCharging()
        {
            methodFlag = "isCharging";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            bool flag = new bool();
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            bw.Write(methodFlag);

            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "isCharging")
                {
                    flag = br.ReadBoolean();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return flag;
        }
        #endregion
    }
}
