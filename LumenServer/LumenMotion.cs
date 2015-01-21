using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//additional
using System.Net;
using System.Net.Sockets;
using System.IO;
using Aldebaran.Proxies;

namespace LumenServer
{
    public class LumenMotion
    {
        private MotionProxy motion;
        MemoryStream msSend;
        BinaryWriter bw;
        byte[] buffer;
        int trialNum;
        public LumenMotion()
        {
            motion = new MotionProxy(Program.naoIP, Program.naoPort);
        }

        //stifness control
        public void wakeUp()
        {
            trialNum = 0;
            tryCode:
            try
            {
                motion.wakeUp();
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void rest()
        {
            trialNum = 0;
            tryCode:
            try
            {
                motion.rest();
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void setStiffness(BinaryReader br)
        {
            string joint = br.ReadString();
            float value = br.ReadSingle();
            trialNum = 0;
            tryCode:
            try
            {
                motion.setStiffnesses(joint, value);
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void getStiffness(Socket socket, BinaryReader br)
        {
            int id = br.ReadInt32();
            float value;
            lock (Program.data.joint[id])
            {
                value = Program.data.joint[id].stiffness;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("getStiffness");
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

        //joint control
        public void setAngle(BinaryReader br)
        {
            string joint = br.ReadString();
            float angle = br.ReadSingle();
            float speed = br.ReadSingle();
            trialNum = 0;
            tryCode:
            try
            {
                motion.setAngles(joint, angle, speed);
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void changeAngle(BinaryReader br)
        {
            string joint = br.ReadString();
            float angle = br.ReadSingle();
            float speed = br.ReadSingle();
            trialNum = 0;
            tryCode:
            try
            {
                motion.changeAngles(joint, angle, speed);
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void getAngle(Socket socket, BinaryReader br)
        {

            int id = br.ReadInt32();
            float angle;
            lock (Program.data.joint[id])
            {
                angle = Program.data.joint[id].angle;
            }
            msSend = new MemoryStream();
            bw = new BinaryWriter(msSend);
            bw.Write("getAngle");
            bw.Write(angle);
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
        public void closeHand(BinaryReader br)
        {
            string hand = br.ReadString();
            trialNum = 0;
            tryCode:
            try
            {
                motion.closeHand(hand);
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void openHand(BinaryReader br)
        {
            string hand = br.ReadString();
            trialNum = 0;
            tryCode:
            try
            {
                motion.openHand(hand);
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }

        //locomotion
        public void moveInit()
        {
            trialNum = 0;
            tryCode:
            try
            {
                motion.moveInit();
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void moveTo(BinaryReader br)
        {
            float x = br.ReadSingle();
            float y = br.ReadSingle();
            float theta = br.ReadSingle();
            trialNum = 0;
            tryCode:
            try
            {
                motion.moveTo(x, y, theta);
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void setWalkArmsEnabled(BinaryReader br)
        {
            bool LHand = br.ReadBoolean();
            bool RHand = br.ReadBoolean();
            trialNum = 0;
            tryCode:
            try
            {
                motion.setWalkArmsEnabled(LHand, RHand);
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
        public void stopMove()
        {
            trialNum = 0;
            tryCode:
            try
            {
                motion.stopMove();
            }
            catch
            {
                if (trialNum < 3)
                {
                    trialNum++;
                    goto tryCode;
                }
            }
        }
    } 
}
