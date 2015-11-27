using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Util;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Drawing;
//using Aldebaran.Proxies;
using NAudio;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;
namespace naoTest
{
    class Connection
    {
        private BasicDeliverEventArgs global = new BasicDeliverEventArgs();
       
        QueueingBasicConsumer consumer;
        static Stopwatch s = new Stopwatch();
        static string ip = "167.205.56.142";
        //static string ip = "127.0.0.1";
        static int port = 9559;
        public bool isConnected = false;
        public IModel channelSend, channelData;
        private IModel channelVisual1, channelVisual2, channelVisual3, channelAudio1, channelAudio2, channelAudio3, channelAvatar1, channelAvatar3;
        public EventingBasicConsumer consumerVisual1, consumerVisual2, consumerVisual3, consumerAudio1, consumerAudio2, consumerAudio3, consumerAvatar1, consumerAvatar2;
        public QueueingBasicConsumer consumerData;
        public IConnection connection;
        Connection connection2;
        public QueueingBasicConsumer  consumerFaceLocation, consumerDataJoint; // buat ack command
        public bool isAck = false;
        public string ackRoutingKey;
        public string corrId;
        TextName2 textName2;
        HOGObject objectNameX, objectNameY;

        public bool isCollecting = false;

        public Connection()
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.Uri = "amqp://lumen:lumen@167.205.66.79/%2F";
                IConnection connection = factory.CreateConnection();
                IModel channel = connection.CreateModel();
                channelSend = connection.CreateModel();
                channelData = connection.CreateModel();
                string routingKey = "avatar.NAO.data.image";
                var arg = new Dictionary<string, object>
                {
                    {"x-message-ttl",50}
                };
                QueueDeclareOk queue = channel.QueueDeclare("", true, false, true, arg);
                channel.QueueBind(queue.QueueName, "amq.topic", routingKey);
                consumer = new QueueingBasicConsumer(channel);
                channel.BasicConsume(queue.QueueName, true, consumer);
                Thread Query = new Thread(QueryImage);
                Query.Start();

                QueueDeclareOk queueData = channelData.QueueDeclare("", true, false, true, null);
                channelData.QueueBind(queueData.QueueName, "amq.topic", "lumen.visual.get.text");
                consumerData = new QueueingBasicConsumer(channelData);
                channelData.BasicConsume(queueData.QueueName, true, consumerData);
                Thread data = new Thread(QueryData);
                data.Start();
                Console.WriteLine("Setting Selesai");
            }
            catch
            {
                throw new InvalidOperationException();
            }
        }

        
        public void dataCollect_textRecognitionReceived(object sender, TextName2 name)
        {
            //if (state == 1)
            //{
            //    Console.WriteLine("Masuk ke state 1");
            //    if (name.text.ToLower() == "find a book")
            //    {
            //        command.NS_tts("" + name.text.ToLower() + "!");
            //        Thread.Sleep(300);
            //        state = 2;
            //        Console.WriteLine("Pindah ke state 2");
            //        //string objekcari=name.text.Substring(7);
            //    }
            //}

        }

        private void QueryImage()
        {
            BasicDeliverEventArgs ev = null;
            while (true)
            {

                ev = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                lock (global)
                {
                    global = ev;
                }
            }
        }
        /*public Image<Bgr, byte> getImage()
        {
            Image<Bgr, byte> ImageFrame;
            BasicDeliverEventArgs ev;
            if (global != null)
            {
                lock (global)
                {
                    ev = global;
                }
                if (ev.Body != null)
                {
                    string body = Encoding.UTF8.GetString(ev.Body);
                    JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                    ImageObject image = JsonConvert.DeserializeObject<ImageObject>(body, setting);
                    string base64 = image.ContentUrl.Replace("data:image/jpeg;base64,", "");
                    if (base64 != null)
                    {
                        byte[] imageByte = Convert.FromBase64String(base64);
                        MemoryStream ms = new MemoryStream(imageByte);
                        Bitmap bmp = (Bitmap)Image.FromStream(ms);
                        ImageFrame = new Image<Bgr, byte>(bmp);
                        return ImageFrame;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }*/

        public void connect()
        {
            if (!isConnected)
            {
                try
                {
                    string routingKey;
                    ConnectionFactory factory = new ConnectionFactory();
                    factory.Uri = "amqp://lumen:lumen@169.254.123.115/%2F";
                    connection = factory.CreateConnection();
                    channelSend = connection.CreateModel(); // untuk mengirim
                    channelData = connection.CreateModel();
                    channelAvatar3 = connection.CreateModel();

                    QueueDeclareOk queueData = channelData.QueueDeclare("", true, false, true, null);
                    QueueDeclareOk queueFaceLocation = channelData.QueueDeclare("", true, false, true, null);

                    channelData.QueueBind(queueData.QueueName, "amq.topic", "lumen.visual.get.text");

                    consumerFaceLocation = new QueueingBasicConsumer(channelData);
                    consumerDataJoint = new QueueingBasicConsumer(channelAvatar3);

                    channelData.BasicConsume(queueData.QueueName, true, consumerData);
                    channelData.BasicConsume(queueFaceLocation.QueueName, true, consumerFaceLocation);

                    isConnected = true;
                    Console.WriteLine("program is connected to server");
                    //Program.panel.btn_connect.Text = "Disconnect";
                }
                catch
                {
                    //MessageBox.Show("unable to connect to server", "connection");
                }
            }
            else
            {
                //MessageBox.Show("already connected to server!", "connection");
            }
        }

        public void disconnect()
        {
            if (isConnected)
            {
                if (!this.isProcessRunning())
                {
                    isConnected = false;
                    //Program.panel.btn_connect.Text = "Connect";
                    this.connection.Close();
                }
                else
                {
                    //MessageBox.Show("Stop Process Before Disconnecting", "Connection");
                }
            }
        }

        public bool isProcessRunning()
        {
            return true;
            //if ((Program.panel.dataCollect.isCollecting) || (Program.panel.command.isHandling))
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }

        

        public delegate void ObjectRecognition_callback(object sender, RecognizedObjects objects);
        public event ObjectRecognition_callback objectRecognitionReceived;
        public void QueryData()
        {
            BasicDeliverEventArgs ev = null;
            Console.WriteLine("start data thread");
            while (true)
            {
                Console.WriteLine("waiting..");
                ev = (BasicDeliverEventArgs)consumerData.Queue.Dequeue();
                Console.WriteLine("data Received");
                string body = Encoding.UTF8.GetString(ev.Body);
                JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                RecognizedObjects objk = JsonConvert.DeserializeObject<RecognizedObjects>(body, setting);
                if (objectRecognitionReceived != null)
                {
                    objectRecognitionReceived(this, objk);

                }
            }
        }
        public void consumerData_Received(object sender, BasicDeliverEventArgs ev)
        {
            string body = Encoding.UTF8.GetString(ev.Body);
            JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            objectNameX = JsonConvert.DeserializeObject<HOGObject>(body, setting);
            objectNameY = JsonConvert.DeserializeObject<HOGObject>(body, setting);
            if (objectRecognitionReceived != null)
            {
                //objectRecognitionReceived(this, objectNameX, objectNameY);
                Console.WriteLine("Data Received {0} : {1}", objectNameX.x, objectNameY.y);

            }
            Console.WriteLine("Data Received {0} : {1}", objectNameX.x, objectNameY.y);
        }

        public void sendGestureName(string name)
        {
            string routingKey = "lumen.visual.gesture.recognition";

            String gestName = name;
            HandGesture ha = new HandGesture { gesture = name };

            string hand = JsonConvert.SerializeObject(ha);
            byte[] buffer = Encoding.UTF8.GetBytes(hand);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);
        }
        public void sendObjectSurf(string name)
        {
            string routingKey = "lumen.visual.surf.recognition";

            String gestName = name;
            Object ob = new Object { objectsurf = name };

            string obj = JsonConvert.SerializeObject(ob);
            byte[] buffer = Encoding.UTF8.GetBytes(obj);
            channelSend.BasicPublish("amq.topic", routingKey, null, buffer);    
        }
        public void speak(String kata)
        {
            Parameter par = new Parameter { text = kata };
            Command cmd = new Command { type = "texttospeech", method = "say", parameter = par };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void berdiri(String command)
        {
            Parameter par = new Parameter { postureName = command };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "Stand", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void chrouch(String comChrouch)
        {
            Parameter par = new Parameter { postureName = comChrouch };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "Crouch", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void sit(String comSit)
        {
            Parameter par = new Parameter { postureName = comSit };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "Sit", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void lyingBelly(String lyingbelly)
        {
            Parameter par = new Parameter { postureName = lyingbelly };
            Command cmd = new Command { type = "posture", method = "gotoposture", parameter = new Parameter { postureName = "LyingBelly", speed = 0.5f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void jalan(float comJalan)
        {
            Parameter par = new Parameter {x = comJalan};
            Command cmd = new Command { type = "motion", method = "moveto", parameter = new Parameter { x = 0.1f, y = 0.0f, tetha = 0.0f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void stop(String comStop)
        {
            Parameter par = new Parameter();
            Command cmd = new Command { type = "motion", method = "rest", parameter = par };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void offNAO(String comOff)
        {
            Parameter par = new Parameter();
            Command cmd = new Command { type = "posture", method = "stopmove", parameter = par };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
            channelSend.BasicPublish("amq.topic", "avatar.NAO.command", null, buffer);
        }
        public void putarBalik(float comPutar)
        {
            Parameter par = new Parameter { x = comPutar };
            Command cmd = new Command { type = "motion", method = "moveto", parameter = new Parameter { x = 0.1f, y = 0.0f, tetha = 3.0f } };
            String body = JsonConvert.SerializeObject(cmd);
            Byte[] buffer = Encoding.UTF8.GetBytes(body);
        }
        //public void testSonar(String A)
        //{
        //    SonarProxy s = new SonarProxy(ip, port);
        //    MemoryProxy m = new MemoryProxy(ip, port);
        //    s.subscribe(A);
        //    while (true)
        //    {
        //        Console.WriteLine(m.getData("Device/SubDeviceList/US/Right/Sensor/Value"));
        //        delay(100);
        //    }
        //}
        public void delay(int time)
        {
            Stopwatch s = new Stopwatch();
            s.Reset();
            s.Start();//
            while (s.ElapsedMilliseconds < time)
            {
            }
            s.Stop();
        }
    }
}
