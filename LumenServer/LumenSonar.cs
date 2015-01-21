using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Net;
using System.Net.Sockets;
using Aldebaran.Proxies;
namespace LumenServer
{
    public class LumenSonar
    {
        private SonarProxy sonar;
        MemoryStream msSend;
        BinaryWriter bw;
        byte[] buffer;
        int trialNum;
        public LumenSonar()
        {
        }
        public void getDistance(Socket socket, BinaryReader br)
        {
            string sensor = br.ReadString();
            float distance = new float() ;
            lock (Program.data.sonar)
            {
                if (sensor == "left")
                {
                    distance = Program.data.sonar.leftSensor;
                }
                else if (sensor == "right")
                {
                    distance = Program.data.sonar.rightSensor;
                }
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("getDistance");
            bw.Write(distance);
            buffer = msSend.ToArray();
            trySend:
            try
            {
                socket.Send(buffer);
            }
            catch
            {
                goto trySend;
            }
        }
    }
}
