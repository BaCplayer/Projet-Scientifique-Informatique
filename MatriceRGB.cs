using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyImage
{
    class MatriceRGB
    {
        private byte red;
        private byte green;
        private byte blue;

        public MatriceRGB(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;

        }

        public byte Red
        {
            get { return red; }
            set { red = value; }
        }
        public byte Green
        {
            get { return green; }
            set { green = value; }
        }
        public byte Blue
        {
            get { return blue; }
            set { blue = value; }
        }
        public string tostring()
        {
            string result = "Le pixel sous le format RGB est : " + this.red + " " + this.green + " " + this.blue;

            return result;

        }



    }
}
