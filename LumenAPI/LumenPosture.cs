using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
namespace LumenAPI
{
    public class LumenPosture
    {
        private Socket handlerSocket;
        private string ip;
        private int port;
        private string methodFlag;
        public LumenPosture(string serverIp, int serverPort)
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
                throw new ArgumentException("unable to create LumenPosture", "serverIp");
            }

            //sending flag to server to create an motion object
            if (isConnected)
            {
                MemoryStream msSend = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(msSend);
                byte[] buffer;
                bw.Write("LumenPosture");
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

        #region to define all methods in LumenPosture Class

        public void goToPosture(string postureName, float speed)
        {
            methodFlag = "goToPosture";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;
            string[] postureList = {"Stand","StandInit","StandZero","Sit","SitRelax","Crouch","LyingBack","LyingBelly"};
            bool flag = false;
            int i = 0;
            //check if postureName is available
            while ((!flag) && (i < postureList.Length))
            {
                if (postureList[i] == postureName)
                {
                    flag = true;
                }
                else
                {
                    i++;
                }
            }
            if (!flag)
            {
                throw new ArgumentException("no posture with name " + postureName, "postureName");
            }

            bw.Write(methodFlag);
            bw.Write(postureName);
            bw.Write(speed);
            buffer = msSend.ToArray();
            try
            {
                handlerSocket.Send(buffer);
            }
            catch
            {
                throw new InvalidOperationException("unable to execute, check connection");
            }
        }
        public void stopMove()
        {
            methodFlag = "goToPosture";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);;
            buffer = msSend.ToArray();
            try
            {
                handlerSocket.Send(buffer);
            }
            catch
            {
                throw new InvalidOperationException("unable to execute, check connection");
            }
        }

        #endregion
    }
}
