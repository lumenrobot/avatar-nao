using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAOserver
{
    class JointInterpolateAngle
    {
        public string JointId { get; set; }
        public double TargetCcwDeg { get; set; }
        public double Duration { get; set; }

        public override string ToString()
        {
            return "JointInterpolateAngle {jointId=" + JointId + ", targetCcwDeg=" + TargetCcwDeg + ", duration=" + Duration + "}";
        }
    }
}
