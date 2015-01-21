using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Aldebaran.Proxies;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Diagnostics;

namespace naoCamera
{
    public partial class Form1 : Form
    {
        Bitmap image;
        bool flagImage;
        Stopwatch s = new Stopwatch();
        public Form1()
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

        private void showImage()
        {
            while (true)
            {
                if (flagImage)
                {
                    pictureBox1.Image = image;
                    flagImage = false;
                }
            }
        }
        private void getImage()
        {
            string ip = "167.205.56.179";
            int port = 9559;

            VideoDeviceProxy video = new VideoDeviceProxy(ip, port);
            string id = "camera";
            int res = 1;
            int colorSpace = 11;
            int fps = 15;
            ArrayList data = new ArrayList();
            byte[] imageData;
            bool flag;
            int width = 320;
            int heigth = 240;
            try
            {
                video.subscribe(id, res, colorSpace, fps);
                flag = true;
            }
            catch
            {
                video.unsubscribe(id);
                video.subscribe(id, res, colorSpace, fps);
                MessageBox.Show("error while subscribe");
                flag = true;
            }
            if (flag)
            {
                try
                {
                    BitmapSource imageBitmap;
                    JpegBitmapEncoder encoder;
                    MemoryStream ms;

                    while (true)
                    {
                        s.Reset();
                        s.Start();
                        
                        data = (ArrayList)video.getImageRemote(id);
                        s.Stop();
                        Console.WriteLine("time : " + s.ElapsedMilliseconds.ToString());
                        imageData = (byte[])data[6];
                        imageBitmap = BitmapSource.Create(width, heigth, 96, 96, PixelFormats.Rgb24, BitmapPalettes.WebPalette, imageData, width * 3);
                        encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(imageBitmap));
                        ms = new MemoryStream();
                        encoder.Save(ms);
                        
                        if (!flagImage)
                        {
                            image = new System.Drawing.Bitmap(ms);
                            flagImage = true;
                        }
                        video.releaseImage(id);
                    }
                    video.unsubscribe(id);
                }
                catch (Exception error)
                {

                    MessageBox.Show(error.ToString());
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        
       
    }
}
