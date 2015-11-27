using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using RabbitMQ;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing;
using RabbitMQ.Util;
//using RabbitMQ.ServiceModel;

using Newtonsoft.Json;

namespace LumenBirdView
{
	class RabbitClass
	{

		public ConnectionFactory factory;
		public IConnection conn = null;
		public IModel model;
		public QueueingBasicConsumer consumer;
		public string server; // = txtServer.Text;
		public int port; // = int.Parse(txtPort.Text);
		public string user;
		public string pass;
		//public string vhost;
		public string routingkey; // = "avatar.nao1.camera.bottom"; // kamera bawah "bottom" -- kamera atas "main"
		public string strexchange; // = "amq.topic";
		public string qname; // defined by server

		// constructor
		public RabbitClass()
		{
			factory = null;
			conn = null;
			model = null;
			consumer = null;
			server = "localhost";	// expected to be changed by user
			port = 5672;
			user = "lumen";
			pass = "lumen";
			//vhost = "%2F";
			routingkey = "";    // expected to be changed by user
			strexchange = "amq.topic";
		}

		public byte[] getRaw() // NON-BLOCKING + Without JSON conversion!
		{
			// default if null
			BasicDeliverEventArgs kosong = new BasicDeliverEventArgs();

			// get from queue
			BasicDeliverEventArgs data;
			data = consumer.Queue.DequeueNoWait(kosong);
			if (data == kosong)
			{
				return null;
			}
			byte[] dataByte = data.Body;

			//ack
			model.BasicAck(data.DeliveryTag, false);

			// define what to return
			if (data != kosong)
			{
				return dataByte;
			}
			else
			{
				return null;
			}
		}

		public RecognizedObjects getData() // NON BLOCKING FUNCTION
		{
			// default if null
			BasicDeliverEventArgs kosong = new BasicDeliverEventArgs();

			// get from queue
			BasicDeliverEventArgs data = consumer.Queue.DequeueNoWait(kosong);
			byte[] dataByte = data.Body;
			if(dataByte == null || data == kosong)
			{
				return null;
			}
			string strdata = Encoding.UTF8.GetString(dataByte);
			JsonSerializerSettings setup = new JsonSerializerSettings() // allow c# class to be exposed
			{
				TypeNameHandling = TypeNameHandling.Objects
			};
			RecognizedObjects ret = JsonConvert.DeserializeObject<RecognizedObjects>(strdata, setup);

			//ack
			model.BasicAck(data.DeliveryTag, false);

			// define what to return
			if (data != kosong) {
				return ret;
			} else {
				return null;
			}
		}

		public RecognizedObjects getDataBlocking() // NON BLOCKING FUNCTION
		{
			// default if null
			BasicDeliverEventArgs kosong = new BasicDeliverEventArgs();

			// get from queue
			// blocking function .Dequeue()
			BasicDeliverEventArgs data = consumer.Queue.Dequeue();
			byte[] dataByte = data.Body;
			string strdata = Encoding.UTF8.GetString(dataByte);
			JsonSerializerSettings setup = new JsonSerializerSettings() // allow c# class to be exposed
			{
				TypeNameHandling = TypeNameHandling.Objects
			};
			RecognizedObjects ret = JsonConvert.DeserializeObject< RecognizedObjects>(strdata, setup);

			//ack
			model.BasicAck(data.DeliveryTag, false);

			// define what to return
			return ret;
		}

		public void sendData(object obj)
		{
			// convert via Json (serialized)
			string strobj = JsonConvert.SerializeObject(obj);
			byte[] objByte = Encoding.UTF8.GetBytes(strobj);
			// send via rabbitmq
			model.BasicPublish(strexchange, routingkey, null, objByte);
		}

		public void connect()
		{
			// connect
			factory = new ConnectionFactory()
			{
				HostName = server,
				Port = port,
				UserName = user, //"guest",
				Password = pass, //"guest",
				//VirtualHost = "%2F" //vhost,
			};
			//factory.Uri = "amqp://lumen:lumen@" + "167.205.66.35" + "/%2F"; // <-- last known work URI
			conn = factory.CreateConnection();
			model = conn.CreateModel();

			// Buat queue di rabbitmq server untuk aplikasi ini.
			// note name "" adalah auto-allocate name dari server
			qname = model.QueueDeclare("", false, true, true, null).QueueName;

			// dan ini penting, untuk menghubungkan exchangre ke queue
			model.QueueBind(qname, strexchange, routingkey);

			// prepare listener
			consumer = new QueueingBasicConsumer(model);
			model.BasicConsume(qname, false, consumer);
		}

		public void close()
		{
			model.Dispose();
			conn.Dispose();
		}

	}
}
