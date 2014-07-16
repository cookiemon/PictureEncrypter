using System.Drawing;
using System.Drawing.Imaging;

namespace PictureEncrypter
{
    /// <summary>
    /// Interface for converting images to/from byte arrays
    /// </summary>
    public interface IImageRawDataConverter
    {
        /// <summary>
        /// Converts image to byte array
        /// </summary>
        /// <param name="img">Image</param>
        /// <param name="pxfm">Used Pixelformat</param>
        /// <returns>Pixel data</returns>
        byte[] ToArray(Image img, PixelFormat pxfm);
        /// <summary>
        /// Converts byte array to image
        /// </summary>
        /// <param name="arr">Pixel data</param>
        /// <param name="height">Image height</param>
        /// <param name="width">Image width</param>
        /// <param name="pxfm">Pixel format</param>
        /// <returns>Image</returns>
        Image ToImage(byte[] arr, int height, int width, PixelFormat pxfm);
    }
}
