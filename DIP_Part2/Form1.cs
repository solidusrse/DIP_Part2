using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DIP_Part1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bitmap, bmResult=null; //normal operations
        Bitmap imageA, imageB, colorgreen; //image subtraction
        Color pixel, pxResult, backpixel;

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
        private void button3_Click(object sender, EventArgs e)
        {
            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B)/3;
            int threshold = 5;
            Bitmap resultImage = new Bitmap(imageB.Width, imageB.Height);

            for (int x=0; x < imageA.Width; x++)
            {
                for(int y=0; y < imageA.Height; y++)
                {
                    pixel = imageB.GetPixel(x, y);
                    backpixel = imageA.GetPixel(x, y);   
                    int grey = (pixel.R + pixel.G + pixel.B)/3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue > threshold)
                        resultImage.SetPixel(x, y, pixel);
                    else
                        resultImage.SetPixel(x, y, backpixel);
                }
            }
            pictureBox3.Image = resultImage;
        }
        //subtraction end

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog3.ShowDialog();
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

        }
    }
}
