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
                this.txtWristDeviation, this.txtWristTwist,
                this.txtNeckIsTwisted,
                this.txtLeg);
        }

        void Window_Closed(object sender, EventArgs eventArgs)
        {
            this.controller.ReleaseSensor();
            Environment.Exit(0); // shutdown the app / terminate
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
                this.canvas.Visibility = System.Windows.Visibility.Visible;
                this.btnSaveCsv.IsEnabled = true;
            } 
            else
            {
                this.controller.CloseReader();
                this.Status.Content = GlobalVal.DISCONNECT;
                this.camera.Source = null;
                this.canvas.Visibility = System.Windows.Visibility.Hidden;
                this.btnSaveCsv.IsEnabled = false;
                setDefaultLabelTextbox();
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
            setWin.Visibility = Visibility.Visible;
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

        public void setDefaultLabelTextbox()
        {
            // Set the textbox and label to default
            this.textUpperArm.Text = "";
            this.txtAbductionStatus.Content = "false";
            this.txtAbductionStatus.Foreground = Brushes.Gray;

            this.txtShoulderRaise.Content = "false";
            this.txtShoulderRaise.Foreground = Brushes.Gray;

            this.textLowerArm.Text = "";
            this.txtLowerArmMidline.Content = "false";
            this.txtLowerArmMidline.Foreground = Brushes.Gray;

            this.textWristArm.Text = "";
            this.txtWristDeviation.Content = "false";
            this.txtWristDeviation.Foreground = Brushes.Gray;
            this.txtWristTwist.Content = "1";

            this.textNeck.Text = "";
            this.txtNeckBending.Content = "false";
            this.txtNeckBending.Foreground = Brushes.Gray;
            this.txtNeckIsTwisted.Content = "false";
            this.txtNeckIsTwisted.Foreground = Brushes.Gray;

            this.textTrunk.Text = "";
            this.txtTrunkBending.Content = "false";
            this.txtTrunkBending.Foreground = Brushes.Gray;
            this.txtTrunkTwisted.Content = "false";
            this.txtTrunkTwisted.Foreground = Brushes.Gray;

            this.txtLeg.Content = 1;

            this.finalScore.Text = "None";
            this.finalScore.Foreground = Brushes.Black;
            this.finalScoreMsg.Text = "None";
            this.finalScore.Foreground = Brushes.Black;

            // re-initialize every variable in GlobalVal
            GlobalVal.scorePosture = new int[5];
            GlobalVal.scoreSetting = new int[11] {
                0, 0, 0, 1, 0, 0, 1, 0, 0, 0, 0
            };
        }
    }
}
