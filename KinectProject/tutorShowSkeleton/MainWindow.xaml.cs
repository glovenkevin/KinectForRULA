using System;
using System.Windows;
using Microsoft.Kinect;

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
