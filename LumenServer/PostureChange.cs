using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAOserver
{
    class PostureChange
    {
        public string PostureId { get; set; }
        public double? Speed { get; set; }

        public override string ToString()
        {
            return "PostureChange {postureId=" + PostureId + " speed=" + Speed + "}";
        }
    }
}
