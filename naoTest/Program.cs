using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using Aldebaran.Proxies;
using NAudio;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna;


namespace naoTest
{
    public struct point
    {
        public int x;
        public int y;
    }
    
    class Program
    {
        static Stopwatch s = new Stopwatch();
        static string ip = "167.205.56.180";
        //static string ip = "127.0.0.1";
        static int port = 9559;

        public static List<point> a = new List<point>();
        
        static void Main(string[] args)
        {
            tesmotion();
            Console.Read();
        }
        static void testSpeaker()
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(@"D:\M1F1-Alaw-AFsp.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("channel : " + file.WaveFormat.BitsPerSample);
        }
        static void tesmotion()
        {
            Console.WriteLine("mulai");
            MotionProxy tesmotion = new MotionProxy("167.205.56.142", 9559);
            List<float> angle = tesmotion.getAngles("Body", false);
            ArrayList angles = new ArrayList();
            angles.Add(0.5f);
            angles.Add(0.5f);
            ArrayList nama = new ArrayList();
            nama.Add("LShoulderPitch");
            nama.Add("RShoulderPitch");
            tesmotion.wakeUp();
            tesmotion.setAngles(nama, angles, 0.4f);
            for (int i = 0; i < angle.Count; i++)
            {
                Console.WriteLine("angle : " + angle[i]);
            }
            tesmotion.rest();
            Console.WriteLine("selesai");
            Console.ReadKey();
        }
        static void testAudio()
        {
            var stream = new MemoryStream(File.ReadAllBytes(@"D:\hohoho.wav"));
            Console.WriteLine("1");
            WaveFileReader a = new WaveFileReader(stream);
            int channel = a.WaveFormat.Channels;
            int rate = a.WaveFormat.SampleRate;
            int bss = a.WaveFormat.BitsPerSample;
            int max = 16384;
            int outputChannel = 2;
            AudioDeviceProxy audio = new AudioDeviceProxy(ip, port);
            audio.setParameter("outputSampleRate", rate);
            byte[] inputData = new byte[max * channel];
            byte[] stereo = new byte[max * outputChannel];
            Console.WriteLine("channel : "+channel);
            Console.WriteLine("size : " + bss / (8 * channel));
            Console.WriteLine("count : " + max);
            int frame = a.Read(inputData, 0, max);
            int i = 0;
            for (int j = 0; j < frame; j++)
            {
                stereo[i] = inputData[j];
                stereo[i + 1] = inputData[j];
                i = i + outputChannel;
            }
            ArrayList data = new ArrayList(stereo);
            Console.WriteLine("frame : " + data.Count);
            try
            {

                audio.sendRemoteBufferToOutput(frame, data);
            }
            catch(Exception e)
            {
                Console.WriteLine("error : "+e.ToString());
            }
            Console.WriteLine("5");
            Console.ReadKey();

            


        }
        static void testMemory()
        {
            MemoryProxy m = new MemoryProxy(ip, port);
            Console.WriteLine(m.getData("BatteryChargeChanged"));
            Console.WriteLine(m.getData("SonarLeftDetected"));
            BatteryProxy b = new BatteryProxy(ip, port);
            int persen = b.getBatteryCharge();
            Console.WriteLine(persen);
            Console.WriteLine(b.getMethodHelp("getBatteryCharge"));
            Console.ReadKey();
        }
        static void testSonar()
        {
            SonarProxy s = new SonarProxy(ip, port);
            MemoryProxy m = new MemoryProxy(ip, port);
            s.subscribe("test");
            while (true)
            {
                Console.WriteLine(m.getData("Device/SubDeviceList/US/Right/Sensor/Value"));
                delay(100);
            }
        }
        static void testTactile()
        {
            SensorsProxy sensor = new SensorsProxy(ip, port);
            MemoryProxy m = new MemoryProxy(ip, port);
            sensor.subscribe("test");
            sensor.run();
            List<string> output = sensor.getOutputNames();
            foreach (string a in output)
            {
                Console.WriteLine(a);
            }
            
            object  b = m.getData("FrontTactilTouched");
            Console.WriteLine(b.GetType());
            Console.ReadKey();

        }
        static void testSearching()
        {
            string[] number = { "1", "2", "3", "4", "5", "6" };
            string coba = "7";
            int i = 0;
            bool flag = false;
            while ((!flag) && (i < number.Length))
            {
                Console.WriteLine("putar");
                if (coba == number[i])
                {
                    flag = true;
                    
                }
                else
                {
                    i++;
                }
            }

            if (flag)
            {
                Console.WriteLine("ada");
            }
            else
            {
                Console.WriteLine("nggak ada");
            }
            Console.ReadKey();
        }
        static void testCamera2()
        {
            VideoDeviceProxy video = new VideoDeviceProxy(ip, port);
            string id = "rec1";
            int res1 = 1;
            int colorspace = 11;
            int fps = 15;
            ArrayList data = new ArrayList();
            try
            {
                Console.WriteLine("subscribe");
                video.subscribe(id, res1, colorspace, fps);
            }
            catch
            {
                video.unsubscribe(id);
                Console.WriteLine("unable to subscribe");
                video.subscribe(id, res1, colorspace, fps);
            }

            try
            {
                while (true)
                {
                    s.Reset();
                    s.Start();
                    data = (ArrayList)video.getImageRemote(id);
                    s.Stop();
                    video.releaseImage(id);
                    Console.WriteLine(s.ElapsedMilliseconds);
                }
            }
            catch
            {
                video.unsubscribe(id);
                Console.WriteLine("error retrive data");
            }
        }
        static void testMotion()
        {
            List<float> angle;
            try
            {
                MotionProxy motion = new MotionProxy(ip, port);
                angle = motion.getAngles("LLeg", false);
                for (int i = 0; i < angle.Count; i++)
                {
                    Console.WriteLine(angle[i].ToString());
                }
                Console.ReadKey();
            }
            catch
            {
                Console.WriteLine("error");
                Console.ReadKey();
            }
        }
        static void testCamera()
        {
            VideoDeviceProxy video = new VideoDeviceProxy(ip, port);
            string id = "rec1";
            int res1 = 0;
            int colorspace = 3;
            int fps = 15;
            ArrayList data = new ArrayList();
            try
            {
                Console.WriteLine("subscribe");
                video.subscribe(id, res1, colorspace, fps);
            }
            catch
            {
                video.unsubscribe(id);
                Console.WriteLine("unable to subscribe");
                video.subscribe(id, res1, colorspace, fps);
            }

            try
            {
                while (true)
                {
                    s.Reset();
                    s.Start();
                    data = (ArrayList)video.getImageRemote(id);
                    s.Stop();
                    video.releaseImage(id);
                    Console.WriteLine(s.ElapsedMilliseconds);
                }
            }
            catch
            {
                video.unsubscribe(id);
                Console.WriteLine("error retrive data");
            }
        }
        static void testRecord()
        {
            AudioRecorderProxy record = new AudioRecorderProxy(ip, port);
            ArrayList channels = new ArrayList();
            channels.Add(0);
            channels.Add(1);
            channels.Add(0);
            channels.Add(0);
            try
            {
                Console.WriteLine("start recording");
                s.Start();
                record.startMicrophonesRecording("/home/nao/test.wav", "wav", 16000, channels);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.ReadKey();
            record.stopMicrophonesRecording();
            s.Stop();
            Console.WriteLine("stop recording "+s.ElapsedMilliseconds.ToString());
        }
        static void playRecord()
        {
            try
            {
                AudioPlayerProxy play = new AudioPlayerProxy(ip, port);
                int id = play.loadFile("/home/nao/test.wav");
                Console.WriteLine("load file "+id.ToString());
                Console.ReadKey();
                Console.WriteLine("start playing");
                s.Start();
                play.setVolume(id, 1.0f);
                play.play(id);
                s.Stop();
                Console.WriteLine("finish playing "+s.ElapsedMilliseconds.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
        }
        static void testEnergy()
        {
            AudioDeviceProxy device = new AudioDeviceProxy(ip, port);
            device.enableEnergyComputation();
            float a, b, c, d;
            while (true)
            {
                a = device.getFrontMicEnergy();
                b = device.getLeftMicEnergy();
                c = device.getRightMicEnergy();
                d = device.getRearMicEnergy();
                Console.WriteLine("{0}   {1}   {2}   {3}", a, b, c, d);
            }
        }
        static void delay(int time)
        {
            Stopwatch s = new Stopwatch();
            s.Reset();
            s.Start();
            while (s.ElapsedMilliseconds < time)
            {
            }
            s.Stop();
        }
    }
}
