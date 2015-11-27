using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace naoTest
{
    class RecognizedObjects
    {
        [JsonProperty("@type")]
        public String type = "RecognizedObjects";
        bool hasPosition = true;
        bool hasDistance = false;
        bool hasYaw = false;
        public List<RecognizedObject> trashes = new List<RecognizedObject>();
        public List<RecognizedObject> trashCans = new List<RecognizedObject>();

        public override string ToString()
        {
            return ""+ string.Join(", ", trashes)+"";
        }
    }
}
