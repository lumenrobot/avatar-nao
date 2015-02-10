using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

namespace LumenServer
{
    class CommandJson
    {
        
    }

    public class Command
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("Method")]
        public string method { get; set; }
        [JsonProperty("parameter")]
        public Parameter parameter { get; set; }
        
    }
    public class Parameter
    {
        //RobotPosture parameter
        [JsonProperty("postureName")]
        public string postureName { get; set; }
        [JsonProperty("speed")]//belong to motion and posture
        public float speed { get; set; }
        
        //motion parameter
        [JsonProperty("jointNames")]
        public List<string> jointName { get; set; }
        [JsonProperty("stiffnessses")]
        public List<float> stiffnessess { get; set; }
        [JsonProperty("angles")]
        public List<float> angles { get; set; }
        [JsonProperty("handName")]
        public string handName { get; set; }
        [JsonProperty("x")]
        public float x { get; set; }
        [JsonProperty("y")]
        public float y { get; set; }
        [JsonProperty("tetha")]
        public float tetha { get; set; }
        [JsonProperty("LHand")]
        public bool LHand { get; set; }
        [JsonProperty("RHand")]
        public bool RHand { get; set; }

        //TextToSpeech parameter
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("language")]
        public string language { get; set; }
    }
    
    
}
