using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tutorShowSkeleton
{
    class DataCsv
    {
        // Body part item
        public double upperArm { get; set; }
        public double upperArmAbduction { get; set; }
        public double shoulderArm { get; set; }

        public double lowerArm { get; set; }
        public double lowerArmMidline { get; set; }

        public double wrist { get; set; }

        public double neck { get; set; }
        public double neckBending { get; set; }

        public double trunk { get; set; }
        public double trunkBending { get; set; }

        // Score every group
        public int GroupA { get; set; }
        public int GroupB { get; set; }
        public int GroupC { get; set; }
    }
}
