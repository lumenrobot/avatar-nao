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
namespace LumenServer
{

    class DataAquisition
    {
        
        
        public NaoBattery battery = new NaoBattery();
        public NaoSonar sonar= new NaoSonar() ;
        public NaoTactile tactile = new NaoTactile();
        private bool imageReady, jointReady, batteryReady, sonarReady, tactileReady;
        public bool connection;
        private Thread jointThread, imageThread, batteryThread, sonarThread, tactileThread;
        public Thread connectionCheck;
        public DataAquisition()
        {
            connectionCheck = new Thread(checkConnection);
        }
        public void startAquisitioning()
        {
            connection = false;
            jointThread = new Thread(getJointData);
            imageThread = new Thread(getImageData);
            batteryThread = new Thread(getBatteryData);
            sonarThread = new Thread(getSonarData);
            tactileThread = new Thread(getTactileData);
            

            //jointThread.Start();
            imageThread.Start();
            //batteryThread.Start();
            //sonarThread.Start();
            //tactileThread.Start();
            imageReady = true;
            jointReady = true;
            batteryReady = true;
            tactileReady = true;
            sonarReady = true;
            //if (connectionCheck.IsAlive == false)
            //{
            //    connectionCheck.Start();
            //}
        }
        private void checkConnection()
        {
            while (true)
            {
                if (imageReady == false || jointReady == false || tactileReady == false || sonarReady == false || batteryReady == false)
                {
                    Console.WriteLine("aborting data Aquistion...");
                    //imageThread.Abort();
                    //batteryThread.Abort();
                    //sonarThread.Abort();
                    //tactileThread.Abort();
                    //jointThread.Abort();
                    //connection = false;
                    //connectionCheck.Abort();
                }
                else
                {
                    connection = true;
                }
            }
        }
        private void getJointData()
        {
            MotionProxy motion = new MotionProxy(Program.naoIP, Program.naoPort);
            List<string> jointNames = new List<string>() { "HeadYaw", "HeadPitch", "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "LHand", "LHipYawPitch", "LHipRoll", "LHipPitch ", "LKneePitch", "LAngklePitch", "LAngkleRoll", "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAngklePitch", "RAngkleRoll", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "RHand" };
            List<float> jointAngles = new List<float>();
            List<float> jointStiffnesses = new List<float>();
            lock (Program.joint)
            {
                Program.joint.names = jointNames;
            }
            bool flag = false;
            tryAgain:
            try
            {
                while (true)
                {
                    jointAngles = motion.getAngles("Body", false);
                    jointStiffnesses = motion.getStiffnesses("Body");
                    lock (Program.joint)
                    {
                        Program.joint.angles = jointAngles;
                        Program.joint.stiffnesses = jointStiffnesses;
                    }
                    if (!flag)
                    {
                        Console.WriteLine("retriving joint data...");
                        flag = true;
                    }

                    jointReady = true;
                }
            }
            catch(Exception e)
            {
                //Console.WriteLine("retriving joint data error");
                //Console.WriteLine(e.ToString());
                jointReady = false;
                goto tryAgain;
            }
        }
        private void getImageData()
        {
            VideoDeviceProxy video = new VideoDeviceProxy(Program.naoIP, Program.naoPort);
            //default is to get image with this spesification
            //resolution : 1    ;320*240
            //colorSpace : 11   ;RGB
            //frameRate  : 15   ;15 frame per second
            string id = "res1"; //resolution = 320*240
            int res = 1;
            int colorSpace = 11;
            int fps = 30;

            ArrayList rawImage = new ArrayList();
            byte[] ImageData;
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
            tryAgain:
            try
            {
                while (true)
                {
                    rawImage = (ArrayList)video.getImageRemote(id);
                    ImageData = (byte[])rawImage[6];
                    lock (Program.image)
                    {
                        Program.image.data = ImageData;
                    }
                    //Console.WriteLine("start encoding image");
                    //BitmapSource image1 = BitmapSource.Create(
                    //                            320,
                    //                            240,
                    //                            96,
                    //                            96,
                    //                            PixelFormats.Rgb24,
                    //                            BitmapPalettes.WebPalette,
                    //                            ImageData,
                    //                            320 * 3);
                    //JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    //encoder.Frames.Add(BitmapFrame.Create(image1));
                    //MemoryStream ms = new MemoryStream();
                    //encoder.Save(ms);
                    //Bitmap image2 = new Bitmap(ms);
                    //string url = "data:image/jpeg;base64," + Convert.ToBase64String(ms.ToArray());
                    //ImageObject imageObject = new ImageObject(url.Length, url);
                    //string body = JsonConvert.SerializeObject(imageObject);
                    //byte[] buffer = Encoding.UTF8.GetBytes(body);
                    //channel.BasicPublish("amq.topic", "lumen.arkan.camera.stream", null, buffer);
                    ////Console.WriteLine("finish encoding image");
                    video.releaseImage(id);
                    if (!flag)
                    {
                        Console.WriteLine("retriving camera data...");
                        flag = true;
                    }
                    imageReady = true;
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine("retriving camera data error");
                //Console.WriteLine("message : " + e.ToString());
                imageReady = false;
                goto tryAgain;
            }
        }
        private void getBatteryData()
        {
            MemoryProxy memory = new MemoryProxy(Program.naoIP, Program.naoPort);
            int per;
            bool isPlugged;
            bool isCharging;
            bool flag = false;
            tryAgain:
            try
            {
                while (true)
                {
                    per = (int)memory.getData("BatteryChargeChanged");
                    isCharging = (bool)memory.getData("BatteryChargingFlagChanged");
                    isPlugged = (bool)memory.getData("BatteryPowerPluggedChanged");
                    lock (this.battery)
                    {
                        this.battery.percentage = per;
                        this.battery.isCharging = isCharging;
                        this.battery.isPlugged = isPlugged;
                    }
                    if (!flag)
                    {
                        Console.WriteLine("retriving battery data...");
                        flag = true;
                    }
                    batteryReady = true;
                }
            }
            catch(Exception e)
            {
                //Console.WriteLine("retriving battery data error");
                //Console.WriteLine("message : " + e.ToString());
                batteryReady = false;
                goto tryAgain;
            }
        }
        private void getSonarData()
        {
            MemoryProxy memory = new MemoryProxy(Program.naoIP, Program.naoPort);
            SonarProxy sonar = new SonarProxy(Program.naoIP, Program.naoPort);
            float left, right;
            bool flag = false;
            tryAgain:
            try
            {
                sonar.subscribe("NaoData");

                while (true)
                {
                    left = (float)memory.getData("Device/SubDeviceList/US/Left/Sensor/Value");
                    right = (float)memory.getData("Device/SubDeviceList/US/Right/Sensor/Value");
                    lock (this.sonar)
                    {
                        this.sonar.leftSensor = left;
                        this.sonar.rightSensor = right;
                    }
                    if (!flag)
                    {
                        Console.WriteLine("retriving sonar data...");
                        flag = true;
                    }
                    sonarReady = true;
                }
            }
            catch (Exception e)
            {
                //sonar.unsubscribe("NaoData");
                //sonar.subscribe("NaoData");
                //Console.WriteLine("retriving sonar data error");
                //Console.WriteLine("message : " + e.ToString());
                sonarReady = false;
                goto tryAgain;
            }
        }
        private void getTactileData()
        {
            MemoryProxy memory = new MemoryProxy(Program.naoIP, Program.naoPort);
            SensorsProxy sensor = new SensorsProxy(Program.naoIP, Program.naoPort);
            List<string> name = new List<string>() { "RightBumper", "LeftBumper", "ChestButton", "FrontTactil", "MiddleTactil", "RearTactil","HandRightBack","HandRightLeft","HandRightRight","HandLeftBack" ,"HandLeftLeft","HandLeftRight"};
            List<float> value;
            lock (this.tactile)
            {
                this.tactile.names = name;
            }
            bool flag = false;
            tryAgain:
            try
            {
                sensor.subscribe("NaoData");
                sensor.run();
                while (true)
                {
                    value = new List<float>();
                    value.Add((float)memory.getData("RightBumperPressed"));
                    value.Add((float)memory.getData("LeftBumperPressed"));
                    value.Add((float)memory.getData("ChestButtonPressed"));
                    value.Add((float)memory.getData("FrontTactilTouched"));
                    value.Add((float)memory.getData("MiddleTactilTouched"));
                    value.Add((float)memory.getData("RearTactilTouched"));
                    value.Add((float)memory.getData("HandRightBackTouched"));
                    value.Add((float)memory.getData("HandRightLeftTouched"));
                    value.Add((float)memory.getData("HandRightRightTouched"));
                    value.Add((float)memory.getData("HandLeftBackTouched"));
                    value.Add((float)memory.getData("HandLeftLeftTouched"));
                    value.Add((float)memory.getData("HandLeftRightTouched"));
                    lock (this.tactile)
                    {
                        this.tactile.values = value;
                    }
                    if (!flag)
                    {
                        Console.WriteLine("retriving tactile, bumper, and button data...");
                        flag = true;
                    }
                    tactileReady = true;
                }
            }
            catch (Exception e)
            {

                //Console.WriteLine("retriving tactile data error");
                //Console.WriteLine("message : " + e.ToString());
                tactileReady = false;
                goto tryAgain;
            }
        }
    }
}
