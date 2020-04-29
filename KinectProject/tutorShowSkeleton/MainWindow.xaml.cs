using System;
using System.Windows;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using System.Globalization;

namespace tutorShowSkeleton
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectControl controller;
        public static MainWindow mainWindow;
        SettingWindow setWin = new SettingWindow();
        
        public static String _mode = "depth";

        // Menyimpan setiap record yang dilakukan
        List<dynamic> data = new List<dynamic>();

        // 0 -> Kiri (default) || 1 -> Kanan
        public static int sisiBadan = 0; 

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

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            openSensor();
            this.Status.Content = "Disconnect";
        }

        void Window_Closed(object sender, EventArgs eventArgs)
        {
            this.controller.ReleaseSensor();
        }

        void Connect_Event(object send, EventArgs eventArgs)
        {
            if ("Disconnect".Equals(this.Status.Content))
            {
                this.controller.openReader();
                this.controller.OpenReaderBodySkeleton();
                this.Status.Content = "Connected";
            } 
            else
            {
                this.controller.CloseReader();
                this.Status.Content = "Disconnect";
                this.camera.Source = null;
                this.canvas.Children.Clear();
            }
        }

        void Save_to_CSV(object send, EventArgs eventArgs)
        {
            dynamic record = new DataCsv { 
                GroupA = GlobalVal.ScoreGroupA, 
                GroupB = GlobalVal.ScoreGroupB, 
                GroupC = GlobalVal.ScoreGroupC,
 
                upperArm = GlobalVal.upperArm, 
                upperArmAbduction = GlobalVal.uperArmAbduction,
                shoulderArm = GlobalVal.shoulderAngle,

                lowerArm = GlobalVal.lowerArm,
                lowerArmMidline = GlobalVal.lowerArmMidline,

                neck = GlobalVal.neck, 
                neckBending = GlobalVal.neckBending,

                trunk = GlobalVal.trunk, 
                trunkBending = GlobalVal.trunkBending,

                wrist = GlobalVal.wrist
            };
            data.Add(record);
            
            String filePath = @"D:\\Data.csv";
            System.IO.File.WriteAllText(filePath, string.Empty); // Clear CSV data
            StreamWriter writer = new StreamWriter(filePath, true); // If not exist -> create else Open it

            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(data);
            }

        }

        void Call_Setting(object send, EventArgs eventArgs)
        {
            // Initialize setting window
            setWin.ShowDialog();
        }

        public void openSensor()
        {
            this.controller = new KinectControl(() => new CanvasBodyDrawer(this.canvas), this.camera, 
                this.textUpperArm , 
                this.textLowerArm ,
                this.textWristArm ,
                this.textNeck,
                this.textTrunk,
                this.finalScore,
                this.finalScoreMsg
                );
            controller.GetSensor(this.canvas);
        }

        public void setBodySkeleton()
        {
            controller.OpenReaderBodySkeleton();
        }
    }
}
