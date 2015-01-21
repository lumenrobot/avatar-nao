using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Net.Sockets;
using System.IO;
namespace LumenServer
{
    public class LumenSensor
    {
        MemoryStream msSend;
        BinaryWriter bw;
        byte[] buffer;
        int trialNum;
        public LumenSensor()
        {
        }
        public void getTactile(Socket socket, BinaryReader br)
        {
            float value;
            int id = br.ReadInt32();
            lock (Program.data.tactile)
            {
                value = Program.data.tactile[id].value;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("getTactile");
            bw.Write(value);
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
        public void getBumper(Socket socket, BinaryReader br)
        {
            float value;
            int id = br.ReadInt32();
            lock (Program.data.tactile)
            {
                value = Program.data.tactile[id].value;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("getBumper");
            bw.Write(value);
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
        public void getButton(Socket socket, BinaryReader br)
        {
            float value;
            int id = br.ReadInt32();
            lock (Program.data.tactile)
            {
                value = Program.data.tactile[id].value;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("getButton");
            bw.Write(value);
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
