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
            handler = new Thread(commandHandling);
            handler.Start();
            if (connectionCheck.IsAlive == false)
            {
                connectionCheck.Start();
            }
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
                        Console.WriteLine("incoming command");
                        JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
                        Command command = JsonConvert.DeserializeObject<Command>(body, setting);
                        switch (command.type.ToLower())
                        {
                            case "motion":
                                Console.WriteLine("case : motion");
                                motionHandling(command);
                                break;
                            case "posture":
                                Console.WriteLine("case : posture");
                                postureHandling(command);
                                break;
                            case "texttospeech":
                                Console.WriteLine("case : tts");
                                ttsHandling(command);
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
                    Console.WriteLine("case : wakeUp");
                    motion.wakeUp();
                    Console.WriteLine("finish : wakeUp");
                    break;
                case "rest":
                    Console.WriteLine("case : rest");
                    motion.rest();
                    Console.WriteLine("finish : rest");
                    break;
                case "setangles":
                    Console.WriteLine("case : setAngles");
                    motion.setAngles(new ArrayList(newCommand.parameter.jointName), new ArrayList(newCommand.parameter.angles), newCommand.parameter.speed);
                    Console.WriteLine("finish : setAngles");
                    break;
                case "changeangle":
                    Console.WriteLine("case : changeangle");
                    motion.changeAngles(new ArrayList(newCommand.parameter.jointName), new ArrayList(newCommand.parameter.angles), newCommand.parameter.speed);
                    Console.WriteLine("finish : changeangle");
                    break;
                case "setStiffnesses":
                    Console.WriteLine("case : setStiffnesses");
                    motion.setStiffnesses (new ArrayList(newCommand.parameter.jointName), new ArrayList(newCommand.parameter.stiffnessess));
                    Console.WriteLine("finish : setStiffnesses");
                    break;
                case "closehand":
                    Console.WriteLine("case : closehand");
                    motion.closeHand (newCommand.parameter.handName);
                    Console.WriteLine("finish : closehand");
                    break;
                case "openhand":
                    Console.WriteLine("case : openhand");
                    motion.closeHand(newCommand.parameter.handName);
                    Console.WriteLine("finish : openhand");
                    break;
                case "moveinit":
                    Console.WriteLine("case : moveinit");
                    motion.moveInit();
                    Console.WriteLine("finish : moveinit");
                    break;
                case "moveto":
                    Console.WriteLine("case : moveto");
                    motion.moveTo(newCommand.parameter.x, newCommand.parameter.y, newCommand.parameter.tetha);
                    Console.WriteLine("finish : moveto");
                    break;
                case "setwalkarmsenabled":
                    Console.WriteLine("case : setwalkarmsenabled");
                    motion.setWalkArmsEnabled(newCommand.parameter.LHand, newCommand.parameter.RHand);
                    Console.WriteLine("finish : setwalkarmsenabled");
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
                    posture.goToPosture(newCommand.parameter.postureName, newCommand.parameter.speed);
                    break;
                case "stopmove":
                    posture.stopMove();
                    break;

            }
        }
        private void ttsHandling(Command newCommand)
        {
            switch (newCommand.method.ToLower())
            {
                case "say":
                    tts.say(newCommand.parameter.text);
                    break;
                case "setlanguage":
                    tts.setLanguage(newCommand.parameter.language);
                    break;
            }
        }
    }
}
