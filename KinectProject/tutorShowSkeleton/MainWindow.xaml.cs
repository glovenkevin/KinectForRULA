using System;
using System.Windows;
using Microsoft.Kinect;
using System.Collections.Generic;
using System.Windows.Media;

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
            InitializeStartUpStatus();
            mainWindow = this;
            openSensor();
        }

        void Window_Closed(object sender, EventArgs eventArgs)
        {
            this.controller.ReleaseSensor();
        }

        void InitializeStartUpStatus()
        {
            // Connection to Kinect Status
            this.Status.Content = GlobalVal.DISCONNECT;

            // Condition of recording
            this.recordStatus.Content = GlobalVal.STOP_RECORD;
            this.btnSaveCsv.IsEnabled = false;

            // Kinect Camera initial state (Depth , Color)
            GlobalVal._mode = GlobalVal.DEPTH;
        }

        void Connect_Event(object send, EventArgs eventArgs)
        {
            if (GlobalVal.DISCONNECT.Equals(this.Status.Content))
            {
                this.controller.openReader();
                this.controller.OpenReaderBodySkeleton();
                this.Status.Content = GlobalVal.CONNECT;
                this.btnSaveCsv.IsEnabled = true;
            } 
            else
            {
                this.controller.CloseReader();
                this.Status.Content = GlobalVal.DISCONNECT;
                this.camera.Source = null;
                this.canvas.Children.Clear();
                this.btnSaveCsv.IsEnabled = false;
            }
        }

        void Start_Stop_Record(object send, EventArgs eventArgs)
        {
            if (!GlobalVal.RECORD_STATUS)
            {
                GlobalVal.RECORD_STATUS = true;
                this.recordStatus.Content = GlobalVal.START_RECORD;
                this.recordStatus.Foreground = Brushes.Green;
                this.btnSaveCsv.Content = "Stop Record";
            }
            else
            {
                GlobalVal.RECORD_STATUS = false;
                this.recordStatus.Content = GlobalVal.STOP_RECORD;
                this.recordStatus.Foreground = Brushes.Gray;
                this.btnSaveCsv.Content = "Start Record";
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
