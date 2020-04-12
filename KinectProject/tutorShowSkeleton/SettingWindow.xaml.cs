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
            "kiri",
            "kanan"
        };

        private String[] kondisiLenganAtas = new String[] {
            "None", // 0 
            "Pundak di angkat atau lengan abducted", // 1
            "Pundak di angkat atau lengan abducted dengan bersandar", // 0
            "Pundak di angkat dan lengan abducted", // 2
            "Pundak di angkat dan lengan abducted dengan bersandar", // 1
            "Lengan disandarkan" // -1
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

        private String[] kondisiPutaranBatangTubuh = new String[] {
            "None",
            "Batang tubuh berputar atau bengkok ke kiri/kanan",
            "Batang tubuh berputar dan bengkok"
        };

        public SettingWindow()
        {
            InitializeComponent();
            InitializeComboboxItem();
        }

        void Apply_Setting(object sender, EventArgs eventArgs)
        {
            applyData();
            this.Hide();
        }

        void InitializeComboboxItem()
        {
            // Setting bagian badan
            this.sidePosition.ItemsSource = sisiBadan;
            this.sidePosition.SelectedIndex = 0;

            // Setting kondisi lengan atas
            this.kondisiLengan.ItemsSource = kondisiLenganAtas;
            this.kondisiLengan.SelectedIndex = 0;

            // Setting putaran pergelangan tangan
            this.putaranPergelanganTangan.ItemsSource = wristRotation;
            this.putaranPergelanganTangan.SelectedIndex = 0;

            // Setting Beban otot tangan
            this.kekuatanOtotTangan.ItemsSource = wristMusclePower;
            this.kekuatanOtotTangan.SelectedIndex = 0;

            // Setting Beban Eskternal tangan
            this.bebanEksternalTangan.ItemsSource = bebanEksternal;
            this.bebanEksternalTangan.SelectedIndex = 0;

            // Seting kondisi batang tubuh
            this.kondisiRotasiBatangTubuh.ItemsSource = kondisiPutaranBatangTubuh;
            this.kondisiRotasiBatangTubuh.SelectedIndex = 0;

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
                tutorShowSkeleton.MainWindow.sisiBadan = 0;
            }
            else
            {
                tutorShowSkeleton.MainWindow.sisiBadan = 1;
            }

            /******************** Group A ****************************************/
            // Lengan atas
            String temp = this.kondisiLengan.Text;
            if (String.Equals(temp, kondisiLenganAtas[1]) ||
                String.Equals(temp, kondisiLenganAtas[4]))
            {
                this.statusLenganAtas = 1;
            }
            else if (String.Equals(temp, kondisiLenganAtas[3]))
            {
                this.statusLenganAtas = 2;
            }
            else if (String.Equals(temp, kondisiLenganAtas[5]))
            {
                this.statusLenganAtas = -1;
            }
            else
            {
                this.statusLenganAtas = 0;
            }

            // Pergelangan tangan
            if (this.wristDeviation.IsChecked == true)
            {
                this.pergelanganTangan = 1;
            }

            temp = this.putaranPergelanganTangan.Text;
            if (String.Equals(temp, wristRotation[0]))
            {
                this.rotasiPergelanganTangan = 1;
            }
            else if (String.Equals(temp, wristRotation[1]))
            {
                this.rotasiPergelanganTangan = 2;
            }

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
                this.kondisiKaki = 1;
            } else {
                this.kondisiKaki = 2;
            }

            // Kondisi Leher
            if (this.neckTwist.IsChecked == true)
            {
                this.kondisiLeher = 1;
            }

            // Kondisi sudut batang tubuh
            temp = this.kondisiRotasiBatangTubuh.Text;
            if (String.Equals(temp, kondisiPutaranBatangTubuh[1]))
            {
                this.kondisiBatangTubuh = 1;
            }
            else if (String.Equals(temp, kondisiPutaranBatangTubuh[2]))
            {
                this.kondisiBatangTubuh = 2;
            }
            else
            {
                this.kondisiBatangTubuh = 0;
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

            tutorShowSkeleton.MainWindow.scoreSetting[0] = this.statusLenganAtas;
            tutorShowSkeleton.MainWindow.scoreSetting[1] = this.statusLenganBawah;
            tutorShowSkeleton.MainWindow.scoreSetting[2] = this.pergelanganTangan;
            tutorShowSkeleton.MainWindow.scoreSetting[3] = this.rotasiPergelanganTangan;

            tutorShowSkeleton.MainWindow.scoreSetting[4] = this.kondisiLeher;
            tutorShowSkeleton.MainWindow.scoreSetting[5] = this.kondisiBatangTubuh;
            tutorShowSkeleton.MainWindow.scoreSetting[6] = this.kondisiKaki;
            
            tutorShowSkeleton.MainWindow.scoreSetting[7] = this.bebanOtotTangan;
            tutorShowSkeleton.MainWindow.scoreSetting[8] = this.totalBebanEksternalTangan;
            tutorShowSkeleton.MainWindow.scoreSetting[9] = this.bebanOtotBadan;
            tutorShowSkeleton.MainWindow.scoreSetting[10] = this.totalBebanEksternalBadan;
        }


    }
}
   

