//Luis Garcia

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;


namespace PictureCoder
{
    class Encoder
    {
        FileStream infile;
        BinaryReader reader;

        FileStream outfile;
        BinaryWriter writer;

        string infilepath;
        string outfilepath;
        string message;

        public Encoder(string infp, string outfp, string consMessage)
        {
            //check for null filepath
            if (string.IsNullOrEmpty(infp) || string.IsNullOrWhiteSpace(infp))
            {
                MessageBox.Show("Input filepath error");
                return;
            }
            else
            {
                infilepath = infp;
            }

            //check for null filepath
            if (string.IsNullOrEmpty(outfp) || string.IsNullOrWhiteSpace(outfp))
            {
                MessageBox.Show("Output filepath error");
                return;
            }
            else
            {
                outfilepath = outfp;
            }

            //no need to check for empty message, will just insert nothing in message
            message = consMessage;
        }

        public void Copy()
        {
            try
            {
                infile = new FileStream(infilepath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(infile);
            }
            catch
            {
                MessageBox.Show("Input filepath error");
                return;
            }

            try
            {
                outfile = new FileStream(outfilepath, FileMode.Create, FileAccess.Write);
                writer = new BinaryWriter(outfile);
            }
            catch
            {
                MessageBox.Show("Output filepath error");
                return;
            }

            byte[] MyByte = new byte[1];

            while((MyByte = reader.ReadBytes(1)) != null)
            {
                try
                {
                    writer.BaseStream.WriteByte(MyByte[0]);
                }
                catch
                {
                    writer.Close();
                    MessageBox.Show("Copy Complete");
                    return;
                }
            }
        }


        public void Encode(ProgressBar progressbar)
        {
            //error catching
            try
            {
                infile = new FileStream(infilepath, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(infile);
            }
            catch
            {
                MessageBox.Show("Input filepath error");
                progressbar.Visible = false;
                return;
            }

            try
            {
                outfile = new FileStream(outfilepath, FileMode.Create, FileAccess.Write);
                writer = new BinaryWriter(outfile);
            }
            catch
            {
                MessageBox.Show("Output filepath error");
                progressbar.Visible = false;
                return;
            }

            //progress bar init
            progressbar.Minimum = 1;
            progressbar.Maximum = Convert.ToInt32(infile.Length);
            progressbar.Value = 1;
            progressbar.Step = 1;

            //var init
            byte[] MyByte = new byte[1];
            char[] mychar = new char[1];

            //copy bmp header info and check if .bmp
            for(int i = 0; i<54; i++)
            {
                MyByte = reader.ReadBytes(1);
                writer.BaseStream.WriteByte(MyByte[0]);
                progressbar.PerformStep();
                if (i == 0 && MyByte[0] != Convert.ToByte(66))
                {
                    MessageBox.Show("Selected input file is not a .bmp image");
                    progressbar.Visible = false;
                    return;
                }
                if (i == 1 && MyByte[0] != Convert.ToByte(77))
                {
                    MessageBox.Show("Selected input file is not a .bmp image");
                    progressbar.Visible = false;
                    return;
                }
            }
            

            foreach (char a in message)
            {
                int[] TextBinData = new int[8];
                uint TextIntHex = Convert.ToUInt32(a);

                //converts a single letter in the message to binary data stored in the TextBinData array
                for (int i = 7; i >= 0; i--)
                {
                    if (TextIntHex >= (Math.Pow(2, i)))
                    {
                        TextBinData[i] = 1;
                        TextIntHex = TextIntHex - Convert.ToUInt32(Math.Pow(2, i));
                    }
                    else
                    {
                        TextBinData[i] = 0;
                    }
                }

                for (int i = 7; i >= 0; i--)
                {
                    MyByte = reader.ReadBytes(1);
                    uint iByte = Convert.ToUInt32(MyByte[0]);

                    if ((MyByte[0] % 2 == 0) && (TextBinData[i] == 1))
                    {
                        iByte = iByte + 1;
                        MyByte[0] = Convert.ToByte(iByte);
                        writer.BaseStream.WriteByte(MyByte[0]);
                        progressbar.PerformStep();
                    }
                    else if ((MyByte[0] % 2 == 1) && (TextBinData[i] == 0))
                    {
                        iByte = iByte - 1;
                        MyByte[0] = Convert.ToByte(iByte);
                        writer.BaseStream.WriteByte(MyByte[0]);
                        progressbar.PerformStep();
                    }
                    else if ((MyByte[0] % 2 == -1) && (TextBinData[i] == 0))
                    {
                        iByte = iByte - 1;
                        MyByte[0] = Convert.ToByte(iByte);
                        writer.BaseStream.WriteByte(MyByte[0]);
                        progressbar.PerformStep();
                    }
                    else
                    {
                        writer.BaseStream.WriteByte(MyByte[0]);
                        progressbar.PerformStep();
                    }
                }
            }

            //this part inserts a null at the end of the encoded message to signal it is complete
            for (int i = 7; i >= 0; i--)
            {
                byte[] ReadByte = new byte[1];
                ReadByte = reader.ReadBytes(1);
                uint iByte = Convert.ToUInt32(ReadByte[0]);

                if (ReadByte[0] % 2 == 1)
                {
                    iByte = iByte - 1;
                    ReadByte[0] = Convert.ToByte(iByte);
                    writer.BaseStream.WriteByte(ReadByte[0]);
                    progressbar.PerformStep();
                }
                else if (ReadByte[0] % 2 == -1)
                {
                    iByte = iByte - 1;
                    ReadByte[0] = Convert.ToByte(iByte);
                    writer.BaseStream.WriteByte(ReadByte[0]);
                    progressbar.PerformStep();
                }
                else
                {
                    writer.BaseStream.WriteByte(ReadByte[0]);
                    progressbar.PerformStep();
                }
            }

            //this part copies the rest of the file
            while ((MyByte = reader.ReadBytes(1)) != null)
            {
                try
                {
                    writer.BaseStream.WriteByte(MyByte[0]);
                    progressbar.PerformStep();
                }
                catch
                {
                    reader.Close();
                    writer.Close();
                    MessageBox.Show("Encoding Complete");
                    progressbar.Visible = false;
                    return;
                }
            }
        }
    }
}
