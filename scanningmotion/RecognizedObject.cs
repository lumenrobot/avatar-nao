using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace naoTest
{
    class RecognizedObject
    {
        [JsonProperty("@type")]
        public String type = "RecognizedObject"; 
        public string name;
        public Vector2 topPosition;//  Data kamera atas mempunya nilai koordinat (X, Y)
        public Vector2 angle;//
        public Vector2 bottomPosition;// Data kamera bawah mempunyai  nilai koordinat (X, Y)

        public override string ToString()
        {
            return "" +topPosition;
            //return topPosition.x+", "top;
        }
        
        public float getX()
        {
            return topPosition.x;
        }

        public float getY()
        {
            return topPosition.y;
        }
    }
}