using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAOserver
{
    class MoveTo
    {
        public double BackDistance { get; set; }
        public double RightDistance { get; set; }
        public double TurnCcwDeg { get; set; }

        public override string ToString()
        {
            return "MoveTo {backDistance=" + BackDistance + ", rightDistance=" + RightDistance + ", turnCcwDeg=" + TurnCcwDeg + "}";
        }
    }
}
