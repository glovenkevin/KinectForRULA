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
        SettingWindow setWin;

        public MainWindow()
        {
            InitializeComponent();
            InitializeStartUpStatus();
            openSensor();
            setWin = new SettingWindow(this.txtTrunkTwisted,
                this.txtWristDeviation, this.txtNeckIsTwisted);
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
                // Angle Text Box
                this.textUpperArm,
                this.txtAbductionStatus,
                this.txtShoulderRaise,
                this.textLowerArm,
                this.txtLowerArmMidline,
                this.textWristArm,
                this.txtWristDeviation,
                this.textNeck,
                this.txtNeckBending,
                this.txtNeckIsTwisted,
                this.textTrunk,
                this.txtTrunkBending,
                this.txtTrunkTwisted,

                // Final Score
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
