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
using Newtonsoft.Json;

namespace LumenServer
{
    public class NaoImage
    {
        public string id;
        public byte[] data;
        public int resolution;
        public int fps;
        public int imageSize;
        public NaoImage()
        {
        }
    }
    public class NaoJoint
    {
        public string name;
        public float angle;
        public float stiffness;
        public NaoJoint()
        {
        }
    }
    public class NaoBattery
    {
        public int percentage;
        public bool isPlugged;
        public bool isCharging;
    }
    public class NaoSonar
    {
        public float rightSensor;
        public float leftSensor;
    }
    public class NaoTactile
    {
        public float value;
    }
    public class NaoData
    {
        public List<NaoImage> image = new List<NaoImage>();
        public int defaultImageId;
        public List<NaoJoint> joint = new List<NaoJoint>();
        public NaoBattery battery = new NaoBattery();
        public NaoSonar sonar = new NaoSonar();
        public List<NaoTactile> tactile = new List<NaoTactile>();
        Stopwatch delayTime = new Stopwatch();

        //code related to Json and RabbitMQ
        private IModel channel;
        private Subscription sub;
        private string imageDataKey = "lumen.NAO.data.image";
        private string jointDataKey = "lumen.NAO.data.joint";
        private string batteryDataKey = "lumen.NAO.data.battery";
        private string sonarDataKey = "lumen.NAO.data.sonar";
        private string tactileDataKey = "lumen.NAO.data.tactile";
        //end
        public NaoData()
        {
            #region to define variable to contain image data
            //create variable to contain the image data
            NaoImage _image;
            _image = new NaoImage();
            _image.id = "res0";
            _image.data = new byte[57600];
            _image.resolution = 0; //160*120
            _image.fps = 30; //optimum frame rate
            image.Add(_image);

            _image = new NaoImage();
            _image.id = "res1";
            _image.data = new byte[230400];
            _image.resolution = 1;//320*240
            _image.fps = 30;// optimum frame rate
            image.Add(_image);

            _image = new NaoImage();
            _image.id = "res2";
            _image.data = new byte[921600];
            _image.resolution = 2;//640*480
            _image.fps = 15; // optimum frame rate
            image.Add(_image);

            _image = new NaoImage();
            _image.id = "res3";
            _image.data = new byte[3686400];
            _image.resolution = 3;//1280*960
            _image.fps = 5; // optimum frame rate
            image.Add(_image);
            #endregion
            #region to define variable to contain joint data
            //create variable to contain joint data;
            NaoJoint _joint;

            #region to define head joint

            _joint = new NaoJoint(); 
            _joint.name = "HeadYaw";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);


            _joint = new NaoJoint(); 
            _joint.name = "HeadPitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);
            #endregion

            #region to define LArm joint
            _joint = new NaoJoint(); 
            _joint.name = "LShoulderPitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LShoulderRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LElbowYaw";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LElbowRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LWristYaw";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LHand";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);
            #endregion

            #region to define LLeg Joint
            _joint = new NaoJoint();
            _joint.name = "LHipYawPitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LHipRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LHipPitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LKneePitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LAngklePitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "LAngkleRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            #endregion

            #region to define RLeg joint
            _joint = new NaoJoint();
            _joint.name = "RHipYawPitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RHipRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RHipPitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RKneePitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RAngklePitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RAngkleRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);
            #endregion

            #region to define RArm joint
            _joint = new NaoJoint();
            _joint.name = "RShoulderPitch";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RShoulderRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RElbowYaw";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RElbowRoll";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RWristYaw";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);

            _joint = new NaoJoint();
            _joint.name = "RHand";
            _joint.angle = 0.0f; //initial value
            _joint.stiffness = 0.0f; //initial value
            joint.Add(_joint);
            #endregion
            #endregion
            #region to define variable to contain tactile data
            NaoTactile _tactile = new NaoTactile();
            _tactile.value = 0.0f;
            for (int i = 0; i < 12; i++)
            {
                tactile.Add(_tactile);
            }
            #endregion


        }
        public void getNaoData()
        {
            Console.WriteLine("start retriving data from NAO");
            Thread imageThread = new Thread(getImageData);
            Thread jointThread = new Thread(getJointData);
            Thread batteryThread = new Thread(getBatteryData);
            Thread sonarThread = new Thread(getSonarData);
            Thread tactilThread = new Thread(getTactilData);
            setUpChannel();
            try
            {
                imageThread.Start();
                //jointThread.Start();
                //batteryThread.Start();
                //sonarThread.Start();
                //tactilThread.Start();
            }
            catch (ThreadStartException e)
            {
                Console.WriteLine("error while retriving data from NAO");
                Console.WriteLine("message : " + e.ToString());
                Console.WriteLine("retrying...");
                getNaoData();
            }
        }
        //code related to Json and RabbitMQ
        //this method is to set the channel for RabbitMQ
        public void setUpChannel()
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.Uri = "amqp://guest:guest@localhost/%2F";
                IConnection conn = factory.CreateConnection();
                channel = conn.CreateModel();
                conn.AutoClose = true;
                QueueDeclareOk jointStream = channel.QueueDeclare("", false, true, true, null);
                
                //channel.QueueBind(jointStream.QueueName, "amq.topic", jointStreamKey);
                //sub = new Subscription(channel, jointStream.QueueName);
            }
            catch (Exception e)
            {
                Console.WriteLine("error in setupRabbitMQ : " + e.ToString());
            }
            
            
        }
        //end
        public void getImageData()
        {
            VideoDeviceProxy video = new VideoDeviceProxy(Program.naoIP, Program.naoPort);
            //default is to get image with this spesification
            //resolution : 1    ;320*240
            //colorSpace : 11   ;RGB
            //frameRate  : 15   ;15 frame per second
            defaultImageId = 1;
            string id = image[defaultImageId].id;
            int res = image[defaultImageId].resolution;
            int colorSpace = 11;
            int fps = image[defaultImageId].fps;
            
            ArrayList rawImage = new ArrayList();
            byte[] ImageData;
            Stopwatch s = new Stopwatch();
            //we must make sure that subcription is done,
            //this code is not safe yet.

            bool flag = false;
            try
            {
                video.subscribe(id, res, colorSpace, fps);
            }
            catch
            {
                video.unsubscribe(id);
                video.subscribe(id, res, colorSpace, fps);
            }
            tryRetriving:
            try
            {
                while (true)
                {
                    //s.Reset();
                    //s.Start();
                    rawImage = (ArrayList)video.getImageRemote(id);
                    ImageData = (byte[])rawImage[6];
                    lock (this.image[defaultImageId])
                    {
                        this.image[defaultImageId].data = ImageData;
                    }
                    //Console.WriteLine("start encoding image");
                    BitmapSource image1 = BitmapSource.Create(
                                                320,
                                                240,
                                                96,
                                                96,
                                                PixelFormats.Rgb24,
                                                BitmapPalettes.WebPalette,
                                                ImageData,
                                                320 * 3);
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image1));
                    MemoryStream ms = new MemoryStream();
                    encoder.Save(ms);
                    Bitmap image2 = new Bitmap(ms);
                    string url = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
                    ImageObject imageObject = new ImageObject(url.Length, url);
                    string body = JsonConvert.SerializeObject(imageObject);
                    byte[] buffer = Encoding.UTF8.GetBytes(body);
                    channel.BasicPublish("amq.topic", "lumen.arkan.camera.stream", null, buffer);
                    //Console.WriteLine("finish encoding image");
                    video.releaseImage(id);
                    if (!flag)
                    {
                        Console.WriteLine("retriving camera data...");
                        flag = true;
                    }
                    //s.Stop();
                    //Console.WriteLine("image : " + s.ElapsedMilliseconds.ToString());
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("retriving image data error");
                Console.WriteLine("message : " + e.ToString());
                goto tryRetriving;
            }
        }
        public void getJointData()
        {
            //Console.WriteLine("entering getJointData");
            List<float> angles = new List<float>();
            List<float> stiffnesses = new List<float>();
            MotionProxy motion = new MotionProxy(Program.naoIP, Program.naoPort);
            Stopwatch s = new Stopwatch();
            bool flag = false;

            //code related to Json and RabbitMQ
            string _name;
            float _angle, _stiffness;
            JointData _data;
            //end
            tryRetriving:
            
            try
            {
                while (true)
                {
                    //s.Reset();
                    //s.Start();
                    angles = motion.getAngles("Body", false);
                    stiffnesses = motion.getStiffnesses("Body");
                    
                    for (int i = 0; i < angles.Count; i++)
                    {
                        lock (this.joint)
                        {
                            joint[i].angle = angles[i];
                            joint[i].stiffness = stiffnesses[i];

                            //code related to Json and RabbitMQ
                            _name = joint[i].name;//save to temporary object
                            _angle = joint[i].angle;
                            _stiffness = joint[i].stiffness;
                            
                            //end
                        }
                        _data = new JointData(_name, _angle, _stiffness);
                        //Console.WriteLine("start serializing");
                        string jointData = JsonConvert.SerializeObject(_data,Formatting.Indented);
                        //Console.WriteLine("finish Serializing");
                        byte[] jointDataByte = Encoding.UTF8.GetBytes(jointData);
                        channel.BasicPublish("amq.topic", jointDataKey, null, jointDataByte);
                    } 
                    //s.Stop();
                    if (!flag)
                    {
                        Console.WriteLine("retriving joint data...");
                        flag = true;
                    }
                    //Console.WriteLine("joint : " + s.ElapsedMilliseconds.ToString());
                    delay(1000);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("retriving joint data error");
                Console.WriteLine(e.ToString());
                delay(1000);
                goto tryRetriving;
            }
        }
        public void getBatteryData()
        {
            MemoryProxy memory = new MemoryProxy(Program.naoIP, Program.naoPort);
            int _percentage;
            bool _isPlugged;
            bool _isCharging;
            bool flag = false;
            Stopwatch s = new Stopwatch();
            tryRetriving:
            try
            {
                while (true)
                {
                    //s.Reset();
                    //s.Start();
                    _percentage = (int)memory.getData("BatteryChargeChanged");
                    _isCharging = (bool)memory.getData("BatteryChargingFlagChanged");
                    _isPlugged = (bool)memory.getData("BatteryPowerPluggedChanged");
                    lock (this.battery)
                    {
                        this.battery.percentage = _percentage;
                        this.battery.isCharging = _isCharging;
                        this.battery.isPlugged = _isPlugged;
                    }

                    var data = new BatteryData(_percentage, _isPlugged, _isCharging);
                    
                    string stringData = JsonConvert.SerializeObject(data, Formatting.Indented);
                    byte[] buffer = Encoding.UTF8.GetBytes(stringData);
                    channel.BasicPublish("amq.topic", batteryDataKey, null, buffer);
                    
                    //s.Stop();
                    //Console.WriteLine("battery : " + s.ElapsedMilliseconds.ToString());
                    if (!flag)
                    {
                        Console.WriteLine("retriving battery data...");
                        flag = true;
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("retriving battery data error");
                Console.WriteLine("message : " + e.ToString());
                goto tryRetriving;
            }
        }
        public void getSonarData()
        {
            MemoryProxy memory = new MemoryProxy(Program.naoIP, Program.naoPort);
            SonarProxy sonar = new SonarProxy(Program.naoIP, Program.naoPort);
            Stopwatch s = new Stopwatch();
            float left, right;
            bool flag = false;
            tryRetriving:
            try
            {
                sonar.subscribe("NaoData");
                
                while (true)
                {
                    //s.Reset();
                    //s.Start();
                    left = (float)memory.getData("Device/SubDeviceList/US/Left/Sensor/Value");
                    right = (float)memory.getData("Device/SubDeviceList/US/Right/Sensor/Value");
                    lock (this.sonar)
                    {
                        this.sonar.leftSensor = left;
                        this.sonar.rightSensor = right;
                    }

                    var data = new SonarData(right, left);
                    string body = JsonConvert.SerializeObject(data);
                    byte[] buffer = Encoding.UTF8.GetBytes(body);
                    channel.BasicPublish("amq.topic", sonarDataKey, null, buffer);
                    //s.Stop();
                    //Console.WriteLine("sonar : " + s.ElapsedMilliseconds.ToString());
                    if (!flag)
                    {
                        Console.WriteLine("retriving sonar data...");
                        flag = true;
                    }
                }
            }
            catch (Exception e)
            {
                sonar.unsubscribe("NaoData");
                sonar.subscribe("NaoData");
                Console.WriteLine("retriving sonar data error");
                Console.WriteLine("message : " + e.ToString());
                goto tryRetriving;
            }
        }
        public void getTactilData()
        {
            MemoryProxy memory = new MemoryProxy(Program.naoIP, Program.naoPort);
            SensorsProxy sensor = new SensorsProxy(Program.naoIP,Program.naoPort);
            Stopwatch s = new Stopwatch();
            float RightBumper;
            float LeftBumper;
            float ChestButton;
            float FrontTactil;
            float MiddleTactil;
            float RearTactil;
            float HandRightBack;
            float HandRightLeft;
            float HandRightRight;
            float HandLeftBack;
            float HandLeftLeft;
            float HandLeftRight;
            bool flag = false;
            tryRetriving:
            try
            {
                sensor.subscribe("NaoData");
                sensor.run();
                while (true)
                {
                    //s.Reset();
                    //s.Start();
                    RightBumper = (float)memory.getData("RightBumperPressed");
                    LeftBumper = (float)memory.getData("LeftBumperPressed");
                    ChestButton = (float)memory.getData("ChestButtonPressed");
                    FrontTactil = (float)memory.getData("FrontTactilTouched");
                    MiddleTactil = (float)memory.getData("MiddleTactilTouched");
                    RearTactil = (float)memory.getData("RearTactilTouched");
                    HandRightBack = (float)memory.getData("HandRightBackTouched");
                    HandRightLeft = (float)memory.getData("HandRightLeftTouched");
                    HandRightRight = (float)memory.getData("HandRightRightTouched");
                    HandLeftBack = (float)memory.getData("HandLeftBackTouched");
                    HandLeftLeft = (float)memory.getData("HandLeftLeftTouched");
                    HandLeftRight = (float)memory.getData("HandLeftRightTouched");
                    lock (this.tactile)
                    {
                        this.tactile[0].value = RightBumper;
                        this.tactile[1].value = LeftBumper;
                        this.tactile[2].value = ChestButton;
                        this.tactile[3].value = FrontTactil;
                        this.tactile[4].value = MiddleTactil;
                        this.tactile[5].value = RearTactil;
                        this.tactile[6].value = HandRightBack;
                        this.tactile[7].value = HandRightLeft;
                        this.tactile[8].value = HandRightRight;
                        this.tactile[9].value = HandLeftBack;
                        this.tactile[10].value = HandLeftLeft;
                        this.tactile[11].value = HandLeftRight;
                    }
                    //s.Stop();
                    //Console.WriteLine("sonar : " + s.ElapsedMilliseconds.ToString());
                    if (!flag)
                    {
                        Console.WriteLine("retriving tactile, bumper, and button data...");
                        flag = true;
                    }
                }
            }
            catch (Exception e)
            {
                
                Console.WriteLine("retriving sonar data error");
                Console.WriteLine("message : " + e.ToString());
                goto tryRetriving;
            }
        }

        private void delay(int ms)
        {
            delayTime.Reset();
            delayTime.Start();
            while ((int)delayTime.ElapsedMilliseconds < ms)
            {
            }
            delayTime.Stop();
        }
    }

}
