using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
using Aldebaran.Proxies;
namespace LumenServer
{
    public class LumenBattery
    {
        private BatteryProxy battery;
        MemoryStream msSend;
        BinaryWriter bw;
        byte[] buffer;
        int trialNum;
        public LumenBattery()
        {
            
        }
        public void getBatteryPercentage(Socket socket)
        {
            int percentage;
            lock (Program.data.battery)
            {
                percentage = Program.data.battery.percentage;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("getBatteryPercentage");
            bw.Write(percentage);
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
        public void isPlugged(Socket socket)
        {
            bool flag;
            lock (Program.data.battery)
            {
                flag = Program.data.battery.isPlugged;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("isPlugged");
            bw.Write(flag);
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
        public void isCharging(Socket socket)
        {
            bool flag;
            lock (Program.data.battery)
            {
                flag = Program.data.battery.isCharging;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("isCharging");
            bw.Write(flag);
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
