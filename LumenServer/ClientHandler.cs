using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace LumenServer
{
    public class ClientHandler
    {
        private Socket handlerSocket;
        private Thread handlerThread;
        private LumenMotion motion;
        private LumenVideoDevice videoDevice;
        private LumenPosture posture;
        private LumenTextToSpeech tts;
        private LumenBattery battery;
        private LumenSonar sonar;
        private LumenSensor sensor;
        public ClientHandler(Socket handlerSocket, string message)
        {
            this.handlerSocket = handlerSocket;
            switch (message)
            {
                case "LumenMotion":
                    motion = new LumenMotion();
                    handlerThread = new Thread(motionHandling);
                    handlerThread.Start();
                    break;
                case "LumenVideoDevice":
                    videoDevice = new LumenVideoDevice();
                    handlerThread = new Thread(videoDeviceHandling);
                    handlerThread.Start();
                    break;
                case "LumenPosture":
                    posture = new LumenPosture();
                    handlerThread = new Thread(postureHandling);
                    handlerThread.Start();
                    break;
                case "LumenTextToSpeech":
                    tts = new LumenTextToSpeech();
                    handlerThread = new Thread(textToSpeechHandling);
                    handlerThread.Start();
                    break;
                case "LumenBattery":
                    battery = new LumenBattery();
                    handlerThread = new Thread(batteryHandling);
                    handlerThread.Start();
                    break;
                case "LumenSonar":
                    sonar = new LumenSonar();
                    handlerThread = new Thread(sonarHandling);
                    break;
                case "LumenSensor":
                    sensor = new LumenSensor();
                    handlerThread = new Thread(sensorHandling);
                    break;
            }
        }
        private void motionHandling()
        {
            int bufferSize;
            string method;
            byte[] bufferReceive = new byte[1024];
            MemoryStream msReceive;
            BinaryReader br;
            try
            {
                while (true)
                {
                    bufferSize = handlerSocket.Receive(bufferReceive);
                    msReceive = new MemoryStream(bufferReceive);
                    br = new BinaryReader(msReceive);
                    method = br.ReadString();
                    switch (method)
                    {
                        //stiffness control
                        case "wakeUp":
                            motion.wakeUp();
                            break;
                        case "rest":
                            motion.rest();
                            break;
                        case "setStiffness":
                            motion.setStiffness(br);
                            break;
                        case "getStiffness":
                            motion.getStiffness(handlerSocket, br);
                            break;

                        //joint control
                        case "getAngle":
                            motion.getAngle(handlerSocket, br);
                            break;
                        case "setAngle":
                            motion.setAngle(br);
                            break;
                        case "changeAngle":
                            motion.changeAngle(br);
                            break;
                        case "openHand":
                            motion.openHand(br);
                            break;
                        case "closeHand":
                            motion.closeHand(br);
                            break;

                        //locomotion
                        case "moveInit":
                            motion.moveInit();
                            break;
                        case "moveTo":
                            motion.moveTo(br);
                            break;
                        case "setWalkArmsEnabled":
                            motion.setWalkArmsEnabled(br);
                            break;
                        case "stopMove":
                            motion.stopMove();
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("client is disconnected");
                handlerSocket.Close();
                handlerThread.Abort();
            }
        }
        private void videoDeviceHandling()
        {
            int bufferSize;
            int id = Program.data.defaultImageId;
            string method;

            byte[] bufferReceive = new byte[299];
            MemoryStream msReceive;
            BinaryReader br;
            try
            {
                while (true)
                {
                    bufferSize = handlerSocket.Receive(bufferReceive);
                    msReceive = new MemoryStream(bufferReceive);
                    br = new BinaryReader(msReceive);
                    method = br.ReadString();
                    switch (method)
                    {
                        case "getImageRemote":
                            videoDevice.getImageRemote(handlerSocket);
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("client is Disconnected");
                handlerThread.Abort();
                handlerSocket.Close();
            }
        }
        private void postureHandling()
        {
            int bufferSize;
            string method;

            byte[] bufferReceive = new byte[1024];
            MemoryStream msReceive;
            BinaryReader br;
            try
            {
                while (true)
                {
                    bufferSize = handlerSocket.Receive(bufferReceive);
                    msReceive = new MemoryStream(bufferReceive);
                    br = new BinaryReader(msReceive);
                    method = br.ReadString();
                    switch (method)
                    {
                        case "goToPosture":
                            posture.goToPosture(br);
                            break;
                        case "stopMove":
                            posture.stopMove();
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("client is Disconnected");
                handlerThread.Abort();
                handlerSocket.Close();
            }
        }
        private void textToSpeechHandling()
        {
            int bufferSize;
            string method;

            byte[] bufferReceive = new byte[1024];
            MemoryStream msReceive;
            BinaryReader br;
            try
            {
                while (true)
                {
                    bufferSize = handlerSocket.Receive(bufferReceive);
                    msReceive = new MemoryStream(bufferReceive);
                    br = new BinaryReader(msReceive);
                    method = br.ReadString();
                    switch (method)
                    {
                        case "say":
                            tts.say(br);
                            break;
                        case "setLanguage":
                            tts.setLanguage(br);
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("client is Disconnected");
                handlerThread.Abort();
                handlerSocket.Close();
            }
        }
        private void batteryHandling()
        {
            int bufferSize;
            string method;

            byte[] bufferReceive = new byte[1024];
            MemoryStream msReceive;
            BinaryReader br;
            try
            {
                while (true)
                {
                    bufferSize = handlerSocket.Receive(bufferReceive);
                    msReceive = new MemoryStream(bufferReceive);
                    br = new BinaryReader(msReceive);
                    method = br.ReadString();
                    switch (method)
                    {
                        case "getBatteryPercentage":
                            battery.getBatteryPercentage(handlerSocket);
                            break;
                        case "isPlugged":
                            battery.isPlugged(handlerSocket);
                            break;
                        case "isCharged":
                            battery.isCharging(handlerSocket);
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("client is Disconnected");
                handlerThread.Abort();
                handlerSocket.Close();
            }
        }
        private void sonarHandling()
        {
            int bufferSize;
            string method;

            byte[] bufferReceive = new byte[1024];
            MemoryStream msReceive;
            BinaryReader br;
            try
            {
                while (true)
                {
                    bufferSize = handlerSocket.Receive(bufferReceive);
                    msReceive = new MemoryStream(bufferReceive);
                    br = new BinaryReader(msReceive);
                    method = br.ReadString();
                    switch (method)
                    {
                        case "getDistance":
                            sonar.getDistance(handlerSocket, br);
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("client is Disconnected");
                handlerThread.Abort();
                handlerSocket.Close();
            }
        }
        private void sensorHandling()
        {
            int bufferSize;
            string method;

            byte[] bufferReceive = new byte[1024];
            MemoryStream msReceive;
            BinaryReader br;
            try
            {
                while (true)
                {
                    bufferSize = handlerSocket.Receive(bufferReceive);
                    msReceive = new MemoryStream(bufferReceive);
                    br = new BinaryReader(msReceive);
                    method = br.ReadString();
                    switch (method)
                    {
                        case "getTactile":
                            sensor.getTactile(handlerSocket, br);
                            break;
                        case "getBumper":
                            sensor.getBumper(handlerSocket, br);
                            break;
                        case "getButton":
                            sensor.getButton(handlerSocket, br);
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("client is Disconnected");
                handlerThread.Abort();
                handlerSocket.Close();
            }
        }
    }
}
