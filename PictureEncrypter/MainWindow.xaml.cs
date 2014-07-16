using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Drawing.Imaging;

namespace PictureEncrypter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //PictureEncrypterDataBinding binding;

        public MainWindow()
        {
            //binding = new PictureEncrypterDataBinding();
            InitializeComponent();
        }

        private void LoadClicked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.ShowReadOnly = false;
            dlg.CheckFileExists = true;
            dlg.Filter = Utilities.CreateFileFilters();

            bool? res = dlg.ShowDialog(this);
            if (res == true)
            {
                PictureEncrypterDataHandler bind = (PictureEncrypterDataHandler)this.Resources["PEDBDataContext"];
                bind.LoadImage(dlg.FileName);
            }
        }

        private void EncryptClicked(object sender, RoutedEventArgs e)
        {
            //SupportedAlgorithm enc = (SupportedAlgorithm)EncrypterCB.SelectedItem;
            //if (enc != null)
            //{
            //    PictureEncrypterDataHandler bind = (PictureEncrypterDataHandler)this.Resources["PEDBDataContext"];
            //    bind.Encrypt(enc);
            //}

            PictureEncrypterDataHandler bind = (PictureEncrypterDataHandler)this.Resources["PEDBDataContext"];
            bind.Encrypt();
        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {
            PictureEncrypterDataHandler bind = (PictureEncrypterDataHandler)this.Resources["PEDBDataContext"];

            if (bind.Decrypted != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.CheckPathExists = true;
                dlg.CheckFileExists = false;
                dlg.Filter = Utilities.CreateFileFilters();

                bool? res = dlg.ShowDialog();
                if (res == true)
                {
                    bind.SaveEncryptedImage(dlg.FileName);
                }
            }
        }

        private void GenerateNewKeyClicked(object sender, RoutedEventArgs e)
        {
            PictureEncrypterDataHandler bind = (PictureEncrypterDataHandler)this.Resources["PEDBDataContext"];
            bind.GenerateKey();
        }
    }
}
