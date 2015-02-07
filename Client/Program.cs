using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.IO;
using LumenAPI;
using System.Drawing;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            testJason();
        }
        static void testJason()
        {
            //setting up connection
            
            Console.WriteLine("setting connection");
            IModel channelJoint,channelBattery;
            Subscription subJoint,subBattery;
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = "amqp://guest:guest@localhost/%2F";
            IConnection conn = factory.CreateConnection();
            channelJoint = conn.CreateModel();
            channelBattery = conn.CreateModel();
            conn.AutoClose = true;

            QueueDeclareOk jointStream = channelJoint.QueueDeclare("", false, true, true, null);
            QueueDeclareOk batteryStream = channelBattery.QueueDeclare("", false, true, true, null);

            string jointDataKey = "lumen.NAO.data.joint";
            string batteryDataKey = "lumen.NAO.data.battery";
            channelJoint.QueueBind(jointStream.QueueName, "amq.topic", jointDataKey);
            channelBattery.QueueBind(batteryStream.QueueName, "amq.topic", batteryDataKey);
            subJoint = new Subscription(channelJoint, jointStream.QueueName);
            subBattery = new Subscription(channelBattery, batteryStream.QueueName);
            Console.WriteLine("waiting...");
            BasicDeliverEventArgs ev;
            while (true)
            {
                if (subBattery.Next(0, out ev))
                {
                    try
                    {
                        string body = Encoding.UTF8.GetString(ev.Body);
                        JsonSerializerSettings setting = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};
                        BatteryData data = JsonConvert.DeserializeObject<BatteryData>(body,setting);
                        Console.WriteLine("percentage : " + data.percentage);
                        Console.WriteLine("isCharging : " + data.isCharging);
                        Console.WriteLine("isPlugged : " + data.isPlugged);
                    }
                    finally
                    {
                        subBattery.Ack(ev);
                    }
                }
                //if (subJoint.Next(0, out ev))
                //{
                //    try
                //    {
                //        string body = Encoding.UTF8.GetString(ev.Body);
                //        JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                //        JointData data = JsonConvert.DeserializeObject<JointData>(body, setting);
                //        Console.WriteLine("message : " + body);
                //    }
                //    finally
                //    {
                //        subJoint.Ack(ev);
                //    }
                //}
                
            }
        }
        static void coba()
        {
            Stopwatch s = new Stopwatch();
            string ip = "127.0.0.1";
            int port = 2020;
            float angle;

            //LumenMotion motion = new LumenMotion(ip, port);
            //angle = motion.getAngle("HeadYaw");
            //Console.WriteLine("success angle :" + angle.ToString());
            Console.WriteLine("start");
            byte[] image;
            LumenVideoDevice video = new LumenVideoDevice(ip, port);
            try
            {
                Console.WriteLine("enter try");
                while (true)
                {
                    image = video.getImageRemote();
                    if (image.Length != 230400)
                    {
                        Console.WriteLine("salah");
                    }
                    else
                    {
                        Console.WriteLine("benarr sekali...............");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error " + e.ToString());
            }
            //while (true)
            //{
            //    s.Reset();
            //    s.Start();
            //    byte[] image = video.getImageRemote();
            //    s.Stop();
            //    Console.WriteLine("time elapsed : " + s.ElapsedMilliseconds.ToString());
            //}
            //Console.WriteLine("success image :" + image.Length.ToString());


            Console.ReadKey();
        }
    }
}
