using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using LumenAPI;
using System.IO;
using System.Diagnostics;
namespace naoCamera
{
    public partial class streaming : Form
    {
        object sync;
        Bitmap image;
        bool flag = false;
        Stopwatch s = new Stopwatch();
        public streaming()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread a = new Thread(getImage);
            Thread b = new Thread(showImage);

            a.Start(); 
            b.Start();
        }
        private void getImage()
        {
            string ip = "167.205.56.134";
            int port = 2020;
            byte[] imagedata;

            LumenVideoDevice video = new LumenVideoDevice(ip, port);
            try
            {
                BitmapSource imageBitmap;
                JpegBitmapEncoder encoder;
                MemoryStream ms;
                while (true)
                {
                    s.Reset();
                    s.Start();
                    imagedata = video.getImageRemote();
                    s.Stop();
                    Console.WriteLine("time : " + s.ElapsedMilliseconds.ToString());
                    imageBitmap = BitmapSource.Create(320, 240, 96, 96, PixelFormats.Rgb24, BitmapPalettes.WebPalette, imagedata, 320 * 3);
                    encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(imageBitmap));
                    ms = new MemoryStream();
                    encoder.Save(ms);
                    
                    if (!flag)
                    {
                        
                        image = new Bitmap(ms);
                        
                        flag = true;
                    }
                }
            }
            catch(Exception error)
            {
                Console.WriteLine("error taking data");
                Console.WriteLine("error : " + error.ToString());
            }
        }

        private Bitmap Bitmap(MemoryStream ms)
        {
            throw new NotImplementedException();
        }
        private void showImage()
        {
            try
            {
                while (true)
                {

                    if (flag)
                    {
                        pictureBox1.Image = image;
                        flag = false;
                    }
                    
                }
            }
            catch
            {
                Console.WriteLine("error showing data");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            
        }
        private void delay(int time)
        {
            s.Reset();
            s.Start();
            while (s.ElapsedMilliseconds < time)
            {
            }
            s.Stop();
        }
    }
}
