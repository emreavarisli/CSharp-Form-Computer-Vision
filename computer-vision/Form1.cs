using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace computer_vision
{
    public partial class Form1 : Form
    {
        private int secilenCombobox;
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("Gri Formata Çevir");
            comboBox1.Items.Add("Görüntünün Histogramını Alma");
            comboBox1.Items.Add("K-Means");
            comboBox1.Items.Add("K-Means Görüntüsünün Histogramını Alma");
            comboBox1.Items.Add("Binary Formata Çevir");
            comboBox1.Items.Add("Görüntünün Sobel Kenar Bulma");

            comboBox1.SelectedIndex = 0;

        }

        int esik;
        private void button1_Click(object sender, EventArgs e)
        {
            //Resim Yükleme Butonu
            OpenFileDialog yukle = new OpenFileDialog();
            yukle.ShowDialog();

            pictureBox1.ImageLocation = yukle.FileName;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //İşlem Butonu
            string secilenSecenek = comboBox1.SelectedItem.ToString();

            switch (secilenSecenek)
            {
                case "Gri Formata Çevir":

                    try
                    {
                        Bitmap image = new Bitmap(pictureBox1.Image);
                        Bitmap gri = griYap(image);
                        pictureBox2.Image = gri;
                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fotoğraf Yükleyiniz!");
                    }

                    break;

                case "Görüntünün Histogramını Alma":

                    Bitmap bmp = new Bitmap(pictureBox2.Image);

                    int[] histogramG = new int[256];

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color color = bmp.GetPixel(x, y);
                            histogramG[color.G]++;
                        }
                    }

                    chart1.Series.Clear();

                    Series seriesG = new Series("Green");
                    seriesG.Color = Color.Green;

                    for (int i = 0; i < 256; i++)
                    {
                        seriesG.Points.AddXY(i, histogramG[i]);
                    }

                    chart1.Series.Add(seriesG);
                    chart1.Visible = true;

                    break;

                case "K-Means":

                    pictureBox2.Image = KMeansSegmentasyon(pictureBox2.Image, secilenCombobox);


                    break;
                case "K-Means Görüntüsünün Histogramını Alma":

                    Bitmap kmeansbmp = new Bitmap(pictureBox2.Image);

                    int[] histogramB = new int[256];

                    for (int y = 0; y < kmeansbmp.Height; y++)
                    {
                        for (int x = 0; x < kmeansbmp.Width; x++)
                        {
                            Color color = kmeansbmp.GetPixel(x, y);
                            histogramB[color.G]++;
                        }
                    }

                    chart2.Series.Clear();

                    Series seriesB = new Series("Blue");
                    seriesB.Color = Color.Blue;

                    for (int i = 0; i < 256; i++)
                    {
                        seriesB.Points.AddXY(i, histogramB[i]);
                    }

                    chart2.Series.Add(seriesB);
                    chart2.Visible = true;

                    break;

                case "Binary Formata Çevir":

                    try
                    {
                        Bitmap image = new Bitmap(pictureBox1.Image);
                        Bitmap binary = binaryYap(image);
                        pictureBox2.Image = binary;
                        pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Fotoğraf Yükleyiniz!");
                    }
                    break;

                case "Görüntünün Sobel Kenar Bulma":

                    Bitmap image2 = new Bitmap(pictureBox1.Image);
                    Bitmap sobel = sobelYap(image2);
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox2.Image = sobel;

                    break;

                default:
                    break;
            }
        }
            private Bitmap griYap(Bitmap bmp)
            {
                for (int i = 0; i < bmp.Height - 1; i++)
                {
                    for (int j = 0; j < bmp.Width - 1; j++)
                    {
                        int deger = (bmp.GetPixel(j, i).R + bmp.GetPixel(j, i).G + bmp.GetPixel(j, i).B) / 3;
                        Color renk = Color.FromArgb(deger, deger, deger);

                        bmp.SetPixel(j, i, renk);
                    }
                }
                return bmp;
            }

            private Bitmap binaryYap(Bitmap bmp)
            {
                int tmp = 0;
                Bitmap gri = griYap(bmp);
                int esik = esikBul(gri);
                Color renk;
                for (int i = 0; i < gri.Height - 1; i++)
                {
                    for (int j = 0; j < gri.Width - 1; j++)
                    {
                        tmp = gri.GetPixel(j, i).G;
                        if (tmp < esik)
                        {
                            renk = Color.FromArgb(0, 0, 0);
                            gri.SetPixel(j, i, renk);
                        }
                        else
                        {
                            renk = Color.FromArgb(255, 255, 255);
                            gri.SetPixel(j, i, renk);
                        }
                    }

                }
                return gri;
            }

            private Bitmap sobelYap(Bitmap image)
            {
                Bitmap gri = griYap(image);
                Bitmap buffer = new Bitmap(gri.Width, gri.Height); //görüntünün boyutlarına sahip boş görüntü oluşturuyorsun
                Color renk;
                int valx, valy, gradient;
                int[,] GX = new int[3, 3];
                int[,] GY = new int[3, 3];

                //Yatay kenar 
                GX[0, 0] = -1; GX[0, 1] = 0; GX[0, 2] = 1;
                GX[1, 0] = -2; GX[1, 1] = 0; GX[1, 2] = 2;
                GX[2, 0] = -1; GX[2, 1] = 0; GX[2, 2] = 1;

                //Dikey kenar
                GY[0, 0] = -1; GY[0, 1] = -2; GY[0, 2] = -1;
                GY[1, 0] = 0; GY[1, 1] = 0; GY[1, 2] = 0;
                GY[2, 0] = 1; GY[2, 1] = 2; GY[2, 2] = 1;


                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        if (i == 0 || i == gri.Height - 1 || j == 0 || j == gri.Width - 1)
                        {
                            renk = Color.FromArgb(255, 255, 255);
                            buffer.SetPixel(j, i, renk);
                            valx = 0;
                            valy = 0;
                        }
                        else
                        {
                            //bütün x koordinatları için bakılıyor
                            valx = gri.GetPixel(j - 1, i - 1).R * GX[0, 0]
                                + gri.GetPixel(j, i - 1).R * GX[0, 1]
                                + gri.GetPixel(j + 1, i - 1).R * GX[0, 2]
                                + gri.GetPixel(j - 1, i).R * GX[1, 0]
                                + gri.GetPixel(j, i).R * GX[1, 1]
                                + gri.GetPixel(j + 1, i).R * GX[1, 2]
                                + gri.GetPixel(j - 1, i + 1).R * GX[2, 0]
                                + gri.GetPixel(j, i + 1).R * GX[2, 1]
                                + gri.GetPixel(j + 1, i + 1).R * GX[2, 2];

                            //bütün y koordinatları için bakılıyor
                            valy = gri.GetPixel(j - 1, i - 1).R * GY[0, 0]
                                 + gri.GetPixel(j, i - 1).R * GY[0, 1]
                                 + gri.GetPixel(j + 1, i - 1).R * GY[0, 2]
                                 + gri.GetPixel(j - 1, i).R * GY[1, 0]
                                 + gri.GetPixel(j, i).R * GY[1, 1]
                                 + gri.GetPixel(j + 1, i).R * GY[1, 2]
                                 + gri.GetPixel(j - 1, i + 1).R * GY[2, 0]
                                 + gri.GetPixel(j, i + 1).R * GY[2, 1]
                                 + gri.GetPixel(j + 1, i + 1).R * GY[2, 2];

                            gradient = (int)(Math.Abs(valx) + Math.Abs(valy));

                            if (gradient < 0)
                                gradient = 0;
                            if (gradient > 255)
                                gradient = 255;

                            renk = Color.FromArgb(gradient, gradient, gradient);
                            buffer.SetPixel(j, i, renk);
                        }
                    }
                }
                return buffer; ;
            }

            int esikBul(Bitmap gri)
            {
                int enb = gri.GetPixel(0, 0).G;
                int enk = gri.GetPixel(0, 0).G;
                for (int i = 0; i < gri.Height - 1; i++)
                {
                    for (int j = 0; j < gri.Width - 1; j++)
                    {
                        if (enb > gri.GetPixel(j, i).G)
                            enb = gri.GetPixel(j, i).G;
                        if (enk < gri.GetPixel(j, i).G)
                            enk = gri.GetPixel(j, i).G;
                    }
                }
                int a = enb;
                int b = enk;
                esik = (a + b) / 2;
                return esik;
            }

            private Bitmap KMeansSegmentasyon(Image image, int deger)
            {
                Bitmap bmp = new Bitmap(image);
                Random random = new Random();

                int k = deger;
                int iterasyonSayisi = 10;

                List<Color> merkezRenkler = new List<Color>(); // rastgele başlangıç merkezleri oluştur
                for (int i = 0; i < k; i++)
                {
                    int x = random.Next(0, bmp.Width);
                    int y = random.Next(0, bmp.Height);
                    merkezRenkler.Add(bmp.GetPixel(x, y));
                }

                for (int iterasyon = 0; iterasyon < iterasyonSayisi; iterasyon++)
                {
                    List<List<Color>> gruplar = new List<List<Color>>(); // her pikseli en yakın merkeze atayarak grupla
                    for (int i = 0; i < k; i++)
                    {
                        gruplar.Add(new List<Color>());
                    }

                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            Color pixelRenk = bmp.GetPixel(x, y);

                            // En yakın merkezi bul
                            int enYakinDeger = 0;
                            double enKMesafe = double.MaxValue;
                            for (int i = 0; i < k; i++)
                            {
                                double mesafe = RenkMesafe(pixelRenk, merkezRenkler[i]);
                                if (mesafe < enKMesafe)
                                {
                                    enYakinDeger = i;
                                    enKMesafe = mesafe;
                                }
                            }

                            gruplar[enYakinDeger].Add(pixelRenk);
                        }
                    }

                    for (int i = 0; i < k; i++)
                    {
                        if (gruplar[i].Count > 0)
                        {
                            Color yeniMerkezDegeri = GrupOrtalamaRenk(gruplar[i]); // gruplara yeni merkezler atar
                            merkezRenkler[i] = yeniMerkezDegeri;
                        }
                    }
                }

                Bitmap segmentasyonSonuc = new Bitmap(bmp.Width, bmp.Height); // yeni bir bitmap oluştur ve pikselleri gruplara göre renklendir
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        Color pixelRenk = bmp.GetPixel(x, y);

                        int enYakinDeger = 0;
                        double enKucukMesafe = double.MaxValue;
                        for (int i = 0; i < k; i++)
                        {
                            double mesafe = RenkMesafe(pixelRenk, merkezRenkler[i]);
                            if (mesafe < enKucukMesafe)
                            {
                                enYakinDeger = i;
                                enKucukMesafe = mesafe;
                            }
                        }
                        segmentasyonSonuc.SetPixel(x, y, merkezRenkler[enYakinDeger]); // pikseli en yakın merkezin rengine ayarla
                    }
                }

                return segmentasyonSonuc;
            }
            private double RenkMesafe(Color renk1, Color renk2)
            {
                double rMesafe = Math.Pow(renk1.R - renk2.R, 2); // red için aralarındaki mesafeyi hesaplar
                double gMesafe = Math.Pow(renk1.G - renk2.G, 2); // green için aralarındaki mesafeyi hesaplar
                double bMesafe = Math.Pow(renk1.B - renk2.B, 2); // blue için aralarındaki mesafeyi hesaplar

                return Math.Sqrt(rMesafe + gMesafe + bMesafe);
            }
            private Color GrupOrtalamaRenk(List<Color> grup)
            {
                int toplamR = 0;
                int toplamG = 0;
                int toplamB = 0;

                foreach (Color renk in grup)
                {
                    toplamR += renk.R;
                    toplamG += renk.G;
                    toplamB += renk.B;
                }

                int ortalamaR = toplamR / grup.Count;
                int ortalamaG = toplamG / grup.Count;
                int ortalamaB = toplamB / grup.Count;

                return Color.FromArgb(ortalamaR, ortalamaG, ortalamaB);
            }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.Visible = false;
            chart2.Visible = false;

            for (int i = 1; i < 10; i++)
            {
                comboBox2.Items.Add(i);
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem != null)
            {
                secilenCombobox = Convert.ToInt32(comboBox2.SelectedItem);

            }
        }
    }
}
