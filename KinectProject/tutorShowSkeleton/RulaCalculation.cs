using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace tutorShowSkeleton
{
    static class RulaCalculation
    {
        /*  Patokan Array Setting Window:
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

        public static void calculateUpperArm(double angle)
        {
            GlobalVal.scorePosture[0] = 1;

            // Hitung sudut dari inputan sensor
            if (-20 >= angle && angle <= 20)
            {
                GlobalVal.scorePosture[0] = 1;
            }
            else if (-20 > angle)
            {
                GlobalVal.scorePosture[0] = 2;
            }
            else if (angle > 20 && angle <= 45)
            {
                GlobalVal.scorePosture[0] = 2;
            }
            else if (angle > 45 && angle <= 105) 
            {
                GlobalVal.scorePosture[0] = 3;
            }
            else if (angle > 105)
            {
                GlobalVal.scorePosture[0] = 4;
            }

            // Add Upper arm is lean -> 0 / -1
            GlobalVal.scorePosture[0] += GlobalVal.scoreSetting[0];
        }

        public static bool calculateUpperArmAbduction(double elbowX, double shoulderX)
        {
            // Note: calculation with angle don't give an acurate calculation
            // Solution: move to coordinate analys
            bool status = false;
            double range = shoulderX - elbowX;
            if (range < 0)
            {
                range *= -1;
            }

            if (range > 0.1)
            {
                status = true;
            }

            if (status)
            {
                GlobalVal.scorePosture[0] += 1;
            }
            return status;
        }

        public static bool calcShoulderRaise(double shoulderRaised) 
        {
            // Shoulder is raised
            if (shoulderRaised < 15)
            {
                GlobalVal.scorePosture[0] += 1;
                return true;
            }
            return false;
        }

        public static void calculateLowerArm(double angle)
        {
            // Hitung sudut lower arm berdasarkan input sensor
            if ((angle > 0 && angle < 60) || angle > 100)
            {
                GlobalVal.scorePosture[1] = 2;
            }
            else if (angle >= 60 && angle <= 100)
            {
                GlobalVal.scorePosture[1] = 1;
            }

            // Tambahkan nilai setting
            // GlobalVal.scorePosture[1] += GlobalVal.scoreSetting[1];
        }

        public static bool calcLowerArmDeviation(double wrist, double shoulder)
        {
            bool status = false;
            // Perhitungan sudut arah lower Arm
            if (wrist < shoulder - 0.1 || wrist > shoulder + 0.1)
            {
                status = true;
            }

            if (status)
            {
                GlobalVal.scorePosture[1] += 1;
            }
                
            return status;
        }

        public static void calculateWrist(double angle)
        {
            // Hitung sudut pergelangan tangan dari sensor
            if ( angle < 10)
            {
                GlobalVal.scorePosture[2] = 1;
            } 
            else if (angle < 25 && angle > 10)
            {
                GlobalVal.scorePosture[2] = 2;
            }
            else if (angle > 25)
            {
                GlobalVal.scorePosture[2] = 3;
            }

            // Tambahkan score dari setting
            GlobalVal.scorePosture[2] += GlobalVal.scoreSetting[2];
        }

        public static void calculateNeck(double angle)
        {
            // Hitung sudut leher terhadap garis tegak lurus dari punggung
            if (angle < 0)
            {
                GlobalVal.scorePosture[3] = 4;
            }
            else if (angle >= 0 && angle <= 10)
            {
                GlobalVal.scorePosture[3] = 1;
            }
            else if (angle > 10 && angle <= 20)
            {
                GlobalVal.scorePosture[3] = 2;
            }
            else if (angle > 20)
            {
                GlobalVal.scorePosture[3] = 3;
            }

            // tambahkan score setting
            GlobalVal.scorePosture[3] += GlobalVal.scoreSetting[4];
        }

        public static bool calcNeckBending(double neckBending)
        {
            // Neck Bending
            if (neckBending > 5)
            {
                GlobalVal.scorePosture[3] += 1;
                return true;
            }
            return false;
        }

        public static void calculateTrunk(double angle)
        {
            // Hitung sudut punggung / trunk dari nilai sensor
            if (angle <= 10)
            {
                GlobalVal.scorePosture[4] = 1;
            }
            else if (angle > 10 && angle <= 20)
            {
                GlobalVal.scorePosture[4] = 2;
            }
            else if (angle > 20 && angle <= 50)
            {
                GlobalVal.scorePosture[4] = 3;
            }
            else if (angle > 50)
            {
                GlobalVal.scorePosture[4] = 4;
            }

            // tambahkan score setting 
            GlobalVal.scorePosture[4] += GlobalVal.scoreSetting[5];
        }

        public static bool calcTrunkbending(double trunkBending)
        {
            // Side Bending
            if (trunkBending < 90) // Actor sit
            {
                if (trunkBending > 15)
                {
                    GlobalVal.scorePosture[4] += 1;
                    return true;
                }
            }
            else // Actor standing 
            {
                if (trunkBending < 165)
                {
                    GlobalVal.scorePosture[4] += 1;
                    return true;
                }
            }
            
            return false;
        }

        public static int calculateRula()
        {
            int ScoreGroupA = 0;
            int ScoreGroupB = 0;
            int ScoreGroupC = 0;

            // Perhitungan nilai group A
            ScoreGroupA = GlobalVal.GroupA[
                    GlobalVal.scorePosture[0] - 1,    // Upper Arm
                    GlobalVal.scorePosture[2] - 1,    // Wrist
                    GlobalVal.scoreSetting[3] - 1,    // Wrist Twist
                    GlobalVal.scorePosture[1] - 1     // Lower Arm
                ];
            ScoreGroupA += GlobalVal.scoreSetting[7] // Beban otot Group A
                + GlobalVal.scoreSetting[8];         // Beban Eksternal
            // Save to Static variable
            GlobalVal.ScoreGroupA = ScoreGroupA;

            // Perhitungan nilai Group B
            ScoreGroupB = GlobalVal.GroupB [
                    GlobalVal.scorePosture[3] - 1,   // Leher
                    GlobalVal.scorePosture[4] - 1,   // Punggung
                    GlobalVal.scoreSetting[6] - 1    // Kaki
                ];
            ScoreGroupB += GlobalVal.scoreSetting[9] // Beban otot Group B
                + GlobalVal.scoreSetting[10];        // Beban eksternal Group B
            // Save to static variable
            GlobalVal.ScoreGroupB = ScoreGroupB;

            // Cek apakah melebihi 7
            if (ScoreGroupA > 8)
            {
                ScoreGroupA = 8;
            }
            if (ScoreGroupB > 7)
            {
                ScoreGroupB = 7;
            }

            // Get Nilai Final
            ScoreGroupC = GlobalVal.GroupC[ScoreGroupA - 1, ScoreGroupB - 1];
            //Save to static variable
            GlobalVal.ScoreGroupC = ScoreGroupC;

            // Kembalikan nilai Final
            return ScoreGroupC;
        }

        public static String getFinalScoreMsg(int ScoreGroupC)
        {
            // Ambil Messagenya
            String msg;
            if (ScoreGroupC <= 2)
            {
                msg = GlobalVal.Message[0];
            }
            else if (ScoreGroupC <= 4 && ScoreGroupC > 2)
            {
                msg = GlobalVal.Message[1];
            }
            else if (ScoreGroupC <= 6 && ScoreGroupC > 4)
            {
                msg = GlobalVal.Message[2];
            }
            else
            {
                msg = GlobalVal.Message[3];
            }

            // Kembalikan Pesannya
            return msg;
        }

        public static void recordDataToCSV()
        {
            dynamic record = new DataCsv
            {
                GroupA = GlobalVal.ScoreGroupA,
                GroupB = GlobalVal.ScoreGroupB,
                GroupC = GlobalVal.ScoreGroupC,

                upperArm = GlobalVal.upperArm,
                upperArmAbduction = GlobalVal.uperArmAbduction,
                shoulderArmIsRaised = GlobalVal.shoulderAngle,
                upperArmLean = GlobalVal.upperArmLean,

                lowerArm = GlobalVal.lowerArm,
                lowerArmMidline = GlobalVal.lowerArmMidline,

                neck = GlobalVal.neck,
                neckBending = GlobalVal.neckBending,
                neckTwisted = GlobalVal.neckTwisted,

                trunk = GlobalVal.trunk,
                trunkBending = GlobalVal.trunkBending,
                trunkTwist = GlobalVal.trunkTwisted,

                wrist = GlobalVal.wrist,
                wristDeviation = GlobalVal.wristDeviation,
                wristTwist = GlobalVal.wristTwist,

                leg = GlobalVal.leg,

                loadUseA = GlobalVal.loadUseA,
                muscleUseA = GlobalVal.muscleUseA,
                loadUseB = GlobalVal.loadUseB,
                muscleUseB = GlobalVal.muscleUseB
            };
            GlobalVal.data.Add(record);

            // Clear CSV data if the file 'exist'
            System.IO.File.WriteAllText(GlobalVal.CSV_SAVE_LOCATION, string.Empty);
            // If not exist -> create else Open it
            StreamWriter writer = new StreamWriter(GlobalVal.CSV_SAVE_LOCATION, true);

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(GlobalVal.data);
            }
        }
    }
}
