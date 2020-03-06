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

        public static int[] scoreSetting = new int[11];
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
                this.controller.openColorReader();
                this.controller.OpenReaderBodySkeleton();
                this.Status.Content = "Connected";
            } 
            else
            {
                this.controller.CloseReader();
                this.Status.Content = "Disconnect";
                this.camera.Source = null;
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
                this.textTrunk
                );
            controller.GetSensor(this.canvas);
        }

        public void setColorVideo()
        {
            controller.openColorReader();
        }
        public void setBodySkeleton()
        {
            controller.OpenReaderBodySkeleton();
        }
    }
}
