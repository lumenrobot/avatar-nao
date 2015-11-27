using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//additional class
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace LumenAPI
{
    
    public class LumenMotion
    {
        private Socket handlerSocket;
        private string ip;
        private int port;
        private string methodFlag;
        public LumenMotion(string serverIp, int serverPort)
        {
            this.ip = serverIp;
            this.port = serverPort;
            connectToServer();
        }
        //handling the connection to server
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
                throw new ArgumentException("unable to create LumenMotion", "serverIp"); 
            }

            //sending flag to server to create an motion object
            if (isConnected)
            {
                MemoryStream msSend = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(msSend);
                byte[] buffer;
                bw.Write("LumenMotion");
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


        #region to define all method for LumenMotion class
        //stiffness Control
        public void wakeUp()
        {
            methodFlag = "wakeUp";

            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
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
        public void rest()
        {
            methodFlag = "rest";

            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
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
        public void setStiffness(string jointName, float value)
        {
            methodFlag = "setStiffness";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
            bw.Write(value);
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
        public float getStiffness(string jointName)
        {
            methodFlag = "getStiffness";
            int jointId;
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            float value = new float();
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            bw.Write(methodFlag);
            //send int to represent joint name instead send string
            switch (jointName)
            {
                case "HeadYaw":
                    jointId = 0;
                    break;
                case "HeadPitch":
                    jointId = 1;
                    break;
                case "LShoulderPitch":
                    jointId = 2;
                    break;
                case "LShoulderRoll":
                    jointId = 3;
                    break;
                case "LElbowYaw":
                    jointId = 4;
                    break;
                case "LElbowRoll":
                    jointId = 5;
                    break;
                case "LWristYaw":
                    jointId = 6;
                    break;
                case "LHand":
                    jointId = 7;
                    break;
                case "LHipYawPitch":
                    jointId = 8;
                    break;
                case "LHipRoll":
                    jointId = 9;
                    break;
                case "LHipPitch":
                    jointId = 10;
                    break;
                case "LKneePitch":
                    jointId = 11;
                    break;
                case "LAnklePitch":
                    jointId = 12;
                    break;
                case "LAnkleRoll":
                    jointId = 13;
                    break;
                case "RHipYawPitch":
                    jointId = 14;
                    break;
                case "RHipRoll":
                    jointId = 15;
                    break;
                case "RHipPitch":
                    jointId = 16;
                    break;
                case "RKneePitch":
                    jointId = 17;
                    break;
                case "RAnklePitch":
                    jointId = 18;
                    break;
                case "RAnkleRoll":
                    jointId = 19;
                    break;
                case "RShoulderPitch":
                    jointId = 20;
                    break;
                case "RShoulderRoll":
                    jointId = 21;
                    break;
                case "RElbowYaw":
                    jointId = 22;
                    break;
                case "RElbowRoll":
                    jointId = 23;
                    break;
                case "RWristYaw":
                    jointId = 24;
                    break;
                case "RHand":
                    jointId = 25;
                    break;
                default:
                    jointId = 0;
                    throw new ArgumentOutOfRangeException("jointName", "no joint with name " + jointName);
                    break;
            }
            bw.Write(jointId);
            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "getStiffness")
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

        //joint control 
        public void setAngle(string jointName, float angle, float speed)
        {
            methodFlag = "setAngle";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
            bw.Write(angle);
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
        public void changeAngle(string jointName, float angle, float speed)
        {
            methodFlag = "changeAngle";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
            bw.Write(angle);
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
        public float getAngle(string jointName)
        {
            methodFlag = "getAngle";
            int jointId;
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            MemoryStream msReceive;
            BinaryReader br;
            float angle = new float();
            byte[] bufferSend;
            byte[] bufferReceive = new byte[1024];
            int bufferSize;

            bw.Write(methodFlag);
            //send int to represent joint name instead send string
            switch (jointName)
            {
                case "HeadYaw":
                    jointId = 0;
                    break;
                case "HeadPitch":
                    jointId = 1;
                    break;
                case "LShoulderPitch":
                    jointId = 2;
                    break;
                case "LShoulderRoll":
                    jointId = 3;
                    break;
                case "LElbowYaw":
                    jointId = 4;
                    break;
                case "LElbowRoll":
                    jointId = 5;
                    break;
                case "LWristYaw":
                    jointId = 6;
                    break;
                case "LHand":
                    jointId = 7;
                    break;
                case "LHipYawPitch":
                    jointId = 8;
                    break;
                case "LHipRoll":
                    jointId = 9;
                    break;
                case "LHipPitch":
                    jointId = 10;
                    break;
                case "LKneePitch":
                    jointId = 11;
                    break;
                case "LAnklePitch":
                    jointId = 12;
                    break;
                case "LAnkleRoll":
                    jointId = 13;
                    break;
                case "RHipYawPitch":
                    jointId = 14;
                    break;
                case "RHipRoll":
                    jointId = 15;
                    break;
                case "RHipPitch":
                    jointId = 16;
                    break;
                case "RKneePitch":
                    jointId = 17;
                    break;
                case "RAnklePitch":
                    jointId = 18;
                    break;
                case "RAnkleRoll":
                    jointId = 19;
                    break;
                case "RShoulderPitch":
                    jointId = 20;
                    break;
                case "RShoulderRoll":
                    jointId = 21;
                    break;
                case "RElbowYaw":
                    jointId = 22;
                    break;
                case "RElbowRoll":
                    jointId = 23;
                    break;
                case "RWristYaw":
                    jointId = 24;
                    break;
                case "RHand":
                    jointId = 25;
                    break;
                default:
                    jointId = 0;
                    throw new ArgumentOutOfRangeException("jointName", "no joint with name " + jointName);
                    break;
            }
            bw.Write(jointId);
            bufferSend = msSend.ToArray();
            try
            {
                handlerSocket.Send(bufferSend);
                bufferSize = handlerSocket.Receive(bufferReceive);
                msReceive = new MemoryStream(bufferReceive);
                br = new BinaryReader(msReceive);
                methodFlag = br.ReadString();
                if (methodFlag == "getAngle")
                {
                    angle = br.ReadSingle();
                }
            }
            catch
            {
                throw new InvalidOperationException("unable execute, check connection");
            }
            return angle;
        }
        public void closeHand(string hand)
        {
            methodFlag = "closeHand";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;
            if (hand == "RHand") { }
            else if (hand == "LHand") { }
            else
            {
                throw new ArgumentOutOfRangeException("hand", "only LHand and RHand as argument");
            }
            bw.Write(methodFlag);
            bw.Write(hand);
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
        public void openHand(string hand)
        {
            methodFlag = "openHand";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;
            if (methodFlag != "RHand" || methodFlag != "LHand")
            {
                throw new ArgumentOutOfRangeException("hand", "only LHand and RHand as argument");
            }
            bw.Write(methodFlag);
            bw.Write(hand);
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

        //locomotion
        public void moveInit()
        {
            methodFlag = "moveInit";

            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
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
        public void moveTo(float x, float y, float theta)
        {
            methodFlag = "moveTo";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
            bw.Write(x);
            bw.Write(y);
            bw.Write(theta);
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
        public void setWalkArmsEnabled(bool LHand, bool RHand)
        {
            methodFlag = "setWalkArmsEnabled";
            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
            bw.Write(LHand);
            bw.Write(RHand);
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
            methodFlag = "stopMove";

            MemoryStream msSend = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(msSend);
            byte[] buffer;

            bw.Write(methodFlag);
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
