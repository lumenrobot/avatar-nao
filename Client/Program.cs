using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using System.IO;
using LumenAPI;
using System.Drawing;
using System.Threading;
namespace Client
{
    class Program
    {
        static void Main(string[] args)
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
            catch(Exception e)
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
