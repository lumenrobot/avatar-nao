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
    public class LumenTextToSpeech
    {
        private TextToSpeechProxy tts;
        MemoryStream msSend;
        BinaryWriter bw;
        byte[] buffer;
        int trialNum;
        public LumenTextToSpeech()
        {
            tts = new TextToSpeechProxy(Program.naoIP, Program.naoPort);
        }

        public void say(BinaryReader br)
        {
            string message = br.ReadString();
            trialNum = 0;
            tryCode:
            try
            {
                tts.say(message);
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
        public void setLanguage(BinaryReader br)
        {
            string language = br.ReadString();
            trialNum = 0;
            tryCode:
            try
            {
                tts.setLanguage(language);
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
