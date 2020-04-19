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

        // Pesan penjelasan nilai Final Score
        public static String[] Message = new String[] {
            "Postur masih bisa diterima",                                           // 1 - 2
            "Perlu investigasi lebih lanjut",                                       // 3 - 4
            "Perlu investigasi lebih lanjut dan harus segera mengganti postur",     // 5 - 6
            "Investigasi dan langsung mengganti postur"                             // 7
        };
    }
}
