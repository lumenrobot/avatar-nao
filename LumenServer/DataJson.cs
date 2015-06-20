using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

//this file contain JSON defenition of NAO data
namespace LumenServer
{
    class RecordingData
    {
        [JsonProperty("name")]
        public String name { get; set; }
        [JsonProperty("content")]
        public String content { get; set; }
    }
    class ImageObject
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
        [JsonProperty("number")]
        public int number { get; set; }
        
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
}
