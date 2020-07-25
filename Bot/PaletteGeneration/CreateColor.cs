using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Sunflower.Bot.PaletteGeneration
{
    public class ColorGen
    {
        public static string RandomColor()
        {
            var codebytes = new byte[8];
            using (var rnd = RandomNumberGenerator.Create())
                rnd.GetBytes(codebytes);

            return BitConverter.ToString(codebytes).ToLower().Replace("-", "").Substring(0, 8);
        }
        public static void CreateImage(string path, int colorNumber, string[] hexArray)
        {

            int width = colorNumber, height = 1;
            Bitmap bmp = new Bitmap(width, height);

            for (int i = 0; i < width; i++)
            {
                string colorcode = "ff" + hexArray[i];
                int argb = Int32.Parse(
                colorcode.Substring(1, 2) + // Alpha
                colorcode.Substring(7, 2) + // Red
                colorcode.Substring(5, 2) + // Green
                colorcode.Substring(3, 2),  // Blue
                    NumberStyles.HexNumber);
                Console.WriteLine();

                for (int x = 0; x < width; x++)
                {
                    bmp.SetPixel(x, 0, Color.FromArgb(Convert.ToInt32(argb)));
                }
            }

            

            bmp.Save((@"C:\Users\anivire\source\repos\Sunflower\Sunflower\bin\Debug\netcoreapp3.1\Palettes\RNGpalette.png"));
            

            /*int width = colorNumber, height = 1;

            Bitmap bmp = new Bitmap(width, height); FromArgb(Convert.ToInt32(hexArray[x], 16))
            Random rand = new Random();

            for (int i = 0; i != colorNumber; i++)
            {
                int a = rand.Next(256);
                int r = rand.Next(256);
                int g = rand.Next(256);
                int b = rand.Next(256);

                //set ARGB value
                bmp.SetPixel(i, 1, Color.FromArgb(a, r, g, b));

                //bmp.SetPixel(i, 1, Color.FromArgb(Convert.ToInt32(hexArray[i], 16)));
                //Console.WriteLine(Color.FromArgb(Convert.ToInt32(hexArray[i], 16)));
            }
            bmp.Save(@"C:\Users\anivire\source\repos\Sunflower\Sunflower\bin\Debug\netcoreapp3.1\Palettes\RNGpalette.png");*/
        }
    }
}
