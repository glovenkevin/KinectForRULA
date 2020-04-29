using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace tutorShowSkeleton
{
    static class GlobalVal
    {
        // Body Part that being use for kalman filter
        public static JointType[] BodyPart = new JointType[] {
            // Group A
            // Left
            JointType.ShoulderLeft,
            JointType.ElbowLeft,
            JointType.WristLeft,
            JointType.HandLeft,
            JointType.HandTipLeft,
            // Right
            JointType.ShoulderRight,
            JointType.ElbowRight,
            JointType.WristRight,
            JointType.HandRight,
            JointType.HandTipRight,
            
            // Group B
            JointType.Head,
            JointType.Neck,
            JointType.SpineShoulder,
            JointType.SpineMid,
            JointType.SpineBase,
            JointType.HipLeft,
            JointType.HipRight,
            JointType.AnkleLeft,
            JointType.AnkleRight
        };

        // Score Group A
        // Urutan { Lengan Atas, Postur pergelangan, Putaran Pergelangan, Lengan Bawah }
        public static int[, , ,] GroupA = new int[6, 4, 2, 3]
                {
                    { 
                        { {1, 2, 2}, {2, 2, 3} },
                        { {2, 2, 3}, {2, 2, 3} },
                        { {2, 3, 3}, {3, 3, 3} },
                        { {3, 3, 4}, {3, 3, 4} }
                    },
                    {
                        { {2, 3, 3}, {3, 3, 4} },
                        { {3, 3, 4}, {3, 3, 4} },
                        { {3, 3, 4}, {4, 4, 4} },
                        { {4, 4, 5}, {4, 4, 5} }
                    },
                    {
                        { {3, 3, 4}, {3, 4, 4} },
                        { {4, 4, 4}, {4, 4, 4} },
                        { {4, 4, 4}, {4, 4, 5} },
                        { {5, 5, 5}, {5, 5, 5} }
                    },
                    {
                        { {4, 4, 4}, {4, 4, 4} },
                        { {4, 4, 4}, {4, 4, 5} },
                        { {4, 4, 5}, {5, 5, 5} },
                        { {5, 5, 6}, {5, 5, 6} }
                    },
                    {
                        { {5, 5, 6}, {5, 6, 6} },
                        { {5, 6, 6}, {5, 6, 7} },
                        { {5, 6, 7}, {6, 7, 7} },
                        { {6, 7, 7}, {7, 7, 8} }
                    },
                    {
                        { {7, 8, 9}, {7, 8, 9} },
                        { {7, 8, 9}, {7, 8, 9} },
                        { {7, 8, 9}, {8, 9, 9} },
                        { {8, 9, 9}, {9, 9, 9} }
                    }
                };
        
        // Score untuk Group B
        // Urutan { Leher, Punggung , Kaki }
        public static int[,,] GroupB = new int[6, 6, 2] 
        {
            {
                {1, 3},
                {2, 3},
                {3, 4},
                {5, 5},
                {6, 6},
                {7, 7}
            },
            {
                {2, 3},
                {2, 3},
                {4, 5},
                {5, 5},
                {6, 7},
                {7, 7}
            },
            {
                {3, 3},
                {3, 4},
                {4, 5},
                {5, 6},
                {6, 7},
                {7, 7}
            },
            {
                {5, 5},
                {5, 6},
                {6, 7},
                {7, 7},
                {7, 7},
                {8, 8}
            },
            {
                {7, 7},
                {7, 7},
                {7, 8},
                {8, 8},
                {8, 8},
                {8, 8}
            },
            {
                {8, 8},
                {8, 8},
                {8, 8},
                {8, 9},
                {9, 9},
                {9, 9}
            }
        };

        // Group C / Tabel Final
        // Urutan { Group A, Group B }
        public static int[,] GroupC = new int[8, 7] { 
            {1, 2, 3, 3, 4, 5, 5},
            {2, 2, 3, 4, 4, 5, 5},
            {3, 3, 3, 4, 4, 5, 6},
            {3, 3, 3, 4, 5, 6, 6},
            {4, 4, 4, 5, 6, 7, 7},
            {4, 4, 5, 6, 6, 7, 7},
            {5, 5, 6, 6, 7, 7, 7},
            {5, 5, 6, 7, 7, 7, 7}
        };

        // Save every part of body that being investigate their angle
        public static double upperArm = 0;
        public static double uperArmAbduction = 0;
        public static double shoulderAngle = 0;

        public static double lowerArm = 0;
        public static double lowerArmMidline = 0;

        public static double wrist = 0;

        public static double neck = 0;
        public static double neckBending = 0;

        public static double trunk = 0;
        public static double trunkBending = 0;

        // Leg is not main body part that being observe but RULA take a score with it
        // Leg score come from manual setting window
        // Default is 1 -> 
        public static double leg = 1; 

        // Save score in a global Variable so it can be called from anywhere
        // purpose: For saving data on CSV
        public static int ScoreGroupA = 0;
        public static int ScoreGroupB = 0;
        public static int ScoreGroupC = 0;

        // Pesan penjelasan nilai Final Score
        public static String[] Message = new String[] {
            "Posture still acceptable",                                             // 1 - 2
            "Posture need to investigate further",                                  // 3 - 4
            "Posture need to investigate furthher and change soon",                 // 5 - 6
            "Posture need to investigate and chan ge immediately"                   // 7
        };
    }
}
