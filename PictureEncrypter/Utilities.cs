using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace PictureEncrypter
{
    /// <summary>
    /// Utility class for... stuff...
    /// </summary>
    class Utilities
    {
        /// <summary>
        /// Creates a BitmapImage from an Image
        /// </summary>
        /// <param name="img">Source</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage CreateBitmapImage(Image img)
        {
            BitmapImage newImg = new BitmapImage();
            MemoryStream mms = new MemoryStream();
            newImg.BeginInit();
            img.Save(mms, ImageFormat.Png);
            newImg.StreamSource = mms;
            newImg.EndInit();
            newImg.Freeze();
            return newImg;
        }

        /// <summary>
        /// Creates file filters for all image files
        /// </summary>
        /// <returns>Filter string for windows file dialog
        /// containing all supported images</returns>
        public static string CreateFileFilters()
        {
            string retVal = String.Format("{0} ({1})|{1}", "All Files", "*.*");
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                retVal = String.Format("{0}|{1} ({2})|{2}", retVal, codecName, c.FilenameExtension);
            }

            return retVal;
        }
    }
}
