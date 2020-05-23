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
        #region Rula Group and Final Score
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
            "Neglibible risk, no action required",                                  // 1 - 2
            "Low risk, change may be needed",                                       // 3 - 4
            "Medium risk, further investigation, change soon",                      // 5 - 6
            "Very high risk, implement change now"                                  // 7
        };

        /*   
         *  0 -> upper arm
         *  1 -> lower arm
         *  2 -> pergelangan tangan
         *  3 -> putaran pergelangan tangan
         *  
         *  4 -> leher
         *  5 -> batang tubuh
         *  6 -> kaki
         *  
         *  7 -> otot Postur A
         *  8 -> beban eksternal postur A
         *  
         *  9 -> otot postur B
         *  10 -> beban eksternal Postur B
         */
        public static int[] scoreSetting = new int[11] {
            0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0                 // Inisialisasi Awal
        };

        /*
         *  Group A:
         *      0 -> Upper Arm
         *      1 -> Lower Arm
         *      2 -> Wrist
         *     
         *  Group B:
         *      3 -> Leher
         *      4 -> Punggung
         */
        public static int[] scorePosture = new int[5];
        #endregion

        // Global Condition
        public static int BODY_SIDE = 0;     // 0 -> left | 1 -> right
        public static bool RECORD_STATUS = false;

        // Camera status
        // For development purpose
        public static String _mode;
        public static readonly String DEPTH = "depth";
        public static readonly String COLOR = "color";

        // Global status 
        public static readonly String CONNECT = "Connected";
        public static readonly String DISCONNECT = "Disconnected";
        public static readonly String START_RECORD = "Start Recording";
        public static readonly String STOP_RECORD = "Stop Recording";

        // Stack for saving data
        public static List<dynamic> data = new List<dynamic>();

        // Save every part of body that being investigate their angle
        public static double upperArm = 0;
        public static bool uperArmAbduction = false;
        public static bool shoulderAngle = false;
        public static bool upperArmLean = false;

        public static double lowerArm = 0;
        public static bool lowerArmMidline = false;

        public static double wrist = 0;
        public static bool wristDeviation = false;

        public static int wristTwist = 1;

        public static double neck = 0;
        public static bool neckBending = false;
        public static bool neckTwisted = false;

        public static double trunk = 0;
        public static bool trunkBending = false;
        public static bool trunkTwisted = false;

        public static int muscleUseA = 0;
        public static int loadUseA = 0;
        public static int muscleUseB = 0;
        public static int loadUseB = 0;

        // Leg is not main body part that being observe but RULA take a score with it
        // Leg score come from manual setting window
        // Default is 1 -> 
        public static int leg = 1; 

        // Save score in a global Variable so it can be called from anywhere
        // purpose: For saving data on CSV
        public static int ScoreGroupA = 0;
        public static int ScoreGroupB = 0;
        public static int ScoreGroupC = 0;

        // Location file CSV saved
        public static readonly string CSV_SAVE_LOCATION = @"F:\\Data.csv";
    }
}
