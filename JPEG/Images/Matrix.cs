using System.Drawing;
using System.Drawing.Imaging;

namespace JPEG.Images
{
    class Matrix
    {
        public readonly Pixel[,] Pixels;
        public readonly int Height;
        public readonly int Width;

        public Matrix(int height, int width)
        {
            Height = height;
            Width = width;

            Pixels = new Pixel[height, width];
            for (var i = 0; i < height; ++i)
            for (var j = 0; j < width; ++j)
                Pixels[i, j] = new Pixel(0, 0, 0, PixelFormat.RGB);
        }

        public unsafe byte[,,] BitmapToByteRgbQ(Bitmap bmp)
        {
            int width = bmp.Width,
                height = bmp.Height;
            byte[,,] res = new byte[3, height, width];
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            try
            {
                byte* curpos;
                fixed (byte* _res = res)
                {
                    byte* _r = _res, _g = _res + width * height, _b = _res + 2 * width * height;
                    for (int h = 0; h < height; h++)
                    {
                        curpos = ((byte*) bd.Scan0) + h * bd.Stride;
                        for (int w = 0; w < width; w++)
                        {
                            *_b = *(curpos++);
                            *_g = *(curpos++);
                            *_r = *(curpos++);
                            Pixels[h, w].SetComponents(*_r, *_g, *_b, PixelFormat.RGB);
                            ++_b;
                            ++_g;
                            ++_r;
                        }
                    }
                }
            }
            finally
            {
                bmp.UnlockBits(bd);
            }

            return res;
        }

        public static explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height - bmp.Height % 8;
            var width = bmp.Width - bmp.Width % 8;


            var matrix = new Matrix(height, width);

            /*for (var j = 0; j < height; j++)
            {
                for (var i = 0; i < width; i++)
                {
                    var pixel = bmp.GetPixel(i, j);
                    matrix.Pixels[j, i].SetComponents(pixel.R, pixel.G, pixel.B, PixelFormat.RGB); 
                }
            }*/
                       
            matrix.BitmapToByteRgbQ(bmp);


            return matrix;
        }

        public static explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.Width, matrix.Height);

            for (var j = 0; j < bmp.Height; j++)
            {
                for (var i = 0; i < bmp.Width; i++)
                {
                    var pixel = matrix.Pixels[j, i];
                    bmp.SetPixel(i, j, Color.FromArgb(ToByte(pixel.R), ToByte(pixel.G), ToByte(pixel.B)));
                }
            }

            return bmp;
        }

        public static int ToByte(double d)
        {
            var val = (int) d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return val;
        }
    }
}