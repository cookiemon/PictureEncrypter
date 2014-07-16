using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace PictureEncrypter
{
    /// <summary>
    /// Class for handling cesar algorithm.
    /// Does not act as the standard symmetric algorithm
    /// because cesar algorithm does not need an IV. It
    /// only needs 1 number as a Key.
    /// Might be extended to Viginere algorithm in the
    /// future.
    /// </summary>
    class Cesar : SymmetricAlgorithm
    {
        /// <summary>
        /// Create class. Initializes a random key.
        /// </summary>
        public Cesar()
        {
            GenerateKey();
        }

        public override KeySizes[] LegalKeySizes { get { return new KeySizes[]{ new KeySizes(8, 8, 1) }; } }

        public override ICryptoTransform CreateDecryptor()
        {
            if (this.Key == null || this.Key.Length < 1)
                throw new ArgumentException("Key");
            return new CesarCryptoTransform(this.Key[0]);
        }

        public override ICryptoTransform CreateEncryptor()
        {
            if (this.Key == null || this.Key.Length < 1)
                throw new ArgumentException("Key");
            return new CesarCryptoTransform((byte)(0 - this.Key[0]));
        }
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
        {
            if (rgbKey == null || rgbKey.Length < 1)
                throw new ArgumentException("rgbKey");
            return new CesarCryptoTransform(rgbKey[0]);
        }

        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
        {
            if (rgbKey == null || rgbKey.Length < 1)
                throw new ArgumentException("rgbKey");
            return new CesarCryptoTransform((byte)(0 - rgbKey[0]));
        }

        public override void GenerateIV()
        {
            return;
        }

        public override void GenerateKey()
        {
            var tmp = new byte[1];
            RandomNumberGenerator.Create().GetNonZeroBytes(tmp);
            this.Key = tmp;
        }
    }

    /// <summary>
    /// CryptoTransform for Cesar algorithm. Is used with Shift = Key
    /// for encryption and with Shift = -key for decryption.
    /// </summary>
    class CesarCryptoTransform : ICryptoTransform
    {
        /// <summary>
        /// The value by which every character is shifted.
        /// </summary>
        public byte Shift { get; private set; }

        /// <summary>
        /// Initializes object with a shift value
        /// </summary>
        /// <param name="shift">Value by which every
        /// character is shifted</param>
        public CesarCryptoTransform(byte shift)
        {
            this.Shift = shift;
        }

        public bool  CanReuseTransform
        {
	        get { return true; }
        }

        public bool  CanTransformMultipleBlocks
        {
	        get { return true; }
        }

        public int  InputBlockSize
        {
	        get { return 1; }
        }

        public int  OutputBlockSize
        {
	        get { return 1; }
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            int bytes = 0;
 	        for(int i = 0;
                i < inputCount && inputOffset + i < inputBuffer.Length && outputOffset + i < outputBuffer.Length;
                ++i, ++bytes)
            {
                outputBuffer[outputOffset + i] = (byte)(inputBuffer[inputOffset + i] + Shift);
            }
            return bytes;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            byte[] arr = new byte[inputCount];
            TransformBlock(inputBuffer, inputOffset, inputCount, arr, arr.Length);
            return arr;
        }

        public void  Dispose()
        {
        }
    }
}
