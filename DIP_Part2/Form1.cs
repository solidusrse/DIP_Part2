using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static WebCamLib.DeviceManager;
using static WebCamLib.Device;
using WebCamLib;

namespace DIP_Part1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
        }


        private Device webcam;
        Bitmap bitmap, bmResult; //normal operations
        Bitmap imageA, imageB, colorgreen; //image subtraction
        Bitmap videoLoaded, videoResult; //webcam
        Color pixel, pxResult, backpixel;
        Boolean copy, greyscale, invert, histogram, sepia, imagesubtract;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            pictureBox1.Image = new Bitmap(openFileDialog1.FileName);
            bitmap = (Bitmap)pictureBox1.Image;
            bmResult = new Bitmap(bitmap.Width, bitmap.Height);
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = @"PNG|*.png" })
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox2.Image.Save(saveFileDialog.FileName);
                }
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = pictureBox2.Image = null;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        //copy start
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    for (int y = 0; y < bitmap.Height; y++)
                    {
                        pixel = (Color)bitmap.GetPixel(x, y);
                        bmResult.SetPixel(x, y, pixel);
                    }
                }
                pictureBox2.Image = bmResult;
        }

        //copy end

        //greyscale start
        private void greyscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int avg;

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);
                    avg = (int)((pixel.R + pixel.G + pixel.B) / 3);
                    pxResult = Color.FromArgb(avg, avg, avg);
                    bmResult.SetPixel(x, y, pxResult);
                }
            }

            pictureBox2.Image = bmResult;
        }
        //greyscale end

        //invert start
        private void invertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);
                    pxResult = Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    bmResult.SetPixel(x, y, pxResult);
                }
            }

            pictureBox2.Image = bmResult;
        }
        //invert end

        //histogram start
        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int avg;

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);
                    avg = (int)((pixel.R + pixel.G + pixel.B) / 3);
                    pxResult = Color.FromArgb(avg, avg, avg);
                    bmResult.SetPixel(x, y, pxResult);
                }
            }

            Color histPix;
            int[] histogram = new int[256];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    histPix = bmResult.GetPixel(x, y);
                    histogram[histPix.R]++;
                }
            }

            Bitmap data = new Bitmap(256, 379);

            //Background
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < 379; y++)
                {
                    data.SetPixel(x, y, Color.White);
                }
            }

            //Histogram Data
            for (int x = 0; x < 256; x++)
            {
                for (int y = 0; y < Math.Min(histogram[x] / 5, 379); y++)
                {
                    data.SetPixel(x, y, Color.Black);
                }
            }
            pictureBox2.Image = data;
        }
        //histogram end

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog2.ShowDialog();
        }

        private void openFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            imageA = new Bitmap(openFileDialog3.FileName);
            pictureBox2.Image = imageA;
        }

        //subtraction start
        private void subtract()
        {
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;
            Bitmap resultImage = new Bitmap(imageB.Width, imageB.Height);

            for (int x = 0; x < imageA.Width; x++)
            {
                for (int y = 0; y < imageA.Height; y++)
                {
                    pixel = imageB.GetPixel(x, y);
                    backpixel = imageA.GetPixel(x, y);
                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue > threshold)
                        resultImage.SetPixel(x, y, pixel);
                    else
                        resultImage.SetPixel(x, y, backpixel);
                }
            }
            pictureBox3.Image = resultImage;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            subtract();
        }
        //subtraction end

        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            StartCam();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Color vidPixel;
            Image bmap;
            if(webcam != null)
            {
                IDataObject data = Clipboard.GetDataObject();
                if(data != null && data.GetDataPresent(DataFormats.Bitmap))
                {
                    bmap = (Image)data.GetData("System.Drawing.Bitmap", true);
                    videoLoaded = new Bitmap(bmap);
                    videoResult = new Bitmap(videoLoaded.Width, videoLoaded.Height);
                    pictureBox4.Image = videoLoaded;
                }
                if (copy)
                {
                    webcam.Sendmessage();
                    for(int x=0; x<videoResult.Width; x++)
                        for(int y=0; y<videoResult.Height; y++)
                        {
                            vidPixel = videoLoaded.GetPixel(x, y);
                            videoResult.SetPixel(x, y, vidPixel);
                        }
                    pictureBox5.Image = videoResult;
                }
                if (greyscale)
                {
                    webcam.Sendmessage();
                    int grey;
                    for(int x=0; x<videoResult.Width; x++)
                        for(int y=0; y < videoResult.Height; y++)
                        {
                            vidPixel = videoLoaded.GetPixel(x, y);
                            grey = (vidPixel.R + vidPixel.G + vidPixel.B) / 3;
                            videoResult.SetPixel(x, y, Color.FromArgb(grey, grey, grey));
                        }
                }
                if (invert)
                {
                    webcam.Sendmessage();
                    for (int x = 0; x < videoResult.Width; x++)
                        for (int y = 0; y < videoResult.Height; y++)
                        {
                            vidPixel = videoLoaded.GetPixel(x, y);
                            videoResult.SetPixel(x, y, Color.FromArgb(255-vidPixel.R, 255-vidPixel.G, 255-vidPixel.B));
                        }
                    pictureBox5.Image = videoResult;
                }
                if (sepia)
                {
                    webcam.Sendmessage();
                    int r, g, b, newR, newG, newB;
                    for (int x = 0; x < videoResult.Width; x++)
                        for (int y = 0; y < videoResult.Height; y++)
                        {
                            vidPixel = videoLoaded.GetPixel(x, y);
                            r = vidPixel.R;
                            g = vidPixel.G;
                            b = vidPixel.B;
                            newR = Math.Min(((int)(.393 * r + .768 * g + .189 * b)), 255);
                            newG = Math.Min(((int)(.349 * r + .686 * g + .168 * b)), 255);
                            newB = Math.Min(((int)(.272 * r + .534 * g + .131 * b)), 255);
                            videoResult.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                        }
                    pictureBox5.Image = videoResult;
                }
                if (histogram)
                {
                    webcam.Sendmessage();
                    int avg;

                    for (int x = 0; x < videoLoaded.Width; x++)
                        for (int y = 0; y < videoLoaded.Height; y++)
                        {
                            vidPixel = videoLoaded.GetPixel(x, y);
                            avg = (int)(vidPixel.R + vidPixel.G +  vidPixel.B)/3;
                            videoResult.SetPixel(x, y, Color.FromArgb(avg, avg, avg));
                        }

                    Color histPix;
                    int[] histogram = new int[256];
                    for(int x = 0; x < videoResult.Width; x++)
                        for(int y = 0; y < videoLoaded.Height; y++)
                        {
                            histPix = videoResult.GetPixel(x, y);
                            histogram[histPix.R]++;
                        }

                    Bitmap data1 = new Bitmap(256, 379);
                    for(int x = 0; x < 256; x++)
                        for( int y = 0;y < 379; y++)
                        {
                            data1.SetPixel(x, y, Color.White);
                        }

                    for(int x = 0;x < 256; x++)
                        for(int  y = 0;y < 379; y++)
                        {
                            data1.SetPixel(x, y, Color.Black);
                        }
                    pictureBox5.Image = data1;
                }
            }
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webcam.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            copy = true;
            invert = false;
            greyscale = false;
            sepia = false;
            imagesubtract = false;
            histogram = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            copy = false;
            invert = false;
            greyscale = true;
            sepia = false;
            imagesubtract = false;
            histogram = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            copy = false;
            invert = true;
            greyscale = false;
            sepia = false;
            imagesubtract = false;
            histogram = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            copy = false;
            invert = false;
            greyscale = false;
            sepia = false;
            imagesubtract = false;
            histogram = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            copy = false;
            invert = false;
            greyscale = false;
            sepia = true;
            imagesubtract = false;
            histogram = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            copy = false;
            invert = false;
            greyscale = false;
            sepia = false;
            imagesubtract = true;
            histogram = false;
        }

        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            imageB = new Bitmap(openFileDialog2.FileName);
            pictureBox1.Image = imageB;
        }

        //sepia start
        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    pixel = bitmap.GetPixel(x, y);

                    int r = (int)(0.393 * pixel.R + 0.769 * pixel.G + 0.189 * pixel.B);
                    int g = (int)(0.349 * pixel.R + 0.686 * pixel.G + 0.168 * pixel.B);
                    int b = (int)(0.272 * pixel.R + 0.534 * pixel.G + 0.131 * pixel.B);

                    if (r > 255) { r = 255; }
                    if (g > 255) { g = 255; }
                    if (b > 255) { b = 255; }

                    pxResult = Color.FromArgb(r, g, b);
                    bmResult.SetPixel(x, y, pxResult);
                }
            }
            pictureBox2.Image = bmResult;
        }
        //sepia end

        private void Form1_Load(object sender, EventArgs e)
        {
            copy = greyscale = invert = sepia = imagesubtract = histogram = false;
        }

        private void StartCam()
        {
            webcam = new Device();
            webcam.ShowWindow(pictureBox4);
            timer1.Start();
        }
    }
}
