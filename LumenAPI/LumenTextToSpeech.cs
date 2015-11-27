using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
namespace LumenAPI
{
    public class LumenTextToSpeech
    {
        private Socket handlerSocket;
        private string ip;
        private int port;
        private string methodFlag;
        public LumenTextToSpeech(string serverIp, int serverPort)
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
                throw new ArgumentException("unable to create LumenTextToSpeech", "serverIp");
            }

            //sending flag to server to create an motion object
            if (isConnected)
            {
                MemoryStream msSend = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(msSend);
                byte[] buffer;
                bw.Write("LumenTextToSpeech");
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

        #region to define all methods of LumenTextToSpeech Class

        public void say(string message)
        {
            methodFlag = "say";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;
           
            bw.Write(methodFlag);
            bw.Write(message);
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
        public void setLanguage(string language)
        {
            methodFlag = "setLanguage";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
            bw.Write(language);
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
