using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LumenBirdView
{

	// actually we don't use this class
	class LumenInfrastructureClass
	{
	}

	// -------------------------------------------------------------
	// ambil dan implementasikan jenis-jenis data pada lumen

	// lumen object
	public class ImageObject
	{
		[JsonProperty("name")]
		public String Name { get; set; }
		[JsonProperty("contentType")]
		public String ContentType { get; set; }
		[JsonProperty("contentSize")]
		public long ContentSize { get; set; }
		[JsonProperty("uploadDate")]
		public String UploadDate { get; set; }
		[JsonProperty("dateCreated")]
		public String DateCreated { get; set; }
		[JsonProperty("dateModified")]
		public String DateModified { get; set; }
		[JsonProperty("datePublished")]
		public String DatePublished { get; set; }
		[JsonProperty("contentUrl")]
		public string ContentUrl { get; set; }
		public ImageObject()
		{
			//this.Name = "from_nao.jpg";
			this.ContentType = "image/jpeg";
			this.UploadDate = DateTime.Now.Date.ToString();
			this.DateCreated = DateTime.Now.Date.ToString();
			this.DateModified = DateTime.Now.Date.ToString();
			this.DatePublished = DateTime.Now.Date.ToString();
		}
		public override string ToString()
		{
			return Name + " (" + ContentSize + ") " + ContentUrl;
		}
	}

	// -----------------------------------------------------------------------------

	class RecognizedObjects
	{
		[JsonProperty("@type")]
		public string type;

		[JsonProperty("hasPosition")]
		public bool hasPosition;
		[JsonProperty("hasDistance")]
		public bool hasDistance;
		[JsonProperty("hasYaw")]
		public bool hasYaw;

		[JsonProperty("trashes")]
		public List<RecognizedObject> trashes;
		[JsonProperty("trashCans")]
		public List<RecognizedObject> trashCans;

		public RecognizedObjects()
		{
			type = "RecognizedObjects";

			hasPosition = false;
			hasDistance = false;
			hasYaw = false;

			trashes = new List<RecognizedObject>();
			trashCans = new List<RecognizedObject>();

			// etc, dummy
			/*
			trashes.Add(new RecognizedObject());
			trashes.Add(new RecognizedObject());
			trashCans.Add(new RecognizedObject());
			*/
		}
	};

	class topPosition
	{
		[JsonProperty("@type")]
		public string type;
		[JsonProperty("x")]
		public float x;
		[JsonProperty("y")]
		public float y;

		public topPosition()
		{
			type = "Vector2";
			x = 0;
			y = 0;
		}
	};
	class bottomPosition
	{
		[JsonProperty("@type")]
		public string type;
		[JsonProperty("x")]
		public float x;
		[JsonProperty("y")]
		public float y;

		public bottomPosition()
		{
			type = "Vector2";
			x = 0;
			y = 0;
		}
	};

	class RecognizedObject
	{
		[JsonProperty("@type")]
		public string type;

		[JsonProperty("topPosition")]
		public topPosition topPos; // { own class }
		[JsonProperty("topDistance")]
		public float topDistance;
		[JsonProperty("topYawAngle")]
		public float topYawAngle;

		[JsonProperty("bottomPosition")]
		public bottomPosition bottomPos; // { own class }
		[JsonProperty("bottomDistance")]
		public float bottomDistance;
		[JsonProperty("bottomYawAngle")]
		public float bottomYawAngle;

		// constructor
		public RecognizedObject()
		{
			type = "RecognizedObject";

			topPos = null;
			bottomPos = null;
			topDistance = 0.0F;
			topYawAngle = 0.0F;
			bottomDistance = 0.0F;
			bottomYawAngle = 0.0F;
		}
	}

}
