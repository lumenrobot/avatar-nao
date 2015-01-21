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
    public class LumenPosture
    {
        private RobotPostureProxy posture;
        MemoryStream msSend;
        BinaryWriter bw;
        byte[] buffer;
        int trialNum;
        public LumenPosture()
        {
            posture = new RobotPostureProxy(Program.naoIP, Program.naoPort);
        }

        public void goToPosture(BinaryReader br)
        {
            string postureName = br.ReadString();
            float speed = br.ReadSingle();
            trialNum = 0;
            tryCode:
            try
            {
                posture.goToPosture(postureName, speed);
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
                posture.stopMove();
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
