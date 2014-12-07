//Luis Garcia

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PictureCoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            progressBar1.Visible = true;
            string infilepath;
            string outfilepath;
            string message = richTextBox1.Text;
            if (string.IsNullOrEmpty(infp.Text) || string.IsNullOrWhiteSpace(infp.Text))
            {
                progressBar1.Visible = false;
                MessageBox.Show("You must enter a file path for the picture you wish to encode");
                return;
            }
            else
            {
                infilepath = infp.Text;
            }

            if (string.IsNullOrEmpty(opfp.Text) || string.IsNullOrWhiteSpace(opfp.Text))
            {
                progressBar1.Visible = false;
                MessageBox.Show("You must enter a file path for the output encoded picture");
                return;
            }
            else
            {
                outfilepath = opfp.Text;
            }

            Encoder MyEncoder = new Encoder(infilepath, outfilepath, message);
            MyEncoder.Encode(progressBar1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Decoder MyDecoder = new Decoder(Decodefp.Text);
            decodemessage.Text = MyDecoder.Decode();
        }

        private void infpBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.bmp)|*.bmp";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.infp.Text = openFileDialog1.FileName;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Image Files (*.bmp)|*.bmp";
            saveFileDialog1.FilterIndex = 1;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.opfp.Text = saveFileDialog1.FileName;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files (*.bmp)|*.bmp";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Decodefp.Text = openFileDialog1.FileName;
            }
        }

    }
}
