﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aldebaran.Proxies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System.Threading;
using System.Collections;
using NAudio;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;
using System.IO;
namespace LumenServer
{
    class CommandHandler
    {
        private IModel channel;
        private Subscription subs;
        BasicDeliverEventArgs basicEvent;
        MotionProxy motion;
        RobotPostureProxy posture;
        TextToSpeechProxy tts;
        AudioDeviceProxy audio;
        Thread handler;
        public Thread connectionCheck;
        public CommandHandler()
        {
            channel = Program.connection.CreateModel();
            QueueDeclareOk channelQueue = channel.QueueDeclare("", true, false, true, null);
            string channelKey = "avatar.NAO.command";
            channel.QueueBind(channelQueue.QueueName, "amq.topic", channelKey);

            connectionCheck = new Thread(checkConnection);
            subs = new Subscription(channel, channelQueue.QueueName);

        }

        public void startHandling()
        {

            motion = new MotionProxy(Program.naoIP, Program.naoPort);
            posture = new RobotPostureProxy(Program.naoIP, Program.naoPort);
            tts = new TextToSpeechProxy(Program.naoIP, Program.naoPort);
            audio = new AudioDeviceProxy(Program.naoIP, Program.naoPort);
            handler = new Thread(commandHandling);
            handler.Start();
            Console.WriteLine("Ready to handle command!");
        }
        private void checkConnection()
        {
            while (true)
            {
                if (Program.aquisition.connection == false)
                {
                    Console.WriteLine("aborting command handler...");
                    //handler.Abort();
                    //connectionCheck.Abort();
                }
            }
        }
        private void commandHandling()
        {
            while (true)
            {
                if (subs.Next(0, out basicEvent))
                {
                    try
                    {
                        string body = Encoding.UTF8.GetString(basicEvent.Body);
                        Console.WriteLine("new incoming command!");
                        JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                        Command command = JsonConvert.DeserializeObject<Command>(body, setting);
                        switch (command.type.ToLower())
                        {
                            case "motion":
                                motionHandling(command);
                                break;
                            case "posture":
                                postureHandling(command);
                                break;
                            case "texttospeech":
                                ttsHandling(command);
                                break;
                            case "audiodevice":
                                audioDeviceHandling(command);
                                break;
                            default:
                                Console.WriteLine(command.type);
                                break;
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        subs.Ack(basicEvent);
                    }
                }
                else
                {
                    
                }

            }
        }
        private void motionHandling(Command newCommand)
        {
            switch (newCommand.method.ToLower())
            {
                case "wakeup":
                    Console.WriteLine("executing motion.wakeUp()");
                    motion.wakeUp();
                    Console.WriteLine("execution motion.wakeUp() finished");
                    break;
                case "rest": 
                    Console.WriteLine("executing motion.rest()");
                    motion.rest();
                    Console.WriteLine("execution motion.rest() finished");
                    break;
                case "setangles":
                    Console.WriteLine("executing motion.setAngle()");
                    motion.setAngles(new ArrayList(newCommand.parameter.jointName), new ArrayList(newCommand.parameter.angles), newCommand.parameter.speed);
                    Console.WriteLine("execution motion.setAngle() finished");
                    break;
                case "changeangle":
                    Console.WriteLine("executing motion.changeangle()");
                    motion.changeAngles(new ArrayList(newCommand.parameter.jointName), new ArrayList(newCommand.parameter.angles), newCommand.parameter.speed);
                    Console.WriteLine("execution motion.changeangle() finished");
                    break;
                case "setstiffnesses":
                    Console.WriteLine("executing motion.setStiffnesses()");
                    motion.setStiffnesses (new ArrayList(newCommand.parameter.jointName), new ArrayList(newCommand.parameter.stiffnessess));
                    Console.WriteLine("execution motion.setStiffnesses() finished");
                    break;
                case "closehand":
                    Console.WriteLine("executing motion.closehand()");
                    motion.closeHand (newCommand.parameter.handName);
                    Console.WriteLine("execution motion.closehand() finished");
                    break;
                case "openhand":
                    Console.WriteLine("executing motion.openhand()");
                    motion.closeHand(newCommand.parameter.handName);
                    Console.WriteLine("execution motion.openhand() finished");
                    break;
                case "moveinit":
                    Console.WriteLine("executing motion.moveinit()");
                    motion.moveInit();
                    Console.WriteLine("execution motion.moveinit() finished");
                    break;
                case "moveto":
                    Console.WriteLine("executing motion.moveto()");
                    motion.moveTo(newCommand.parameter.x, newCommand.parameter.y, newCommand.parameter.tetha);
                    Console.WriteLine("execution motion.moveto() finished");
                    break;
                case "setwalkarmsenabled":
                    Console.WriteLine("executing motion.setwalkarmsenabled()");
                    motion.setWalkArmsEnabled(newCommand.parameter.LHand, newCommand.parameter.RHand);
                    Console.WriteLine("execution motion.setwalkarmsenabled() finished");
                    break;
                default:
                    Console.WriteLine(newCommand.method);
                    break;
            }

        }
        private void postureHandling(Command newCommand)
        {
            switch (newCommand.method.ToLower())
            {
                case "gotoposture":
                    Console.WriteLine("executing posture.gotoposture()");
                    posture.goToPosture(newCommand.parameter.postureName, newCommand.parameter.speed);
                    Console.WriteLine("execution posture.gotoposture() finished");
                    break;
                case "stopmove":
                    Console.WriteLine("executing posture.stopmove()");
                    posture.stopMove();
                    Console.WriteLine("execution posture.stopmove() finished");
                    break;

            }
        }
        private void ttsHandling(Command newCommand)
        {
            switch (newCommand.method.ToLower())
            {
                case "say":
                    Console.WriteLine("executing texttospeech.say()");
                    tts.say(newCommand.parameter.text);
                    Console.WriteLine("execution texttospeech.say() finished");
                    break;
                case "setlanguage":
                    Console.WriteLine("executing texttospeech.setlanguage()");
                    tts.setLanguage(newCommand.parameter.language);
                    Console.WriteLine("execution texttospeech.setlanguage() finished");
                    break;
            }
        }
        private void audioDeviceHandling(Command newCommand)
        {
            switch (newCommand.method.ToLower())
            {
                case "sendremotebuffertooutput":
                    Console.WriteLine("executing audioDevice.sendRemoteBufferToOutput()");
                    byte[] wavData = Convert.FromBase64String(newCommand.parameter.wavFile);
                    MemoryStream ms = new MemoryStream(wavData);
                    WaveFileReader wavFile = new WaveFileReader(ms);
                    int nbOfChannels = wavFile.WaveFormat.Channels;
                    int sampleRate = wavFile.WaveFormat.SampleRate;
                    int numberOfOutputChannels = 2;
                    byte[] buffer = new byte[2 * (int)wavFile.SampleCount];
                    int byteRead = wavFile.Read(buffer, 0, 2 * (int)wavFile.SampleCount);
                    int nbOfFrames = byteRead / 2;
                    short[] fInputAudioData = new short[(int)wavFile.SampleCount];
                    short[] fStereoAudioData = new short[2* (int)wavFile.SampleCount];
                    int sample = 0;
                    for (int index = 0; index < nbOfFrames; index++)
                    {
                        fInputAudioData[index] = BitConverter.ToInt16(buffer, sample);
                        sample += 2;
                    }
                    if (nbOfChannels == 1)
                    {
                        int i = 0;
                        for (int j = 0; j < nbOfFrames; j++)
                        {
                            fStereoAudioData[i] = fInputAudioData[j];
                            fStereoAudioData[i + 1] = fInputAudioData[j];
                            i += numberOfOutputChannels;
                        }
                    }
                    else if (nbOfChannels == 2)
                    {
                        for (int i = 0; i < nbOfFrames; i++)
                        {
                            fStereoAudioData[i] = fInputAudioData[i];
                        }
                    }
                    try
                    {
                        audio.setParameter("outputSampleRate", sampleRate);
                        audio.sendRemoteBufferToOutput(nbOfFrames, fStereoAudioData);
                    }
                    catch
                    {
                    }
                    Console.WriteLine("execution audioDevice.sendRemoteBufferToOutput() finished");
                    break;
            }
        }
    }
}