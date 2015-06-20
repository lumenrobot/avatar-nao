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
using Renci.SshNet;

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
        static public SftpClient sftpClient;
        static KeyboardInteractiveAuthenticationMethod keyboard = new KeyboardInteractiveAuthenticationMethod("nao");
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
            //Application.Run();
            //setting up the server
           
        }
        
        private static void connectToNao()
        {
            Console.WriteLine("NAO SERVER VERSION 3.0");
            //ProcessStartInfo start = new ProcessStartInfo();
            //start.FileName = @"C:/Python27/python.exe";
            //start.WorkingDirectory = Environment.CurrentDirectory;
            //start.Arguments = @"server.py";
            //start.UseShellExecute = false;
            //start.RedirectStandardOutput = true;
            //Process.Start(start);
            tryConnect:
            try
            {
                //bool flag = false;
                //Console.WriteLine("Waiting for NAO");
                //while (!flag)
                //{
                //    flag = File.Exists(Environment.CurrentDirectory + "/ipaddress.txt");
                //    if (flag)
                //    {
                //        naoIP = File.ReadAllText(Environment.CurrentDirectory + "/ipaddress.txt");
                //        File.Delete(Environment.CurrentDirectory + "/ipaddress.txt");
                //        Console.WriteLine("NAO is Online");
                //        Console.WriteLine("configuring NAO...");
                //        Thread.Sleep(10000);
                //    }
                //}

                //Console.Write("Please enter NAO IP address : ");
                Console.Write("please enter NAO IP : ");
                naoIP = Console.ReadLine();
                Console.WriteLine("Connecting to NAO...");
                MotionProxy motion = new MotionProxy(naoIP, naoPort);
                RobotPostureProxy posture = new RobotPostureProxy(naoIP, naoPort);
                
                keyboard.AuthenticationPrompt += new EventHandler<Renci.SshNet.Common.AuthenticationPromptEventArgs>(keyboard_AuthenticationPrompt);
                ConnectionInfo info = new ConnectionInfo(naoIP, 22, "nao", new AuthenticationMethod[] { keyboard });
                sftpClient = new SftpClient(info);
                sftpClient.Connect();

                //Console.WriteLine("Connecting to NAO...");
                //Console.WriteLine("Getting Motion proxy...");
                //MotionProxy motion = new MotionProxy(naoIP, naoPort);
                //Console.WriteLine("Getting Robot Posture proxy...");
                //RobotPostureProxy posture = new RobotPostureProxy(naoIP, naoPort);

                //Console.WriteLine("Getting Text-to-Speech proxy...");
                //TextToSpeechProxy tts = new TextToSpeechProxy(naoIP, naoPort);
                //motion.wakeUp();
                //posture.goToPosture("Stand", 0.9f);
                //posture.goToPosture("Crouch", 0.9f);
                //tts.say("I am connected to nao Server");
                //motion.rest();
                //motion.Dispose();
                //posture.Dispose();
                //tts.Dispose();
                // */
            }
            catch
            {
                //if can't connect to NAO, program will ask user to enter IP address again
                Console.WriteLine("unable to connect to NAO");
                goto tryConnect;
            }
            //Console.WriteLine("Initializing NAO Server...");
            //Console.WriteLine("NAO Server is ready!");
        }
        static private void keyboard_AuthenticationPrompt(object sender, Renci.SshNet.Common.AuthenticationPromptEventArgs e)
        {
            foreach (var prompt in e.Prompts)
            {
                if (prompt.Request.Equals("Password: ", StringComparison.InvariantCultureIgnoreCase))
                {
                    prompt.Response = "nao";
                }
            }
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
