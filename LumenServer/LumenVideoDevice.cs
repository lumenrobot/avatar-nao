using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using Aldebaran.Proxies;

using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace LumenServer
{
    public class LumenVideoDevice
    {
        public VideoDeviceProxy videoDevice;
        byte[] buffer;
        int id = Program.data.defaultImageId;
        public LumenVideoDevice()
        {
            videoDevice = new VideoDeviceProxy(Program.naoIP, Program.naoPort);
        }
        public void getImageRemote(Socket handlerSocket)
        {
            byte[] buffer;
            lock (Program.data.image[id])
            {
                buffer = Program.data.image[id].data;
                
            }
            
            trySend:
            try
            {
                handlerSocket.Send(buffer);
            }
            catch
            {
                goto trySend;
            }
            
        }
    }
}
