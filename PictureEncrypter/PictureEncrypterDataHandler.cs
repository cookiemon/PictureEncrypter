using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Threading;

namespace PictureEncrypter
{
    /// <summary>
    /// Dataprovider for main window
    /// </summary>
    public class PictureEncrypterDataHandler : INotifyPropertyChanged
    {
        private Image _decrypted;
        private Image _encrypted;
        /// <summary>
        /// Decrypted image
        /// </summary>
        public Image Decrypted
        {
            get { return _decrypted; }
            set { changeProperty(value, ref _decrypted, "Decrypted", "DecryptedImage"); }
        }
        /// <summary>
        /// Encrypted image
        /// </summary>
        public Image Encrypted
        {
            get { return _encrypted; }
            set { changeProperty(value, ref _encrypted, "Encrypted", "EncryptedImage"); }
        }
        /// <summary>
        /// Key to encrypt with
        /// </summary>
        public float Key
        {
            get
            {
                return SelectedEncryption.GetKey();
            }
            set
            {
                SelectedEncryption.SetKey(value);
                notifyPropertyChange("Key");
                if (UpdateOnKeyChange)
                {
                    DispatchNewEncrypt();
                }
            }
        }

        private SupportedAlgorithm _selectedEncryption;
        /// <summary>
        /// Used encryption algorithm
        /// </summary>
        public SupportedAlgorithm SelectedEncryption
        {
            get
            {
                return _selectedEncryption;
            }
            set
            {
                changeProperty(value, ref _selectedEncryption, "SelectedEncryption", "Key");
            }
        }

        /// <summary>
        /// List of encrypters
        /// </summary>
        public SupportedAlgorithm[] Encrypters { get { return SupportedAlgorithm.All(); } }

        private bool _updateOnKeyChange = false;
        public bool UpdateOnKeyChange
        {
            get { return _updateOnKeyChange; }
            set { changeProperty(value, ref _updateOnKeyChange, "UpdateOnKeyChange"); }
        }

        /// <summary>
        /// Decrypted image as BitmapSource for
        /// display
        /// </summary>
        public BitmapSource DecryptedImage { get { return Utilities.CreateBitmapImage(_decrypted); } }
        /// <summary>
        /// EncryptedImage as BitmapSource for
        /// display
        /// </summary>
        public BitmapSource EncryptedImage { get { return Utilities.CreateBitmapImage(_encrypted); } }

        /// <summary>
        /// Does nothing
        /// </summary>
        public PictureEncrypterDataHandler()
        {
            //LoadImage(@"C:\Users\Public\Pictures\Sample Pictures\Chrysanthemum.jpg");
            SupportedAlgorithm[] list = SupportedAlgorithm.All();
            if (list.Length > 0)
                _selectedEncryption = list[0];

            //LoadImage(@"C:\Users\Wardragon\Downloads\220px-Tux.png");
        }

        /// <summary>
        /// Load image to encrypted and
        /// decrypted image
        /// </summary>
        /// <param name="path">Path to image file</param>
        public void LoadImage(string path)
        {
            Image img = Image.FromFile(path);
            if (Encrypted != null)
                Encrypted.Dispose();
            if (Decrypted != null)
                Decrypted.Dispose();
            Decrypted = img;
            Encrypted = (Image)img.Clone();
        }

        /// <summary>
        /// Saves encrypted image
        /// </summary>
        /// <param name="path">Path for image file</param>
        public void SaveEncryptedImage(string path)
        {
            if(Encrypted != null)
                Encrypted.Save(path);
        }

        /// <summary>
        /// Saves decrypted image
        /// </summary>
        /// <param name="path">Path for image file</param>
        public void SaveDecryptedImage(string path)
        {
            if(Decrypted != null)
                Decrypted.Save(path);
        }

        /// <summary>
        /// Encrypts the decrypted image
        /// </summary>
        /// <param name="alg">Algorithm to use for encryption</param>
        public void Encrypt(SupportedAlgorithm alg)
        {
            if(Decrypted != null)
                Encrypted = ImageEncrypter.EncryptImage(Decrypted, alg.Encrypter);
        }

        /// <summary>
        /// Encrypts the decrypted image with selected algorithm
        /// </summary>
        public void Encrypt()
        {
            if(Decrypted != null)
                Encrypted = ImageEncrypter.EncryptImage(Decrypted, SelectedEncryption.Encrypter);
        }

        private ThreadWorker _worker = new ThreadWorker();
        public void DispatchNewEncrypt()
        {
            if(Decrypted != null)
                lock (_worker)
                {
                    if (!_worker.IsExecuting())
                        _worker.StartWork();
                    if (_worker.CountWorkItems < 2)
                        _worker.addWork(Encrypt);
                }
        }

        /// <summary>
        /// Generates a key
        /// </summary>
        public void GenerateKey()
        {
            SelectedEncryption.GenerateNewKey();
            notifyPropertyChange("Key");
        }

        /// <summary>
        /// Fired if a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void changeProperty<T>(T newValue, ref T oldValue, params string[] name)
        {
            if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                oldValue = newValue;
                notifyPropertyChange(name);
            }
        }
        private void notifyPropertyChange(params string[] name)
        {
            if (PropertyChanged != null)
            {
                for (int i = 0; i < name.Length; ++i)
                    PropertyChanged(this, new PropertyChangedEventArgs(name[i]));
            }
        }
    }
}
