using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Numerics;

namespace PictureEncrypter
{
    /// <summary>
    /// Class defining supported symmetric
    /// encryption algorithms.
    /// </summary>
    public class SupportedSymmetricAlgorithm : SupportedAlgorithm
    {
        private string _name;
        public override string Name { get { return _name; } }
        /// <summary>
        /// Used algorithm
        /// </summary>
        protected SymmetricAlgorithm _algorithm;
        public override ICryptoTransform Encrypter { get { return _algorithm.CreateEncryptor(); } }

        /// <summary>
        /// Create supported algorithm
        /// </summary>
        /// <param name="name">Name of algorithm</param>
        /// <param name="algo">Algorithm</param>
        public SupportedSymmetricAlgorithm(string name, SymmetricAlgorithm algo)
        {
            _name = name;
            _algorithm = algo;
        }

        public override void GenerateNewKey()
        {
            _algorithm.GenerateKey();
            _algorithm.GenerateIV();
        }

        public override void SetKey(float key)
        {
            if (key < 0.0f)
            {
                key = 0.0f;
            }
            if (key > 1.0f)
            {
                key = 1.0f;
            }
            int keysize = _algorithm.KeySize;

            if (keysize <= (sizeof(long) * 8))
            {
                ulong newKey;

                if (0.0001 < key && key < 0.9999)
                {
                    // Workaround because shift only considers least six significant bit
                    // Therefore you can't shift 1 until it is out of the byte in 1 operation
                    if (keysize != (sizeof(long) * 8))
                        newKey = (1U << keysize) - 1;
                    else
                        newKey = ulong.MaxValue;
                    newKey = (ulong)(newKey * key + 0.5);
                }
                else if (key < 0.0001)
                    newKey = 0LU;
                else
                    newKey = ulong.MaxValue;

                // Number
                // 0x00000001
                // Index
                //    3 2 1 0
                byte[] keyBytes = new byte[keysize / 8];
                for (int i = 0; i < keysize / 8; ++i)
                {
                    keyBytes[i] = (byte)(newKey >> (i * 8));
                }

                if (_algorithm is DES)
                {
                    MakeDESKey(ref keyBytes);
                }
                _algorithm.Key = keyBytes;
            }
            else
            {
                throw new NotImplementedException("TODO: Implement keysizes > 64 Bit");
            }
        }

        /// <summary>
        /// Converts 64 bit key to DES Key.
        /// Slashes lower values, spreads the bits and calculates
        /// parity
        /// </summary>
        /// <param name="keyBytes">Key</param>
        private void MakeDESKey(ref byte[] keyBytes)
        {
            keyBytes[0] = 0;
            for (int i = 0; i < keyBytes.Length - 1; ++i)
            {
                keyBytes[i] >>= 8 - i;
                keyBytes[i] |= (byte)((keyBytes[i + 1] & (0xFF >> (i + 1))) << (i));
                keyBytes[i] <<= 1;

                keyBytes[i] |= CalculateParity(keyBytes[i]);
            }
            keyBytes[keyBytes.Length - 1] &= 0xFE;
            keyBytes[keyBytes.Length - 1] |= CalculateParity(keyBytes[keyBytes.Length - 1]);
        }

        /// <summary>
        /// Calculates uneven parity
        /// </summary>
        /// <param name="singleByte">Byte to calculate parity for</param>
        /// <returns>value of parity bit</returns>
        private static byte CalculateParity(byte singleByte)
        {
            int bitCount = 0;
            for (int k = 0; k < 8; ++k) bitCount += (singleByte >> k) & 0x01;
            return (byte)(0x01 - (bitCount % 2));
        }

        public override float GetKey()
        {
            byte[] key = _algorithm.Key;

            if (key.Length <= sizeof(ulong))
            {
                if (_algorithm is DES)
                {
                    key = UnmakeDESKey(key);
                }

                ulong keyNumber = 0;
                for (int i = 0; i < key.Length; ++i)
                {
                    keyNumber |= (ulong)(key[i]) << (i * 8);
                }

                ulong keyMax;
                if (key.Length != sizeof(ulong))
                    keyMax = (1LU << (key.Length * 8));
                else
                    keyMax = ulong.MaxValue;
                return (float)(keyNumber) / keyMax;
            }
            else
            {
                throw new NotImplementedException("TODO: Implement keysizes > 64 Bit");
            }
        }

        /// <summary>
        /// Removes parity bits from key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private byte[] UnmakeDESKey(byte[] key)
        {
            byte[] retVal = new byte[key.Length - 1];

            for (int i = 0; i < retVal.Length; ++i)
            {
                retVal[i] = (byte)(key[i] & 0xFE);
                retVal[i] >>= 1 + i;
                byte upperValue = (byte)((0xFF >> (7 - i)) & (key[i+1] >> 1));

                retVal[i] |= (byte)(upperValue << (7 - i));
            }

            return retVal;
        }
    }

    /// <summary>
    /// Class defining supported encryption
    /// algorithms.
    /// </summary>
    public abstract class SupportedAlgorithm
    {
        /// <summary>
        /// Name of algorithm
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Cryptographic Transformer for algorithm
        /// </summary>
        public abstract ICryptoTransform Encrypter { get; }
        /// <summary>
        /// Generates a new random key
        /// </summary>
        public abstract void GenerateNewKey();
        /// <summary>
        /// Sets a new encryption key
        /// </summary>
        /// <param name="key">Value between 0 and 1 representing the key</param>
        public abstract void SetKey(float key);
        /// <summary>
        /// Gets the encryption key
        /// </summary>
        /// <returns>Value between 0 and 1 representing the key</returns>
        public abstract float GetKey();

        /// <summary>
        /// List of supported algorithms
        /// </summary>
        private static List<SupportedAlgorithm> _supportedEncrypters = new List<SupportedAlgorithm>();
        /// <summary>
        /// List of supported algorithms
        /// </summary>
        /// <returns></returns>
        public static SupportedAlgorithm[] All()
        {
            return _supportedEncrypters.ToArray();
        }

        /// <summary>
        /// Create the list of supported algorithms
        /// </summary>
        static SupportedAlgorithm()
        {
            Cesar ces = new Cesar();
            ces.GenerateKey();
            ces.GenerateIV();
            ces.Key = new byte[] { 128 };
            _supportedEncrypters.Add(new SupportedSymmetricAlgorithm("Rot", ces));
            DES des = DESCryptoServiceProvider.Create();
            des.GenerateIV();
            des.GenerateKey();
            des.Mode = CipherMode.ECB;
            _supportedEncrypters.Add(new SupportedSymmetricAlgorithm("DES ECB", des));
            des = DESCryptoServiceProvider.Create();
            des.GenerateIV();
            des.GenerateKey();
            des.Mode = CipherMode.CBC;
            _supportedEncrypters.Add(new SupportedSymmetricAlgorithm("DES CBC", des));
        }
    }
}
