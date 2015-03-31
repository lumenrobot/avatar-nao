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

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace LumenServer
{
    
    class Program
    {
        public static string naoIP;
        public static int naoPort = 9559;

        public static DataAquisition aquisition;
        public static CommandHandler commandHandler;
        public static DataBroadcast broadcast;
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


            connectToRabbitMQ();

            aquisition = new DataAquisition();
            broadcast = new DataBroadcast();
            commandHandler = new CommandHandler();
            connectToNao();
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
            Console.WriteLine("NAO SERVER VERSION 3.0");
            tryConnect:
            try
            {
                Console.Write("Please enter NAO IP address : ");
                naoIP = Console.ReadLine();
                if (naoIP == "localhost")
                {
                    naoIP = "127.0.0.1";
                }

                /*
                Console.WriteLine("Connecting to NAO...");
                Console.WriteLine("Getting Motion proxy...");
                MotionProxy motion = new MotionProxy(naoIP, naoPort);
                Console.WriteLine("Getting Robot Posture proxy...");
                RobotPostureProxy posture = new RobotPostureProxy(naoIP, naoPort);
                Console.WriteLine("Getting Text-to-Speech proxy...");
                TextToSpeechProxy tts = new TextToSpeechProxy(naoIP, naoPort);
                //motion.wakeUp();
                //posture.goToPosture("Stand", 0.9f);
                //posture.goToPosture("Crouch", 0.9f);
                //tts.say("I am connected to nao Server");
                //motion.rest();
                motion.Dispose();
                posture.Dispose();
                tts.Dispose();
                 */
            }
            catch
            {
                Console.WriteLine("unable to connect to NAO");
                goto tryConnect;
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
