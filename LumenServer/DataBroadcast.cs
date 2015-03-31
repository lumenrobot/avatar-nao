using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Media;
using System.Windows.Media; //presentationCore
using System.Windows.Media.Imaging;//presentationCore
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Aldebaran.Proxies;
using System.Threading;
using System.Diagnostics;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Content;
using Newtonsoft.Json;

//this class will handle data broadcasting to RabbitMQ server
namespace LumenServer
{
    class DataBroadcast
    {
        //initialize all channel,routing, and thread key for each type of Data
        private IModel imageChannel, jointChannel, sonarChannel, tactileChannel, batteryChannel;
        private string imageKey, jointKey, sonarKey, tactileKey, batteryKey;
        private Thread imageThread, jointThread, sonarThread, tactileThread, batteryThread;
        public Thread connectionCheck;
        public DataBroadcast()
        {
            //connectionCheck = new Thread(checkConnection);
        }

        //this method is to start broadcasting all data to rabbitMQ server
        //this method must be executed after startAquistion in DataAquisiton Class
        public void startBroadcasting()
        {
            //each data broadcasting will be handle by separated thread 
            imageThread = new Thread(broadcastImage);
            jointThread = new Thread(broadcastJoint);
            sonarThread = new Thread(broadcastSonar);
            tactileThread = new Thread(broadcastTactile);
            batteryThread = new Thread(broadcastBattery);
            createChannel();

            //start all thread
            imageThread.Start();
            //jointThread.Start();
            //sonarThread.Start();
            //tactileThread.Start();
            //batteryThread.Start(); 
            //if (connectionCheck.IsAlive == false)
            //{
            //    connectionCheck.Start();
            //}

        }
        //this method is to check whether NAO is still connected or not
        //not finished yet
        private void checkConnection()
        {
            while (true)
            {
                if (Program.aquisition.connection == false)
                {
                    Console.WriteLine("aborting data broadcaster...");
                    //imageThread.Abort();
                    //jointThread.Abort();
                    //sonarThread.Abort();
                    //tactileThread.Abort();
                    //batteryThread.Abort();
                    //connectionCheck.Abort();
                }
            }
        }
        //this method is to create channel and routing key for each data 
        private void createChannel()
        {
            imageChannel = Program.connection.CreateModel();
            tactileChannel = Program.connection.CreateModel();
            jointChannel = Program.connection.CreateModel();
            sonarChannel = Program.connection.CreateModel();
            batteryChannel = Program.connection.CreateModel();

            imageKey = "avatar.NAO.data.image";
            jointKey = "avatar.NAO.data.joint";
            sonarKey = "avatar.NAO.data.sonar";
            tactileKey = "avatar.NAO.data.tactile";
            batteryKey = "avatar.NAO.data.battery";

        }
        //this method will broadcast image data to rabbitMQ server
        public void broadcastImage()
        {
            bool flag = false;
            ImageObject image;
            byte[] data;
            long i=0;
            while (true)
            {
                try
                {
                    
                    image = new ImageObject();//we create new JSON object

                    lock (Program.aquisition.image) //locking data for multithreading synchronization
                    {
                        data = Program.aquisition.image.data;
                    }
                    //this code below is to encode byte of image data to stringBase64
                    //this encoding process is to encode the image file into more reliable format to send through network
                    BitmapSource imageBitmap = BitmapSource.Create(
                                                320,
                                                240,
                                                96,
                                                96,
                                                PixelFormats.Rgb24,
                                                BitmapPalettes.WebPalette,
                                                data,
                                                320 * 3);
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(imageBitmap));
                    MemoryStream ms = new MemoryStream();
                    encoder.Save(ms);
                    Bitmap imageFinal = new Bitmap(ms);
                    string url = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());

                    //set image parameter
                    image.ContentSize = url.Length;
                    image.ContentUrl = url;
                    image.Name = i.ToString(); //name the image with number of image created
                    i++;
                    string body = JsonConvert.SerializeObject(image); //convert image into string 
                    byte[] buffer = Encoding.UTF8.GetBytes(body);//encoding the string into byte using UTF8 encoding
                    
                    imageChannel.BasicPublish("amq.topic", imageKey, null, buffer);//send the image data to rabbitMQ
                    if (!flag)
                    {
                        Console.WriteLine("broadcasting image data...");//inform user that image broadcasting is running
                        flag = true;
                    }
                }
                catch(Exception)
                {
                }
            }
        }
        public void broadcastJoint()
        {
            bool flag = false;
            JointData joint;
            tryAgain:
            while (true)
            {
                try
                {
                    joint = new JointData();
                    lock (Program.aquisition.joint)
                    {
                        joint.Names = Program.aquisition.joint.names;
                        joint.Angles = Program.aquisition.joint.angles;
                        joint.Stiffnesses = Program.aquisition.joint.stiffnesses;
                    }
                    string body = JsonConvert.SerializeObject(joint);
                    byte[] buffer = Encoding.UTF8.GetBytes(body);
                    jointChannel.BasicPublish("amq.topic", jointKey, null, buffer);
                    if (!flag)
                    {
                        Console.WriteLine("broadcasting joint data...");
                        flag = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error broadcasting joint data");
                    //Console.WriteLine(e.ToString());
                    goto tryAgain;
                }
            }
        }
        public void broadcastBattery()
        {
            bool flag = false;
            BatteryData battery;
            tryAgain:
            while (true)
            {
                try
                {
                    battery = new BatteryData();
                    lock (Program.aquisition.battery)
                    {
                        battery.Percentage = Program.aquisition.battery.percentage;
                        battery.IsCharging = Program.aquisition.battery.isCharging;
                        battery.IsPlugged = Program.aquisition.battery.isPlugged;
                    }
                    string body = JsonConvert.SerializeObject(battery);
                    byte[] buffer = Encoding.UTF8.GetBytes(body);
                    batteryChannel.BasicPublish("amq.topic", batteryKey, null, buffer);
                    if (!flag)
                    {
                        Console.WriteLine("broadcasting battery data...");
                        flag = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error broadcasting battery data");
                    //Console.WriteLine(e.ToString());
                    goto tryAgain;
                }
            }
        }
        public void broadcastSonar()
        {
            bool flag = false;
            SonarData sonar;
            tryAgain:
            while (true)
            {
                try
                {
                    sonar = new SonarData();
                    lock (Program.aquisition.sonar)
                    {
                        sonar.LeftSensor = Program.aquisition.sonar.leftSensor;
                        sonar.RightSensor = Program.aquisition.sonar.rightSensor;
                    }
                    string body = JsonConvert.SerializeObject(sonar);
                    byte[] buffer = Encoding.UTF8.GetBytes(body);
                    sonarChannel.BasicPublish("amq.topic", sonarKey, null, buffer);
                    if (!flag)
                    {
                        Console.WriteLine("broadcasting sonar data...");
                        flag = true;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("error broadcasting sonar data");
                    //Console.WriteLine(e.ToString());
                    goto tryAgain;
                }
            }
        }
        public void broadcastTactile()
        {
            bool flag = false;
            TactileData tactile;
            tryAgain:
            while (true)
            {
                try
                {
                    tactile = new TactileData();
                    lock (Program.aquisition.tactile)
                    {
                        tactile.Names = Program.aquisition.tactile.names;
                        tactile.Values = Program.aquisition.tactile.values;
                    }

                    string body = JsonConvert.SerializeObject(tactile);
                    byte[] buffer = Encoding.UTF8.GetBytes(body);
                    tactileChannel.BasicPublish("amq.topic", tactileKey, null, buffer);
                    if (!flag)
                    {
                        Console.WriteLine("broadcasting tactile data...");
                        flag = true;
                    }
                }
                catch (InvalidOperationException e)
                {
                    Console.WriteLine("error broadcasting tactile data");
                    //Console.WriteLine(e.ToString());
                    break;
                    goto tryAgain;
                }
            }
        }
    }
}
