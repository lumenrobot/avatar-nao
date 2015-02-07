using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace LumenServer
{
    class DataJson
    {
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
        public ImageObject( long contentSize, string contentUrl)
        {
            this.Name = "wajah1_240.jpg";
            this.ContentSize = contentSize;
            this.ContentType = "image/jpeg";
            this.ContentUrl = contentUrl;
            this.UploadDate = DateTime.Now.ToString();
            this.DateCreated = DateTime.Now.ToString();
            this.DateModified = DateTime.Now.ToString();
            this.DatePublished = DateTime.Now.ToString();
        }
        public override string ToString()
        {
            return Name + " (" + ContentSize + ") " + ContentUrl;
        }
    }
    public class JointData 
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("angle")]
        public float Angle { get; set; }
        [JsonProperty("stiffness")]
        public float Stiffness { get; set; }

        public JointData(string name, float angle,float stiffness)
        {
            this.Name = name;
            this.Angle = angle;
            this.Stiffness = stiffness;
        }

        public override string ToString()
        {
            return Name + " " + Angle + " " + Stiffness;
        }
        
    }
    public class BatteryData 
    {
        [JsonProperty("percentage")]
        public int percentage { get; set; }
        [JsonProperty("isPlugged")]
        public bool isPlugged { get; set; }
        [JsonProperty("isCharging")]
        public bool isCharging { get; set; }

        public BatteryData(int Percentage, bool IsPlugged, bool IsCharging)
        {
            this.percentage = Percentage;
            this.isPlugged = IsPlugged;
            this.isCharging = isCharging;
        }
    }
    public class SonarData
    {
        [JsonProperty("rightSensor")]
        public float rightSensor { get; set; }
        [JsonProperty("leftSensor")]
        public float leftSensor { get; set; }

        public SonarData(float RightSensor, float LeftSensor)
        {
            this.rightSensor = RightSensor;
            this.leftSensor = LeftSensor;
        }
    }
    public class TactileData
    {
        [JsonProperty("value")]
        public float value { get; set; }
        public TactileData(float Value)
        {
            this.value = Value;
        }
    }
}
