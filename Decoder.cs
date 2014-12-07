//Luis Garcia

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PictureCoder
{
    class Decoder
    {
        FileStream infile;
        BinaryReader reader;
        bool hasErrors = false;

        public Decoder(string infp)
        {
            //check for null filepath
            if (string.IsNullOrEmpty(infp) || string.IsNullOrWhiteSpace(infp))
            {
                MessageBox.Show("An input filepath must be entered.");
                hasErrors = true;
                return;
            }
            else
            {
                try
                {
                    infile = new FileStream(infp, FileMode.Open, FileAccess.Read);
                    reader = new BinaryReader(infile);
                }
                catch
                {
                    MessageBox.Show("Input filepath error.");
                    hasErrors = true;
                    return;
                }
            }
        }


        public string Decode()
        {
            if (infile == default(FileStream) && hasErrors == false)
            {
                MessageBox.Show("Input filepath error.");
                hasErrors = true;
                return "";
            }
            if (hasErrors == true)
            {
                return "";
            }

            StringBuilder message = new StringBuilder();
            bool done = false;

            //get past header and check to see if .bmp
            for (int i = 0; i < 54; i++)
            {
                byte[] MyByte = new byte[1];
                MyByte = reader.ReadBytes(1);
                if (i == 0 && MyByte[0] != Convert.ToByte(66))
                {
                    MessageBox.Show("Selected input file is not a .bmp image");
                    return "";
                }
                if (i == 1 && MyByte[0] != Convert.ToByte(77))
                {
                    MessageBox.Show("Selected input file is not a .bmp image");
                    return "";
                }
            }

            while (!done)
            {
                byte[] MyByte = new byte[1];
                int[] TextBinData = new int[8];

                //fill TextBinData with one char
                for (int i = 7; i >= 0; i--)
                {
                    MyByte = reader.ReadBytes(1);
                    if (MyByte[0] % 2 == 0)
                    {
                        TextBinData[i] = 0;
                    }
                    else
                    {
                        TextBinData[i] = 1;
                    }
                }

                //add the char to the string
                UInt32 intChar = 0;
                for (int i = 0; i <= 7; i++)
                {
                    if (TextBinData[i] % 2 == 1)
                    {
                        intChar = intChar + Convert.ToUInt32(TextBinData[i] * Math.Pow(2, i));
                    }
                }

                if (intChar == 0)
                {
                    done = true;
                }
                else
                {
                    char myChar = Convert.ToChar(intChar);
                    message.Append(myChar);
                }
            }

            infile.Close();
            return message.ToString();
        }

    }
}
