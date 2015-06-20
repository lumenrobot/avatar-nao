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


//this file contains defenition of NAO data.
namespace LumenServer
{
    public class NaoImage
    {
        public string id;
        public byte[] data;
        public int resolution;
        public int fps;
        public int imageSize;

    }
    public class NaoJoint
    {
        public List<string> names;
        public List<float> angles;
        public List<float> stiffnesses;

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
        public List<string> names;
        public List<float> values;
    }
    
}
