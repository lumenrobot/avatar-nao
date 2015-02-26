using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Client
{
    class DataJson
    {
    }
    class sound
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("content")]
        public string content { get; set; }

    }
    class recognizer
    {
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("result")]
        public string result { get; set; }
        [JsonProperty("date")]
        public string date { get; set; }
        
    }
    class JointData
    {
        [JsonProperty("name")]
        public string Names { get; set; }
        [JsonProperty("angle")]
        public float Angles { get; set; }
        [JsonProperty("stiffness")]
        public float Stiffnesses { get; set; }

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

    public class Command
    {
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Method")]
        public string Method { get; set; }
        [JsonProperty("Parameter")]
        public object Parameter { get; set; }

        public Command(string type, string method, object parameter)
        {
            this.Type = type;
            this.Method = method;
            this.Parameter = parameter;
        }
    }
   

   
}
