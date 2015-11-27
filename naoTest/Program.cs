using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using Aldebaran.Proxies;
using NAudio;
using NAudio.Wave;
using NAudio.WindowsMediaFormat;
using System.Threading;
using Renci.SshNet;

namespace naoTest
{
    public struct point
    {
        public int x;
        public int y;
    }
    
    
    class Program
    {
        static Stopwatch s = new Stopwatch();
        static string ip = "167.205.56.142";
        //static string ip = "127.0.0.1";
        static int port = 9559;

        public static List<point> a = new List<point>();

        static KeyboardInteractiveAuthenticationMethod keyboard = new KeyboardInteractiveAuthenticationMethod("nao");

        static void Main(string[] args)
        {
            // MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            // RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            grab();
            //tengkurap_berdiri();
            rest();
        }

        static void rest()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
            m.rest();
        }

        static void maju()
        {
            Console.Write("State Maju...");
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            p.goToPosture("Stand", 0.5f);
            Thread.Sleep(1000);
            m.moveTo(0.5f, 0.0f, 0.0f);
            m.setWalkArmsEnable(false,false);
        }

        static void mundur()
        {
            Console.Write("State Maju...");
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            p.goToPosture("StandInit", 0.5f);
            Thread.Sleep(1000);
            m.moveTo(-0.2f, 0.0f, 0.0f);
            m.setWalkArmsEnable(false, false);
        }

        static void kekiri()
        {
            Console.Write("State Maju...");
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            p.goToPosture("StandInit", 0.5f);
            Thread.Sleep(1000);
            //m.moveTo(0.8f, 0.0f, 0.0f);
            //m.setWalkArmsEnable(false, false);
            m.moveTo(0.0f, 0.0f, 1.57f);
            m.setWalkArmsEnable(false, false);
            maju();
        }

        static void kekanan()
        {
            Console.Write("Ke Kanan ....");
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            p.goToPosture("StandInit", 0.5f);
            Thread.Sleep(1000);
            //m.moveTo(0.8f, 0.0f, 0.0f);
            //m.setWalkArmsEnable(false, false);
            m.moveTo(0.0f, 0.0f, -1.57f);
            m.setWalkArmsEnable(false, false);
            maju();
        }

        static void terlentang_berdiri()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            m.wakeUp();
            p.goToPosture("LyingBack", 0.5f);
            //p.goToPosture("Sit", 0.5f);
            p.goToPosture("Stand", 0.5f);
        }

        static void Berdiri_terlentang()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            m.wakeUp();
            p.goToPosture("Stand", 0.5f);
            //p.goToPosture("Sit", 0.5f);
            p.goToPosture("LyingBack", 0.5f);
        }

        static void tengkurap_berdiri()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            m.wakeUp();
            p.goToPosture("LyingBelly", 0.5f);
           // p.goToPosture("Sit", 0.5f);
            p.goToPosture("Stand", 0.5f);
        }

        static void berdiri_tengkurap()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            m.wakeUp();
            p.goToPosture("Stand", 0.5f);
            // p.goToPosture("Sit", 0.5f);
            p.goToPosture("LyingBelly", 0.5f);
        }

        static void head_zero()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            m.wakeUp();
            m.setAngles(new List<string>() { "HeadYaw", "HeadPitch" }, new List<float>() { 0.0f, 0.0f }, 0.2f);
        }

        //digunakan untuk me-release objek di tangan
        static void release()
        {
            Console.Write("Release object ...");
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            m.wakeUp();
            p.goToPosture("Stand", 0.5f);
            m.closeHand("RHand");
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.046f, 0.078f, 0.219f, 0.061f, -0.189f }, 0.2f);
            Thread.Sleep(2000);
            m.openHand("RHand");
            Thread.Sleep(1000);
            m.closeHand("RHand");
            Thread.Sleep(1000);
            p.goToPosture("Stand", 0.5f);
        }
        
        //digunakan untuk mengambil objek
        static void grab()
        {
            Console.Write("Grab object ...");
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            m.wakeUp();
            p.goToPosture("StandInit", 0.5f);
            Thread.Sleep(1000);
            //COBA 1
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.845f, -0.376f, -1.164f, 1.907f, -0.288f, -0.398f }, 0.01f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.856f, 0.130f, -0.520f, 2.112f, -1.186f, -0.086f }, 0.01f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.979f, 0.367f, -0.365f, -0.351f, 0.141f }, 0.01f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.895f, 0.127f, 2.086f, 0.058f, -0.445f }, 0.01f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.934f, -0.376f, -1.345f, 2.112f, -0.730f, 0.229f }, 0.01f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.936f, 0.288f, -0.854f, 2.112f, -1.186f, -0.067f }, 0.01f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.813f, 0.399f, -0.351f, -0.353f, 0.141f }, 0.01f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.884f, -0.285f, 1.49f, 0.719f, 0.2f }, 0.01f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.497f, -0.095f, -1.084f, 2.112f, -0.818f, 0.109f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.498f, 0.087f, -0.718f, 2.112f, -1.186f, -0.08f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.114f, 0.394f, -0.354f, -0.340f, 0.141f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.109f, -0.314f, 1.703f, 0.479f, 0.448f }, 0.05f);

            //COBA 2
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.322f, -0.307f, -0.327f, 0.468f, -0.08f, 0.176f }, 0.05f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.32f, -0.27f, -0.54f, 0.68f, -0.095f, 0.210f }, 0.05f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.38f, 0.109f, -1.078f, -0.428f, -1.583f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.111f, -0.175f, 0.896f, 0.552f, 0.745f }, 0.05f);
            //Thread.Sleep(2000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { 0.477f, -0.320f, -0.865f, 2.011f, -0.820f, 0.245f }, 0.05f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.477f, -0.328f, -1.003f, 2.090f, -0.828f, 0.252f }, 0.05f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.403f, 0.176f, -1.088f, -0.431f, -1.583f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.061f, -0.198f, 0.917f, 0.551f, 0.745f }, 0.05f);
            //Thread.Sleep(2000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.808f, -0.376f, -1.144f, 2.112f, -0.816f, 0.253f }, 0.05f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.808f, -0.205f, -1.124f, 2.112f, -0.919f, 0.281f }, 0.05f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.419f, 0.339f, -1.088f, -0.380f, -1.583f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.834f, 0.314f, 1.143f, 0.117f, -0.167f }, 0.05f);
            //Thread.Sleep(2000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.811f, -0.373f, -1.423f, 2.112f, -0.856f, 0.066f }, 0.05f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.811f, -0.014f, -1.411f, 2.112f, -0.974f, 0.262f }, 0.05f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.571f, 0.147f, -1.786f, -0.25f, -1.586f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.646f, 0.314f, 1.262f, 0.172f, 0.118f }, 0.05f);
            //Thread.Sleep(2000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.606f, -0.316f, -1.384f, 2.112f, -0.798f, 0.189f }, 0.05f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.606f, -0.07f, -1.367f, 2.112f, -0.859f, 0.196f }, 0.05f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.246f, 0.242f, -1.795f, -0.236f, -1.586f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.0f, -0.281f, 1.275f, 0.090f, -0.118f }, 0.05f);

            //COBA3
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.158f, 0.348f, 0.114f, -0.092f, 0.244f, -0.274f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.156f, 0.204f, 0.156f, -0.067f, -0.173f, -0.130f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.752f, 0.359f, -0.819f, -0.537f, 0.136f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.542f, -0.109f, 0.537f, 0.278f, -0.12f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.216f, 0.261f, 0.484f, -0.092f, -0.047f, -0.285f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.219f, -0.061f, 0.262f, 0.588f, -0.532f, -0.009f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.727f, 0.347f, -0.822f, -0.535f, 0.138f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.177f, -0.163f, 0.594f, 0.255f, -0.054f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.294f, 0.338f, -0.864f, 1.853f, -0.8f, -0.216f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.293f, 0.273f, -0.765f, 1.767f, -0.788f, -0.189f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.683f, 0.325f, -0.821f, -0.535f, 0.138f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.227f, -0.325f, 0.591f, 0.255f, 0.055f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.353f, 0.219f, -1.353f, 2.112f, -0.755f, -0.258f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.353f, 0.31f, -1.378f, 2.112f, -0.795f, -0.278f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.215f, 0.222f, -0.836f, -0.035f, 0.138f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.91f, -0.06f, 0.615f, 0.284f, -0.052f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.397f, -0.069f, -1.149f, 2.112f, -1.189f, -0.055f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.397f, 0.176f, -1.201f, 2.112f, -1.186f, -0.086f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.799f, -0.015f, -0.825f, -0.044f, 0.136f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.531f, 0.221f, 0.363f, 0.087f, -0.041f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.462f, -0.238f, -1.485f, 2.112f, -1.189f, -0.028f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.462f, 0.322f, -1.529f, 2.112f, -1.186f, -0.08f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.535f, -0.308f, -0.824f, -0.06f, 0.138f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.198f, 0.219f, 0.365f, 0.057f, -0.041f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.446f, -0.058f, -0.859f, 2.112f, -1.189f, 0.008f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.447f, 0.116f, -0.904f, 2.112f, -1.186f, -0.092f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.845f, -0.224f, -0.772f, -0.061f, 0.138f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.663f, -0.044f, 0.048f, 0.057f, -0.04f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0,386f, -0.04f, -0.607f, 2.112f, -1.189f, 0.064f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.386f, 0.029f, -0.626f, 2.112f, -1.186f, -0.090f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.499f, 0.083f, -0.811f, -0.043f, 0.126f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.456f, -0.11f, -0.12f, 0.049f, -0.04f }, 0.05f);

            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.423f, 0.129f, -0.581f, 1.379f, -0.594f, -0.121f }, 0.02f);
            //  m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.423f, 0.023f, -0.713f, 1.398f, -0.508f, -0.058f }, 0.02f);
            //  m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.621f, 0.367f, -0.819f, -0.06f, 0.143f }, 0.05f);
            //  m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.884f, -0.285f, 1.49f, 0.719f, 0.2f }, 0.05f);
            //  m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.529f, 0.397f, -0.563f, 1.445f, -0.614f, -0.35f }, 0.02f);
            //  m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.529f, -0.015f, -0.747f, 1.789f, -0.862f, -0.092f }, 0.02f);
            //  m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.555f, 0.313f, -0.82f, -0.086f, 0.135f }, 0.05f);
            //  m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.865f, -0.283f, 1.491f, 0.729f, 0.202f }, 0.05f);

            //  m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.736f, 0.008f, 0.189f, 0.690f, -0.673f, -0.429f }, 0.02f);
            // m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.736f, -0.503f, -1.058f, 1.721f, -0.831f, 0.327f }, 0.02f);
            // m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.417f, -0.084f, -0.752f, -0.966f, 0.143f , 0.184f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.72f, -0.31f, 0.833f, 0.037f, -0.41f }, 0.05f);

            //COBA4
            //  m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.1f, -0.095f, -0.577f, 0.934f, -0.477f, -0.021f }, 0.02f);
            //  m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.103f, -0.190f, -0.667f, 1.065f, -0.534f, 0.12f }, 0.02f);
            //Thread.Sleep(1000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.101f, -0.067f, -0.897f, 1.53f, -0.71f, 0.024f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.103f, -0.155f, -0.992f, 1.54f, -0.67f, 0.152f }, 0.02f);
            //Thread.Sleep(1000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.104f, -0.06f, -1.33f, 2.112f, -0.867f, 4.196f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.106f, -0.170f, -1.465f, 2.112f, -0.798f, 0.155f }, 0.02f);
            //Thread.Sleep(1000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { 0.012f, 0.003f, -1.486f, 2.112f, -1.097f, -0.028f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.021f, -0.132f, -1.586f, 2.112f, -1.057f, 0.117f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.33f, 0.08f, -1.58f, -0.27f, -0.38f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.76f, 0.31f, 0.82f, 0.25f, -0.24f }, 0.05f);
            //Thread.Sleep(1000);
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.24f, -0.055f, -1.59f, 2.112f, -1.189f, -0.08f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.271f, 0.05f, -1.657f, 2.112f, -1.186f, 0.066f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.726f, -0.218f, -1.531f, -0.276f, 0.212f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.594f, 0.314f, 0.9f, 0.256f, -0.238f }, 0.05f);
            //COBA5
            //m.setAngles(new List<string>() { "LHipPitch", "RHipPitch"}, new List<float>() { -1.4f, -1.4f }, 0.01f);
            //Thread.Sleep(5000);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.06f, 0.13f, -0.81f, -1.07f, 0.15f }, 0.01f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.61f, 0.01f, 1.08f, 0.17f, 0.63f }, 0.01f);
            //COBA6
            //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll" }, new List<float>() { -0.649f, -0.115f, -0.45f, 0.7f, -0.35f, 0.0f }, 0.02f);
            //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "LAnkleRoll" }, new List<float>() { -0.649f, 0.0f, -4.45f, 0.7f, -0.35f, 0.0f }, 0.02f);
            //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.726f, -0.218f, -1.531f, -0.276f, 0.212f }, 0.05f);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.594f, 0.314f, 0.9f, 0.256f, -0.238f }, 0.05f);

            //COBA7-21/11/2015
            m.setAngles(new List<string>() { "LHipYawpitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "RAnkleRoll"}, new List<float>() {  }, 0.02f);

            Thread.Sleep(1000);
            m.openHand("RHand");

            Thread.Sleep(5000);
            m.closeHand("RHand");
            p.goToPosture("StandInit", 0.5f);
            Thread.Sleep(1000);
}



//ini buat bangun
static void wakeup()
{
  MotionProxy m = new MotionProxy("167.205.66.96", 9559);
  RobotPostureProxy p = new RobotPostureProxy("167.205.66.96", 9559);
  TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.96", 9559);
  m.wakeUp();
  //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "LAnkleRoll" }, new List<float>() { -0.24f, -0.14f, -1.02f, 2.11f, -1.18f, 0.07f }, 0.05f); //badan ke bawah
  //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "RAnkleRoll" }, new List<float>() { -0.24f, 0.08f, -1.09f, 2.11f, -1.18f,-0.07f }, 0.05f);
  //Thread.Sleep(5000);
  //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "LAnkleRoll" }, new List<float>() { -0.24f, -0.08f, -0.66f, 2.11f, -1.18f, 0.07f }, 0.05f); // badan kembali ke rest
  //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "RAnkleRoll" }, new List<float>() { -0.24f, 0.07f, -0.70f, 2.11f, -1.18f, -0.08f }, 0.05f);
  //Thread.Sleep(1000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.41f, -0.08f, 1.51f, 1.07f, -0.05f }, 0.05f); // tangan buka
  m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.98f, 0.38f, -2.05f, -1.28f, 0.60f }, 0.00f);
  //Thread.Sleep(1000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.97f, 0.31f, 0.75f, 1.54f, -0.19f }, 0.05f); // tangan sembah
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.11f, -0.31f, -0.80f, -0.89f, -1.56f }, 0.05f);
  //Thread.Sleep(5000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.29f, 0.17f, 1.60f, 0.95f, -0.08f }, 0.05f); // tangan buka
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.37f, -0.06f, -1.74f, -1.06f, 0.08f }, 0.05f);
  //Thread.Sleep(2000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.30f, 0.22f, 1.03f, 1.54f, -0.19f }, 0.05f); // tangan Segitiga
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.31f, -0.23f, -1.04f, -1.54f, 0.15f }, 0.05f);
  //Thread.Sleep(4000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.06f, 0.31f, 0.89f, 1.22f, -0.10f }, 0.05f); // tangan ke kiri
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.14f, -0.19f, -1.49f, -1.54f, -0.24f }, 0.05f);
  //Thread.Sleep(3000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.03f, 0.31f, 1.55f, 1.32f, -0.22f }, 0.05f); // tangan ke kanan
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.31f, -0.31f, -0.92f, -0.98f, -0.23f }, 0.05f);
  //Thread.Sleep(3000);

  //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "LAnkleRoll" }, new List<float>() { -0.24f, -0.13f, -0.88f, 2.11f, -1.18f, 0.07f }, 0.05f); //badan ke bawah
  //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "RAnkleRoll" }, new List<float>() { -0.24f, 0.09f, -0.94f, 2.11f, -1.18f, -0.07f }, 0.05f);
  //Thread.Sleep(5000);

  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.21f, -0.16f, 1.58f, 0.03f, -0.13f }, 0.1f); // tangan lebar
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.23f, 0.28f, -1.51f, -0.06f, -0.33f }, 0.1f);
  //Thread.Sleep(5000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.15f, 0.09f, 1.36f, 0.06f, 0.19f }, 0.05f); // tangan ambil
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.13f, -0.09f, -1.40f, -0.03f, -0.30f }, 0.05f);
  //m.setWalkArmsEnable(false, false);
  //Thread.Sleep(6000);
  //m.setAngles(new List<string>() { "LHipYawPitch", "LHipRoll", "LHipPitch", "LKneePitch", "LAnklePitch", "LAnkleRoll" }, new List<float>() { -0.24f, -0.08f, -0.66f, 2.11f, -1.18f, 0.07f }, 0.05f); // badan kembali ke rest
  //m.setAngles(new List<string>() { "RHipYawPitch", "RHipRoll", "RHipPitch", "RKneePitch", "RAnklePitch", "RAnkleRoll" }, new List<float>() { -0.24f, 0.07f, -0.70f, 2.11f, -1.18f, -0.08f }, 0.05f);
  //Thread.Sleep(4000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.45f, 0.09f, 1.36f, 1.03f, 0.19f }, 0.1f); // tangan turun
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.45f, -0.09f, -1.40f, -1.11f, -0.30f }, 0.1f);
  //m.setWalkArmsEnable(false, false);
  //Thread.Sleep(5000);

  //p.goToPosture("Stand",0.5f);
  Thread.Sleep(1500);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.21f, -0.16f, 1.58f, 0.03f, -0.13f }, 0.1f); // tangan lebar
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.23f, 0.28f, -1.51f, -0.06f, -0.33f }, 0.1f);
  //Thread.Sleep(2000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.15f, 0.09f, 1.36f, 0.06f, 0.19f }, 0.05f); // tangan ambil
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.13f, -0.09f, -1.40f, -0.03f, -0.30f }, 0.05f);
  //Thread.Sleep(3000);
  //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.45f, 0.09f, 1.36f, 1.03f, 0.19f }, 0.1f); // tangan turun
  //m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.45f, -0.09f, -1.40f, -1.11f, -0.30f }, 0.1f);
  //Thread.Sleep(2500);
  //m.moveTo(0.8f, 0.0f, 0.0f);
  //m.setWalkArmsEnable(false,false);
  //Thread.Sleep(1000);

  /*m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.21f, -0.16f, 1.58f, 0.03f, -0.13f }, 0.1f); // tangan lebar
  m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.23f, 0.28f, -1.51f, -0.06f, -0.33f }, 0.1f);
  Thread.Sleep(5000);
  m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.15f, 0.09f, 1.36f, 0.06f, 0.19f }, 0.05f); // tangan ambil
  m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.13f, -0.09f, -1.40f, -0.03f, -0.30f }, 0.05f);
  m.setWalkArmsEnable(false, false);
  Thread.Sleep(6000);

  m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.33f, 0.09f, 1.36f, 0.06f, 0.19f }, 0.1f); // tangan ambil
  m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.35f, -0.09f, -1.40f, -0.03f, -0.30f }, 0.1f);
  m.setWalkArmsEnable(false,false);
  Thread.Sleep(5000);*/
            //m.rest();

        }
        static void testDada()
        {
            MotionProxy m = new MotionProxy("169.254.89.225", 9559);
            RobotPostureProxy p = new RobotPostureProxy("169.254.89.225", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("169.254.89.225", 9559);
            p.goToPosture("StandInit", 0.5f);
            m.openHand("RHand");
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 0.3f, -0.2f }, 0.2f);
            Thread.Sleep(1000);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 1.2f, -0.2f }, 0.2f);
            Thread.Sleep(1000);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 0.3f, -0.2f }, 0.2f);
            Thread.Sleep(1000);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 1.2f, -0.2f }, 0.2f);
            Thread.Sleep(1000);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 0.3f, -0.2f }, 0.2f);
            Thread.Sleep(1000);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 1.2f, -0.2f }, 0.2f);
            //Thread.Sleep(1000);
            //m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.9f, -0.44f, 0.53f, 0.3f, -0.2f }, 0.2f);
            //Thread.Sleep(1000);
            m.rest();
        }
        static void tangan()
        {
            MotionProxy m = new MotionProxy("169.254.89.225", 9559);
            RobotPostureProxy p = new RobotPostureProxy("169.254.89.225", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("169.254.89.225", 9559);
            p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll","LElbowYaw","LElbowRoll","LWristYaw" }, new List<float>() { 1.1f, -0.2f,-1.5f,-1.5f,-1.4f }, 0.2f);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll","RElbowYaw","RElbowRoll","RWristYaw" }, new List<float>() { 1.1f, 0.1f,1.4f,1.5f,1.4f }, 0.2f);
           // m.openHand("LHand");
            Thread.Sleep(5500);
            kasih();
            m.rest();
        }
        static void kasih()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
            //s.say("Hallo");
           // p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 0.1f, -0.2f, -1.9f, -0.5f, -0.9f }, 0.2f);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.1f, 0.2f, 1.6f, 0.6f, 1.6f }, 0.2f);
            // m.openHand("LHand");
            Thread.Sleep(5500);
            m.rest();
        }
        static void kepala()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
            p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "HeadPitch" }, new List<float>() { -0.36f }, 0.2f);
            
            // m.openHand("LHand");
            Thread.Sleep(2500);
            m.rest();
        }
        static void kepala1()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
            p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { 0.5f }, 0.1f);
            // m.openHand("LHand");
            kepala2();
            Thread.Sleep(1000);
            kepala3();
            Thread.Sleep(1000);
            kepala4();
            Thread.Sleep(1000);
            kepala5();
            Thread.Sleep(1000);
            kepala3();
            Thread.Sleep(1000);
            m.rest();
        }
        static void kepala2()
        {
            
                MotionProxy m = new MotionProxy("167.205.66.76", 9559);
                RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
                TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
                //p.goToPosture("Stand", 0.5f);
                m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { 0.9f }, 0.1f);
                // m.openHand("LHand");
                //Thread.Sleep(2500);
                //m.rest();
            
        }
        static void kepala3()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
            //p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { 0.06f }, 0.1f);
            // m.openHand("LHand");
            //Thread.Sleep(2500);
            //m.rest();
        }
        static void kepala4()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
            //p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { -0.6f }, 0.1f);
            // m.openHand("LHand");
            //Thread.Sleep(2500);
            //m.rest();
        }
        static void kepala5()
        {
            MotionProxy m = new MotionProxy("167.205.66.76", 9559);
            RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("167.205.66.76", 9559);
            //p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { -1.0f }, 0.1f);
            // m.openHand("LHand");
            //Thread.Sleep(2500);
            //m.rest();
        }
        static void testPose()
        {
            MotionProxy m = new MotionProxy("169.254.89.225", 9559);
            RobotPostureProxy p = new RobotPostureProxy("169.254.89.225", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("169.254.89.225", 9559);
            p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.12f, 0.31f, 0.34f, 1.25f, -0.1f }, 0.2f);
            m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.9f, 0.71f, -0.79f, -0.88f, 0.42f }, 0.2f);
            m.openHand("LHand");
            m.rest();
        }
        
        static void testAmbil()
        {
            MotionProxy m = new MotionProxy("169.254.89.225", 9559);
            RobotPostureProxy p = new RobotPostureProxy("169.254.89.225", 9559);
            TextToSpeechProxy s = new TextToSpeechProxy("169.254.89.225", 9559);
            p.goToPosture("Stand", 0.5f);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -0.21f, -0.05f, -0.11f, 0.08f, 1.75f }, 0.2f);
            Thread.Sleep(200);
            //m.openHand("RHand");
            //Thread.Sleep(300);
            //m.closeHand("RHand");
            //Thread.Sleep(5000);
           // m.setWalkArmsEnable(false,false);
            m.rest();
        }
        static void testMusic()
        {
            AudioPlayerProxy audio = new AudioPlayerProxy("169.254.89.225", 9559);
            int id = audio.loadFile("/home/nao/Gangnam Style - PSY(MyMp3Song.Com).mp3");
            Console.WriteLine("start music");
            audio.play(id);
            Console.WriteLine("waiting to stop");
            
            Console.WriteLine("stop music");
            audio.stopAll();
        }
        static void testSing()
        {
            MotionProxy m = new MotionProxy("169.254.89.225", 9559);
            RobotPostureProxy p = new RobotPostureProxy("169.254.89.225", 9559);
            AudioPlayerProxy audio = new AudioPlayerProxy("169.254.89.225", 9559);
            int id = audio.loadFile("/home/nao/manuk.mp3");
            p.goToPosture("Stand", 0.5f);
           
            new Thread(delegate()
            {
                audio.play(id);
            }).Start();
            Thread.Sleep(15000);
            p.goToPosture("StandInit", 0.5f);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 0.19f, 0.05f, 0.54f, 1.54f, 0.16f }, 0.2f);
            for (int i = 0; i < 10; i++)
            {
                m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.93f, 0.01f, -0.73f, -0.47f, 0.26f }, 0.1f);
                Thread.Sleep(1500);
                m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.89f, 0.36f, -0.74f, -0.05f, 0.26f }, 0.1f);
                Thread.Sleep(1500);
            }
          
        }
        static void testDance()
        {
            MotionProxy m = new MotionProxy("169.254.89.225", 9559);
            RobotPostureProxy p = new RobotPostureProxy("169.254.89.225", 9559);
            AudioPlayerProxy audio = new AudioPlayerProxy("169.254.89.225", 9559);
            int id = audio.loadFile("/home/nao/gangnam.mp3");
            Console.WriteLine("start music");
            p.goToPosture("StandInit", 0.5f);
            new Thread(delegate()
            {
                audio.play(id);
            }).Start();
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { 1.28f, 0.18f, 0.71f, 0.71f, -0.7f }, 0.2f);
            m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { -0.72f, 0.04f, -0.67f, -1.06f, 0.97f }, 0.2f);
            for (int i = 0; i < 6; i++)
            {
                m.setAngles(new List<string>() { "HeadPitch", "RShoulderPitch", "LShoulderPitch" }, new List<float>() { 0.0f, 1.28f, -0.72f }, 0.3f);
                Thread.Sleep(500);
                m.setAngles(new List<string>() { "HeadPitch", "RShoulderPitch", "LShoulderPitch" }, new List<float>() { 0.51f, 1.10f, -0.60f }, 0.3f);
                Thread.Sleep(500);
            }
            m.setAngles(new List<string>() { "HeadPitch", "RShoulderPitch", "LShoulderPitch" }, new List<float>() { 0.0f, 1.28f, -0.72f }, 0.3f);
            Thread.Sleep(500);
            m.setAngles(new List<string>() { "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw" }, new List<float>() { -1.28f, -0.22f, -0.13f, 1.1f, 0.2f }, 0.2f);
            m.setAngles(new List<string>() { "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw" }, new List<float>() { 1.28f, -0.31f, -0.46f, -0.59f, 0.62f }, 0.2f);
            for (int i = 0; i < 7; i++)
            {
                m.setAngles(new List<string>() { "HeadYaw", "RShoulderRoll", "LShoulderRoll" }, new List<float>() { 0.0f, -0.22f, -0.31f }, 0.2f);
                Thread.Sleep(500);
                m.setAngles(new List<string>() { "HeadYaw", "RShoulderRoll", "LShoulderRoll" }, new List<float>() { 0.51f, -0.73f, 0.78f }, 0.3f);
                Thread.Sleep(500);
            }
           
        }
        static void testAudioEnergy()
        {
            AudioDeviceProxy audio = new AudioDeviceProxy("169.254.89.225", 9559);
            audio.enableEnergyComputation();
            int i = 0;
            while (true)
            {
                float threshold = 700.0f;
               
                float a = audio.getFrontMicEnergy();
                if (a > threshold)
                {
                    Console.WriteLine("voice detected {0}",i);
                    i++;
                }

            }
        }
        static void testRecording()
        {
            AudioRecorderProxy audio = new AudioRecorderProxy("169.254.89.225", 9559);
            audio.startMicrophonesRecording("/home/nao/recordings/microphones/recording.wav", "wav", 16000, new List<int> { 0, 0, 1, 0 });
            Console.WriteLine("start recording");
            Thread.Sleep(5000);
            audio.stopMicrophonesRecording();
            Console.WriteLine("stop recording");
            PasswordAuthenticationMethod password = new PasswordAuthenticationMethod("nao", "nao");
            keyboard.AuthenticationPrompt+=new EventHandler<Renci.SshNet.Common.AuthenticationPromptEventArgs>(keyboard_AuthenticationPrompt);
            ConnectionInfo info = new ConnectionInfo("169.254.89.225", 22, "nao", new AuthenticationMethod[] { keyboard});
            SftpClient client = new SftpClient(info);
            try
            {
                client.Connect();
                var files = client.ListDirectory(client.WorkingDirectory+"/recordings/microphones/");
                foreach (var file in files)
                {
                    if (file.Name == "recording.wav")
                    {
                        Console.WriteLine("begin");
                        Stream fileName = File.OpenWrite(@"D:/recordingNAO.wav");
                        client.DownloadFile(client.WorkingDirectory + "/recordings/microphones/recording.wav", fileName, null);
                        Console.WriteLine("finish");
                        fileName.Close();
                    }
                }
                client.Disconnect();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.Read();
        }
        static private void keyboard_AuthenticationPrompt(object sender, Renci.SshNet.Common.AuthenticationPromptEventArgs e)
        {
            foreach (var prompt in e.Prompts)
            {
                if (prompt.Request.Equals("Password: ", StringComparison.InvariantCultureIgnoreCase))
                {
                    prompt.Response = "nao";
                }
            }
        }
        static void testSourceLocalization()
        {
            AudioSourceLocalizationProxy source = new AudioSourceLocalizationProxy(ip, port);
            MemoryProxy m = new MemoryProxy(ip, port);
            
        }
        static void testSpeaker()
        {
            MemoryStream ms = new MemoryStream(File.ReadAllBytes(@"D:\M1F1-Alaw-AFsp.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("channel : " + file.WaveFormat.BitsPerSample);
        }
        
        static void tesmotion()
        {
            Console.WriteLine("mulai");
            MotionProxy tesmotion = new MotionProxy("167.205.56.142", 9559);
            List<float> angle = tesmotion.getAngles("Body", false);
            ArrayList angles = new ArrayList();
            angles.Add(0.5f);
            angles.Add(0.5f);
            ArrayList nama = new ArrayList();
            nama.Add("LShoulderPitch");
            nama.Add("RShoulderPitch");
            tesmotion.wakeUp();
            tesmotion.setAngles(nama, angles, 0.4f);
            for (int i = 0; i < angle.Count; i++)
            {
                Console.WriteLine("angle : " + angle[i]);
            }
            tesmotion.rest();
            Console.WriteLine("selesai");
            Console.ReadKey();
        }
        static void testAudio4()
        {
            Stream ms = new MemoryStream(File.ReadAllBytes(@"D:\wav\aaaa.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            int nbOfChannels = file.WaveFormat.Channels;
            int sampleRate = file.WaveFormat.SampleRate;
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("bitpersample : " + file.WaveFormat.BitsPerSample);
            Console.WriteLine("sample rate : " + file.WaveFormat.SampleRate);
            Console.WriteLine("lenght : " + file.SampleCount);
            int outputBufferSize = 16384;
            int numberOfOutputChannels = 2;


            byte[] buffer = new byte[2 * (int)file.SampleCount];
            int byteRead = file.Read(buffer, 0, 2 * (int)file.SampleCount);
            int nbOfFrames = byteRead / 2;
            Console.WriteLine("byte Read : " + byteRead);
            Console.WriteLine("frame Read : " + nbOfFrames);

            short[] fInputAudioData = new short[(int)file.SampleCount];
            short[] fStereoAudioData = new short[2* (int)file.SampleCount];
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

            ArrayList output = new ArrayList(fStereoAudioData);
            try
            {
                AudioDeviceProxy audio = new AudioDeviceProxy(ip, port);
                audio.setParameter("outputSampleRate", sampleRate);
                audio.sendRemoteBufferToOutput(nbOfFrames, fStereoAudioData);
                Console.WriteLine("finish");
            }
            catch (Exception e)
            {
                Console.WriteLine("error : " + e.Message);
            }


            Console.ReadKey();
        }
        static void testAudio3()
        {
            Stream ms = new MemoryStream(File.ReadAllBytes(@"D:\wav\aaaa.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            int nbOfChannels = file.WaveFormat.Channels;
            int sampleRate = file.WaveFormat.SampleRate;
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("bitpersample : " + file.WaveFormat.BitsPerSample);
            Console.WriteLine("sample rate : " + file.WaveFormat.SampleRate);
            Console.WriteLine("lenght : " + file.SampleCount);
            
            int outputBufferSize = 16384;
            int numberOfOutputChannels = 2;
            byte[] firstBuffer = new byte[2*file.SampleCount];
            int byteRead = file.Read(firstBuffer, 0, 2 * (int)file.SampleCount);
            Console.WriteLine("byteRead : " + byteRead);
            int bufferMaxSize = 2*nbOfChannels*outputBufferSize;
            byte[] buffer1 = new byte[bufferMaxSize];
            byte[] buffer2 = new byte[bufferMaxSize];
            for (int i = 0; i < bufferMaxSize; i++)
            {
                buffer1[i] = firstBuffer[i];
            }

            short[] fInputAudioData1 = new short[nbOfChannels * outputBufferSize];
            short[] fStereoAudioData1 = new short[outputBufferSize * numberOfOutputChannels];
            short[] fInputAudioData2 = new short[nbOfChannels * outputBufferSize];
            short[] fStereoAudioData2 = new short[outputBufferSize * numberOfOutputChannels];
            int sample = 0;
            for (int index = 0; index < buffer1.Length/2; index++)
            {
                fInputAudioData1[index] = BitConverter.ToInt16(buffer1, sample);
                sample += 2;
            }
            sample = 0;
            for (int index = 0; index < buffer2.Length / 2; index++)
            {
                fInputAudioData2[index] = BitConverter.ToInt16(buffer2, sample);
                sample += 2;
            }
            if (nbOfChannels == 1)
            {
                int i = 0;
                for (int j = 0; j < buffer1.Length/2; j++)
                {
                    fStereoAudioData1[i] = fInputAudioData1[j];
                    fStereoAudioData1[i + 1] = fInputAudioData1[j];
                    i += numberOfOutputChannels;
                }
                i = 0;
                for (int j = 0; j < buffer2.Length / 2; j++)
                {
                    fStereoAudioData2[i] = fInputAudioData2[j];
                    fStereoAudioData2[i + 1] = fInputAudioData2[j];
                    i += numberOfOutputChannels;
                }
            }
            else if (nbOfChannels == 2)
            {
                for (int i = 0; i < buffer1.Length / 2; i++)
                {
                    fStereoAudioData1[i] = fInputAudioData1[i];
                }
                for (int i = 0; i < buffer2.Length / 2; i++)
                {
                    fStereoAudioData2[i] = fInputAudioData2[i];
                }
            }
            try
            {
                AudioDeviceProxy audio = new AudioDeviceProxy(ip, port);
                audio.setParameter("outputSampleRate", sampleRate);
                audio.sendRemoteBufferToOutput(buffer2.Length/2, fStereoAudioData2);
                Console.WriteLine("finish 1");
                audio.sendRemoteBufferToOutput(buffer1.Length / 2, fStereoAudioData1);
                Console.WriteLine("finish 2");
            }
            catch (Exception e)
            {
                Console.WriteLine("error : " + e.Message);
            }


            Console.ReadKey();
        }
        static void testAudio2()
        {
            Stream ms = new MemoryStream(File.ReadAllBytes(@"D:\wav\aaaa.wav"));
            Console.WriteLine("succesfully open file ");
            WaveFileReader file = new WaveFileReader(ms);
            int nbOfChannels = file.WaveFormat.Channels;
            int sampleRate = file.WaveFormat.SampleRate;
            Console.WriteLine("channel : " + file.WaveFormat.Channels);
            Console.WriteLine("bitpersample : " + file.WaveFormat.BitsPerSample);
            Console.WriteLine("sample rate : " + file.WaveFormat.SampleRate);
            Console.WriteLine("lenght : " + file.SampleCount);
            int outputBufferSize = 16384;
            int numberOfOutputChannels = 2;
            

            byte[] buffer = new byte[2*nbOfChannels*outputBufferSize];
            int byteRead = file.Read(buffer, 0, 2 * nbOfChannels * outputBufferSize);
            int nbOfFrames = byteRead / 2;
            Console.WriteLine("byte Read : " + byteRead);
            Console.WriteLine("frame Read : " + nbOfFrames);
            
            short[] fInputAudioData = new short[nbOfChannels * outputBufferSize];
            short[] fStereoAudioData = new short[outputBufferSize * numberOfOutputChannels];
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
                for (int i = 0; i < byteRead; i++)
                {
                    fStereoAudioData[i] = fInputAudioData[i];
                }
            }

            ArrayList output = new ArrayList(fStereoAudioData);

            try
            {
                AudioDeviceProxy audio = new AudioDeviceProxy(ip, port);
                audio.setParameter("outputSampleRate", sampleRate);
                audio.sendRemoteBufferToOutput(nbOfFrames, fStereoAudioData);
                Console.WriteLine("finish");
            }
            catch(Exception e)
            {
                Console.WriteLine("error : "+e.Message);
            }


            Console.ReadKey();

        }
        //static void testAudio()
        //{
        //    var stream = new MemoryStream(File.ReadAllBytes(@"D:\hello.wav"));
        //    Console.WriteLine("1");
        //    WaveFileReader a = new WaveFileReader(stream);
        //    int channel = a.WaveFormat.Channels;
        //    int rate = a.WaveFormat.SampleRate;
        //    int bss = a.WaveFormat.BitsPerSample;
        //    int max = 16384;
        //    int outputChannel = 2;
        //    AudioDeviceProxy audio = new AudioDeviceProxy("167.205.56.142", 9559);
        //    //audio.setParameter("outputSampleRate", rate);
        //    byte[] inputData = new byte[max * channel];
        //    byte[] stereo = new byte[max * outputChannel];
        //    short[] output = new short[max * outputChannel];
        //    Console.WriteLine("channel : " + channel);
        //    Console.WriteLine("size : " + rate);
        //    Console.WriteLine("count : " + bss);
        //    a.Read(inputData, 0, max);
        //    output = Array.ConvertAll(inputData, (b) => (short)b);

        //    try
        //    {
        //        audio.sendRemoteBufferToOutput(max * outputChannel, output);
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("error : " + e.ToString());
        //    }
        //    Console.WriteLine("5");
        //    Console.ReadKey();
        //}
        //static void testMemory()
        //{
        //    MemoryProxy m = new MemoryProxy(ip, port);
        //    Console.WriteLine(m.getData("BatteryChargeChanged"));
        //    Console.WriteLine(m.getData("SonarLeftDetected"));
        //    BatteryProxy b = new BatteryProxy(ip, port);
        //    int persen = b.getBatteryCharge();
        //    Console.WriteLine(persen);
        //    Console.WriteLine(b.getMethodHelp("getBatteryCharge"));
        //    Console.ReadKey();
        //}
        static void testSonar()
        {
            SonarProxy s = new SonarProxy("169.254.89.225", 9559);
            MemoryProxy m = new MemoryProxy("169.254.89.225", 9559);
            s.subscribe("test");
            while (true)
            {
                Console.WriteLine(m.getData("Device/SubDeviceList/US/Right/Sensor/Value"));
                delay(100);
            }
        }
        //static void testTactile()
        //{
        //    SensorsProxy sensor = new SensorsProxy(ip, port);
        //    MemoryProxy m = new MemoryProxy(ip, port);
        //    sensor.subscribe("test");
        //    sensor.run();
        //    List<string> output = sensor.getOutputNames();
        //    foreach (string a in output)
        //    {
        //        Console.WriteLine(a);
        //    }
            
        //    object  b = m.getData("FrontTactilTouched");
        //    Console.WriteLine(b.GetType());
        //    Console.ReadKey();

        //}
        //static void testSearching()
        //{
        //    string[] number = { "1", "2", "3", "4", "5", "6" };
        //    string coba = "7";
        //    int i = 0;
        //    bool flag = false;
        //    while ((!flag) && (i < number.Length))
        //    {
        //        Console.WriteLine("putar");
        //        if (coba == number[i])
        //        {
        //            flag = true;
                    
        //        }
        //        else
        //        {
        //            i++;
        //        }
        //    }

        //    if (flag)
        //    {
        //        Console.WriteLine("ada");
        //    }
        //    else
        //    {
        //        Console.WriteLine("nggak ada");
        //    }
        //    Console.ReadKey();
        //}
        //static void testCamera2()
        //{
        //    VideoDeviceProxy video = new VideoDeviceProxy(ip, port);
        //    string id = "rec1";
        //    int res1 = 1;
        //    int colorspace = 11;
        //    int fps = 15;
        //    ArrayList data = new ArrayList();
        //    try
        //    {
        //        Console.WriteLine("subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("unable to subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }

        //    try
        //    {
        //        while (true)
        //        {
        //            s.Reset();
        //            s.Start();
        //            data = (ArrayList)video.getImageRemote(id);
        //            s.Stop();
        //            video.releaseImage(id);
        //            Console.WriteLine(s.ElapsedMilliseconds);
        //        }
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("error retrive data");
        //    }
        //}
        //static void testMotion()
        //{
        //    List<float> angle;
        //    try
        //    {
        //        MotionProxy motion = new MotionProxy(ip, port);
        //        angle = motion.getAngles("LLeg", false);
        //        for (int i = 0; i < angle.Count; i++)
        //        {
        //            Console.WriteLine(angle[i].ToString());
        //        }
        //        Console.ReadKey();
        //    }
        //    catch
        //    {
        //        Console.WriteLine("error");
        //        Console.ReadKey();
        //    }
        //}
        //static void testCamera()
        //{
        //    VideoDeviceProxy video = new VideoDeviceProxy(ip, port);
        //    string id = "rec1";
        //    int res1 = 0;
        //    int colorspace = 3;
        //    int fps = 15;
        //    ArrayList data = new ArrayList();
        //    try
        //    {
        //        Console.WriteLine("subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("unable to subscribe");
        //        video.subscribe(id, res1, colorspace, fps);
        //    }

        //    try
        //    {
        //        while (true)
        //        {
        //            s.Reset();
        //            s.Start();
        //            data = (ArrayList)video.getImageRemote(id);
        //            s.Stop();
        //            video.releaseImage(id);
        //            Console.WriteLine(s.ElapsedMilliseconds);
        //        }
        //    }
        //    catch
        //    {
        //        video.unsubscribe(id);
        //        Console.WriteLine("error retrive data");
        //    }
        //}
        //static void testRecord()
        //{
        //    AudioRecorderProxy record = new AudioRecorderProxy(ip, port);
        //    ArrayList channels = new ArrayList();
        //    channels.Add(0);
        //    channels.Add(1);
        //    channels.Add(0);
        //    channels.Add(0);
        //    try
        //    {
        //        Console.WriteLine("start recording");
        //        s.Start();
        //        record.startMicrophonesRecording("/home/nao/test.wav", "wav", 16000, channels);
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //    }
        //    Console.ReadKey();
        //    record.stopMicrophonesRecording();
        //    s.Stop();
        //    Console.WriteLine("stop recording "+s.ElapsedMilliseconds.ToString());
        //}
        //static void playRecord()
        //{
        //    try
        //    {
        //        AudioPlayerProxy play = new AudioPlayerProxy(ip, port);
        //        int id = play.loadFile("/home/nao/test.wav");
        //        Console.WriteLine("load file "+id.ToString());
        //        Console.ReadKey();
        //        Console.WriteLine("start playing");
        //        s.Start();
        //        play.setVolume(id, 1.0f);
        //        play.play(id);
        //        s.Stop();
        //        Console.WriteLine("finish playing "+s.ElapsedMilliseconds.ToString());
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //        Console.ReadKey();
        //    }
        //}
        //static void testEnergy()
        //{
        //    AudioDeviceProxy device = new AudioDeviceProxy(ip, port);
        //    device.enableEnergyComputation();
        //    float a, b, c, d;
        //    while (true)
        //    {
        //        a = device.getFrontMicEnergy();
        //        b = device.getLeftMicEnergy();
        //        c = device.getRightMicEnergy();
        //        d = device.getRearMicEnergy();
        //        Console.WriteLine("{0}   {1}   {2}   {3}", a, b, c, d);
        //    }
        //}
        static void delay(int time)
        {
            Stopwatch s = new Stopwatch();
            s.Reset();
            s.Start();
            while (s.ElapsedMilliseconds < time)
            {
            }
            s.Stop();
        }
    }
}
