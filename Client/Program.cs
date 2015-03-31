using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
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
            sendCommandWav();
        }
        static void sendCommandWav()
        {
            Console.WriteLine("setting connection");
            IModel channel;
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = "amqp://guest:guest@localhost/%2F";
            string channelKey = "avatar.NAO.command";
            IConnection conn = factory.CreateConnection();
            channel = conn.CreateModel();
            byte[] byteWav = File.ReadAllBytes(@"D:\cobalagi.wav");
            string stringWav = Convert.ToBase64String(byteWav);
            Parameter par = new Parameter { wavFile = stringWav };
            Command com = new Command { type = "audiodevice", method = "sendremotebuffertooutput", parameter = par };
            string body = JsonConvert.SerializeObject(com);
            byte[] buffer = Encoding.UTF8.GetBytes(body);
            channel.BasicPublish("amq.topic", channelKey, null, buffer);
            Console.WriteLine("succesfully sending file");
            Console.ReadKey();
        }
        static void sendWav()
        {
            Console.WriteLine("setting connection");
            IModel channel,cha;
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = "amqp://lumen:lumen@167.205.66.130/%2F";
            string channelKey = "lumen.arkan.wav.stream";
            string key = "lumen.arkan.speech.recognition";
            
            factory.Uri = "amqp://lumen:lumen@167.205.66.186/%2F";
            IConnection conn = factory.CreateConnection();
            channel = conn.CreateModel();
            cha = conn.CreateModel();
            QueueDeclareOk qu = cha.QueueDeclare("", true, false, true, null);
            cha.QueueBind(qu.QueueName, "amq.topic", key);
            Subscription sub = new Subscription(cha, qu.QueueName);
            conn.AutoClose = true;

            byte[] data = File.ReadAllBytes(@"D:\wav\aaaa.wav");
            string cont = Convert.ToBase64String(data);
            sound s = new sound { name = "M1F1-Alaw-AFsp.wav", content = cont };
            string body = JsonConvert.SerializeObject(s);
            byte[] buffer = Encoding.UTF8.GetBytes(body);
            channel.BasicPublish("amq.topic", channelKey, null, buffer);

            BasicDeliverEventArgs ev;
            while (true)
            {
                if (sub.Next(0, out ev))
                {
                    string bodyterima = Encoding.UTF8.GetString(ev.Body);
                    JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                    recognizer r = JsonConvert.DeserializeObject<recognizer>(bodyterima, setting);
                    Console.WriteLine(r.result);
                }
                else
                {
                }
            }
        }
        static void testData()
        {
            try
            {
                Console.WriteLine("setting connection");
                IModel channel;
                ConnectionFactory factory = new ConnectionFactory();
                factory.Uri = "amqp://lumen:lumen@167.205.66.130/%2F";
                string channelKey = "lumen.arkan.speech.recognition";
                IConnection conn = factory.CreateConnection();
                channel = conn.CreateModel();
                QueueDeclareOk queue = channel.QueueDeclare("", true, false, true, null);
                channel.QueueBind(queue.QueueName, "amq.topic", channelKey);
                Subscription sub = new Subscription(channel, queue.QueueName);
                conn.AutoClose = true;
                BasicDeliverEventArgs ev;
                Console.WriteLine("waiting..");
                while (true)
                {
                    try
                    {
                        if (sub.Next(0, out ev))

                        {

                            string hasil = Encoding.UTF8.GetString(ev.Body);
                            JsonSerializerSettings set = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                            recognizer coba = JsonConvert.DeserializeObject<recognizer>(hasil, set);

                            Console.WriteLine(coba.name);
                            Console.WriteLine(coba.result);
                            Console.WriteLine(coba.date);
                        }
                        string body = Encoding.UTF8.GetString(ev.Body);
                        Console.WriteLine("okokoko ko");
                        //JsonSerializerSettings setting = new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects};
                        //BatteryData data = JsonConvert.DeserializeObject<BatteryData>(body,setting);
                        //Console.WriteLine("percentage : " + data.percentage);
                        //Console.WriteLine("isCharging : " + data.isCharging);
                        //Console.WriteLine("isPlugged : " + data.isPlugged);
                    }
                    finally
                    {
                    }
                }
            }
            catch
            {
            }
        }
        //static void testCommandJson()
        //{
        //    try
        //    {
        //        Console.WriteLine("setting connection");
        //        IModel channel;
        //        ConnectionFactory factory = new ConnectionFactory();
        //        factory.Uri = "amqp://lumen:lumen@167.205.66.130/%2F";
        //        string channelKey = "avatar.NAO.command";
        //        IConnection conn = factory.CreateConnection();
        //        channel = conn.CreateModel();
        //        conn.AutoClose = true;
        //        QueueDeclareOk queue = channel.QueueDeclare("", true, false, true, null);
        //        Command newCommand = new Command("Motion", "wakeUp", "");
        //        string body = JsonConvert.SerializeObject(newCommand);
        //        byte[] buffer = Encoding.UTF8.GetBytes(body);
        //        Console.WriteLine("sending command");
        //        channel.BasicPublish("amq.topic", channelKey, null, buffer);
        //        Console.WriteLine("command Sent");
        //        Console.ReadKey();

                
        //        ArrayList joint = new ArrayList();
        //        joint.Add("RShoulderPitch");
        //        joint.Add("LShoulderPitch");
        //        object o1 = (object)joint;
        //        ArrayList angle = new ArrayList();
        //        angle.Add(0.5f);
        //        angle.Add(0.5f);
        //        object o2 = (object)angle;
        //        object o3 = (object)0.2f;
        //        ArrayList parameter = new ArrayList();
        //        parameter.Add(o1);
        //        parameter.Add(o2);
        //        parameter.Add(o3);
        //        object par = (object)parameter;

        //        Command setangle = new Command("motion", "setAngles", par);
        //        string body3 = JsonConvert.SerializeObject(setangle);
        //        byte[] buf3 = Encoding.UTF8.GetBytes(body3);
        //        //if (subJoint.Next(0, out ev))
        //        //{
        //        //    try
        //        //    {
        //        //        string body = Encoding.UTF8.GetString(ev.Body);
        //        //        JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
        //        //        JointData data = JsonConvert.DeserializeObject<JointData>(body, setting);
        //        //        Console.WriteLine("message : " + body);
        //        //    }
        //        //    finally
        //        //    {
        //        //        subJoint.Ack(ev);
        //        //    }
        //        //}
        //        Console.WriteLine("sending command");
        //        channel.BasicPublish("amq.topic", channelKey, null, buf3);
        //        Console.WriteLine("command Sent");
        //        Console.ReadKey();

        //        Command comm2 = new Command("Motion", "rest", "");
        //        string body2 = JsonConvert.SerializeObject(comm2);
        //        byte[] buffer2 = Encoding.UTF8.GetBytes(body2);

        //        Console.WriteLine("sending command");
        //        channel.BasicPublish("amq.topic", channelKey, null, buffer2);
        //        Console.WriteLine("command Sent");
        //        Console.ReadKey();

        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //        Console.Read();
        //    }
        //}
        static void testJason()
        {
            //setting up connection
            try
            {
                Console.WriteLine("setting connection");
                IModel channelJoint, channelBattery;
                Subscription subJoint, subBattery;
                ConnectionFactory factory = new ConnectionFactory();
                factory.Uri = "amqp://lumen:lumen@167.205.66.130/%2F";
                IConnection conn = factory.CreateConnection();
                channelJoint = conn.CreateModel();
                channelBattery = conn.CreateModel();
                conn.AutoClose = true;

                QueueDeclareOk jointStream = channelJoint.QueueDeclare("", false, true, true, null);
                QueueDeclareOk batteryStream = channelBattery.QueueDeclare("", false, true, true, null);

                string jointDataKey = "avatar.NAO.data.joint";
                string batteryDataKey = "avatar.NAO.data.battery";
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
                            Console.WriteLine("data available");
                            JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                            BatteryData data = JsonConvert.DeserializeObject<BatteryData>(body, setting);
                            Console.WriteLine("percentage : " + data.Percentage);
                            Console.WriteLine("isCharging : " + data.IsCharging);
                            Console.WriteLine("isPlugged : " + data.IsPlugged);
                        }
                        finally
                        {
                            subBattery.Ack(ev);
                        }
                    }
                    else
                    {
                        //Console.WriteLine("wait...");
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
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Read();
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
