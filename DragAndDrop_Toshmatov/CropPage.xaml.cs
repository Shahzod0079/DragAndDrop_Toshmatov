using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace DragAndDrop_Toshmatov
{
    public partial class CropPage : Page
    {
        private Point startPoint;
        private BitmapImage originalImage;
        private double scaleX, scaleY;

        public CropPage()
        {
            InitializeComponent();
            LoadImage();
        }

        private void LoadImage()
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image Files|*.jpg;*.png;*.bmp;*.jpeg";

            if (openDialog.ShowDialog() == true)
            {
                originalImage = new BitmapImage();
                originalImage.BeginInit();
                originalImage.UriSource = new Uri(openDialog.FileName);
                originalImage.CacheOption = BitmapCacheOption.OnLoad;
                originalImage.EndInit();
                image.Source = originalImage;

                // Обновляем размеры
                originalSize.Text = $"{originalImage.PixelWidth} x {originalImage.PixelHeight}";
                scaleSize.Text = $"{originalImage.PixelWidth} x {originalImage.PixelHeight}";

                cropRect.Visibility = Visibility.Collapsed;
            }
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(canvas);
            cropRect.Visibility = Visibility.Visible;
            cropRect.Width = 0;
            cropRect.Height = 0;
            Canvas.SetLeft(cropRect, startPoint.X);
            Canvas.SetTop(cropRect, startPoint.Y);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && cropRect.Visibility == Visibility.Visible)
            {
                Point currentPoint = e.GetPosition(canvas);

                double left = Math.Min(startPoint.X, currentPoint.X);
                double top = Math.Min(startPoint.Y, currentPoint.Y);
                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);

                Canvas.SetLeft(cropRect, left);
                Canvas.SetTop(cropRect, top);
                cropRect.Width = width;
                cropRect.Height = height;

                // Масштабируем координаты под оригинал
                double imgWidth = image.ActualWidth;
                double imgHeight = image.ActualHeight;
                scaleX = originalImage.PixelWidth / imgWidth;
                scaleY = originalImage.PixelHeight / imgHeight;

                int outW = (int)(width * scaleX);
                int outH = (int)(height * scaleY);
                outputSize.Text = $"{outW} x {outH}";
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Рамка остаётся
        }

        private void ClearCrop(object sender, RoutedEventArgs e)
        {
            cropRect.Visibility = Visibility.Collapsed;
            outputSize.Text = "0 x 0";
        }

        private void CropImage(object sender, RoutedEventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Сначала загрузите фото!");
                LoadImage();
                return;
            }

            if (cropRect.Width <= 0 || cropRect.Height <= 0)
            {
                MessageBox.Show("Выделите область для обрезки!");
                return;
            }

            double cropLeft = Canvas.GetLeft(cropRect);
            double cropTop = Canvas.GetTop(cropRect);

            int x = (int)(cropLeft * scaleX);
            int y = (int)(cropTop * scaleY);
            int w = (int)(cropRect.Width * scaleX);
            int h = (int)(cropRect.Height * scaleY);

            CroppedBitmap cropped = new CroppedBitmap(originalImage, new Int32Rect(x, y, w, h));

            using (MemoryStream ms = new MemoryStream())
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(cropped));
                encoder.Save(ms);
                ms.Position = 0;

                BitmapImage croppedImage = new BitmapImage();
                croppedImage.BeginInit();
                croppedImage.CacheOption = BitmapCacheOption.OnLoad;
                croppedImage.StreamSource = ms;
                croppedImage.EndInit();

                image.Source = croppedImage;
                originalImage = croppedImage;

                // Обновляем размеры
                originalSize.Text = $"{w} x {h}";
                scaleSize.Text = $"{w} x {h}";
            }

            cropRect.Visibility = Visibility.Collapsed;
            outputSize.Text = "0 x 0";
        }

        private void BackToMain(object sender, RoutedEventArgs e)
        {
            MainWindow main = (MainWindow)Application.Current.MainWindow;
            main.ShowMainContent();
        }
    }
}