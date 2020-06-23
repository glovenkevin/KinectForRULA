using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace tutorShowSkeleton
{
    /// <summary>
    /// Interaction logic for SettingWindow.xaml
    /// </summary>
    public partial class SettingWindow : Window
    {

        private void setStatus(Label label, bool status)
        {
            if (status)
            {
                label.Content = "True";
                label.Foreground = Brushes.Green;
            }
            else
            {
                label.Content = "False";
                label.Foreground = Brushes.Gray;
            }
        }

        // Variabel Nilai Tambahan untuk postur Tubuh A
        private int statusLenganAtas = 0;
        private int statusLenganBawah = 0;
        private int pergelanganTangan = 0;
        private int rotasiPergelanganTangan = 1;
        private int bebanOtotTangan = 0;
        private int totalBebanEksternalTangan = 0;

        // Variabel Nilai Tambahan untuk postur Tubuh B
        private int kondisiKaki = 1;
        private int kondisiLeher = 0;
        private int kondisiBatangTubuh = 0;
        private int bebanOtotBadan = 0;
        private int totalBebanEksternalBadan = 0;

        // List jawaban
        private String[] sisiBadan = new String[] {
            "Left side",
            "Right side"
        };

        private String[] wristRotation = new String[] {
            "Twisted mainly in mid-range",
            "Twist at near end of twisting"
        };

        private String[] wristMusclePower = new String[] {
            "None",
            "Posture mainly static",
            "Action repeatedly occurs 4 times per minute"
        };

        private String[] bebanEksternal = new String[] {
            "intermittent", 
            "Load 2 kg - 10 kg ", // 1  
            "Load 2 kg - 10 kg (static or repeated)", // 2 
            "Load more than 10 kg or repeated or shock" // 3
        };
        private Label txtTrunkTwist;
        private Label txtWristDeviation, txtWristTwist;
        private Label txtNeckTwist;
        private Label txtLeg;

        public SettingWindow(Label txtTrunkTwist, 
            Label txtWristDeviation, Label txtWristTwist, 
            Label txtNeckTwist,
            Label txtLeg)
        {
            InitializeComponent();
            InitializeComboboxItem();
            this.txtTrunkTwist = txtTrunkTwist;
            this.txtWristDeviation = txtWristDeviation;
            this.txtWristTwist = txtWristTwist;
            this.txtNeckTwist = txtNeckTwist;
            this.txtLeg = txtLeg;
        }

        void Apply_Setting(object sender, EventArgs eventArgs)
        {
            applyData();
            this.Visibility = Visibility.Hidden;
        }

        void InitializeComboboxItem()
        {
            // Setting bagian badan
            this.sidePosition.ItemsSource = sisiBadan;
            this.sidePosition.SelectedIndex = 0;

            // Setting putaran pergelangan tangan
            this.putaranPergelanganTangan.ItemsSource = wristRotation;
            this.putaranPergelanganTangan.SelectedIndex = 0;

            // Setting Beban otot tangan
            this.kekuatanOtotTangan.ItemsSource = wristMusclePower;
            this.kekuatanOtotTangan.SelectedIndex = 0;

            // Setting Beban Eskternal tangan
            this.bebanEksternalTangan.ItemsSource = bebanEksternal;
            this.bebanEksternalTangan.SelectedIndex = 0;

            // Setting kekuatan otot Badan
            this.kekuatanOtotBadan.ItemsSource = wristMusclePower;
            this.kekuatanOtotBadan.SelectedIndex = 0;

            // Setting beban eksternal badan
            this.bebanEksternalBadan.ItemsSource = bebanEksternal;
            this.bebanEksternalBadan.SelectedIndex = 0;
        }

        void applyData()
        {
            // Menentukan Posisi badan 
            if (String.Equals(this.sidePosition.Text, sisiBadan[0]))
            {
                GlobalVal.BODY_SIDE = 0;
            }
            else
            {
                GlobalVal.BODY_SIDE = 1;
            }

            /******************** Group A ****************************************/
            // Lengan atas
            String temp = "";
            if (this.upperArmLean.IsChecked == true)
            {
                this.statusLenganAtas = -1;
                GlobalVal.upperArmLean = true;
            }
            else
            {
                this.statusLenganAtas = 0;
                GlobalVal.upperArmLean = false;
            }

            // Pergelangan tangan
            if (this.wristDeviation.IsChecked == true)
            {
                this.pergelanganTangan = 1;
                setStatus(this.txtWristDeviation, true);
                GlobalVal.wristDeviation = true;
            }
            else
            {
                setStatus(this.txtWristDeviation, false);
                GlobalVal.wristDeviation = false;
            }

            temp = this.putaranPergelanganTangan.Text;
            if (String.Equals(temp, wristRotation[0]))
            {
                this.rotasiPergelanganTangan = 1;
                GlobalVal.wristTwist = 1;
            }
            else if (String.Equals(temp, wristRotation[1]))
            {
                this.rotasiPergelanganTangan = 2;
                GlobalVal.wristTwist = 2;
            }
            this.txtWristTwist.Content = GlobalVal.wristTwist.ToString();

            // Beban Postur A 
            // kekuatan otot tangan 
            temp = this.kekuatanOtotTangan.Text;
            if (String.Equals(temp, wristMusclePower[1]) || 
                String.Equals(temp, wristMusclePower[2]))
            {
                this.bebanOtotTangan = 1;
            }
            else
            {
                this.bebanOtotTangan = 0;
            }

            temp = this.bebanEksternalTangan.Text;
            if (String.Equals(temp, bebanEksternal[1]))
            {
                this.totalBebanEksternalTangan = 1;
            }
            else if (String.Equals(temp, bebanEksternal[2]))
            {
                this.totalBebanEksternalTangan = 2;
            }
            else if (String.Equals(temp, bebanEksternal[3]))
            {
                this.totalBebanEksternalTangan = 3;
            }
            else
            {
                this.totalBebanEksternalTangan = 0;
            }
            // End Postur A -------------------------------------------------------

            /********************************* Group B ****************************************/
            // Beban Kaki
            if (this.bebanKaki.IsChecked == true)
            {
                this.kondisiKaki = 2;
            } else {
                this.kondisiKaki = 1;
            }
            this.txtLeg.Content = this.kondisiKaki;

            // Kondisi Leher
            if (this.neckTwist.IsChecked == true)
            {
                this.kondisiLeher = 1;
                GlobalVal.neckTwisted = true;
                setStatus(this.txtNeckTwist, true);
            }
            else
            {
                this.kondisiLeher = 0;
                GlobalVal.neckTwisted = false;
                setStatus(this.txtNeckTwist, false);
            }

            // Kondisi sudut batang tubuh
            if (trunkTwist.IsChecked == true)
            {
                this.kondisiBatangTubuh = 1;
                setStatus(this.txtTrunkTwist, true);
            }
            else
            {
                this.kondisiBatangTubuh = 0;
                setStatus(this.txtTrunkTwist, false);
            }

            // Beban postur B
            temp = this.kekuatanOtotBadan.Text;
            if (String.Equals(temp, wristMusclePower[1]) || 
                String.Equals(temp, wristMusclePower[2]))
            {
                this.bebanOtotBadan = 1;
            }
            else                            
            {
                this.bebanOtotBadan = 0;
            }

            temp = this.bebanEksternalBadan.Text;
            if (String.Equals(temp, bebanEksternal[1]))
            {
                this.totalBebanEksternalBadan = 1;
            }
            else if (String.Equals(temp, bebanEksternal[2]))
            {
                this.totalBebanEksternalBadan = 2;
            }
            else if (String.Equals(temp, bebanEksternal[3]))
            {
                this.totalBebanEksternalBadan = 3;
            }
            else
            {
                this.totalBebanEksternalBadan = 0;
            }

            // End Postur Tubuh B -----------------------------------------------------

            // Pindahkan ke Array Global 

            /*  Patokan Array:
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

            GlobalVal.scoreSetting[0] = this.statusLenganAtas;
            // GlobalVal.scoreSetting[1] = this.statusLenganBawah;
            GlobalVal.scoreSetting[2] = this.pergelanganTangan;
            GlobalVal.scoreSetting[3] = this.rotasiPergelanganTangan;

            GlobalVal.scoreSetting[4] = this.kondisiLeher;
            GlobalVal.scoreSetting[5] = this.kondisiBatangTubuh;
            GlobalVal.scoreSetting[6] = this.kondisiKaki;
            GlobalVal.leg = this.kondisiKaki;
            
            GlobalVal.scoreSetting[7] = this.bebanOtotTangan;
            GlobalVal.muscleUseA = this.bebanOtotTangan;
            GlobalVal.scoreSetting[8] = this.totalBebanEksternalTangan;
            GlobalVal.loadUseA = this.totalBebanEksternalTangan;
            GlobalVal.scoreSetting[9] = this.bebanOtotBadan;
            GlobalVal.muscleUseB = this.bebanOtotBadan;
            GlobalVal.scoreSetting[10] = this.totalBebanEksternalBadan;
            GlobalVal.loadUseB = this.totalBebanEksternalBadan;
        }
    }
}
   

