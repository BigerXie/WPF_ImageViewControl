using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf_ImageViewControl.Extensions;

namespace Wpf_ImageViewControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void browseImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "图片文件 (*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp|全部文件 (*.*)|*.*";
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != true)
                return;
            var stream = openFileDialog.OpenFile();
            var bitmap = stream.ConvertToDecodeBitmap();
            this.imageView.ImageSource = bitmap;
        }
    }
}
