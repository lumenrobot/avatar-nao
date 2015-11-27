using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace naoTest
{
    class Vector2
    {
        [JsonProperty("@type")]
        public String type = "Vector2";
        public float x;
        public float y;
        public float x_angle;
        public float y_angle;


        public override string ToString()
        {
            return "{x =" + x + ", y =" +y+"}";
            //return x+","+y;
        }

     
    }
}
