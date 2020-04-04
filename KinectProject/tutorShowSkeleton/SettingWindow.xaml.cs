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
        private int rotasiPergelanganTangan = 0;
        private int bebanOtotTangan = 0;
        private int totalBebanEksternalTangan = 0;

        // Variabel Nilai Tambahan untuk postur Tubuh B
        private int kondisiKaki = 0;
        private int kondisiLeher = 0;
        private int kondisiBatangTubuh = 0;
        private int bebanOtotBadan = 0;
        private int totalBebanEksternalBadan = 0;

        // List jawaban

        private String[] kondisiLenganAtas = new String[] {
            "None", // 0 
            "Pundak di angkat atau lengan abducted", // 1
            "Pundak di angkat atau lengan abducted dengan bersandar", // 0
            "Pundak di angkat dan lengan abducted", // 2
            "Pundak di angkat dan lengan abducted dengan bersandar", // 1
            "Lengan disandarkan" // -1
        };

        private String[] wristDeviasi = new String[] {
            "None",
            "Menyimpang ke kiri/ke kanan"
        };

        private String[] wristRotation = new String[] {
            "None",
            "Kisaran menengah",
            "Perputaran sampai di ujung"
        };

        private String[] wristMusclePower = new String[] {
            "None",
            "Sebagian statis dan dipertahankan > 1 menit",
            "Gerakan diulang lebih dari 4 kali / menit"
        };

        private String[] bebanEksternal = new String[] {
            "Kurang dari 2kg atau tenaga intermitten", 
            "Beban 2kg - 10kg ", // 1  
            "Beban 2kg - 10kg dan dilakukan berulang / dengan kejut", // 2 
            "Lebih dari 10kg dan dilakukan berulang" // 3
        };

        private String[] kondisiStatusLeher = new String[] {
            "None",
            "Leher berputar",
            "Leher bengkok ke kiri / kanan",
            "Leher berputar dan bengkok"
        };

        private String[] kondisiPutaranBatangTubuh = new String[] {
            "None",
            "Batang tubuh berputar",
            "Batang tubuh bengkok ke kiri / kanan",
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
            // Setting kondisi lengan atas
            this.kondisiLengan.ItemsSource = kondisiLenganAtas;
            this.kondisiLengan.SelectedIndex = 0;

            // Setting sudut tangan
            this.deviasiPergelanganTangan.ItemsSource = wristDeviasi;
            this.deviasiPergelanganTangan.SelectedIndex = 0;

            // Setting putaran pergelangan tangan
            this.putaranPergelanganTangan.ItemsSource = wristRotation;
            this.putaranPergelanganTangan.SelectedIndex = 0;

            // Setting Beban otot tangan
            this.kekuatanOtotTangan.ItemsSource = wristMusclePower;
            this.kekuatanOtotTangan.SelectedIndex = 0;

            // Setting Beban Eskternal tangan
            this.bebanEksternalTangan.ItemsSource = bebanEksternal;
            this.bebanEksternalTangan.SelectedIndex = 0;

            // Setting kondisi leher
            this.kondisiSudutLeher.ItemsSource = kondisiStatusLeher;
            this.kondisiSudutLeher.SelectedIndex = 0;

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
            String sisiBadan = this.sidePosition.Text;

            // Postur Tubuh A -------------------------------------------------
            // Lengan atas
            String temp = this.kondisiLengan.Text;
            if (String.Equals(temp, kondisiLenganAtas[1]) ||
                String.Equals(temp, kondisiLenganAtas[4]))
            {
                this.statusLenganAtas += 1;
            }
            else if (String.Equals(temp, kondisiLenganAtas[3]))
            {
                this.statusLenganAtas += 2;
            }
            else if (String.Equals(temp, kondisiLenganAtas[5]))
            {
                this.statusLenganAtas -= 1;
            }

            // Pergelangan tangan
            temp = this.deviasiPergelanganTangan.Text;
            if (String.Equals(temp, wristDeviasi[1]))
            {
                this.pergelanganTangan += 1;
            }

            temp = this.putaranPergelanganTangan.Text;
            if (String.Equals(temp, wristRotation[1]))
            {
                this.rotasiPergelanganTangan += 1;
            }
            else if (String.Equals(temp, wristRotation[2]))
            {
                this.rotasiPergelanganTangan += 2;
            }

            // Beban Posutr A 
            // kekuatan otot tangan 
            temp = this.kekuatanOtotTangan.Text;
            if (String.Equals(temp, wristMusclePower[1]))
            {
                this.bebanOtotTangan += 1;
            }
            else if (String.Equals(temp, wristMusclePower[2]))
            {
                this.bebanOtotTangan += 2;
            }

            temp = this.bebanEksternalTangan.Text;
            if (String.Equals(temp, bebanEksternal[1]))
            {
                this.totalBebanEksternalTangan += 1;
            }
            else if (String.Equals(temp, bebanEksternal[2]))
            {
                this.totalBebanEksternalTangan += 2;
            }
            else if (String.Equals(temp, bebanEksternal[3]))
            {
                this.totalBebanEksternalTangan += 3;
            }
            // End Postur A -------------------------------------------------------
            
            // Postur Tubuh B -----------------------------------------------------
            // Beban Kaki
            if (this.bebanKaki.IsChecked == true)
            {
                this.kondisiKaki += 1;
            } else {
                this.kondisiKaki += 2;
            }

            // Kondisi Leher
            temp = this.kondisiSudutLeher.Text;
            if (String.Equals(temp, kondisiStatusLeher[1]) || 
                String.Equals(temp, kondisiStatusLeher[2]))
            {
                this.kondisiLeher += 1;
            }
            else if (String.Equals(temp, kondisiStatusLeher[3]))
            {
                this.kondisiLeher += 2;
            }

            // Kondisi sudut batang tubuh
            temp = this.kondisiRotasiBatangTubuh.Text;
            if (String.Equals(temp, kondisiPutaranBatangTubuh[1]) ||
                String.Equals(temp, kondisiPutaranBatangTubuh[2]))
            {
                this.kondisiBatangTubuh += 1;
            }
            else if (String.Equals(temp, kondisiPutaranBatangTubuh[3]))
            {
                this.kondisiBatangTubuh += 2;
            }

            // Beban postur B
            temp = this.kekuatanOtotBadan.Text;
            if (String.Equals(temp, wristMusclePower[1]))
            {
                this.bebanOtotBadan += 1;
            }
            else if (String.Equals(temp, wristMusclePower[2]))
            {
                this.bebanOtotBadan += 2;
            }

            temp = this.bebanEksternalBadan.Text;
            if (String.Equals(temp, bebanEksternal[1]))
            {
                this.totalBebanEksternalBadan += 1;
            }
            else if (String.Equals(temp, bebanEksternal[2]))
            {
                this.totalBebanEksternalBadan += 2;
            }
            else if (String.Equals(temp, bebanEksternal[3]))
            {
                this.totalBebanEksternalBadan += 3;
            }

            // End Postur Tubuh B -----------------------------------------------------

            // Start Perhitungan 

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
   

