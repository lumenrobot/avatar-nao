using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
namespace LumenAPI
{
    public class LumenSensor
    {
        private Socket handlerSocket;
        private string ip;
        private int port;
        private string methodFlag;
        public LumenSensor(string serverIp, int serverPort)
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

        #region to define all method in LumenSensor Class
        public float getTactile(string tactileName)
        {
            methodFlag = "getTactile";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            float value = new float();
            int tactileId;
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            switch (tactileName)
            {
                case "FrontTactile":
                    tactileId = 3;
                    break;
                case "MiddleTactil":
                    tactileId = 4;
                    break;
                case "RearTactile":
                    tactileId = 5;
                    break;
                case "HandRightBack":
                    tactileId = 6;
                    break;
                case "HandRightLeft":
                    tactileId = 7;
                    break;
                case "HandRightRight":
                    tactileId = 8;
                    break;
                case "HandLeftBack":
                    tactileId = 9;
                    break;
                case "HandLeftLeft":
                    tactileId = 10;
                    break;
                case "HandLeftRight":
                    tactileId = 11;
                    break;
                default:
                    tactileId = 0;
                    throw new ArgumentOutOfRangeException("tactileName", "no tactile with name " + tactileName);
                    break;
            }
            
            bw.Write(methodFlag);
            bw.Write(tactileId);
            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "getTactile")
                {
                    value = br.ReadSingle();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return value;
        }
        public float getBumper(string bumperName)
        {
            methodFlag = "getBumper";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            float value = new float();
            int bumperId;
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            switch (bumperName)
            {
                case "RightBumper":
                    bumperId = 0;
                    break;
                case "LeftBumper":
                    bumperId = 1;
                    break;
                default:
                    bumperId = 0;
                    throw new ArgumentOutOfRangeException("bumperName", "no tactile with name " + bumperName);
                    break;
            }

            bw.Write(methodFlag);
            bw.Write(bumperId);
            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "getBumper")
                {
                    value = br.ReadSingle();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return value;
        }
        public float getButton(string buttonName)
        {
            methodFlag = "getButton";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            float value = new float();
            int buttonId;
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            switch (buttonName)
            {
                case "ChestButton":
                    buttonId = 2;
                    break;
                default:
                    buttonId = 0;
                    throw new ArgumentOutOfRangeException("buttonName", "no tactile with name " + buttonName);
                    break;
            }

            bw.Write(methodFlag);
            bw.Write(buttonId);
            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "getButton")
                {
                    value = br.ReadSingle();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return value;
        }
        #endregion
    }
}
