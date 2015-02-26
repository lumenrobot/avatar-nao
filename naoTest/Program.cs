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
        static string ip = "169.254.108.110";
        //static string ip = "127.0.0.1";
        static int port = 9559;

        public static List<point> a = new List<point>();
        
        static void Main(string[] args)
        {
            testAudio4();
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
        static void testAudio4()
        {
            Stream ms = new MemoryStream(File.ReadAllBytes(@"D:\wav\hasil.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            int nbOfChannels = file.WaveFormat.Channels;
            int sampleRate = file.WaveFormat.SampleRate;
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("bitpersample : " + file.WaveFormat.BitsPerSample);
            Console.WriteLine("sample rate : " + file.WaveFormat.SampleRate);
            Console.WriteLine("lenght : " + file.SampleCount);
            int outputBufferSize = 16384;
            int numberOfOutputChannels = 2;


            byte[] buffer = new byte[2 * (int)file.SampleCount];
            int byteRead = file.Read(buffer, 0, 2 * (int)file.SampleCount);
            int nbOfFrames = byteRead / 2;
            Console.WriteLine("byte Read : " + byteRead);
            Console.WriteLine("frame Read : " + nbOfFrames);

            short[] fInputAudioData = new short[(int)file.SampleCount];
            short[] fStereoAudioData = new short[2* (int)file.SampleCount];
            int sample = 0;
            for (int index = 0; index < nbOfFrames; index++)
            {
                fInputAudioData[index] = BitConverter.ToInt16(buffer, sample);
                sample += 2;
            }
            if (nbOfChannels == 1)
            {
                int i = 0;
                for (int j = 0; j < nbOfFrames; j++)
                {
                    fStereoAudioData[i] = fInputAudioData[j];
                    fStereoAudioData[i + 1] = fInputAudioData[j];
                    i += numberOfOutputChannels;
                }
            }
            else if (nbOfChannels == 2)
            {
                for (int i = 0; i < nbOfFrames; i++)
                {
                    fStereoAudioData[i] = fInputAudioData[i];
                }
            }

            ArrayList output = new ArrayList(fStereoAudioData);

            try
            {
                AudioDeviceProxy audio = new AudioDeviceProxy(ip, port);
                audio.setParameter("outputSampleRate", sampleRate);
                audio.sendRemoteBufferToOutput(nbOfFrames, fStereoAudioData);
                Console.WriteLine("finish");
            }
            catch (Exception e)
            {
                Console.WriteLine("error : " + e.Message);
            }


            Console.ReadKey();
        }
        static void testAudio3()
        {
            Stream ms = new MemoryStream(File.ReadAllBytes(@"D:\wav\aaaa.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            int nbOfChannels = file.WaveFormat.Channels;
            int sampleRate = file.WaveFormat.SampleRate;
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("bitpersample : " + file.WaveFormat.BitsPerSample);
            Console.WriteLine("sample rate : " + file.WaveFormat.SampleRate);
            Console.WriteLine("lenght : " + file.SampleCount);
            
            int outputBufferSize = 16384;
            int numberOfOutputChannels = 2;
            byte[] firstBuffer = new byte[2*file.SampleCount];
            int byteRead = file.Read(firstBuffer, 0, 2 * (int)file.SampleCount);
            Console.WriteLine("byteRead : " + byteRead);
            int bufferMaxSize = 2*nbOfChannels*outputBufferSize;
            byte[] buffer1 = new byte[bufferMaxSize];
            byte[] buffer2 = new byte[bufferMaxSize];
            for (int i = 0; i < bufferMaxSize; i++)
            {
                buffer1[i] = firstBuffer[i];
            }

            short[] fInputAudioData1 = new short[nbOfChannels * outputBufferSize];
            short[] fStereoAudioData1 = new short[outputBufferSize * numberOfOutputChannels];
            short[] fInputAudioData2 = new short[nbOfChannels * outputBufferSize];
            short[] fStereoAudioData2 = new short[outputBufferSize * numberOfOutputChannels];
            int sample = 0;
            for (int index = 0; index < buffer1.Length/2; index++)
            {
                fInputAudioData1[index] = BitConverter.ToInt16(buffer1, sample);
                sample += 2;
            }
            sample = 0;
            for (int index = 0; index < buffer2.Length / 2; index++)
            {
                fInputAudioData2[index] = BitConverter.ToInt16(buffer2, sample);
                sample += 2;
            }
            if (nbOfChannels == 1)
            {
                int i = 0;
                for (int j = 0; j < buffer1.Length/2; j++)
                {
                    fStereoAudioData1[i] = fInputAudioData1[j];
                    fStereoAudioData1[i + 1] = fInputAudioData1[j];
                    i += numberOfOutputChannels;
                }
                i = 0;
                for (int j = 0; j < buffer2.Length / 2; j++)
                {
                    fStereoAudioData2[i] = fInputAudioData2[j];
                    fStereoAudioData2[i + 1] = fInputAudioData2[j];
                    i += numberOfOutputChannels;
                }
            }
            else if (nbOfChannels == 2)
            {
                for (int i = 0; i < buffer1.Length / 2; i++)
                {
                    fStereoAudioData1[i] = fInputAudioData1[i];
                }
                for (int i = 0; i < buffer2.Length / 2; i++)
                {
                    fStereoAudioData2[i] = fInputAudioData2[i];
                }
            }
            try
            {
                AudioDeviceProxy audio = new AudioDeviceProxy(ip, port);
                audio.setParameter("outputSampleRate", sampleRate);
                audio.sendRemoteBufferToOutput(buffer2.Length/2, fStereoAudioData2);
                Console.WriteLine("finish 1");
                audio.sendRemoteBufferToOutput(buffer1.Length / 2, fStereoAudioData1);
                Console.WriteLine("finish 2");
            }
            catch (Exception e)
            {
                Console.WriteLine("error : " + e.Message);
            }


            Console.ReadKey();
        }
        static void testAudio2()
        {
            Stream ms = new MemoryStream(File.ReadAllBytes(@"D:\wav\aaaa.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            int nbOfChannels = file.WaveFormat.Channels;
            int sampleRate = file.WaveFormat.SampleRate;
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("bitpersample : " + file.WaveFormat.BitsPerSample);
            Console.WriteLine("sample rate : " + file.WaveFormat.SampleRate);
            Console.WriteLine("lenght : " + file.SampleCount);
            int outputBufferSize = 16384;
            int numberOfOutputChannels = 2;
            

            byte[] buffer = new byte[2*nbOfChannels*outputBufferSize];
            int byteRead = file.Read(buffer, 0, 2 * nbOfChannels * outputBufferSize);
            int nbOfFrames = byteRead / 2;
            Console.WriteLine("byte Read : " + byteRead);
            Console.WriteLine("frame Read : " + nbOfFrames);
            
            short[] fInputAudioData = new short[nbOfChannels * outputBufferSize];
            short[] fStereoAudioData = new short[outputBufferSize * numberOfOutputChannels];
            int sample = 0;
            for (int index = 0; index < nbOfFrames; index++)
            {
                fInputAudioData[index] = BitConverter.ToInt16(buffer, sample);
                sample += 2;
            }
            if (nbOfChannels == 1)
            {
                int i = 0;
                for (int j = 0; j < nbOfFrames; j++)
                {
                    fStereoAudioData[i] = fInputAudioData[j];
                    fStereoAudioData[i + 1] = fInputAudioData[j];
                    i += numberOfOutputChannels;
                }
            }
            else if (nbOfChannels == 2)
            {
                for (int i = 0; i < byteRead; i++)
                {
                    fStereoAudioData[i] = fInputAudioData[i];
                }
            }

            ArrayList output = new ArrayList(fStereoAudioData);

            try
            {
                AudioDeviceProxy audio = new AudioDeviceProxy(ip, port);
                audio.setParameter("outputSampleRate", sampleRate);
                audio.sendRemoteBufferToOutput(nbOfFrames, fStereoAudioData);
                Console.WriteLine("finish");
            }
            catch(Exception e)
            {
                Console.WriteLine("error : "+e.Message);
            }


            Console.ReadKey();

        }
        //static void testAudio()
        //{
        //    var stream = new MemoryStream(File.ReadAllBytes(@"D:\hello.wav"));
        //    Console.WriteLine("1");
        //    WaveFileReader a = new WaveFileReader(stream);
        //    int channel = a.WaveFormat.Channels;
        //    int rate = a.WaveFormat.SampleRate;
        //    int bss = a.WaveFormat.BitsPerSample;
        //    int max = 16384;
        //    int outputChannel = 2;
        //    AudioDeviceProxy audio = new AudioDeviceProxy("167.205.56.142", 9559);
        //    //audio.setParameter("outputSampleRate", rate);
        //    byte[] inputData = new byte[max * channel];
        //    byte[] stereo = new byte[max * outputChannel];
        //    short[] output = new short[max * outputChannel];
        //    Console.WriteLine("channel : " + channel);
        //    Console.WriteLine("size : " + rate);
        //    Console.WriteLine("count : " + bss);
        //    a.Read(inputData, 0, max);
        //    output = Array.ConvertAll(inputData, (b) => (short)b);

        //    try
        //    {
        //        audio.sendRemoteBufferToOutput(max * outputChannel, output);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("error : " + e.ToString());
        //    }
        //    Console.WriteLine("5");
        //    Console.ReadKey();
        //}
        //static void testMemory()
        //{
        //    MemoryProxy m = new MemoryProxy(ip, port);
        //    Console.WriteLine(m.getData("BatteryChargeChanged"));
        //    Console.WriteLine(m.getData("SonarLeftDetected"));
        //    BatteryProxy b = new BatteryProxy(ip, port);
        //    int persen = b.getBatteryCharge();
        //    Console.WriteLine(persen);
        //    Console.WriteLine(b.getMethodHelp("getBatteryCharge"));
        //    Console.ReadKey();
        //}
        //static void testSonar()
        //{
        //    SonarProxy s = new SonarProxy(ip, port);
        //    MemoryProxy m = new MemoryProxy(ip, port);
        //    s.subscribe("test");
        //    while (true)
        //    {
        //        Console.WriteLine(m.getData("Device/SubDeviceList/US/Right/Sensor/Value"));
        //        delay(100);
        //    }
        //}
        //static void testTactile()
        //{
        //    SensorsProxy sensor = new SensorsProxy(ip, port);
        //    MemoryProxy m = new MemoryProxy(ip, port);
        //    sensor.subscribe("test");
        //    sensor.run();
        //    List<string> output = sensor.getOutputNames();
        //    foreach (string a in output)
        //    {
        //        Console.WriteLine(a);
        //    }
            
        //    object  b = m.getData("FrontTactilTouched");
        //    Console.WriteLine(b.GetType());
        //    Console.ReadKey();

        //}
        //static void testSearching()
        //{
        //    string[] number = { "1", "2", "3", "4", "5", "6" };
        //    string coba = "7";
        //    int i = 0;
        //    bool flag = false;
        //    while ((!flag) && (i < number.Length))
        //    {
        //        Console.WriteLine("putar");
        //        if (coba == number[i])
        //        {
        //            flag = true;
                    
        //        }
        //        else
        //        {
        //            i++;
        //        }
        //    }

        //    if (flag)
        //    {
        //        Console.WriteLine("ada");
        //    }
        //    else
        //    {
        //        Console.WriteLine("nggak ada");
        //    }
        //    Console.ReadKey();
        //}
        //static void testCamera2()
        //{
        //    VideoDeviceProxy video = new VideoDeviceProxy(ip, port);
        //    string id = "rec1";
        //    int res1 = 1;
        //    int colorspace = 11;
        //    int fps = 15;
        //    ArrayList data = new ArrayList();
        //    try
        //    {
        //        Console.WriteLine("subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("unable to subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }

        //    try
        //    {
        //        while (true)
        //        {
        //            s.Reset();
        //            s.Start();
        //            data = (ArrayList)video.getImageRemote(id);
        //            s.Stop();
        //            video.releaseImage(id);
        //            Console.WriteLine(s.ElapsedMilliseconds);
        //        }
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("error retrive data");
        //    }
        //}
        //static void testMotion()
        //{
        //    List<float> angle;
        //    try
        //    {
        //        MotionProxy motion = new MotionProxy(ip, port);
        //        angle = motion.getAngles("LLeg", false);
        //        for (int i = 0; i < angle.Count; i++)
        //        {
        //            Console.WriteLine(angle[i].ToString());
        //        }
        //        Console.ReadKey();
        //    }
        //    catch
        //    {
        //        Console.WriteLine("error");
        //        Console.ReadKey();
        //    }
        //}
        //static void testCamera()
        //{
        //    VideoDeviceProxy video = new VideoDeviceProxy(ip, port);
        //    string id = "rec1";
        //    int res1 = 0;
        //    int colorspace = 3;
        //    int fps = 15;
        //    ArrayList data = new ArrayList();
        //    try
        //    {
        //        Console.WriteLine("subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("unable to subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }

        //    try
        //    {
        //        while (true)
        //        {
        //            s.Reset();
        //            s.Start();
        //            data = (ArrayList)video.getImageRemote(id);
        //            s.Stop();
        //            video.releaseImage(id);
        //            Console.WriteLine(s.ElapsedMilliseconds);
        //        }
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("error retrive data");
        //    }
        //}
        //static void testRecord()
        //{
        //    AudioRecorderProxy record = new AudioRecorderProxy(ip, port);
        //    ArrayList channels = new ArrayList();
        //    channels.Add(0);
        //    channels.Add(1);
        //    channels.Add(0);
        //    channels.Add(0);
        //    try
        //    {
        //        Console.WriteLine("start recording");
        //        s.Start();
        //        record.startMicrophonesRecording("/home/nao/test.wav", "wav", 16000, channels);
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //    Console.ReadKey();
        //    record.stopMicrophonesRecording();
        //    s.Stop();
        //    Console.WriteLine("stop recording "+s.ElapsedMilliseconds.ToString());
        //}
        //static void playRecord()
        //{
        //    try
        //    {
        //        AudioPlayerProxy play = new AudioPlayerProxy(ip, port);
        //        int id = play.loadFile("/home/nao/test.wav");
        //        Console.WriteLine("load file "+id.ToString());
        //        Console.ReadKey();
        //        Console.WriteLine("start playing");
        //        s.Start();
        //        play.setVolume(id, 1.0f);
        //        play.play(id);
        //        s.Stop();
        //        Console.WriteLine("finish playing "+s.ElapsedMilliseconds.ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //        Console.ReadKey();
        //    }
        //}
        //static void testEnergy()
        //{
        //    AudioDeviceProxy device = new AudioDeviceProxy(ip, port);
        //    device.enableEnergyComputation();
        //    float a, b, c, d;
        //    while (true)
        //    {
        //        a = device.getFrontMicEnergy();
        //        b = device.getLeftMicEnergy();
        //        c = device.getRightMicEnergy();
        //        d = device.getRearMicEnergy();
        //        Console.WriteLine("{0}   {1}   {2}   {3}", a, b, c, d);
        //    }
        //}
        //static void delay(int time)
        //{
        //    Stopwatch s = new Stopwatch();
        //    s.Reset();
        //    s.Start();
        //    while (s.ElapsedMilliseconds < time)
        //    {
        //    }
        //    s.Stop();
        //}
    }
}
