//standar library
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//additional library
using System.Net;
using System.Net.Sockets;
using System.IO;

using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

using Aldebaran.Proxies;//naoqi-dotnet.dll
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace LumenServer
{
    
    class Program
    {
        //nao IP and port, it will be set globally
        public static string naoIP;
        public static int naoPort = 9559;

        //create each of process handling class
        public static DataAquisition aquisition;
        public static CommandHandler commandHandler;
        public static DataBroadcast broadcast;
        
        //create rabbitMQ connection for global use
        public static ConnectionFactory factory;
        public static IConnection connection;
        public static NaoImage image = new NaoImage();
        public static NaoJoint joint = new NaoJoint();
        //[STAThread]//**run this code to activate GUI**//
        static void Main(string[] args)
        {
            //**run this code to activate GUI**//
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new ControlPanel());

            //set connection
            connectToRabbitMQ();
            connectToNao();

            //set process handling
            aquisition = new DataAquisition();
            broadcast = new DataBroadcast();
            commandHandler = new CommandHandler();
            
            //start to retrive NAO data;
            aquisition.startAquisitioning();
            //start broadcasting data
            broadcast.startBroadcasting();
            //start handling command
            commandHandler.startHandling();

            //setting up the server
            //while (true)
            //{
            //    if (aquisition.connection == false)
            //    {
            //        if (aquisition.connectionCheck.IsAlive == false && broadcast.connectionCheck.IsAlive == false && commandHandler.connectionCheck.IsAlive == false)
            //        {
            //            aquisition = new DataAquisition();
            //            broadcast = new DataBroadcast();
            //            commandHandler = new CommandHandler();
            //            connectToNao();
            //            //start to retrive NAO data;

            //            aquisition.startAquisitioning();
            //            //start broadcasting data

            //            broadcast.startBroadcasting();
            //            //start handling command
            //            commandHandler.startHandling();
            //        }
            //    }
            //}
        }
        
        private static void connectToNao()
        {
            // NOTE Hendy to Syarif: tinggal ganti ini aja untuk switch dari autoconfig ke ngetik IP manual
            bool autoconfiguration = false;

            Console.WriteLine("NAO SERVER VERSION 3.0");

            if (autoconfiguration)
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = @"C:/Python27/python.exe";
                start.Arguments = @"""C:/Users/Ahmad Syarif/git/NaoServer/LumenServer/bin/Debug/server.py""";
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                Process.Start(start);
            tryConnect:
                try
                {
                    bool flag = false;
                    Console.WriteLine("Waiting for NAO");
                    while (!flag)
                    {
                        flag = File.Exists(Environment.CurrentDirectory + "/ipaddress.txt");
                        if (flag)
                        {
                            naoIP = File.ReadAllText(Environment.CurrentDirectory + "/ipaddress.txt");
                            File.Delete(Environment.CurrentDirectory + "/ipaddress.txt");
                            Console.WriteLine("NAO is Online");
                            Console.WriteLine("configuring NAO...");
                            Thread.Sleep(10000);
                        }
                    }
                }
                catch
                {
                    //if can't connect to NAO, program will ask user to enter IP address again
                    Console.WriteLine("unable to connect to NAO");
                    goto tryConnect;
                }
            }
            else
            {
            tryConnect:
                try
                {
                    Console.Write("Please enter NAO IP address : ");
                    naoIP = Console.ReadLine();
                    if (naoIP == "localhost")
                    {
                        naoIP = "127.0.0.1";
                    }
                    Console.WriteLine("Connecting to NAO...");
                    //using (MotionProxy motion = new MotionProxy(naoIP, naoPort)) {}
                    //using (RobotPostureProxy posture = new RobotPostureProxy(naoIP, naoPort)) {}
                    using (TextToSpeechProxy tts = new TextToSpeechProxy(naoIP, naoPort))
                    {
                        tts.say("I am connected to nao Server");
                    }
                    //motion.wakeUp();
                    //posture.goToPosture("Stand", 0.9f);
                    //posture.goToPosture("Crouch", 0.9f);
                    //tts.say("I am connected to nao Server");
                    //motion.rest();
                }
                catch
                {
                    //if can't connect to NAO, program will ask user to enter IP address again
                    Console.WriteLine("unable to connect to NAO");
                    goto tryConnect;
                }
            }
            //Console.WriteLine("Initializing NAO Server...");
            //Console.WriteLine("NAO Server is ready!");
        }

        private static void connectToRabbitMQ()
        {
            factory = new ConnectionFactory();
            //factory.Uri = "amqp://lumen:lumen@167.205.56.130/%2F";
            factory.Uri = "amqp://guest:guest@127.0.0.1/%2F";
            connection = factory.CreateConnection();
        }
        private static void delay(int time)
        {
            Stopwatch s = new Stopwatch();
            s.Reset();
            s.Start();
            //don't do this... while (s.ElapsedMilliseconds < time) { }
            Thread.Sleep(time);
            s.Stop();
        }
    }
}
