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
        public bool upperArmAbduction { get; set; }
        public bool shoulderArmIsRaised { get; set; }
        public bool upperArmLean { get; set; }

        public double lowerArm { get; set; }
        public bool lowerArmMidline { get; set; }

        public double wrist { get; set; }
        public bool wristDeviation { get; set; }
        public int wristTwist { get; set; }

        public int muscleUseA { get; set; }
        public int loadUseA { get; set; }
        
        // Score group A
        public int GroupA { get; set; }

        public double neck { get; set; }
        public bool neckBending { get; set; }
        public bool neckTwisted { get; set; }

        public double trunk { get; set; }
        public bool trunkBending { get; set; }
        public bool trunkTwist { get; set; }

        public int leg { get; set; }

        public int muscleUseB { get; set; }
        public int loadUseB { get; set; }

        // Score group B
        public int GroupB { get; set; }
        // Final Score
        public int GroupC { get; set; }
    }
}
