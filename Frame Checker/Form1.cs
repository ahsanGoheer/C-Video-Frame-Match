using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;

namespace Frame_Checker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.DoWork += BackgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += BackgroundWorker1_ProgressChanged;
            backgroundWorker1.WorkerReportsProgress = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            backgroundWorker1.CancelAsync();
            Application.Exit();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog()==DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog video_di = new OpenFileDialog();
            video_di.Filter = "Video Files(*.MP4)|*.MP4";

            if (video_di.ShowDialog()==DialogResult.OK)
            {
                textBox2.Text = video_di.FileName;

            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> files = new List<string>();
            files = Directory.GetFiles(textBox1.Text,"*.jpg").ToList<string>();
            //progressBar1.Minimum = 0;
            //progressBar1.Maximum = files.Count;
            //Set_ProgressBar(0,files.Count);
            

            foreach (string file in files)
            {
                pictureBox1.Image = null;
                pictureBox1.ImageLocation = file;
                Emgu.CV.VideoCapture videoCapture = new VideoCapture(textBox2.Text);
                Mat image = new Mat();
                Mat Img2 = new Mat();
                Mat img = new Mat(file);
                Emgu.CV.CvInvoke.Resize(img, img, new Size(16, 16), 0, 0, Emgu.CV.CvEnum.Inter.Linear);
                double nrmse = 0;
              
           
                while (videoCapture.IsOpened)
                {
                    image = videoCapture.QueryFrame();
                    Emgu.CV.CvInvoke.Resize(image, Img2, new Size(16, 16), 0, 0, Emgu.CV.CvEnum.Inter.Linear);
                    nrmse = calc_mse(img, Img2)*100;
                    
                   
                    if (nrmse <= 10)
                    {
                        pictureBox2.Image = image.Bitmap;
                        textBox3.Text = nrmse.ToString();

                        break;

                    }
                    else
                    {
                        textBox4.Text = nrmse.ToString();
                        pictureBox2.Image = null;
                    }
                    //if (nrmse <= 0.05)
                    //{
                    //    pictureBox2.Image = image.Bitmap;
                    //    textBox3.Text = nrmse.ToString();

                    //    break;

                    //}
                    ////else if(nrmse<=0.1)
                    //{
                    //    pictureBox2.Image = image.Bitmap;
                    //    textBox3.Text = nrmse.ToString();

                    //    break;
                    //}
                    //else if(nrmse<=0.15)
                    //{
                    //    pictureBox2.Image = image.Bitmap;
                    //    textBox3.Text = nrmse.ToString();

                    //    break;
                    //}


                }
                System.Threading.Thread.Sleep(1000);
                backgroundWorker1.ReportProgress(1);
                //System.Threading.Thread.Sleep(1000);
            }

        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;

        }
        void Set_ProgressBar(int start,int end)
        {
            progressBar1.Maximum = start;
            progressBar1.Minimum = end;
        }

        //public byte[] imageToByteArray(System.Drawing.Image imageIn)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
        //    return ms.ToArray();
        //}
        public double calc_mse(Mat image1, Mat image2)
        {
            double diff;
            byte img1;
            byte img2;
            //float nrmse = 0;
            double sum = 0;
            double total_size = image1.Width * image1.Height;
            //float y1=0, y2=0;
            double max=0, min=0;
            double square_of_Diff = 0;
            for (int x = 0; x < image1.Width; x++)
            {
                for (int y = 0; y < image1.Height; y++)
                {
                   img1= image1.ToImage<Gray,Byte>().Data[x,y,0];
                   img2 = image2.ToImage<Gray, Byte>().Data[x, y, 0];
                   diff =Convert.ToDouble( img1 - img2);
                    square_of_Diff = (diff*diff);
                    if(max==0)
                    {
                        max = diff;
                    }
                   if(max<diff)
                    {
                        max = diff;
                    }
                   if(diff<min)
                    {
                        min = diff;
                    }
                   sum += (square_of_Diff);
                }
            }
            return ((System.Math.Sqrt(sum / total_size))/(max-min));
        }
    }

}
