using System;
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
using NAudio.CoreAudioApi;
using System.IO;
using System.Timers;
using System.Diagnostics;


namespace LumenServer
{
    class CommandHandler
    {
        private IModel channel;
        private IModel channelSend;
        private QueueingBasicConsumer consumer;
        BasicDeliverEventArgs basicEvent;
        MotionProxy motion;
        RobotPostureProxy posture;
        TextToSpeechProxy tts;
        AudioDeviceProxy audio;
        AudioRecorderProxy audioRecording;
        AudioPlayerProxy audioPlayer;
        Thread handler;
        public Thread connectionCheck;
        NAudio.Wave.WaveInEvent sourceStream;
        NAudio.Wave.WaveFileWriter streamWriter;
        bool isRecording = false;
        
        public CommandHandler()
        {
            channel = Program.connection.CreateModel();
            
            channelSend = Program.connection.CreateModel();//buat recording
            QueueDeclareOk channelQueue = channel.QueueDeclare("", true, false, true, null);
            string channelKey = "avatar.NAO.command";   
            channel.QueueBind(channelQueue.QueueName, "amq.topic", channelKey);
            consumer = new QueueingBasicConsumer(channel);
            channel.BasicConsume(channelQueue, false, consumer);
        }

        public void startHandling()
        {
            Console.WriteLine("Getting Motion proxy...");
            motion = new MotionProxy(Program.naoIP, Program.naoPort);
            Console.WriteLine("Getting Robot Posture proxy...");
            posture = new RobotPostureProxy(Program.naoIP, Program.naoPort);
            try
            {
                Console.WriteLine("Getting Text-to-Speech proxy...");
                tts = new TextToSpeechProxy(Program.naoIP, Program.naoPort);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: Cannot get Text-to-speech Proxy: " + e);
            }
            audio = new AudioDeviceProxy(Program.naoIP, Program.naoPort);
            
            audioRecording = new AudioRecorderProxy(Program.naoIP, 9559);
            audioPlayer = new AudioPlayerProxy(Program.naoIP, 9559);
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
                
                
                
                BasicDeliverEventArgs basicEvent = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                var propertiesReceived = basicEvent.BasicProperties;
                var propertiesReplied = channel.CreateBasicProperties();
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
                   var responseByte = Encoding.UTF8.GetBytes("command finished");
                    propertiesReplied.CorrelationId = propertiesReceived.CorrelationId;
                    Console.WriteLine("send back acknowledgment corrId={0} to {1}", propertiesReceived.CorrelationId, propertiesReceived.ReplyTo);
                    channel.BasicPublish("", propertiesReceived.ReplyTo, propertiesReplied, responseByte);
                    channel.BasicAck(basicEvent.DeliveryTag, false);    
                    Console.WriteLine("acknowledgment sent");
                    //subs.Ack(basicEvent);
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
                case "goodbye":
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" ,"RHand"}, new List<float>() { -0.9f, -0.44f, 0.53f, 0.3f, -0.2f,1.0f }, 0.5f);
                    Thread.Sleep(500);
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 1.2f, -0.2f }, 0.2f);
                    Thread.Sleep(1000);
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 0.3f, -0.2f }, 0.2f);
                    Thread.Sleep(1000);
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 1.2f, -0.2f }, 0.2f);
                    Thread.Sleep(1000);
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw","RHand" }, new List<float>() { -0.9f, -0.44f, 0.53f, 0.3f, -0.2f,1.0f }, 0.2f);
                    Thread.Sleep(1000);
                    break;
                case "photopose":
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.12f, 0.31f, 0.34f, 1.25f, -0.1f }, 0.2f);
                    motion.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.9f, 0.71f, -0.79f, -0.88f, 0.42f }, 0.2f);
                    motion.openHand("LHand");
                    break;
                case "dance":
                    Console.WriteLine("start music");
                    posture.goToPosture("StandInit", 0.5f);
                    new Thread(delegate()
                    {
                        audioPlayer.playFile("/home/nao/gangnam.mp3");
                    }).Start();
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.28f, 0.18f, 0.71f, 0.71f, -0.7f }, 0.2f);
                    motion.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.72f, 0.04f, -0.67f, -1.06f, 0.97f }, 0.2f);
                    for (int i = 0; i < 6; i++)
                    {
                        motion.setAngles(new List<string>() { "HeadPitch", "RShoulderPitch", "LShoulderPitch" }, new List<float>() { 0.0f, 1.28f, -0.72f }, 0.3f);
                        Thread.Sleep(500);
                        motion.setAngles(new List<string>() { "HeadPitch", "RShoulderPitch", "LShoulderPitch" }, new List<float>() { 0.51f, 1.10f, -0.60f }, 0.3f);
                        Thread.Sleep(500);
                    }
                    motion.setAngles(new List<string>() { "HeadPitch", "RShoulderPitch", "LShoulderPitch" }, new List<float>() { 0.0f, 1.28f, -0.72f }, 0.3f);
                    Thread.Sleep(500);
                    motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -1.28f, -0.22f, -0.13f, 1.1f, 0.2f }, 0.2f);
                    motion.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.28f, -0.31f, -0.46f, -0.59f, 0.62f }, 0.2f);
                    
                    for (int i = 0; i < 7; i++)
                    {
                        motion.setAngles(new List<string>() { "HeadYaw", "RShoulderRoll", "LShoulderRoll" }, new List<float>() { 0.0f, -0.22f, -0.31f }, 0.2f);
                        Thread.Sleep(500);
                        motion.setAngles(new List<string>() { "HeadYaw", "RShoulderRoll", "LShoulderRoll" }, new List<float>() { 0.51f, -0.73f, 0.78f }, 0.3f);
                        Thread.Sleep(500);
                    }
                    posture.goToPosture("Stand", 0.5f);
                    break;

                case "sing":
                    Console.WriteLine("Start singing");
                    Random ran = new Random();
                    //int select = ran.Next(1, 2);
                    int select = 1;
                    if (select == 1)
                    {
                        new Thread(delegate()
                        {
                            audioPlayer.playFile("/home/nao/manuk.mp3");
                        }).Start();
                        Thread.Sleep(15000);
                        posture.goToPosture("StandInit", 0.5f);
                        motion.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.19f, 0.05f, 0.54f, 1.54f, 0.16f }, 0.2f);
                        for (int i = 0; i < 9; i++)
                        {
                            motion.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.93f, 0.01f, -0.73f, -0.47f, 0.26f }, 0.1f);
                            Thread.Sleep(1500);
                            motion.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.89f, 0.36f, -0.74f, -0.05f, 0.26f }, 0.1f);
                            Thread.Sleep(1500);
                        }
                        posture.goToPosture("Stand", 0.5f);
                    }
                    else if (select == 2)
                    {
                        posture.goToPosture("StandInit", 0.5f);
                        new Thread(delegate()
                        {
                            audioPlayer.playFile("/home/nao/uptown.mp3");
                        }).Start();
                        for (int i =0;i<=10;i++){
                            motion.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.87f, -0.03f, -1.4f, -1.23f, 0.05f, 0.63f, -0.07f, 1.08f, 0.88f, 0.37f }, 0.3f);
		                    Thread.Sleep(500);
                            motion.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.89f, -0.26f, -0.99f, -1.24f, 0.06f, 0.63f, 0.26f, 0.95f, 0.87f, 0.37f }, 0.4f);
		                    Thread.Sleep(500);
                        }

                    }
                    
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
                        audio.setParameter("outputSampleRate",22050);
                    }
                    catch
                    {
                    }
                    Console.WriteLine("execution audioDevice.sendRemoteBufferToOutput() finished");
                    break;
                case "record":
                    Console.WriteLine("executing audioDevice.record()");
                    audioRecording.stopMicrophonesRecording();
                    audioRecording.startMicrophonesRecording("/home/nao/recordings/microphones/recording.wav", "wav", 16000, new List<int> { 0, 0, 1, 0 });
                    Thread.Sleep(5000);
                    audioRecording.stopMicrophonesRecording();
                    MemoryStream file = new MemoryStream();
                    Program.sftpClient.DownloadFile(Program.sftpClient.WorkingDirectory + "/recordings/microphones/recording.wav", file, null);
                    byte[] wavBuffer = file.ToArray();
                    string wavBase64 = Convert.ToBase64String(wavBuffer);
                    RecordingData dataToSend = new RecordingData { name = newCommand.parameter.recordingName, content = wavBase64 };
                    string stringToSend = JsonConvert.SerializeObject(dataToSend);
                    byte[] bufferToSend = Encoding.UTF8.GetBytes(stringToSend);
                    
                    channelSend.BasicPublish("amq.topic", "avatar.NAO.data.recording", null, bufferToSend);
                    Console.WriteLine("execution audioDevice.record() finished");
                    break;
            }
        }
        
    }
}
