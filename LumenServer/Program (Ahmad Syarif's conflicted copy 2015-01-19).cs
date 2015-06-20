//standar library
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//additional library
using System.Net;
using System.Net.Sockets;
using System.IO;
using Aldebaran.Proxies;//naoqi-dotnet.dll
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace LumenServer
{
    
    class Program
    {
        private static Socket listenerSocket;
        private static Socket handlerSocket;
        public static string naoIP;
        public static int naoPort = 9559;
        public static NaoData data;
        //[STAThread]//**run this code to activate GUI**//
        static void Main(string[] args)
        {
            //**run this code to activate GUI**//
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ControlPanel());

            demo();

            //setting up the server
            //setUp();
            ////start to retrive NAO data;
            //data = new NaoData();
            //data.getNaoData();
            ////start wait for client to connect
            //waitForClient();
            
        }
        private static void demo()
        {
            while (true)
            {
                Console.WriteLine("NAO status : OK");
                delay(500);
                Console.WriteLine("NAO data retriving status : OK");
                delay(500);
                Console.WriteLine("Network status : OK");
                delay(500);
            }

        }
        public static void delay(int time)
        {
            Stopwatch s = new Stopwatch();
            s.Reset();
            s.Start();
            while (s.ElapsedMilliseconds < time) { };
            s.Stop();
        }
        private static void setUp()
        {
            Console.WriteLine("LUMEN SERVER VERSION 2.0");
            tryConnect:
            try
            {
                Console.Write("Please enter NAO IP address : ");
                naoIP = Console.ReadLine();
                MotionProxy motion = new MotionProxy(naoIP, naoPort);
                RobotPostureProxy posture = new RobotPostureProxy(naoIP, naoPort);
                TextToSpeechProxy tts = new TextToSpeechProxy(naoIP, naoPort);
                //motion.wakeUp();
                //posture.goToPosture("Stand", 0.7f);
                //posture.goToPosture("Crouch", 0.7f);
                tts.say("I am connected to Lumen Server");
                //motion.rest();
            }
            catch
            {
                Console.WriteLine("unable to connect to NAO");
                goto tryConnect;
            }

            Console.WriteLine("Initializing LUMEN Server...");
            listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenerSocket.Bind(new IPEndPoint(0, 2020));
            listenerSocket.Listen(0);
            Console.WriteLine("LUMEN Server is ready!");
        }
        private static void waitForClient()
        {
            byte[] buffer = new byte[1024];
            int bufferSize;
            MemoryStream msReceive;
            BinaryReader br;
            string message;
            try
            {
                while (true)
                {
                    handlerSocket = listenerSocket.Accept();
                    bufferSize = handlerSocket.Receive(buffer);
                    msReceive = new MemoryStream(buffer);
                    br = new BinaryReader(msReceive);
                    message = br.ReadString();
                    Console.WriteLine("new client is connected");
                    ClientHandler client = new ClientHandler(handlerSocket, message);
                }
                listenerSocket.Close();
                handlerSocket.Close();
            }
            catch(SocketException e)
            {
                Console.WriteLine("error in server");
                Console.WriteLine("message : " + e.ToString());
            }
        }
    }
}
