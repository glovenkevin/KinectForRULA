using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            // Hitung sudut dari inputan sensor
            if (-20 >= angle && angle <= 20)
            {
                tutorShowSkeleton.MainWindow.scorePosture[0] = 1;
            }
            else if (-20 < angle || (angle > 20 && angle <= 45 ))
            {
                tutorShowSkeleton.MainWindow.scorePosture[0] = 2;
            } 
            else if (angle > 45 && angle <= 90) 
            {
                tutorShowSkeleton.MainWindow.scorePosture[0] = 3;
            }
            else if (angle > 90)
            {
                tutorShowSkeleton.MainWindow.scorePosture[0] = 4;
            }

            // Tambahkan nilai sudut dengan setting yang diberikan
            tutorShowSkeleton.MainWindow.scorePosture[0] += tutorShowSkeleton.MainWindow.scoreSetting[0];
        }

        public static void calculateLowerArm(double angle, double angleDeviation)
        {
            // Hitung sudut lower arm berdasarkan input sensor
            if ((angle > 0 && angle < 60) || angle > 100)
            {
                tutorShowSkeleton.MainWindow.scorePosture[1] = 2;
            }
            else if (angle >= 60 && angle <= 100)
            {
                tutorShowSkeleton.MainWindow.scorePosture[1] = 1;
            }

            // Perhitungan sudut arah lower Arm
            if (angleDeviation < -10 && angleDeviation > 10)
            {
                tutorShowSkeleton.MainWindow.scorePosture[1] += 1;
            }

            // Tambahkan nilai setting
            tutorShowSkeleton.MainWindow.scorePosture[1] += tutorShowSkeleton.MainWindow.scoreSetting[1];
        }

        public static void calculateWrist(double angle)
        {
            // Hitung sudut pergelangan tangan dari sensor
            if (angle > 0 && angle <= 5)
            {
                tutorShowSkeleton.MainWindow.scorePosture[2] = 1;
            } 
            else if (angle <= 15 && angle > 5)
            {
                tutorShowSkeleton.MainWindow.scorePosture[2] = 2;
            }
            else if (angle > 15)
            {
                tutorShowSkeleton.MainWindow.scorePosture[2] = 3;
            }

            // Tambahkan score dari setting
            tutorShowSkeleton.MainWindow.scorePosture[2] += tutorShowSkeleton.MainWindow.scoreSetting[2];
        }

        public static void calculateNeck(double angle)
        {
            // Hitung sudut leher terhadap garis tegak lurus dari punggung
            if (angle < 0)
            {
                tutorShowSkeleton.MainWindow.scorePosture[3] = 4;
            }
            else if (angle > 0 && angle <= 10)
            {
                tutorShowSkeleton.MainWindow.scorePosture[3] = 1;
            }
            else if (angle > 10 && angle <= 20)
            {
                tutorShowSkeleton.MainWindow.scorePosture[3] = 2;
            }
            else if (angle > 20)
            {
                tutorShowSkeleton.MainWindow.scorePosture[3] = 3;
            }

            // tambahkan score setting
            tutorShowSkeleton.MainWindow.scorePosture[3] += tutorShowSkeleton.MainWindow.scoreSetting[4];
        }

        public static void calculateTrunk(double angle)
        {
            // Hitung sudut punggung / trunk dari nilai sensor
            if (angle > -10 && angle <= 10)
            {
                tutorShowSkeleton.MainWindow.scorePosture[4] = 1;
            }
            else if (angle > 10 && angle <= 20)
            {
                tutorShowSkeleton.MainWindow.scorePosture[4] = 2;
            }
            else if (angle > 20 && angle <= 60)
            {
                tutorShowSkeleton.MainWindow.scorePosture[4] = 3;
            }
            else if (angle > 60)
            {
                tutorShowSkeleton.MainWindow.scorePosture[4] = 4;
            }

            // tambahkan score setting 
            tutorShowSkeleton.MainWindow.scorePosture[4] += tutorShowSkeleton.MainWindow.scoreSetting[5];
        }

        public static int calculateRula()
        {
            int ScoreGroupA = 0;
            int ScoreGroupB = 0;
            int ScoreGroupC = 0;

            // Perhitungan nilai group A
            ScoreGroupA = GlobalVal.GroupA[
                    tutorShowSkeleton.MainWindow.scorePosture[0] - 1,    // Upper Arm
                    tutorShowSkeleton.MainWindow.scorePosture[2] - 1,    // Wrist
                    tutorShowSkeleton.MainWindow.scoreSetting[3] - 1,    // Putaran pergelangan tangan
                    tutorShowSkeleton.MainWindow.scorePosture[1] - 1     // Lower Arm
                ];
            ScoreGroupA += tutorShowSkeleton.MainWindow.scoreSetting[7] // Beban otot Group A
                + tutorShowSkeleton.MainWindow.scoreSetting[8];         // Beban Eksternal

            // Perhitungan nilai Group B
            ScoreGroupB = GlobalVal.GroupB [
                    tutorShowSkeleton.MainWindow.scorePosture[3] - 1,   // Leher
                    tutorShowSkeleton.MainWindow.scorePosture[4] - 1,   // Punggung
                    tutorShowSkeleton.MainWindow.scoreSetting[6] - 1    // Kaki
                ];
            ScoreGroupB += tutorShowSkeleton.MainWindow.scoreSetting[9] // Beban otot Group B
                + tutorShowSkeleton.MainWindow.scoreSetting[10];        // Beban eksternal Group B

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
            ScoreGroupC = GlobalVal.GroupC[ScoreGroupA, ScoreGroupB];

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
    }
}
