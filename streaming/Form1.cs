using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
namespace streaming
{
    public partial class Form1 : Form
    {
        IModel channel;
        Subscription sub;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = "amqp://lumen:lumen@167.205.66.130/%2F";
            IConnection conn = factory.CreateConnection();
            channel = conn.CreateModel();
            conn.AutoClose = true;
            Console.WriteLine("Connected to AMQP broker '{0}:{1}'", conn.RemoteEndPoint, conn.RemotePort);

            QueueDeclareOk cameraStream = channel.QueueDeclare("", false, true, true, null);
            Console.WriteLine("Declared anonymous exclusive queue '{0}'", (object)cameraStream.QueueName);
            string cameraStreamKey = "avatar.NAO.data.image";
            channel.QueueBind(cameraStream.QueueName, "amq.topic", cameraStreamKey);
            Console.WriteLine("Bound queue '{0}' to topic '{1}'", cameraStream.QueueName, cameraStreamKey);
            sub = new Subscription(channel, cameraStream.QueueName);

            showImage();

        }

        private void showImage()
        {
            BasicDeliverEventArgs ev;
            string imageString = null;
            Console.WriteLine("starting taking data");
            while (true)
            {
                  if (sub.Next(0, out ev))
                    {
                        try
                        {
                            string body = Encoding.UTF8.GetString(ev.Body);
                            JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                            ImageObject image = JsonConvert.DeserializeObject<ImageObject>(body, setting);
                            if (image.ContentUrl.StartsWith("data:image/jpeg;base64,"))
                            {
                                imageString = image.ContentUrl.Replace("data:image/jpeg;base64,", "");
                            }
                            byte[] buffer = Convert.FromBase64String(imageString);
                            MemoryStream ms = new MemoryStream(buffer);
                            Bitmap imageFinal = new Bitmap(ms);
                            Console.WriteLine("show inage");
                            pictureBox1.Image = imageFinal;
                        }
                        finally
                        {
                            sub.Ack(ev);
                        }
                        Thread.Sleep(500);   
                    }
                
               
            }

        }

    }
}
