using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace naoTest
{
    public class recognizer
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("result")]
        public string result { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
    }

    public class sound
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("content")]
        public string content { get; set; }
    }

    public class textData
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
    }

    public class soundResult
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("sound")]
        public string result { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
    }

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
    public class JointData
    {
        [JsonProperty("name")]
        public List<string> Names { get; set; }
        [JsonProperty("angle")]
        public List<float> Angles { get; set; }
        [JsonProperty("stiffness")]
        public List<float> Stiffnesses { get; set; }

    }
    public class BatteryData
    {
        [JsonProperty("percentage")]
        public int Percentage { get; set; }
        [JsonProperty("isPlugged")]
        public bool IsPlugged { get; set; }
        [JsonProperty("isCharging")]
        public bool IsCharging { get; set; }

    }
    public class SonarData
    {
        [JsonProperty("rightSensor")]
        public float RightSensor { get; set; }
        [JsonProperty("leftSensor")]
        public float LeftSensor { get; set; }

    }
    public class TactileData
    {
        [JsonProperty("name")]
        public List<string> Names { get; set; }
        [JsonProperty("value")]
        public List<float> Values { get; set; }
    }
    public class FaceLocation
    {
        [JsonProperty("x")]
        public int x { get; set; }
        [JsonProperty("y")]
        public int y { get; set; }
    }
    public class HandGesture
    {
        [JsonProperty("gesture")]
        public string gesture { get; set; }
    }
    public class Object
    {
        [JsonProperty("objectsurf")]
        public string objectsurf { get; set; }
    }
    public class UpperBodyLocation
    {
        [JsonProperty("x")]
        public int x { get; set; }
        [JsonProperty("y")]
        public int y { get; set; }
    }
    public class HOGObject
    {
        [JsonProperty("x")]
        public float x { get; set; }
        [JsonProperty("y")]
        public float y { get; set; }
    }
    public class genderResult
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("gender")]
        public string gender { get; set; }
    }
    public class FaceName
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class TextName2
    {
        [JsonProperty("text2")]
        public string text2 { get; set; }
    }
}
