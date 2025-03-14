using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using QRCoder;
using System.IO;
using Microsoft.Win32;

namespace QR_code_generator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            userTxt.Focus(); // фокусирвока курсора 
            userTxt.SelectionStart = userTxt.Text.Length; // фокусировка  в конец строки
        }

        private void SaveImageBtnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog(); // экземпляр файлового проводника
            save.Title = "Save QR Code as ";
            save.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp";

            if (QRCodeImage.Source != null)
            {
                save.ShowDialog();
                if (string.IsNullOrEmpty(save.FileName)) return;

                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)QRCodeImage.Source));

                string savePath = save.FileName;
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            else
            {
                MessageBox.Show("Image source is null", "Null error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void userTxtChangedEvent(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (userTxt.Text.Length == 0)
                {
                    QRCodeImage.Source = null;
                    return;
                }
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode($"{userTxt.Text}", QRCodeGenerator.ECCLevel.Q);

                QRCode qrCode = new QRCode(qrCodeData);
                Bitmap qrCodeImage = qrCode.GetGraphic(20);

                var handle = qrCodeImage.GetHbitmap();
                QRCodeImage.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex}", "Error, too many characters", MessageBoxButton.OK, MessageBoxImage.Error);
                userTxt.Text = "";
            }
        }

        private void ClearImageBtnClick(object sender, RoutedEventArgs e)
        {
            QRCodeImage.Source = null;
            userTxt.Text = "";
            userTxt.Focus();
        }
    }
}
