using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace PictureEncrypter
{
    /// <summary>
    /// Encrypts an Image with a CryptoTransform
    /// </summary>
    public class ImageEncrypter
    {
        /// <summary>
        /// Encrypt Image with a CryptoTransform
        /// </summary>
        /// <param name="decrypted">Decrypted Image</param>
        /// <param name="encrypter">Cryptographic Transformer</param>
        /// <returns>Encrypted Image</returns>
        public static Image EncryptImage(Image decrypted, ICryptoTransform encrypter)
        {
            FastImageRawDataConverter conv = new FastImageRawDataConverter();
            using (MemoryStream mms = new MemoryStream())
            {
                using (CryptoStream crs = new CryptoStream(mms, encrypter, CryptoStreamMode.Write))
                {
                    byte[] rawData = conv.ToArray(decrypted, PixelFormat.Format24bppRgb);
                    crs.Write(rawData, 0, rawData.Length);
                    int blockRest = rawData.Length % encrypter.InputBlockSize;
                    if (blockRest != 0)
                    {
                        for (; blockRest < encrypter.InputBlockSize; ++blockRest)
                        {
                            crs.WriteByte(0x8);
                        }
                    }
                    return conv.ToImage(mms.ToArray(),
                        decrypted.Height, decrypted.Width, PixelFormat.Format24bppRgb);
                }
            }
        }
    }
}
