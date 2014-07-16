using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace PictureEncrypter
{
    /// <summary>
    /// Converts image to raw byte array or vice
    /// versa via use of fast but unsafe memory
    /// mapping.
    /// </summary>
    public class FastImageRawDataConverter : IImageRawDataConverter
    {
        public byte[] ToArray(Image img, PixelFormat pxfm)
        {
            using (Bitmap bmp = new Bitmap(img))
            {
                int bytesPerPixel = Image.GetPixelFormatSize(pxfm)/8;

                BitmapData bits = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    pxfm);

                try
                {
                    byte[] arr = new byte[bmp.Width * bmp.Height * bytesPerPixel];
                    unsafe
                    {
                        byte* ptr = (byte*)bits.Scan0;
                        for (int i = 0; i < arr.Length; ++i)
                        {
                            arr[i] = ptr[i];
                        }
                    }
                    return arr;
                }
                finally
                {
                    bmp.UnlockBits(bits);
                }
            }
        }

        public Image ToImage(byte[] arr, int height, int width, PixelFormat pxfm)
        {
            Bitmap bmp = new Bitmap(width, height, pxfm);
            BitmapData bits = bmp.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                pxfm);

            int bytesPerPixel = Image.GetPixelFormatSize(pxfm)/8;

            if(arr.Length < width*height*bytesPerPixel)
                throw new ArgumentException("Not enough raw data to fill picture");

            try
            {
                unsafe
                {
                    byte* ptr = (byte*)bits.Scan0;
                    for (int i = 0; i < height*width*bytesPerPixel; ++i)
                    {
                        ptr[i] = arr[i];
                    }
                }
                return bmp;
            }
            finally
            {
                bmp.UnlockBits(bits);
            }
        }
    }
}
