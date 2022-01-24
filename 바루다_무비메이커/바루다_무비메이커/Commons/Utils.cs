using FFMediaToolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 바루다_무비메이커.Commons
{
    public static class Utils
    {
        private static Random rnd = new Random();
        public static int Random(int min = 0, int max = 100)
        {
            return rnd.Next(min, max);
        }

        public static unsafe Bitmap ToBitmap(this ImageData bitmap)
        {
            fixed (byte* p = bitmap.Data)
            {
                return new Bitmap(bitmap.ImageSize.Width, bitmap.ImageSize.Height, bitmap.Stride, PixelFormat.Format24bppRgb, new IntPtr(p));
            }
        }
    }
}
