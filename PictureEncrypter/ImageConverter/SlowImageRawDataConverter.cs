using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace PictureEncrypter
{
    /// <summary>
    /// Slow implementation of converting images to/from
    /// raw byte arrays. Uses GetPixel method of Bitmap.
    /// Slow as hell.
    /// </summary>
    public class SlowImageRawDataConverter : IImageRawDataConverter
    {
        public byte[] ToArray(Image img, PixelFormat pxfm)
        {
            Bitmap bmp = new Bitmap(img);
            byte[] arr = new byte[bmp.Height * bmp.Width * 3];
            for (int i = 0; i < bmp.Height * bmp.Width; ++i )
            {
                var col = bmp.GetPixel(i % bmp.Width, i / bmp.Width);
                arr[3*i+2] = col.R;
                arr[3*i+1] = col.G;
                arr[3*i] = col.B;
            }
            return arr;
        }

        public Image ToImage(byte[] arr, int height, int width, PixelFormat pxfm)
        {
            if(arr.Length != height*width*3)
                throw new ArgumentException("Array length != width*height*bpp");

            Bitmap bmp = new Bitmap(width, height);
            for (int i = 0; i < width*height; ++i)
            {
                bmp.SetPixel(i % width, i / width, Color.FromArgb(arr[3*i+2], arr[3*i+1], arr[3*i]));
            }
            return bmp;
        }
    }
}
