using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aldebaran.Proxies;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;

namespace naoTest
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        public void Run()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = "amqp://lumen:lumen@167.205.66.35/%2F";

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            string routingKey = "lumen.visual.hogobj.recognition";
            var queue = channel.QueueDeclare("", false, true, true, null); 
            channel.QueueBind(queue.QueueName, "amq.topic", routingKey, null);

            var obejectConsumer = new EventingBasicConsumer(channel);
            obejectConsumer.Received += ObjectConsumer_Received;
            String consumerTag = channel.BasicConsume(queue.QueueName, false, obejectConsumer);
        }

        private void ObjectConsumer_Received(IBasicConsumer sender, BasicDeliverEventArgs args)
        {
            Console.WriteLine("data Received");
            string body = Encoding.UTF8.GetString(args.Body);
            JsonSerializerSettings setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };
            RecognizedObjects objk = JsonConvert.DeserializeObject<RecognizedObjects>(body, setting);
            Console.WriteLine("Nilai dari Modul visual = {0}",objk.trashes[0].getX(), objk.trashes[0].getY()); //
            
            //MotionProxy m = new MotionProxy("167.205.66.69", 9559);
            //RobotPostureProxy p = new RobotPostureProxy("167.205.66.69", 9559);
            p.goToPosture("Stand", 0.5f);
            float dataX = objk.trashes[0].getX();
            float dataY = objk.trashes[0].getY();
            float x_angle=0;
            float y_angle = 0;
            bool right = true;//
            bool left = false;
            bool found = false;
            do
            {
                if (dataX != null)
                {
                    found = true;
                }
                if (right = true)
                {
                    x_angle += (float)0.532063622;
                }
                else if (left = true)
                {
                    x_angle -= (float)0.532063622;
                }
                if (x_angle == 1.596190866)
                {
                    right = false;
                    left = true;
                }
                else if (x_angle == -1.596190866)
                {
                    right = true;
                    left = false;
                }
            } while (!found);
            if (dataX >= 0 && dataX <= 159)
            {
                x_angle = (float)((160 - dataX) * 0.00083135);
                m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { x_angle }, 0.1f);
            }
            else if (dataX >= 160 && dataX <= 319)
            {
                x_angle = (float)((dataX - 159) * 0.00083135);
                m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { x_angle }, 0.1f);
            }
            y_angle = (float)((dataY - 119) * 0.00086612);
            m.setAngles(new List<string>() { "HeadPitch" }, new List<float>() { y_angle }, 0.1f);
            m.moveTo(0.0f, 0.0f, x_angle);
        }

            //  float nilai = float.Parse(objk.ToString());
    }

        //static void scanning()
        //{
        //      string input;
        //    float x_angle= (float)-1.5708;
        //    float y_angle;
        //    float x_angle2;
        //    float y_angle2;
        //    float kX.x;
        //    float kY.y;
        //    bool xy = true;
        //    xy = (kX.x == null && kY.y == null);
        //    MotionProxy m = new MotionProxy("167.205.66.76", 9559);
        //    RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);

        //    p.goToPosture("Stand", 0.5f);
        //    m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { x_angle }, 0.1f);
        //    do
        //    {
        //        x_angle = (float)(x_angle + 0.532063622);
        //    } while (xy || x_angle != 1.596190866);
        //    if (float x_angle = 1.596190866)
        //    {
        //        do
        //        {
        //            x_angle = (float)(x_angle - 0.532063622);
        //        } while (xy || x_angle != -1.596190866);
        //    }
            //do
            //{
            //m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { (float)1.5708 }, 0.1f);
            //x_angle = x_angle - 0.532063622;
           //}while (xy || x_angle = 1.578);
            //if (kX.x >= 0 && kX.x <= 159)
            //{
            //    x_angle = (float)((160 - kX.x) * 0.00083135);
            //    m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { x_angle }, 0.1f);
            //}
            //else if (kX.x >= 160 && kX.x <= 319)
            //{
            //    x_angle = (float)((kX.x - 159) * 0.00083135);
            //    m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { x_angle }, 0.1f);
            //}
            //y_angle = (float)((kY.y - 119) * 0.00086612);
            //m.setAngles(new List<string>() { "HeadPitch" }, new List<float>() { y_angle }, 0.1f);
            //m.moveTo(0.0f, 0.0f, x_angle);
            // m.moveTo(x_distance, 0.0f, 0.0f);
            //grab
}

        // // // ////m.moveTo(x_distance, 0.0f, 0.0f);
        ////static void scanning()
        ////{
        ////    string input;
        ////    float x_angle1;
        ////    float y_angle1;
        ////    float x_angle2;
        ////    float y_angle2;
        ////    float kX.x;
        ////    float kY.y;
        ////    bool xy = true;
        ////    xy = (kX.x == null && kY.y == null);
        ////    MotionProxy m = new MotionProxy("167.205.66.76", 9559);
        ////    RobotPostureProxy p = new RobotPostureProxy("167.205.66.76", 9559);

        ////    p.goToPosture("Stand", 0.5f);

        ///    do
        ////    {
        ////        m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { (float)-1.5708 }, 0.1f);
        ////        x_angle = x_angle + 0.532063622;
        ////    } while (xy && x_angle != 1.578);
        ////    do
        ////    {
        ////        m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { (float)1.5708 }, 0.1f);
        ////        x_angle = x_angle - 0.532063622;
        ////    } while (xy || x_angle1 = 1.578);
        ////    if (kX.x >= 0 && kX.x <= 159)
        ////    {
        ////        x_angle = (float)((160 - kX.x) * 0.00083135);
        ////        m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { x_angle }, 0.1f);
        ////    }
        ////    else if (kX.x >= 160 && kX.x <= 319)
        ////    {
        ////        x_angle = (float)((kX.x - 159) * 0.00083135);
        ////        m.setAngles(new List<string>() { "HeadYaw" }, new List<float>() { x_angle }, 0.1f);
        ////    }
        ////    y_angle = (float)((kY.y - 119) * 0.00086612);
        ////    m.setAngles(new List<string>() { "HeadPitch" }, new List<float>() { y_angle }, 0.1f);
        ////    m.moveTo(0.0f, 0.0f, x_angle);
        ////   // m.moveTo(x_distance, 0.0f, 0.0f);
        ////    //grab
        ////}

        ////m.moveTo(x_distance, 0.0f, 0.0f);
        /*private void con_obejctRecognitionReceived(object sender, HOGObject koorX, HOGObject koorY)
        {
            try
            {
                Console.WriteLine("Berhasil {0} : {1}", koorX.x, koorY.y);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }*/

