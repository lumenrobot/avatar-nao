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

    class JointData
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("angle")]
        public float Angle { get; set; }
        [JsonProperty("stiffness")]
        public float Stiffness { get; set; }

        public JointData(string name, float angle, float stiffness)
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
   

   
}
